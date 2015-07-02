/**
 * Created by comdiv on 20.05.2015.
 */
define(["the-root", "the-angular"], function (the) {
    the.uploader = {
        upload: function (e, callback) {

            var progress = e.previousElementSibling;
            if (!!progress) {
                progress = progress.previousElementSibling;

            }
            if (null != progress) {
                progress = $(progress);
                progress.show();
            }
            var fd = new FormData();
            var file = $(e)[0].files[0];
            if (file.size > 26214400) {
                the.dialog.alert("Недопустимый файл - размер более 25мб", "Невозможно подгрузить файл");
                return;
            }
            fd.append('file', file);
            $.ajax({
                url: '/accident/upload',
                data: fd,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (data) {
                    console.log('success');
                    if (null != progress) {
                        progress.hide();
                    }
                    if (!!callback) {
                        callback(data);
                    }
                },
                error: function (x, s, e) {
                    if (null != progress) {
                        progress.hide();
                    }
                    if (!!callback) {
                        callback(null, e, s, x);
                    }
                }
            });
        }
    };
    if (null == the.modules)return;
    the.modules.all.directive("uploader", function () {
        return {
            templateUrl: "views/the/uploader.html",
            scope: true,
            link: function (scope, e, attr) {
                scope.__upload = function (el) {
                    the.uploader.upload(el, function (data, e, s, x) {
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