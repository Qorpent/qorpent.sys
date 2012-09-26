set sys=%~p0
set root=%sys%..\
mklink %root%syncall.bat %sys%sys-syncall.bat
mklink %root%updateimports.bat %sys%sys-updateimports.bat
