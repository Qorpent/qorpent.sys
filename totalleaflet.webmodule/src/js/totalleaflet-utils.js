/**
 * Created by comdiv on 05.01.2015.
 */
define([],function(){
    return {
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
        getBounds : function(data, latname, lonname){
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
        }
    }
})