@taskkill /im mesen.exe

@..\..\Tools\msxsl.exe %USERPROFILE%\Documents\Mesen\Debugger\rom_ntsc.Workspace.xml ..\..\..\NES\tools\bin\cleandebug.xml -o %USERPROFILE%\Documents\Mesen\Debugger\rom_ntsc.Workspace.xml

@findstr /V "zeropage.*type=equ @LOCAL-MACRO_SYMBOL" rom_mmc5_ntsc.dbg > rom_mmc5_ntsc.dbg.new
@del rom_mmc5_ntsc.dbg
@ren rom_mmc5_ntsc.dbg.new rom_mmc5_ntsc.dbg

@del %USERPROFILE%\Documents\Mesen\Debugger\rom_ntsc.cdl
@del %USERPROFILE%\Documents\Mesen\RecentGames\rom_ntsc.*

PackFds fds.fds fdstoc.bin fdsdata.bin fds_patched.fds
copy fds.dbg fds_patched.dbg
rem ..\..\..\NES\tools\bin\Mesen.exe fds_patched.fds

 ..\..\Tools\Mesen.exe rom_mmc5_ntsc.nes
