define(['angular','bxlui_types','bxlui_api','bxlui_controllers','layout'],function(angular,types,apictor){
	angular.module('app',['bxlui_controllers','Layout'])
		.controller('root',function($scope,$http){
			$scope.api = apictor($http);
			$scope.layout = 'bxlui_dev.html';
		});
	}
);
