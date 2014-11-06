define([],function(){
    require.config({
    baseUrl : "scripts",
    paths : {
        jquery : ".rjs/jquery",
        errorcatcher : ".rjs/errorcatcher",
        angular : '.rjs/angular',
        moment : '.rjs/moment',
        layout : '.rjs/layout',
        settings: '.rjs/settings',
        leaflet: '.rjs/leaflet',
        leafletawsome: './rjs/leaflet-awesome-markers',

        menu: '.rjs/menu',
        "ui-bootstrap":'.rjs/ui-bootstrap',
        "the" : '.rjs/the',
        "the-action" : '.rjs/the-action',
        
        "the-object" : '.rjs/the-object',
        "the-jsonify" : '.rjs/the-jsonify',
        "the-http" : '.rjs/the-http',
        "the-interpolation" : '.rjs/the-interpolation',
        "the-expression" : '.rjs/the-expression',
        "the-collections-core" : '.rjs/the-collections-core',
        "the-collections-linked" : '.rjs/the-collections-linked',
        "the-collections-linq" : '.rjs/the-collections-linq',
        "the-design-textfitter": '.rjs/the-design-textfitter',

        "the-angular": '.rjs/the-angular',
        "the-angular-all": '.rjs/the-angular-all',
        "the-angular-unsafe": '.rjs/the-angular-unsafe',
        "the-angular-leaflet": '.rjs/the-angular-leaflet',
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