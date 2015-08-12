define(["the-object"], function ($the) {
    return $the(function (root, privates) {
        var hash = root.hash = (root.hash || {});
        hash.get=function(name, userhash, decode){
            if(typeof(userhash)=="undefined")
            {
                decode = true;
                userhash = document.location.hash;
            }
            userhash = userhash || document.location.search || "";
            if(decode){
                name = encodeURIComponent(name);
            }
            var searchName = name.replace(/\./g,"\\.");
            if(decode){
                searchName = searchName.replace(/%20/g,'(\\+|%20)')
            }
            var regex = new RegExp("[/&\\#\\?]" + searchName + "=([^&]*)","i");
            var match = userhash.match(regex);
            if (match){
                var val = match[match.length-1];
                if(decode){
                    return decodeURIComponent(val.replace(/\+/g,' '));
                }
                return val;
            }
            return "";
        };

        hash.setFlag = function(name,value,userhash,encode){
            if(!!value){
                value= 1;
            }else{
                value = null;
            }
                return this.set(name,value,userhash,encode);
        }

        hash.set=function(name,value,userhash,encode){

            var setlocation = false;
            if(typeof userhash == "undefined"){
                userhash = document.location.hash;
                encode = true;
                setlocation = true;
            }

            var remove = (null==value || typeof(value)=="undefined");
            name= name || "__value";
            userhash = userhash || "";
            value = value || "";
            if(encode){
                value = encodeURIComponent(value);
                name = encodeURIComponent(name);
            }
            value= (remove ? "": ("&"+name+"="+value));

            var searchName = name.replace(/\./g,"\\.");
            if(encode){
                searchName = searchName.replace(/%20/g,'(\\+|%20)')
            }
            var regex = new RegExp("&" + searchName + "=([^&]*)","i");
            if(userhash.match(regex)){
                userhash = userhash.replace(regex,value.replace("$","$$"));
            }else{
                userhash+=value;
            }


            if(setlocation){
                if(userhash.length==0)userhash = "#";
                if(userhash[0]!="#")userhash = "#"+userhash;
                if(!document.location.hash){
                    document.location.href+=userhash;
                }else{

                    document.location.href =
                        document.location.href.match(/^([^#]+)/)[0]+userhash;
                }
            }
            return userhash;
        };
    });
});