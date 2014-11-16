/**
 * Created by comdiv on 24.09.14.
 */
define(["the","the-angular","the-angular-unsafe","chai"],function($the,chai){

    var $ = $the.object;
    var should= chai.Should();

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

        var getService = function(name,module){
            module = module || "the-all";
            return $root.$angular.injector(["ng",module]).get(name);
        };


    });
});