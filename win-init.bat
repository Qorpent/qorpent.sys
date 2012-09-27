set sys=%~p0
set root=%sys%..\
if "%~n0" == "init" (
	set sys=.\qorpent.sys\
	set root=%sys%\
)
mklink %root%init.bat %sys%win-init.bat
mklink %root%syncall.bat %sys%sys-syncall.bat
mklink %root%installapp %sys%installapp
mklink %root%buildall %sys%buildall