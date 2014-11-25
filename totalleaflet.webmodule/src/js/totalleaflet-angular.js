/**
 * Created by comdiv on 15.11.2014.
 */
define(["leaflet-amd"],function(L){
    if(typeof angular === "undefined"){
        console.warn("angular required for totalleaflet-angular");
        return null;
    }
    var mod = angular.module("totalleaflet",[]);
    mod.directive("lmap",[function(){
        return {
            priority:100,
            replace:true,
            link : 	function($scope, element, iAttrs){
                //require timeout to have time to match real flex size
                var mapConfig = {
                    center : [iAttrs["lon"]||0, iAttrs["lat"]||lat],
                    zoom : iAttrs["zoom"]||13,
                    minZoom : iAttrs["minzoom"]||1,
                    maxZoom : iAttrs["maxzoom"]||18
                };
                if (!!iAttrs["bounds"]){
                    var coords = iAttrs["bounds"].match(/[\d\.]+/g);
                    if(coords.length==4) {
                        mapConfig.maxBounds = new L.LatLngBounds(new L.LatLng(coords[0],coords[1]), new L.LatLng(coords[2],coords[3]));
                    }else{
                        console.error("Invalid bounds specification "+iAttrs["bounds"]);
                    }
                }
                if("zoomcontrol" in iAttrs){
                    mapConfig.zoomControl = iAttrs["zoomcontrol"]!=="false";
                }
                var map = L.map(element[0],mapConfig);

                if("tiles" in iAttrs) {
                    var url = iAttrs["tiles"].replace(/\{hostname\}/, document.location.hostname);
                    L.tileLayer(url, {
                        reuseTiles: true,
                        updateWhenIdle: false
                    }).addTo(map);
                }

                if(!!iAttrs["onload"]) {
                    if (iAttrs["onload"] in $scope) {
                        $scope[iAttrs["onload"]](map, element, iAttrs);
                    }
                }
            }
        }
    }]);
    return mod;
});