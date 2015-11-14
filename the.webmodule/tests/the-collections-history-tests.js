/**
 * Created by comdiv on 24.09.14.
 */
define(["the", "chai"], function ($the, chai) {
    var should = chai.Should();
    var $ = $the.collections;
    var $h = $.History;
    describe("the.collections.History", function () {
        this.timeout(5000);
        it("can be created",function(){
            var h = new $h(3);
        });
        it("can be populated",function(){
            var h = new $h(3);
            h.add(1);
            h.add(2);
            h.add(3);
            h.getItems().should.eql([3,2,1]);
        });
        it("keeps size",function(){
            var h = new $h({size:3});
            h.add(1);
            h.add(2);
            h.add(3);
            h.add(4);
            h.getItems().should.eql([4,3,2]);
        });
        it("tracks version",function(){
            var h = new $h({size:3});
            h.add(1);
            h.add(2);
            h.add(3);
            h.add(1);
            h.getItems().should.eql([1,3,2]);
        });
        it("uses digest",function(){
            var h = new $h({size:3});
            h.add({x:1,y:1});
            h.add({x:2,y:2});
            h.add({x:3,y:3});
            h.add({y:1,x:1});
            h.getItems().should.eql([{x:1,y:1},{x:3,y:3},{x:2,y:2}]);
        });
        it("can write localStorage",function(){
            var ls = typeof( localStorage ) == "undefined" ?  $the.localStorage : localStorage;
            ls.removeItem('can_write_localStorage');
            var h = new $h({size:3,lskey:'can_write_localStorage'});
            h.add(1);
            h.add(2);
            h.add(3);
            var items = ls.getItem('can_write_localStorage');
            items.should.equal('[{"item":3,"version":2},{"item":2,"version":1},{"item":1,"version":0}]');
        });
        it("can read localStorage",function(){
            var ls = typeof( localStorage ) == "undefined" ?  $the.localStorage : localStorage;
            ls.setItem('can_write_localStorage','[{"item":3,"version":2},{"item":2,"version":1},{"item":1,"version":0}]');
            var h = new $h({size:3,lskey:'can_write_localStorage'});
            h.add(1);
            h.add(2);
            h.add(3);
            h.getItems().should.eql([3,2,1]);
        });
    });
});