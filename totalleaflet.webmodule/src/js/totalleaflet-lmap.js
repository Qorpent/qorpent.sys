/**
 * Created by comdiv on 05.01.2015.
 */
define([],function(){
    return {
        priority:100,
        replace:true,
        link : 	function($scope, element, iAttrs){
            var root = $scope; while(root.$parent){root=root.$parent};

            var getHomeCoordinates = function(uselocal){
                var hash = document.location.hash.match(/lat=(\d+\.\d+)&lon=(\d+\.\d+)&zoom=(\d+)/);
                if(hash){
                    return {
                        center : new L.latLng(hash[1],hash[2]),
                        zoom: hash[3]
                    }
                }
                if(uselocal) {
                    var local = localStorage.getItem("mapposition");
                    if(!!local){
                        try{
                            return JSON.parse(local);
                        }catch(e){
                            localStorage.removeItem("mapposition");
                        }
                    }
                }
                return {
                    center : [ iAttrs["lat"]||lat , iAttrs["lon"]||0],
                    zoom : iAttrs["zoom"]||13
                };
            }

            var mapConfig = {

                minZoom : iAttrs["minzoom"]||1,
                maxZoom : iAttrs["maxzoom"]||18
            };

            var homeCoordinates = getHomeCoordinates(true);
            mapConfig.center = homeCoordinates.center;
            mapConfig.zoom = homeCoordinates.zoom;

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

            mapConfig.attributionControl = false;

            var map = L.map(element[0],mapConfig);
            map.getHomeCoordinates = getHomeCoordinates;

            map.goHome = function(animate,uselocal){
                var homeCoordinates = getHomeCoordinates(uselocal);
                console.log(homeCoordinates);
                if(animate){
                    map.panTo(homeCoordinates.center);
                    map.setZoom(homeCoordinates.zoom);
                }else{
                    map.setView(homeCoordinates.center, homeCoordinates.zoom);
                }
            };
            map.goToPersistentUrl = function () {
                var url = document.location.href;
                url = url.replace(/&?lat=(\d+\.\d+)&lon=(\d+\.\d+)&zoom=(\d+)/,"");
                if(!document.location.hash){
                    url+="#";
                }
                url += "&lat="+map.getCenter().lat+"&lon="+map.getCenter().lng+"&zoom="+map.getZoom();
                window.open(url,"_blank");
            }
            map.on("moveend",function(){
                localStorage.setItem("mapposition",JSON.stringify({
                    center : map.getCenter(),
                    zoom : map.getZoom()
                }));
            });

            root.myMap = root.myMap || map;

            $scope.$watch(function(){
                return document.location.hash;
            },function(newvalue,oldvalue){
                if(oldvalue!=newvalue){
                    if(newvalue.match(/&?lat=(\d+\.\d+)&lon=(\d+\.\d+)&zoom=(\d+)/)){
                        map.goHome();
                    }
                }
            });

            if("tiles" in iAttrs) {
                var url = iAttrs["tiles"];
                if(!url.match(/\./)){
                    url = $scope[url];
                }
                url = url.replace(/\{hostname\}/, document.location.hostname);
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
});