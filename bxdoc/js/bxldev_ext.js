define([
    'angular'
], function (angular) {
    angular.module('bxldev_ext',[])
        .factory('bxldev_ext', ['$sce',function ($sce) {
			return function($scope) {
				$scope.result = $sce.trustAsHtml("<div>Наберите исходный код на BXL или B# и нажмите кнопку 'построить' или включите опцию 'автокомиляция'</div>");
				$scope.__compileQueryCounter = 0;
				$scope.checkAutoCompile = function(){
					if ($scope.bxoptions.AutoCompile){
						$scope.__compileQueryCounter++;
						var myId = $scope.__compileQueryCounter;
						window.setTimeout (function(){
							if (myId == $scope.__compileQueryCounter){
								$scope.compile();
							}
						},1000);
					}
				}
				$scope.$watch ( 'bxquery.Script', $scope.checkAutoCompile);
				$scope.$watch ( 'bxoptions.AutoCompile', $scope.checkAutoCompile);
				
				$scope.compile = function(){
					$scope.api.ToXml ( $scope.bxquery, function(r) {
						$scope.result = $sce.trustAsHtml(r);
					});
				};
            }
        }]);
        
});