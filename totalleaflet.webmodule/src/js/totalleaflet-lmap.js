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
            var injector = angular.injector(['THE_ANGULAR_STUB', 'ng']);
            var $q = injector.get("$q");
            var $templateCache = injector.get("$templateCache");
            var $http = injector.get("$http");
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
                    center : [ iAttrs["lat"]||0 , iAttrs["lon"]||0],
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

            if("zoomcontrol" in iAttrs || iAttrs.hasOwnProperty("noninteractive")){
                mapConfig.zoomControl = false;//iAttrs["zoomcontrol"]!=="false";
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

            if("dblclickzoom" in iAttrs){
                map.doubleClickZoom.disable();
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
            if(iAttrs["yandex"]){
                map.tiles = new L.Yandex();
                map.addLayer(map.tiles);
            }else if(iAttrs["sources"]){
                var layers = [];
                var yandex = L.Yandex && (iAttrs["sources"].match(/yandex/)|| iAttrs["sources"].match(/all/));
                var google = L.Google && (iAttrs["sources"].match(/google/)|| iAttrs["sources"].match(/all/));
                var osm =  iAttrs["sources"].match(/osm/) || iAttrs["sources"].match(/all/);
                var int = iAttrs["sources"].match(/int/)|| iAttrs["sources"].match(/all/);
                var ctl = iAttrs["sources"].match(/control/)|| iAttrs["sources"].match(/all/);
                if(google){
                    layers.push({name:"Google",layer:new L.Google('ROADMAP')});
                }
                if(yandex){
                    layers.push({name:"Яндекс",layer:new L.Yandex()});
                }
                if(google){
                    layers.push({name:"Google земля",layer:new L.Google()});
                    layers.push({name:"Google поверхность",layer:new L.Google('TERRAIN')});
                }
                if(yandex){
                    layers.push({name:"Яндекс траффик",layer: new L.Yandex("null", {traffic:true, opacity:0.9, overlay:false})});
                }
                if(osm) {
                    layers.push({
                        name: "OSM",
                        layer: new L.TileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png')
                    });
                }
                if(int) {
                    layers.push({name:"Встроенная",layer:L.tileLayer(url, {
                        reuseTiles: true,
                        updateWhenIdle: false
                    })})
                }
                map.tiles = layers[0].layer;
                map.addLayer(map.tiles);
                if(ctl){
                    var layeobj = {};
                    layers.forEach(function(_){
                       layeobj[_.name] = _.layer;
                    });
                    map.addControl(new L.Control.Layers( layeobj, {}));
                }
            }  else {
                map.tiles = L.tileLayer(url, {
                    reuseTiles: true,
                    updateWhenIdle: false
                }).addTo(map);
            }


            map.doload = function(){
                if(!!iAttrs["onload"]) {
                    if (iAttrs["onload"] in $scope) {
                        $scope[iAttrs["onload"]](map, element, iAttrs, $scope);
                    }
                }
            };



            if(!!iAttrs["subviews"]){
                var subviews = JSON.parse(iAttrs["subviews"]);
                var awaits = [];
                subviews.forEach(function(_){
                    awaits.push($http.get(_, {cache: $templateCache}));
                });
                $q.all(awaits).then(function(){
                    map.subviews = {items:[]};
                    subviews.forEach(function(_){
                        map.subviews[_] = $templateCache.get(_)[1];
                        map.subviews.items.push({name:_,template:map.subviews[_]});
                        utils.subviews[_] = map.subviews[_];
                    });
                    map.doload();
                })
            }else{
                map.doload();
            }



            window.setTimeout(function(){
                map.goHome(false,true);
				map.invalidateSize(true);
            },4);

            map.infoLevels =false;
            map.toggleInfoLayer  = function(){
                map.infoLevels = !map.infoLevels;
                if(map.infoLevels){
                    if(!map.infoLayer){
                        map.infoLayer = L.layerGroup();
                        require(["ekb-subregion"],function(data){
                            new L.GeoJSON(
                                data,
                                {
                                    style : function(f){
                                        return {
                                            weight: 4,
                                            opacity: .6,
                                            fill:true,
                                            color: "#5E5E5E",
                                            fillOpacity:.3,
                                            fillColor: f.geometry.color
                                            //  className: "nomouse"
                                        };
                                    },
                                    onEachFeature : function(f,l){
                                        l.bindPopup( f.name);

                                    }
                                }).addTo(map.infoLayer);

                        });
                    }
                    map.infoLayer.addTo(map);
                }else{
                    map.removeLayer(map.infoLayer);
                }
            }
        }
    }
});