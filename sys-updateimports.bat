set file=%~n0
if '%file%' == 'sys-updateimport' (
	cd ..
) 

for /R %%1 in (Qorpent.*\*.export) do (
	for /R %%2 in (Qorpent.*\%%~n1.import) do (
		echo %%2
	)
)


if '%file%' == 'sys-updateimport' (
	cd qorpent.sys
) 