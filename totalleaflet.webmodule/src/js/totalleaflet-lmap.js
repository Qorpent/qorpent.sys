/**
 * Created by comdiv on 05.01.2015.
 */
define(["totalleaflet-utils"],function(utils){
    var getQorpentTilesUrl = function(){
        var result = "http://{hostname}:14060/map/{z}/{x}/{y}.png";
        if (document.location.href.match(/(map144)|(\:148)/) && !document.location.href.match(/maplocal/)) {
            result = "http://144.76.82.130:14060/map/{z}/{x}/{y}.png";
        }
        return result;
    }
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
            };

            var getHomeBounds = function(){
                var hash = document.location.hash.match(/swlat=(\d+\.\d+)&swlon=(\d+\.\d+)&nelat=(\d+\.\d+)&nelon=(\d+\.\d+)/);

                if(hash){
                    return L.latLngBounds(
                        L.latLng(hash[1],hash[2]),
                        L.latLng(hash[3],hash[4])
                    );
                }

                return null;
            }

            var mapConfig = {

                minZoom : iAttrs["minzoom"]||1,
                maxZoom : iAttrs["maxzoom"]||18
            };

            var homeCoordinates = getHomeCoordinates(true);
            mapConfig.center = homeCoordinates.center;
            mapConfig.zoom = homeCoordinates.zoom;

            if (!!iAttrs["bounds"]){
                var coords = iAttrs["bounds"];
                if(coords=="EKB"){
                    coords = "56.600,60.300,57.000,60.900";
                }
                coords = coords.match(/[\d\.]+/g);
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

            if(iAttrs.hasOwnProperty("noninteractive")){
                map.dragging.disable();
                map.touchZoom.disable();
                map.doubleClickZoom.disable();
                map.scrollWheelZoom.disable();
                map.boxZoom.disable();
                map.keyboard.disable();

            }

            map.goHome = function(animate,uselocal){
                var homeBounds = getHomeBounds();
                if(!!homeBounds){
                    map.fitBounds(homeBounds);

                    return;
                }
                var homeCoordinates = getHomeCoordinates(uselocal);
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
            var dotrackposition = !("notrackposition" in iAttrs);
            var dotrackdataset = !("notrackdataset" in iAttrs);

            map.on("moveend",function(){

                if(dotrackposition) {
                    localStorage.setItem("mapposition", JSON.stringify({
                        center: map.getCenter(),
                        zoom: map.getZoom()
                    }));
                }
                if(dotrackdataset){
                    map.updateDataset();
                    root.$broadcast("MAPDATASETUPDATED",map);
                }
            });




            map.dataset = {
                data : null,
                currentData : null,
                options : {},
                total : 0,
                current : 0
            };

            map.updateDataset = function(newdata,newoptions){
                var total = 0;
                var current = 0;
                var data = newdata || map.dataset.data;
                var currentData= [];
                var options = newoptions || map.dataset.options;

                if(!!data) {
                    var bounds = map.getBounds();
                    data.forEach(function(_){
                        var latlng = utils.getLatLng(_);
                        if(!!options.count){
                            total+=_[options.count];
                        }else {
                            total++;
                        }
                        if(!!latlng && bounds.contains(latlng)){
                            if(!!options.count){
                                currentData.push(_);
                                current+=_[options.count];
                            }else {
                                current++;
                            }
                        }
                    });
                }
                var update = function(){
                    return [
                        !!newoptions?map.dataset.options=newoptions:null,
                        !!newdata?map.dataset.data=data:null,
                        map.dataset.total = total,
                        map.dataset.current = current,
                        map.dataset.currentData = currentData
                    ];
                }
                return root.$$phase ? update() : root.$apply(update);
            }

            map.fitToData = function(data,context){
                context = context || {};
                var options = {
                    paddingTopLeft:[context.PaddingLeft||300,context.PaddingTop||0],
                    paddingBottomRight:[context.PaddingRight||100,context.PaddingBottom||0]
                };
                if(!!context.X && !!context.Y && !!context.Distance){
                    map.fitBounds(utils.getBounds({x:context.X,y:context.Y,d:context.Distance}),options);
                }
                else {
                    if(!!data && data.length!=0) {
                        map.fitBounds(utils.getBounds(data), options);
                    }
                }

            }


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


            var url = iAttrs["tiles"] || getQorpentTilesUrl();
            if(url=='qorpent')url = getQorpentTilesUrl();

            if(!url.match(/\./)){
                url = $scope[url];
            }

            url = url.replace(/\{hostname\}/, document.location.hostname);

            L.tileLayer(url, {
                reuseTiles: true,
                updateWhenIdle: false
            }).addTo(map);


            if(!!iAttrs["onload"]) {
                if (iAttrs["onload"] in $scope) {
                    $scope[iAttrs["onload"]](map, element, iAttrs);
                }
            }


            window.setTimeout(function(){
                map.goHome(false,true);
            })
        }
    }
});