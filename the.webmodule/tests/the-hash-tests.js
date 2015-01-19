define(["the","chai","moment"],function($the,chai,$m) {
    var should = chai.Should();
    describe("hash", function () {
        this.timeout(5000);
        it("can get value",function(){
            $the.hash.get("x","#&y=афк&x=2").should.eq("2");
            $the.hash.get("y","#&y=афк&x=2").should.eq("афк");
            $the.hash.get("y","#&y=%D0%B0%D1%84%D0%BA&x=2",true).should.eq("афк");
            $the.hash.get("y 1","#&y%201=%D0%B0%D1%84%D0%BA&x=2",true).should.eq("афк");
            $the.hash.get("y 1","#&y+1=%D0%B0%D1%84%D0%BA&x=2",true).should.eq("афк");
        });
        it("can set value",function(){
            $the.hash.set("y 1","гаф","#&y%201=%D0%B0%D1%84%D0%BA&x=2",true)
                .should.eq("#&y%201=%D0%B3%D0%B0%D1%84&x=2");
            $the.hash.set("x","3","#&y=афк&x=2").should.eq("#&y=афк&x=3");
            $the.hash.set("y","гаф","#&y=афк&x=2").should.eq("#&y=гаф&x=2");

        });
    });
});
