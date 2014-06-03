(function () {
    require.config({
        shim: {
            'jquery': { 'exports': '$' },
            'bootstrap': { 'deps': ['jquery', 'moment'] },
            'angular': { 'exports': 'angular' }
        }
    });
    require(['angular', 'jquery', 'bootstrap', 'moment', '__APPNAME__-root'], function (angular, $) {
        $(function () {
            $('body').html('<div id="extraHelp" style="position: absolute;top: 0;right: 100px;padding:15px;font-size: 25px;background-color: red;color: white;"></div><div ng-controller="root"><ng-include src="layout"/></div>');
            angular.bootstrap(window.document, ["app"]);
        });
    });
})();