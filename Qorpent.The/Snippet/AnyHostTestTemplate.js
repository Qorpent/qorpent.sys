/**
 * Created by comdiv on 24.09.14.
 */
/**
 * Created by comdiv on 24.09.14.
 */
describe("AnyRunner is mocha-oriented template to run tests for AMD both in Node.js and Browser", function(){

    /* we assumed that somehow we will have our module in "me" variable and optionally some required dependency*/

     var me = null;
    var pureAMD = null;
    //var myDependency =null;

    /* assume we have some tests defined */

    // here I use pure exception generation asserts because u should use any assertion lib in real project
    describe ("it works", function(){
        it("as it loaded", function(){
            if(me.execute()!="hello world"){
                throw "fail!";
            }
        }) ;
    });
    describe ("AMD works too", function(){
        it("as it loaded", function(){
            if(pureAMD.execute()!="hello world me too!"){
                throw "fail!";
            }
        }) ;
    });


    /* but u have to use this to setup fixture, because u don't know where (nodejs/browser) tests are started */

    this.timeout(1000); // required as require.js is ASYNC

    before (function(done){
        var requirejs= null;
        try{
            if(!!define){ // simplect way to check that requirejs is loaded to global scope (in browser)
                requirejs = require;
            }
        }catch(e){
            requirejs = require("requirejs"); //otherwise load it with Node.js require
            requirejs.config({baseurbaseUrl: '.', nodeRequire: require}); //and setup
        }
        try{
            requirejs(["./AnyHostModuleTemplate","./AnyHostAMDSample.js"/*,someOtherDependency*/], function($me,$amd/*,someOtherDependency*/){
                //myDependency = someOtherDependency; //move dependency to scope
                me = (typeof $me == "function"? $me : $me.module ); // resolves AMD/commonJS style of loading
                pureAMD = (typeof $amd == "function"? $amd : $amd.module );
                done();
            });
        }catch(e){ //here we force done, but rethrow exception to have before hook be invalid
            done();
            throw e;
        }
    });


});