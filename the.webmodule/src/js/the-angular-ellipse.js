/**
 * Created by comdiv on 04.11.2014.
 */
    define(["the-angular"], function ($the) {
        return $the(function(root, privates)
        {
            if(null==root.modules)return;
            var ellipsetext = function(text,size){
                text = text.substring(0,size);
                var lastspace = text.lastIndexOf(' ');
                return text.substring(0,lastspace)+"...";
            }
            var ellipsehtml = function(html,size){
                var buffer = "";
                var e = $(html);
                var text = e.text();
                return ellipsetext(text,size);
            }
            var ellipse = [function() {
                return function (val,size) {
                    size = size || 100;
                    if(val.length<=size)return val;

                    if(val.match(/^</)){
                        return ellipsehtml(val,size);
                    }else{
                        return ellipsetext(val,size);
                    }
                };
            }];
               root.modules.all.filter('ellipse', ellipse);


        });
    });