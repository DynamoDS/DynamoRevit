#Overview

This is a simple package downloader for AutoCAD. 

```
usage: aget [-h] -os {win,osx,uwp,ios,android,web,linux} -config {debug,release}
            -iset {intel32,intel64,arm32,arm64,asmjs32} -toolchain
            {v140,clang,gcc} -linkage {shared,static} -agettable file -packagesDir
            dir -refsDir dir -server
            [-framework string]
            (-server {santaclara,novi,singapore,shanghai,neuchatel} | -serverURL url)

Downloads package references.

optional arguments:
  -h, --help            show this help message and exit
  -os {win,osx,uwp,ios,android,web,linux}
  -config {debug,release}
  -iset {intel32,intel64,arm32,arm64,asmjs32}
  -toolchain {v140,clang,gcc}
  -linkage {shared,static}
  -agettable file       Source .aget file to process.
  -packagesDir dir      Directory where packages will be downloaded to.
  -refsDir dir          Directory where unqualified symbolic links will be be
                        created for easy referencing during the build.
  -nuget file           Full path to nuget.exe.
  -framework string     Framework version (native (default), net4, net45, net46...)
  -server {santaclara,novi,singapore,shanghai,neuchatel}
                        Artifactory server location.
  -serverURL url        Artifactory server URL.
```
  
For example (using sample.aget in this project):

`python aget.py -os win -config debug -iset intel64 -toolchain v140 -linkage shared -agettable sample.aget -refsDir e:\refs -packagesDir e:\packages -server santaclara`

Note that specifying the `framework` other than `native` will continue to allow downloading native packages but it will also enable you to download .NET packages compatible with the framework you specify. In other words, if you have mix of native and .net packages in your aget file then you should set `framework` with non-default argument.

#Syntax used in .aget files

See comments in [sample.aget] (sample.aget).

#How it works

aget.py is a wrapper around nuget. It takes 4 inputs:
  1. .aget file that contains package references
  2. Target qualifiers (os, config, toolchain etc.)
  3. Packages directory where packages will be downloaded
  4. References directory where symbolic links will be created that have no version and target qualifiers so that they can be easily referenced during build.

aget.py generates a [project.json] (https://docs.nuget.org/Consume/ProjectJson-Intro) file and calls `nuget restore` it then stores [project.lock.json] (https://docs.nuget.org/consume/projectjson-format) file that nuget generates in the same folder where the .aget file is located and qualifies it with all the target qualifiers (e.g. `sample_win_debug_intel64_v140_shared.lock`). aget.py also generates symbolic links in the references directory that have no version and target qualifiers so that they stable for build scripts to reference.

#Status
Only the basics are implemented.The following are still yet to come:

1. Support 'build' and 'author' mode. Build mode should compare before and after state of lock file and error when there are differences.
2. Support automatic checkout of the lock file in 'author' mode.
