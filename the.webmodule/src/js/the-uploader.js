/**
 * Created by comdiv on 20.05.2015.
 */
define(["the-root", "the-angular"], function (the) {
    var defaultuploadurl = '/accident/upload';
    the.uploader = {
        uploadsingle : function(file, success, error, url){
         //   console.trace('here 8');
            var max = 5242880;
            var maxtext = "5Мб";
            if(file.type.match(/video/)){
                max = 26214400;
                maxtext = "25Мб";
            }
            if (file.size > max) {
                the.dialog.alert("Недопустимый файл - размер более "+file.name+" "+maxtext, "Невозможно подгрузить файл");
                return false;
            }

            var fd = new FormData();
            fd.append('file', file);
            $.ajax({
                url: url||defaultuploadurl,
                data: fd,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (data) {
                    if (!!success) {
                        success(data);
                    }
                },
                error: function (x, s, e) {
                    if (!!error) {
                        error(e, s, x);
                    }
                }
            });
        },
        upload: function (dt,e,progress, callback) {
           // console.trace('here 40');
          //  console.trace([dt,e,progress,callback]);
            var files = null;
            var input = $(e);
            files = $(dt||e)[0].files;

            var self = this;
            var progress = progress || $(e).prevAll('i') || $(e);

            progress.addClass('button progress primary');
            var idx = -1;
            var errors = [];
            var results = [];
            var emit = function(){
               // console.log(input);
                input.replaceWith(input.val('').clone(true));
                if(!!callback){
                    callback(results);
                }
                errors.forEach(function(_){
                   if(!!callback){
                       callback(null, _.e, _.s, _.x);
                   }
                });
            }
            var call = function(){
                idx++;
                if(idx>=files.length){
                    progress.removeClass('button progress primary');
                    emit();
                    return;
                }

                var file = files[idx];
              //  console.log(file.name,"enter");

               var result = self.uploadsingle(file,
                    function(data){
                  //      console.log(file.name,"upload success");
                        results.push(data);
                        call();
                    },
                    function(e,s,x){
                        console.error(file.name,"upload error",e, x.responseText);
                        errors.push({x:x,s:s,e:e});
                        call();
                    }
                )
                if(result===false){
                    call();
                }
            }
            call();

        }
    };
    if (null == the.modules)return;
    the.modules.all.directive("uploader", function () {
        return {
            templateUrl: "views/the/uploader.html",
            scope: true,
            link: function (scope, e, attr) {
                scope.__dragenter = function(e, ev){
                   // console.log('enter');
                   $(e).addClass("dragging");
                    return true;
                };

                scope.__dragover = function(e, ev){
                  // console.log('over');
                    return false;
                };
                scope.__dragleave = function(e,ev){
                 //   console.log(ev);
                 //   console.log('leave');
                    $(e).removeClass("dragging");
                    return false;
                };
                scope.__drop = function(e, ev){
                  //  console.trace("here!!! Firefox!!!")
                    $(e).removeClass("dragging");
                  //  console.log('drop',ev.dataTransfer);
                    ev.preventDefault();
                    ev.stopPropagation();
                    scope.__uploaddrop(ev.dataTransfer, $(e));
                    return false;
                };

                scope.__uploaddrop = function(dt,e){
                    if(!!attr["onfiles"]){
                        scope.$eval(attr["onfiles"],{$files:dt.files});
                    }
                    if(!!attr["noupload"]){
                        return;
                    }
                    the.uploader.upload(dt, null, e, function (data, e, s, x) {
                        if (!data) {
                            console.error("upload error", e, s, x.responseText);
                            if (attr["onuploaderror"]) {
                                scope.$tryApply(function () {
                                    scope.$eval(attr["onuploaderror"], {"error": e, "status": s, "xhr": x});
                                });
                            } else {
                                the.dialog.alert("Если ошибка Вам нeпонятна, передайте сообщение в службу поддержки:</br>Статус: " + s + "<br/>Ошибка: " + e + "<br/>Текст: " + x.responseText, "Ошибка при загрузке файла");
                            }
                        } else {
                            scope.$tryApply(function () {
                                if (attr["onupload"]) {
                                    scope.$eval(attr["onupload"], {"$result": data});
                                }
                            });
                        }
                    },attr["uploadurl"]);
                };

                scope.__upload = function (el) {
                    if(!!attr["onfiles"]){
                        scope.$eval(attr["onfiles"],{$files:$(el)[0].files});
                    }
                    if(!!attr["noupload"]){
                        return;
                    }
                    the.uploader.upload(null,el,null, function (data, e, s, x) {
                        if (!data) {
                            console.error("upload error", e, s, x.responseText);
                            if (attr["onuploaderror"]) {
                                scope.$tryApply(function () {
                                    scope.$eval(attr["onuploaderror"], {"error": e, "status": s, "xhr": x});
                                });
                            } else {
                                the.dialog.alert("Если ошибка Вам нeпонятна, передайте сообщение в службу поддержки:</br>Статус: " + s + "<br/>Ошибка: " + e + "<br/>Текст: " + x.responseText, "Ошибка при загрузке файла");
                            }
                        } else {
                            scope.$tryApply(function () {
                                if (attr["onupload"]) {
                                    scope.$eval(attr["onupload"], {"$result": data});
                                }
                            });
                        }
                    });
                }
            }
        }
    })
    return the;

});