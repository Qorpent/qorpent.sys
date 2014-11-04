/**
 * Created by comdiv on 24.09.14.
 */
(function (describe, it, before) {



    if (typeof window === "undefined") {
        describe("the.angular - design test can be executed only in browser context", function () {
            it("it's Node.js");
        });
        return;
    }
    describe("the.angular", function () {
        describe("creates modules", function () {
            it("has #all module",function(){
                should.exist($root.modules.all);
                should.exist(angular.module("the-all"));
            });
            it("#unsafe",function(){
                should.exist($root.modules.f_unsafe);
                should.exist(getService("unsafeFilter","the-unsafe"));
                should.exist(getService("unsafeFilter"));
                var us=getService("unsafeFilter");
                us("<div x='1'/>").$$unwrapTrustedValue().should.equal("<div x='1'/>");
            });
        });


        var $ = null;
        var $root = null;
        var should = null;
        var angular = null;

        var getService = function(name,module){
            module = module || "the-all";
            return $root.$angular.injector(["ng",module]).get(name);
        };

        before(function (done) {
            require(["./lib/chai", "../the-angular-all"], function ($should, $the) {
                $root = $the;
                should = $should.Should();
                $ = window.$;
                angular = window.angular;
                done();
            });
        });

    });
})(describe, it, before);