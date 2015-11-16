
define(["the-root"], function ($the, $m) {
    return $the(function (root, privates) {
        root.biz = {
            toFio : function(fio){
                if (!fio)return fio;
                var result = "";
                if (fio.match(/^\s*[А-Я]+\s+[А-Я]+\s+[А-Я]+\s*$/i)) {
                    result = fio.replace(/^\s*([А-Я]+)\s+([А-Я])[А-Я]+\s+([А-Я])[А-Я]+\s*$/i, "$1 $2.$3.")
                }else if (fio.match(/^\s*[А-Я]+\s+[А-Я]+\s*$/i)) {
                    result = fio.replace(/^\s*([А-Я]+)\s+([А-Я])[А-Я]+\s*$/i, "$1 $2.")
                }
                return result;
            },
            translitTable : {
                'а': 'a',
                'б': 'b',
                'в': 'v',
                'г': 'g',
                'д': 'd',
                'е': 'e',
                'ё': 'e',
                'ж': 'zh',
                'з': 'z',
                'и': 'i',
                'й': 'y',
                'к': 'k',
                'л': 'l',
                'м': 'm',
                'н': 'n',
                'о': 'o',
                'п': 'p',
                'р': 'r',
                'с': 's',
                'т': 't',
                'у': 'u',
                'ф': 'f',
                'х': 'h',
                'ц': 'ts',
                'ч': 'ch',
                'ш': 'sh',
                'щ': 'sh',
                'ъ': '',
                'ы': 'i',
                'ь': '',
                'э': 'e',
                'ю': 'yu',
                'я': 'ya'
            },
            translit : function(s){
                if(!s)return;
                var r = "";
                for(i=0;i< s.length;i++){
                    var c = s[i];
                    var uc = c.toUpperCase()==c;
                    var ic = c;
                    if(uc)ic = c.toLowerCase();

                    if(this.translitTable.hasOwnProperty(ic)){
                        var rc = this.translitTable[ic];
                        if(uc){
                            if(rc.length==1) {
                                rc = rc.toUpperCase();
                            }else{
                                rc = rc[0].toUpperCase() + rc.substring(1);
                            }
                        }
                        r+=rc;
                    }else{
                        r+=c;
                    }
                }
                return r;
            },
            orgSysName : function(s){
                if(!s){
                    return "";
                }
                s = s.replace(/^\s*(ООО)|(ОАО)|(ПО)|(ТС)|(ЗАО)|(ИП)\s+/,"");
                s = s.replace(/["'\-]/g,"");
                s = s.trim().toLowerCase();
                s = s.replace(/\s+/g," ");
                var ss = s.split(/ /);
                if(ss.length <=2){
                    s = s.replace(/ /g,'_');
                    return this.translit(s);
                }else{
                    var r = ss[0]+"_";
                    for(var i=1;i<ss.length;i++){
                        r+=ss[i][0];
                    }
                    return this.translit(r);
                }
            }
        }
    });
});