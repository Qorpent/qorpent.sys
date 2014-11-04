require.config({
    baseUrl : "",
    paths: {
        "jquery" : "lib/jquery",
        "mocha" : "lib/mocha",
        "angular" : "lib/angular",
        "teamcity" : "lib/teamcity",
        "angular-mocks" : "lib/angular-mocks"
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
        },
        "angular-mocks":{
            deps: ['angular']
        }
    },
    deps : [
        'jquery',
        'angular',
        'mocha',
        'angular-mocks'
    ],
    callback : function() {
           require (["the-action-tests"
                ,"the-collections-core-tests"
                ,"the-collections-layered-tests"
                ,"the-collections-linked-tests"
                ,"the-collections-linq-tests"
                ,"the-design-tests"
               ,"the-angular-tests"
                ,"the-expression-tests"
                ,"the-http-tests"
               ,"the-interpolation-tests"
                ,"the-jsonify-tests"
                ,"the-object-tests"
            ],function(){
               if (!Function.prototype.bind) {
                   Function.prototype.bind = function (oThis) {
                       if (typeof this !== "function") {
                           // closest thing possible to the ECMAScript 5
                           // internal IsCallable function
                           throw new TypeError("Function.prototype.bind - what is trying to be bound is not callable");
                       }

                       var aArgs = Array.prototype.slice.call(arguments, 1),
                           fToBind = this,
                           fNOP = function () {},
                           fBound = function () {
                               return fToBind.apply(this instanceof fNOP && oThis
                                   ? this
                                   : oThis,
                                   aArgs.concat(Array.prototype.slice.call(arguments)));
                           };

                       fNOP.prototype = this.prototype;
                       fBound.prototype = new fNOP();

                       return fBound;
                   };
               }
               document.finished = false;
                mocha.run(function(){
                    if (typeof window.callPhantom === 'function') {
                        window.callPhantom();
                    }
                });
            });
    }
});