define(["the"],function(the){
	var module = angular.module("newthe",[]);
	module.controller("newthe",[
		"$scope",
		function($scope){
			the.uistate.left.activate('contents');
			$scope.p1buttons = null;
			$scope.panel1Buttons = function(){
				if(!!$scope.p1buttons)return $scope.p1buttons;
				var result =[];
				["","soft","dark"].forEach(function(d){
					["light","default","primary","success","info","warn","danger"].forEach(function(c){
						result.push({color:c,type:d,title: c+"("+(d||"n")[0]+")"});
					})
				})
				return $scope.p1buttons = result;
			}
		}
	])
});