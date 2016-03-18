# Подготовка среды разработки

* Должен быть установлен .NET Framework 4.5(+) на Windows или mono-complete (на Linux)
* Создать директорию, например "code"
* Внутри этой директории склонировать [http://gitlab.pressindex.ru/agency/qorpent.kernel] - git clone ssh://git@gitlab.pressindex.ru:22322/agency/qorpent.kernel.git
* Склонировать данный репозиторий git clone ssh://git@gitlab.pressindex.ru:22322/agency/qorpent.sys.git
* cd qorpent.sys
* Для Windows - msbuild build
* Для Linux - xbuild build