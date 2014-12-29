/**
 * Created by comdiv on 15.11.2014.
 */
define(["leaflet-amd","the"], function (L,the) {
    return function (map,scope,options) {
        return {
            layer: null,
            show: function (position, radius) {
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
                    this.layer = L.circle(position, radius,opts).addTo(map);
                }
                this.layer.setLatLng(position);
                this.layer.setRadius(radius);
            },
            hide: function () {
                if (!!this.layer) {
                    this.show([0,0],1);
                }
            }
        }
    }
});