import json
import argparse
import os
import subprocess
import tempfile
import shutil
import platform
import sys
import re
import zipfile
import hashlib
import xml.etree.ElementTree as ET

def ispython2x():
    python_ver = platform.python_version_tuple()
    if int(python_ver[0])<3:
        return True
    return False

def url_lib():
    #We want to remain compatible with python2.7 on Mac
    if ispython2x():
        import urllib
        return urllib
    import urllib.request
    return urllib.request

def remove_conditionals(id, qualifiers):
    #We can't use the python3 nonlocal feature so we need "mutable reference type" out this bool
    notapplicable = [False]
    #nested function that replaces {<qualifier>=<token>[,<token>]} with the current
    #value of qualifier
    def repl(match):
        var = match.groups()[0] #name of the qualifier
        op = match.groups()[1] #operator (i.e. = or ==)
        tokens = match.groups()[2:] #allowed values
        value = qualifiers[var] #value of qualifier
        if not value in tokens :
            #nonlocal notapplicable <-- this how it would work in python3
            notapplicable[0] = True
        if op == '=':
            return value
        elif op == '==':
            return ''
        else:
           raise Exception('Unknown operator.')

    regex = re.compile('\{(os|config|toolchain|iset|linkage|build_os)(=|==)([0-9a-zA-Z]+)(?:,([0-9a-zA-Z]+))*\}')
    new = regex.sub(repl, id)

    if notapplicable[0]:
        return None #drop it completely
    return new

def generate_nuget_project_json(refs, projectJsonPath, qualifiers, framework):
    """generate project.json as documented here http://docs.nuget.org/consume/projectjson-format"""
    deps = dict()
    for id, version in refs.items():
        id = remove_conditionals(id, qualifiers)
        if (id != None):
            qid = id.format(**qualifiers)
            deps[qid] = version
    frame = dict()
    frame[framework] = {}
    projectJsonContent = dict(dependencies=deps, frameworks=frame)
    json.dump(projectJsonContent, open(projectJsonPath, "w"))

def generate_nuget_config(repos ,config, username, password):
    """generate nuget.config as documented here https://docs.nuget.org/Consume/NuGet-Config-File"""
    configuration = ET.Element("configuration")
    packageSources = ET.SubElement(configuration,"packageSources")
    packageSourceCredentials = ET.SubElement(configuration,"packageSourceCredentials")
    i = 0
    for repo in repos:
        name = 'NugetSource' + str(i)
        i=i+1
        addSource = ET.SubElement(packageSources, "add")
        addSource.set("key", name)
        addSource.set("value", repo)
        if username and password:
            credential = ET.SubElement(packageSourceCredentials, name)
            addUsername = ET.SubElement(credential, "add")
            addUsername.set("key", 'Username')
            addUsername.set("value", username)
            addPassword = ET.SubElement(credential, "add")
            addPassword.set("key", 'ClearTextPassword')
            addPassword.set("value", password)
    ET.ElementTree(configuration).write(config, "utf-8", True)

def create_symlink(src, dest):
    print('Creating {dest} -> {src}'.format(src=src, dest=dest))
    if os.path.exists(dest):
        if os.path.samefile(dest, src):
            return
        else:
            os.remove(dest)
    os.symlink(src, dest)

def create_package_map_from_projectLock(projectLock, package_map):
    projectLockContent = json.load(open(projectLock,"r"))
    for id, contents in projectLockContent['libraries'].items():
        unqualifiedId = id.split('/')[0].split('_')[0]
        package_map[unqualifiedId] = dict(path=id, sha512=contents['sha512'])

def generate_symlinks(package_map, packagesDir, refsDir):
    """generate unqualified symbolic links for easy referencing"""
    if not os.path.exists(refsDir):
        os.makedirs(refsDir)
    for unqualifiedId, desc in package_map.items():
        destDir = os.path.join(refsDir, unqualifiedId)
        srcDir = os.path.join(packagesDir, desc['path'])
        create_symlink(srcDir, destDir)

