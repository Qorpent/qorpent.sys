define([],function(){
    require.config({
    baseUrl : "scripts",
    paths : {
        jquery : ".rjs/jquery",
        angular : '.rjs/angular',
        moment : '.rjs/moment',
        leaflet: '.rjs/leaflet',
        leafletawsome: '.rjs/leaflet-awesome-markers',   
        "ui-bootstrap":'.rjs/ui-bootstrap'
    },
    map : {
         "*":{
             settings: "the-settings",
             errorcatcher: "the-errorcatcher",
             menu : "the-menu",
        } 
    },
    shim : {
        angular : {
            deps : ['jquery'],
            exports : 'angular'
        },
        moment : {
            exports : "moment"
        },
        "ui-bootstrap": {
            deps : ['angular']
        },
        leaflet :{
                exports : "L"
            },
        leafletawsome : {
            deps: [
                "leaflet"
            ]
        }
    },
    deps: ["jquery", "angular", "moment", "ui-bootstrap"]
})});