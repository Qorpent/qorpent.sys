openssl genrsa -out qorpentCA.key 2048
openssl req -x509 -new -nodes -key qorpentCA.key -days 2048 -out qorpentCA.pem -config "../../../bin/ssl/share/openssl.cnf"  -subj "/C=RU/ST=SV/L=EK/O=BIT/OU=IT Department/CN=qorpent.myjetbrains.com"  
openssl genrsa -out local.key 2048
openssl genrsa -out outer.key 2048
openssl req -new -key local.key -out local.csr -config "../../../bin/ssl/share/openssl.cnf" -subj "/C=RU/ST=SV/L=EK/O=BIT/OU=IT Department/CN=127.0.0.1" 
openssl req -new -key outer.key -out outer.csr -config "../../../bin/ssl/share/openssl.cnf" -subj "/C=RU/ST=SV/L=EK/O=BIT/OU=IT Department/CN=outer.qorpent" 
openssl x509 -req -in local.csr -CA qorpentCA.pem -CAkey qorpentCA.key -CAcreateserial -out local.crt -days 1024
openssl x509 -req -in outer.csr -CA qorpentCA.pem -CAkey qorpentCA.key -CAcreateserial -out local.crt -days 1024
openssl pkcs12 -export -in local.crt -inkey local.key -name local -out local.p12
openssl pkcs12 -export -in outer.crt -inkey outer.key -name outer -out outer.p12

openssl x509 -in local.crt -fingerprint -noout | sed "s/SHA1 Fingerprint=//g" | sed "s/://g" > local.thmb
openssl x509 -in outer.crt -fingerprint -noout | sed "s/SHA1 Fingerprint=//g" | sed "s/://g" > outer.thmb