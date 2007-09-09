@echo off
set version=0.9

set zipfile=..\..\Releases\AlbumArtDownloaderXUI-%version%.zip
del %zipfile%
"%ProgramFiles%\7-Zip\7z.exe" a -tzip %zipfile% @ziplist.txt -x!*\.svn
set version=
set zipfile=