def build_os():
    buildplatforms = dict(Windows='win', Darwin='osx', Linux='linux')
    return buildplatforms[platform.system()]

def filesafe_encode(url_segment):
    """Parse the input string and replace all characters that are invalid characters in a path"""
    #we are very conservative and restrict the allowed
    #characters to the ones that we know work across
    #os and filesystems.
    regex = re.compile('[^0-9a-zA-Z\-\.]')
    def repl(match):
        return '_'
    return regex.sub(repl,url_segment)

def handle_nuget(nuget, server, qualifiers, packages_dir, nuget_refs, package_map, username, password, framework):
    #1. generate project.json
    print('Generating project references...')
    tmpdir = tempfile.mkdtemp()
    try:
        projectJson = os.path.join(tmpdir, 'project.json')
        if os.path.exists(projectJson):
            os.remove(projectJson)
        generate_nuget_project_json(nuget_refs['references'],projectJson, qualifiers, framework)
        if not os.path.exists(projectJson):
            raise IOError(projectJson)

        # 2. generate config file with sources, usernames, passwords
        config = os.path.join(tmpdir, 'aget.config')
        if os.path.exists(config):
            os.remove(config)
        print('server: '+ str(server))
        if server == 'https://www.nuget.org/api/v2':
        	repos = [server]
        else:
        	repos = [server + '/artifactory/api/nuget/' + repo for repo in nuget_refs['repos']]

        generate_nuget_config(repos,config, username, password)
        if not os.path.exists(config):
            raise IOError(config)

        #3. call nuget restore <project.json> -Source <artifactory> -PackagesDirectory <dir> to download the references
        print('Executing nuget.exe to restore packages...')
        cmdLine = [nuget, 'restore', projectJson, '-NoCache', "-NonInteractive", '-PackagesDirectory',  packages_dir, '-ConfigFile', config]
        if platform.system() != 'Windows':
            cmdLine = ["mono"] + cmdLine #.NET code must use mono
        sys.stdout.flush()
        if subprocess.call(cmdLine):
            print("fatal error: Nuget returned a non-zero return code.")
        #see if we got lock file if so rename it.
        projectLockJson = os.path.join(tmpdir, 'project.lock.json')
        if not os.path.exists(projectLockJson):
            raise IOError(projectLockJson)   
        #4. read .lock file to determine what symbolic links we should create
        create_package_map_from_projectLock(projectLockJson, package_map)
    finally:
        shutil.rmtree(tmpdir)

def calc_sha512(file):
    BLOCKSIZE = 65536
    hasher = hashlib.sha512()
    with open(file, 'rb') as afile:
        buf = afile.read(BLOCKSIZE)
        while len(buf) > 0:
            hasher.update(buf)
            buf = afile.read(BLOCKSIZE)
    return hasher.hexdigest()

def urlretrieve_with_basic_auth(url, filename, username, password):
    class OpenerWithAuth(url_lib().FancyURLopener):
        def prompt_user_passwd(self, host, realm):
            return username, password
    return OpenerWithAuth().retrieve(url, filename)

def unzip(zipPath, folder):
    #extractall does not preserve the permissions https://bugs.python.org/issue15795
    #this matters on non-windows platforms
    print('Unzipping to {folder}...'.format(folder=folder))
    if platform.system() == 'Windows':
        zipfile.ZipFile(zipPath).extractall(folder)
    else:
        cmdLine = ['unzip', zipPath, '-d', folder]
        print(' '.join(cmdLine))
        sys.stdout.flush()
        if subprocess.call(cmdLine):
            raise Exception("fatal error: unzip returned non-zero error code.")

