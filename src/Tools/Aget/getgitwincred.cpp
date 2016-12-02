#include <windows.h>
#include <wincred.h>
#include <tchar.h>
#include <stdio.h>

//Compile as follows:
//cl /O2 getgitwincred.cpp
void usage()
{
    wprintf(L"getgitwincred 1.0.2\n");
    wprintf(L"Reads the Windows credential cache for the given target and prints the credential blob to stdout.\n");
    wprintf(L"Source: http://git.autodesk.com/AutoCAD/aget/getgitwincred.cpp\n");
    wprintf(L"USAGE:\ngetgitwincred <target>\n");
    wprintf(L"E.G.:\ngetgitwincred git:https://szilvaa@git.autodesk.com\n");
}
int wmain(int argc, wchar_t *argv[])
{
    if (argc != 2)
    {
        usage();
        return 1;
    }
    PCREDENTIALW pcred;
    BOOL ok = ::CredReadW (argv[1], CRED_TYPE_GENERIC, 0, &pcred);
    if (!ok)
        return 1;
    //allocate enough space so that we can escape characters (up to 3 chars) and have 
    //room for a null terminator
    wchar_t* orig = (wchar_t*)pcred->CredentialBlob;
    wchar_t* buffer = (wchar_t*)malloc(pcred->CredentialBlobSize+2);
    int i = 0;
    for (;i < pcred->CredentialBlobSize / 2;i++)
        buffer[i] = orig[i];
    //add null terminator
    buffer[i] = 0;
    wprintf(buffer);
    ::CredFree (pcred);
    return 0;
}