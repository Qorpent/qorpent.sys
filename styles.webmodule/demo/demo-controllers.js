define(['angular'], function () {
    var module = angular.module('demo-controllers', []);
    module.controller('body-layout-example', ['$scope', function ($scope) {
        $scope.htmlClass = {
            app: false,
            site: true
        };
        $scope.bodyClass = {
            debug: false
        };
        $scope.showLargeText = true;
        $scope.sizefactor = "dynamic";
        $scope.changeHtmlClass = function () {
            var el = document.getElementsByTagName('html')[0];
            var cls = '';
            if ($scope.htmlClass.app) {
                cls += ' app ';
            }
            if ($scope.htmlClass.site) {
                cls += ' site ';
            }
            el.setAttribute('class', cls);
        }
        $scope.changeHtmlClass();
    }]);
});