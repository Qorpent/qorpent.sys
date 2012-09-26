for /D %%1 in (*) do (
	cd %%1
	git pull
	cd ..
)


for /D %%1 in (*) do (
	cd %%1
	git add --all
	git commit -a
	cd ..
)

if '%1' == 'push' (

for /D %%1 in (*) do (
	cd %%1
	git push --all
	cd ..
)


)