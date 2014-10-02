require.config({
    baseUrl : "",
    paths: {
        "jquery" : "lib/jquery",
        "mocha" : "lib/mocha",
        "angular" : "lib/angular",
        "teamcity" : "lib/teamcity"
    },

    shim: {
        jquery : {
            exports: "$"
        },
        mocha : {
            exports : 'mocha',
            deps : ["teamcity"],
            init: function (tc) {
                mocha.setup({ui:"bdd",ignoreLeaks: true,reporter: function(runner) {
                    new Mocha.reporters.HTML(runner);
                    new tc(Mocha.reporters.Base,runner);
                }});
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