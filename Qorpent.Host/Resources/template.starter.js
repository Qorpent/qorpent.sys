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
            $('body').html('<div ng-controller="root"><ng-include src="layout"/></div>');
            angular.bootstrap(window.document, ["app"]);
        });
    });
})();