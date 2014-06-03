(function () {
    require.config({
        shim: {
            'jquery': { 'exports': '$' },
            'bootstrap': { 'deps': ['jquery', 'moment.min'] },
            'angular': { 'exports': 'angular' }
        }
    });
    try {
        require(['jquery', 'bootstrap', 'root'], function($) {
            $(function() {
                $('body').html('<div id="extraHelp" style="position: absolute;top: 0;right: 100px;padding:15px;font-size: 25px;background-color: red;color: white;"></div><div ng-controller="root"><ng-include src="layout"/></div>');
                angular.bootstrap(window.document, ["app"]);
            });
        });
    }
    catch (e) {
        var extraHelp = $('extraHelp');
        var email = ('error.assoi@ugmk.com');
        var subject = ('[auto] Что-то пошло не так');
        var body = e.stack;
        extraHelp.html('<a href="mailto:' + email + '?subject=' + subject + '&body=' + body +'">Что-то пошло не так, нужна помощь!</a>');
    }
})();