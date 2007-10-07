@echo off
set version=0.10.1

set zipfile=..\..\Releases\AlbumArtDownloaderXUI-%version%.zip
del %zipfile%
"%ProgramFiles%\7-Zip\7z.exe" a -tzip %zipfile% @ziplist.txt -x@zipexcludelist.txt
set version=
set zipfile=