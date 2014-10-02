require.config({
    baseUrl : "",
    paths: {
        "jquery" : "lib/jquery",
        "mocha" : "lib/mocha",
        "angular" : "lib/angular"
    },

    shim: {
        jquery : {
            exports: "$"
        },
        mocha : {
            init: function () {
                this.mocha.setup({ui:"bdd",ignoreLeaks: true});
                return this.mocha;
            }
        },
        chai : {
            deps: ["mocha"]
        },
        angular : {
            deps : ['jquery'],
            exports : 'angular'
        }
    },
    deps : [
        'jquery',
        'angular',
        'mocha'
    ],
    callback : function() {
           require (["the-action-tests"
                ,"the-collections-core-tests"
                ,"the-collections-layered-tests"
                ,"the-collections-linked-tests"
                ,"the-collections-linq-tests"
                ,"the-design-tests"
                ,"the-expression-tests"
                ,"the-http-tests"
               ,"the-interpolation-tests"
                ,"the-jsonify-tests"
                ,"the-object-tests"
            ],function(){
                mocha.run();
            });
    }
});