/*QPT:::AUTOGENERATED*/define(['bxlui_api','bxlui_types','angular'], function (Api, Types) {
    angular.module('bxlui_controllers',[])
        .controller('bxlui_BxlDev', ['$scope','$http','$rootScope', function ($scope, $http, $rootScope) { 
                $scope.api = Api($http, $rootScope);
                $scope.view = 'BxlDev.html';
				$scope.title= 'BxlDev';

		}])
;
});