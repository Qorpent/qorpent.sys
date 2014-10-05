define([],function(){
    require.config({
    baseUrl : "scripts",
    paths : {
        jquery : ".rjs/jquery",
        errorcatcher : ".rjs/errorcatcher",
        angular : '.rjs/angular',
        moment : '.rjs/moment',
        layout : '.rjs/layout',
        settings : '.rjs/settings',
        menu : '.rjs/menu',
        "the" : '.rjs/the',
        "the-action" : '.rjs/the-action',
        "the-object" : '.rjs/the-object',
        "the-jsonify" : '.rjs/the-jsonify',
        "the-http" : '.rjs/the-http',
        "the-interpolation" : '.rjs/the-interpolation',
        "the-expression" : '.rjs/the-expression',
        "the-collections" : '.rjs/the-collections',
        "the-collections-linked" : '.rjs/the-collections-linked',
        "the-design-textfitter" : '.rjs/the-design-textfitter'
    },
    shim : {
        angular : {
            deps : ['jquery'],
            exports : 'angular'
        },
        moment : {
            exports : "moment"
        }
    },
    deps:["jquery","angular","moment"]
})});