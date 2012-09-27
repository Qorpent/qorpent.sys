set sys=%~n0
if '%sys%' == 'sys-syncall' (
	cd ..
) 


for /D %%1 in (*) do (
	cd %%1
	git pull
	cd ..
)


for /D %%1 in (*) do (
	cd %%1
	git add --all
	git commit -m %1
	cd ..
)



for /D %%1 in (*) do (
	cd %%1
	git push --all
	cd ..
)

if '%sys%' == 'sys-syncall' (
	cd qorpent.sys
) 