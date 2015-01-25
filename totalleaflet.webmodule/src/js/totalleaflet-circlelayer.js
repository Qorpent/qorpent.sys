/**
 * Created by comdiv on 15.11.2014.
 */
define(["leaflet-amd","the","totalleaflet-utils"], function (L,the,utils) {
    return function (map,scope,options) {
        return {
            layer: null,
            show: function (position, radius) {
                if(position.hasOwnProperty("Distance") || !radius){

                    if(!position.X || !position.Distance)return;

                    radius= position.Distance;
                    position = utils.getLatLng(position);
                }
                if (!this.layer) {
                    var opts = {
                        stroke:true,
                        fill:true,
                        pointerEvents:"none",
                        weight:5,
                        clickable:false,
                        fillColor:"#CCF6C8",
                        opacity:0.7,
                        color:"green"
                    };
                    the.extend(opts,options);
                    this.layer = L.featureGroup();
                    this.layer.circle = L.circle(position, radius,opts).addTo(this.layer);
                    this.layer.marker = L.circleMarker(position,{
                        fill:true,
                        fillColor:"red",
                        color:"green",
                        radius:5,
                        opacity:1,
                        fillOpacity:1
                    }).addTo(this.layer);
                    map.circleLayer = this.layer;
                }
                this.layer.circle.setLatLng(position);
                this.layer.circle.setRadius(radius);
                this.layer.marker.setLatLng(position);
                this.layer.addTo(map);
            },
            hide: function () {
                if (!!this.layer) {
                    map.removeLayer(this.layer);
                }
            }
        }
    }
});