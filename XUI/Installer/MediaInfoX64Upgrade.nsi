!define PRODUCT_NAME "Album Art Downloader XUI MediaInfo x64 upgrade"
!define PRODUCT_WEB_SITE "https://sourceforge.net/projects/album-art"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\AlbumArt.exe"

!include x64.nsh

SetCompressor lzma

Name "${PRODUCT_NAME}"
OutFile "../../Releases/AlbumArtDownloaderXUI-MediaInfoX64Upgrade.exe"

Icon "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
InstallDir "$PROGRAMFILES\AlbumArtDownloader"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
DirText "Please choose the folder where Album Art Downloader XUI is installed. Installation can only proceed when Album Art Downloader XUI has been detected."
ShowInstDetails hide
XPStyle on

Page directory
Page instfiles

Function .onInit
  ${IfNot} ${RunningX64}
    MessageBox MB_ICONEXCLAMATION|MB_OK "64 bit Windows not detected.$\n$\nThe MediaInfo x64 Upgrade is only required when running under 64 bit windows, and the installer will now exit."
    Abort
  ${EndIf}
FunctionEnd

Function .onVerifyInstDir
  IfFileExists "$INSTDIR\AlbumArt.exe" Good
    Abort
  Good:
FunctionEnd

Section
  SetOutPath -
  SetOverwrite on
  File /oname=MediaInfo.dll "..\Third Party Assemblies\MediaInfo.dll.x64"
  File "..\Third Party Assemblies\MediaInfo.Readme.txt"
SectionEnd