for i in {1401..1499}
do
	 # netsh.exe http delete urlacl url="http://*:$(($i * 10))/"
	 # netsh.exe http delete urlacl url="https://*:$(($i * 10 + 1))/"
	 # netsh.exe http delete urlacl url="http://127.0.0.1:$(($i * 10 + 5))/"
	 # netsh.exe http delete urlacl url="https://*:$(($i * 10 + 6))/"

	netsh.exe http add urlacl url="http://*:$(($i * 10))/" user="$1" listen=yes

	netsh.exe http add urlacl url="https://*:$(($i * 10 + 1))/" user="$1" listen=yes
	netsh.exe http add urlacl url="http://127.0.0.1:$(($i * 10 + 5))/" user="$1" listen=yes
	netsh.exe http add urlacl url="https://127.0.0.1:$(($i * 10 + 6))/" user="$1" listen=yes

	# netsh http add sslcert ipport=0.0.0.0:$(($i * 10 + 1)) certhash=1EA5A22BCFB03DBC218229C7F9E199D255A15C89 appid={1EA5A22B-CFB0-3DBC-2182-99D255A$(($i * 10 + 1))} 
	# netsh http add sslcert ipport=127.0.0.1:$(($i * 10 + 6)) certhash=579DFE59D6FD7E029F3B627C4C6B5441968376AD appid={579DFE59-D6FD-7E02-9F3B-627C4C6$(($i * 10 + 6))} 
	
done