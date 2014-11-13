/**
 * Created by comdiv on 04.11.2014.
 */
(function (define) {
    define(["./the-angular","leaflet"], function ($the) {

        return $the(function(root, privates)
        {
            var leaflet = [function(){
                return {
                    priority:100,
                    replace:true,
                    link : 	function($scope, element, iAttrs){
                        //require timeout to have time to match real flex size
                        setTimeout(function() {
                            var map = L.map(element[0]).setView([iAttrs["lon"]||0, iAttrs["lat"]||lat], 13);
                            if("tiles" in iAttrs) {
                                var url = iAttrs["tiles"].replace(/\{hostname\}/, document.location.hostname);
                                console.log(url);
                                L.tileLayer(url, {
                                    maxZoom: 18,
                                    reuseTiles: true,
                                    updateWhenIdle: false
                                }).addTo(map);
                            }
                            if("onMapSetup" in $scope){
                                $scope.onMapSetup(map,element,iAttrs);
                            }
                        },4);
                    }
                }
            }];
            root.modules.f_unsafe = root.$angular.module("the-leaflet",[]).filter('lmap',leaflet);
            root.modules.all.filter('lmap', leaflet);
        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));