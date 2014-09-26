/**
 * Created by comdiv on 24.09.14.
 */
describe("Thenum", function(){
    this.timeout(300);
    describe ("is accessible and",function(){
        it("can be loaded by commonjs",function(){
            var asCommonJs = require("../thenum");
            asCommonJs.should.be.ok;
        });

        it("can be loaded by requirejs",function(done){
            var requirejs = require("requirejs");
            requirejs.config({baseurbaseUrl: '.', nodeRequire: require});
            requirejs(["../thenum"],function($qu){
                done();
            });
        });


    });



});
describe("Thenum.Collections", function(){
    this.timeout(300);
    describe ("is accessible and",function(){
        it("can be loaded by commonjs",function(){
            var asCommonJs = require("../thenum-collections");
            asCommonJs.should.be.ok;
        });

        it("can be loaded by requirejs",function(done){
            var requirejs = require("requirejs");
            requirejs.config({baseurbaseUrl: '.', nodeRequire: require});
            requirejs(["../thenum-collections"],function($qu){
                done();
            });
        });


    });



});