/**
 * Created by comdiv on 15.11.2014.
 */
define([
    "leaflet-amd",
    "totalleaflet-circlelayer",
    "totalleaflet-utils",
    "totalleaflet-lmap",
    ],function(L,ttc,tlu,tllm){
    if(typeof angular === "undefined"){
        console.warn("angular required for totalleaflet-angular");
        return null;
    }
    if(!!L) {
        L.Icon.Default.imagePath = "/css/image/marker-icon.png";
    }
    var mod = angular.module("totalleaflet",[]);
    mod.factory("tl-circle",[function(){return ttc;}])
    mod.factory("llutils",[function(){return tlu;}]);
    mod.directive("lmap",[function(){return tllm;}]);
    return mod;
});