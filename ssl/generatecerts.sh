openssl genrsa -out qorpentCA.key 2048
openssl req -x509 -new -nodes -key qorpentCA.key -days 1024 -out qorpentCA.pem   -subj "/C=RU/ST=SV/L=EK/O=BIT/OU=IT_Department/CN=qorpent.myjetbrains.com"  -config "../../../bin/ssl/share/openssl.cnf"