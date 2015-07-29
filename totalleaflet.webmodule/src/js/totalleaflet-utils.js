/**
 * Created by comdiv on 05.01.2015.
 */
define(["leaflet-amd","jquery"],function(){
    var popupMatchDiv = $('<div style="z-index: -1000;visibility: hidden"></div>').appendTo(document.body);
    try { angular.module("THE_ANGULAR_STUB") } catch(err) { angular.module("THE_ANGULAR_STUB",[])}
    var injector = angular.injector(['THE_ANGULAR_STUB', 'ng']);
    var $compile = injector.get("$compile");

    var result = {
        stubDiv : popupMatchDiv,
        subviews : {
            _stub_ : '<div ng-bind="obj"></div>',
            items:[]
        },
        getMarkerXY : function(marker){
            var point = marker.getLatLng();
            return this.getXY(point);
        },
        getXY : function(point){
            var xy = L.Projection.SphericalMercator.project(point);
            var result = {};
            result.X = (xy.x * 6378137).toFixed(0);
            result.Y = (xy.y * 6378137).toFixed(0);
            result.x = result.X;
            result.y = result.Y;
            return result;
        },
        getLatLng : function(any){
            var x = any.x || any.X;
            var y = any.y || any.Y;
            if(x && y){
                return L.Projection.SphericalMercator.unproject(L.point(x/6378137,y/6378137));
            }
            var lat =  any.lat ||any.Lat || any.lt;
            var lon =  any.lng || any.Lng || any.Lon || any.ln;
            if(lat && lon){
                return  L.latLng(lat,lon);
            }
            return null;
        },
        getBounds : function(data, latname, lonname){
            if(!!data.x && !!data.y && !!data.d){
                var x1 = data.x - data.d * 1.5;
                var y1 = data.y - data.d * 1.5;
                var x2 = Number(data.x) + Number(data.d) * 1.5;
                var y2 = Number(data.y) + Number(data.d) * 1.5;
                var bound = L.latLngBounds(this.getLatLng({x:x1,y:y1}),this.getLatLng({x:x2,y:y2}));
                return bound;
            }

            latname = latname||"Lat";
            lonname = lonname||"Lon";
            minlat = 500;
            maxlat = -500;
            minlon = 500;
            maxlon = -500;
            data.forEach(function(_){
                var lat = Number(_[latname]);
                var lon = Number(_[lonname]);
                if(lat==NaN || lon==NaN)return;
                minlat = Math.min(minlat,lat);
                minlon = Math.min(minlon,lon);
                maxlat = Math.max(maxlat,lat);
                maxlon = Math.max(maxlon,lon);
            });
            if(Math.abs(minlat)>=180||Math.abs(maxlat)>=180||Math.abs(minlon)>=180||Math.abs(maxlon)>=180)return null;
            var southWest = L.latLng(minlat,minlon),
                northEast = L.latLng(maxlat, maxlon),
                bounds = L.latLngBounds(southWest, northEast);
            return bounds;
        },
        createDynamicIcon : function(options,callback) {
            var self = this;
            options = options || {};
            var item = options.item || {};
            var parentscope = options.scope;
            var onscope = options.onscope || function(item) {
                    if(parentscope){
                        var s = parentscope.$new();
                        s.obj = item;
                        return s;
                    }else {
                        return {obj: item}
                    }
                };
            var view = options.view || '_stub_';
            var classname = options.classname || "";
            var scope = onscope(item);
            var template = self.subviews[view] || self.subviews['_stub_'];
            var e = $compile(template)(scope)[0];

            setTimeout(function () {
                var icon = L.divIcon({
                    className: classname,
                    html: e.innerHTML
                });
                if(!!callback){
                    callback(icon);
                }

            },0);
        },
        makeDynamicIcon : function(options){

            var marker = options.marker;
            this.createDynamicIcon(options,function(icon){
                marker.setIcon(icon);
            })

        },
        makeDynamicPopup : function(options) {
            options = options || {};
            var item = options.item || {};
            var parentscope = options.scope;
            var marker = options.marker || {on:function(){},bindPopup:function(){},openPopup:function(){}};
            var view = options.view || '_stub_';
            var onscope = options.onscope || function(item) {
                    if(parentscope){
                        var s = parentscope.$new();
                        s.obj = item;
                        return s;
                    }else {
                        return {obj: item}
                    }
                };
            var margin = options.margin || 20;

            var scope = onscope(item);

            var self = this;
            var preparePopup = function(show) {
                var template = self.subviews[view] || self.subviews['_stub_'];
                var popup = $compile(template)(scope)[0];
                var jp = $(popup);
                var ready = function () {
                    jp.off("show", ready);
                    var width = jp.outerWidth();
                    jp.detach();
                    marker.bindPopup(popup, {maxWidth: width + margin});
                    marker._popupPrepared = true;
                    if (show)  marker.openPopup();

                }
                jp.show(ready);
                jp.appendTo(self.stubDiv);
                jp.show();
                scope.$apply();
            };
            marker.on("click", function (e) {
                if (!marker._popupPrepared) {
                    preparePopup(true);
                }
            });
        }
    };
    L.Marker.prototype.makeDynamicPopup = function(options){
        options = options || {};
        options.marker = options.marker || this;
        result.makeDynamicPopup(options);
    };
    L.Marker.prototype.makeDynamicIcon = function(options){
        options = options || {};
        options.marker = options.marker || this;
        result.makeDynamicIcon(options);
    };
    return result;
})