def download_package(server, url_segment, root_folder, username, password):
    parent_folder = filesafe_encode(url_segment)
    folder = os.path.join(root_folder, parent_folder)
    if not os.path.exists(folder):
        if not os.path.exists(folder):
            os.makedirs(folder)
        zipPath = os.path.join(folder,"package.zip")
        try:
            try:
                url = server + url_segment
                print('Downloading from {url}...'.format(url=url))
                urlretrieve_with_basic_auth(url, zipPath, username, password)
            except:
                print('fatal error: Download from {} failed.\n'.format(url))
                shutil.rmtree(folder)
            hash = calc_sha512(zipPath)
            f = open(os.path.join(folder, ".sha512"), "w")
            f.write(hash)
            f.close()
            try:
                unzip(zipPath, folder)
            except:
                print('fatal error: Unzip of {} failed.\n'.format(zipPath))
                shutil.rmtree(folder)
        finally:
            if os.path.exists(zipPath):
                os.remove(zipPath)
    f = open(os.path.join(folder, ".sha512"), "r")
    hash = f.read()
    return dict(dir = parent_folder, sha512 = hash)

def find_onlychild_subdirs(dir, path):
    """Recursively find subdirs that have no siblings"""
    subdirs = [ name for name in os.listdir(dir) if os.path.isdir(os.path.join(dir, name)) ]
    if len(subdirs)==1:
        path = os.path.join(path, subdirs[0])
        return find_onlychild_subdirs(os.path.join(dir, subdirs[0]),path)
    return path

def handle_generic(server, qualifiers, packages_dir, generic_refs, package_map, username, password):
    print('Installing generic packages...')
    deps = dict()
    for id, repo_path in generic_refs['references'].items():
        id = remove_conditionals(id, qualifiers)
        if (id != None):
            qid = id.format(**qualifiers)
            if server == 'https://www.nuget.org/api/v2':
            	package = download_package(server, repo_path, os.path.join(packages_dir,qid))
            else:
            	package = download_package(server + '/artifactory/', repo_path, os.path.join(packages_dir,qid), username, password)
            parent_folder = package['dir']
            #the package name is everything up to the first '_'
            name = qid.split('_')[0]
            #see if there's only a single subfolder under the package folder and skip over it
            linear_path = find_onlychild_subdirs(os.path.join(packages_dir, qid, parent_folder),'')
            package_map[name] = dict(path=os.path.join(qid, parent_folder, linear_path), sha512=package['sha512'])

class Choices:
    """Provides case insenstive choices"""
    def __init__(self, list):
        self.data = list
    def __iter__(self):
        return self.data.__iter__()
    def __contains__(self, item):
        lc = item.lower()
        return lc in self.data

servers = dict(
    santaclara='https://art-bobcat.autodesk.com',
    novi='https://art-cougar.autodesk.com',
    singapore='https://art-lion.autodesk.com',
    shanghai='https://art-panda.autodesk.com',
    neuchatel='https://art-chamois.autodesk.com',
    nugetOnline = 'https://www.nuget.org/api/v2'
)

choices = dict(
    os = ['win', 'osx', 'uwp', 'ios', 'android', 'web', 'linux'],
    config = ['debug', 'release'],
    iset = ['intel32', 'intel64', 'arm32', 'arm64', 'asmjs32'],
    toolchain = ['v140', 'clang', 'gcc'],
    linkage = ['shared', 'static']
)

#used by some clients to validate their qualifiers
def validate_qualifiers(target):
    for key in choices.keys():
        if not target[key] in choices[key]:
            raise Exception('Invalid qualifier {value} for {qualifier}'.format(value = target[key], qualifier=key))

