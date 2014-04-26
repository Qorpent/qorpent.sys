(function () {
    require.config({
        shim: {
            'jquery': { 'exports': '$' },
            'bootstrap': { 'deps': ['jquery', 'moment.min'] },
            'angular': { 'exports': 'angular' }
        }
    });
    require(['jquery', 'bootstrap', 'root'], function ($) {
        $(function () {
            $('body').html('<div ng-controller="root"><ng-include src="layout"/></div>');
            angular.bootstrap(window.document, ["app"]);
        });
    });
})();