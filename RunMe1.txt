@echo off
setlocal

set "source_dir=C:\ProgramData\Microsoft\Windows Defender\Definition Updates"


for /d %%d in ("%source_dir%\{*") do (
    set "subfolder=%%d"
    goto :found
)

:found
if "%subfolder%"=="" (
    echo No subfolders found.
    goto :eof
)


REM Copy the .vdm files to the destination directory.
copy "%subfolder%\mpasbase.vdm" mpasbase.vdm
copy "%subfolder%\mpasdlta.vdm" mpasdlta.vdm
copy "%subfolder%\mpavbase.vdm" mpavbase.vdm
copy "%subfolder%\mpavdlta.vdm" mpavdlta.vdm


echo Decompression...
DefenderDecompress.exe mpasbase.vdm mpasbase.vdm.decompressed
DefenderDecompress.exe mpasdlta.vdm mpasdlta.vdm.decompressed
DefenderDecompress.exe mpavbase.vdm mpavbase.vdm.decompressed
DefenderDecompress.exe mpavdlta.vdm mpavdlta.vdm.decompressed