#sample command line:
# -os win -config Debug -iset intel64 -toolchain v140 -linkage shared -agettable sample.aget -refsDir e:\packaging\refs -packagesDir e:\packages -server santaclara -framework net46
def main():
    #we require python 3.x on Windows because symlink support does not exist in earlier versions
    if platform.system() == 'Windows' and ispython2x():
        raise ValueError('This script requires python 3.x on Windows.')
    parser = argparse.ArgumentParser(prog='aget', description='Downloads package references.')
    parser.add_argument('-os', choices=Choices(choices['os']), required=True)
    parser.add_argument('-config', choices=Choices(choices['config']), required=True)
    parser.add_argument('-iset', choices=Choices(choices['iset']), required=True)
    parser.add_argument('-toolchain', choices=Choices(choices['toolchain']), required=True)
    parser.add_argument('-linkage', choices=Choices(choices['linkage']), required=True)
    parser.add_argument('-agettable', metavar='file', required=True, help="Source .aget file to process.")
    parser.add_argument('-packagesDir', metavar='dir', required=True, help="Directory where packages will be downloaded to.")
    parser.add_argument('-refsDir', metavar='dir', required=True, help="Directory where unqualified symbolic links will be be created for easy referencing during the build.")
    parser.add_argument('-nuget', metavar='file', help="Full path to nuget.exe.", default="nuget.exe")
    parser.add_argument('-framework', metavar='string', help="Framework version (native, net4, net45, net46...)", default="native")
    server_group = parser.add_mutually_exclusive_group(required=True)
    server_group.add_argument('-server', choices=Choices(['santaclara','novi','singapore','shanghai','neuchatel','nugetOnline']), help="Artifactory server location.")
    server_group.add_argument('-serverURL', metavar='url', help="Artifactory server URL.")
    args = parser.parse_args()

    qualifiers = dict(os = args.os.lower(), build_os = build_os(), config = args.config.lower(), iset = args.iset.lower(), toolchain=args.toolchain.lower(), linkage=args.linkage.lower())
    if not os.path.exists(args.packagesDir):
        os.makedirs(args.packagesDir)

    from json_minify import json_minify
    minified = json_minify(open(args.agettable, "r").read())
    aget = json.loads(minified)

    #the top level nodes 'generic' and 'nuget' are optional, when
    #missing we assume that the whole object is 'nuget'.
    nuget_refs = None
    generic_refs = None
    if 'nuget' in aget:
        nuget_refs = aget['nuget']
    if 'generic' in aget:
        generic_refs = aget['generic']
    #try legacy default
    if nuget_refs == None and generic_refs == None:
        if 'references' in aget:
            nuget_refs = aget
        else:
            raise Exception('fatal error: No references in aget file')

    username = None
    if 'username' in aget:
        username = aget['username']
    password = None
    if 'password' in aget:
        password = aget['password']

    #read the server url from the args settings (server and serverURL)
    server_url = servers[args.server] if args.server else args.serverURL

    package_map = dict()

    if not nuget_refs == None:
        handle_nuget(args.nuget, server_url, qualifiers, args.packagesDir, nuget_refs, package_map, username, password, args.framework)
    if not generic_refs == None:
        handle_generic(server_url, qualifiers, args.packagesDir, generic_refs,package_map, username, password)

    #3. read .lock file to determine what symbolic links we should create
    print("Generating symlinks...")
    generate_symlinks(package_map, args.packagesDir, args.refsDir)
    #make generate versionless symlinks for nuget, python (the one currently executing) and
    #aget itself.
    create_symlink(os.path.dirname(args.nuget), os.path.join(args.refsDir, "nuget"))
    create_symlink(os.path.dirname(sys.executable), os.path.join(args.refsDir, "python"))
    create_symlink(os.path.dirname(__file__), os.path.join(args.refsDir, "aget"))

    #lockFile = format_with_qualifiers(dict(qualifiers, name=os.path.splitext(args.agettable)[0]), '{name}_on_{build_os}_targetting_{os}_{config}_{iset}_{toolchain}_{linkage}')
    lockFile = '{name}_on_{build_os}_targetting_{os}_{config}_{iset}_{toolchain}_{linkage}'.format(**dict(qualifiers, name= os.path.splitext(args.agettable)[0])) + '.lock'

    json.dump(package_map, open(lockFile, "w"),indent=True)

    print("Reference files are in {refs}".format(refs = args.refsDir))
    print("Packages are in {packages}".format(packages = args.packagesDir))


#sample command line:
# -os win -config Debug -iset intel64 -toolchain v140 -linkage shared -agettable sample.aget -refsDir e:\packaging\refs -packagesDir e:\packages -server santaclara -framework net46
if __name__ == "__main__":
    main()
