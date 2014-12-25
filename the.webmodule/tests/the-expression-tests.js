/**
 * Created by comdiv on 24.09.14.
 */
define(["the","chai"],function($the,chai){
    var should = chai.Should();
    var $ = $the.expression;
    describe("the.expression", function () {
        this.timeout(5000);
        it("proxise prepared functions",function(){
            $(function(_){return _==1;})(1).should.equal(true);
            $(function(_){return _==1;})(2).should.equal(false);
        });
        it("null mean null",function(){
            should.not.exist($());
            should.not.exist($(undefined));
            should.not.exist($(null));
        });


        if(typeof window!=="undefined" && !!window.phantomCallback){
            it("can detect 'comparer signature' - SOME PHANTOM ISSUES");
        }else{
            it("can detect 'comparer signature'",function(){
                var f = $(function(a,b){},{annotate:true});
                should.exist(f.annotation.comparer);
                var f = $(function(arg){},{annotate:true});
                should.not.exist(f.annotation.comparer);
            });
            it("can detect 'comparer signature BUG check'",function(){
                var f = $(function (a, b){},{annotate:true});
                should.exist(f.annotation.comparer);
                var f = $(function (arg){},{annotate:true});
                should.not.exist(f.annotation.comparer);
            });
        }
        describe("'check' mode (default behavior)",function(){
           it("number --> _==number",function(){
               $(1)(1).should.equal(true);
               $(1)("1").should.equal(true);
               $(1)(2).should.equal(false);
           });
            it("number (counter option) --> number times of (true)",function(){
                var f = $(3,{counter:true});
                f().should.be.equal(true);
                f().should.be.equal(true);
                f().should.be.equal(true);
                f().should.be.equal(false);
                f().should.be.equal(false);
            });
            it("string --> _==string",function(){
                $("test")("test").should.equal(true);
                $("test")("test2").should.equal(false);
            });
            it("exString --> func with expression",function(){
                $(">2")(3).should.equal(true);
                $("\_>2")(3).should.equal(true);
                $(">2")(1).should.equal(false);
                $("\_>2")(1).should.equal(false);
            });
            it("regex --> _.match(regex)",function(){
                $(/test/)("test").should.equal(true);
                $(/test/)("test23").should.equal(true);
                $(/test/)("tst23").should.equal(false);
            });
            it("array (or mode - default) --> _ in [...], and recursive",function(){
                $([1,2])(1).should.equal(true);
                $([1,2])(2).should.equal(true);
                $([1,2])(3).should.equal(false);
                $([1,2,">2"])(3).should.equal(true);
                $([1,2,">2"])(0).should.equal(false);
            });
            it("array (and mode) --> _ all [...],  and recursive",function(){
                var c = [">2","<5"];
                $([">2","<5"],{and:true})(3).should.equal(true);
                $([">2","<5"],{and:true})(4).should.equal(true);
                $([">2","<5"],{and:true})(5).should.equal(false);
                $([">2","<5"],{and:true})(1).should.equal(false);
            });
            it("object (and mode - default) --> full pattern match ",function(){
                $({x:">2", y:"<3"})({x:3,y:1}).should.be.equal(true);
                $({x:">2", y:"<3"})({x:3,y:4}).should.be.equal(false);
                $({x:">2", y:"<3"})({x:2,y:1}).should.be.equal(false);
                $({x:">2", y:"<3"})({x:3,y:1,z:4}).should.be.equal(true);
                $({x:">2", y:"<3"})({x:3,z:4}).should.be.equal(false);
                $({x:">2", y:"<3","$z":"_.hasOwnProperty('z')"})({x:3,y:1}).should.be.equal(false);
                $({x:">2", y:"<3","$z":"_.hasOwnProperty('z')"})({x:3,y:1,z:4}).should.be.equal(true);
            });
            it("object (or mode) --> partial pattern match ",function(){
                $({x:">2", y:"<3"},{or:true})({x:3,y:1}).should.be.equal(true);
                $({x:">2", y:"<3"},{or:true})({x:3,y:4}).should.be.equal(true);
                $({x:">2", y:"<3"},{or:true})({x:2,y:1}).should.be.equal(true);
                $({x:">2", y:"<3"},{or:true})({x:2,y:4}).should.be.equal(false);
                $({x:">2", y:"<3","$z":"_.hasOwnProperty('z')"},{or:true})({x:3,y:1}).should.be.equal(true);
                $({x:">2", y:"<3","$z":"_.hasOwnProperty('z')"},{or:true})({x:3,y:1,z:4}).should.be.equal(true);
                $({x:">2", y:"<3","$z":"_.hasOwnProperty('z')"},{or:true})({x:1,y:4,z:4}).should.be.equal(true);
                $({x:">2", y:"<3","$z":"_.hasOwnProperty('z')"},{or:true})({x:1,y:4}).should.be.equal(false);
            });
        });

    });
});