define([
    'angular'
], function (angular) {
    angular.module('ErrorCatcher', [])
        .factory('$exceptionHandler', function () {
            return function errorCatcherHandler(exception, cause) {
                var extraHelp = $('<div id="extraHelp" />');
                $('body').html(extraHelp);
                var email = ('error.assoi@ugmk.com');
                var subject = ('[auto] Что-то пошло не так');
                var body = exception.stack;
                extraHelp.html('<a href="mailto:' + email + '?subject=' + subject + '&body=' + body + '">Что-то пошло не так, нужна помощь!</a><div>'+body+"</div>");
            };
        });
});