/**
 * Created by comdiv on 24.09.14.
 */
define(["the","chai"],function($the,chai){
    var $ = $the.biz;
    var should= chai.Should();
    describe("the.biz", function () {
        describe("#toFio", function () {
            it("simple", function () {
                $.toFio("Иванов Петр Ильич").should.eq("Иванов П.И.");
            });
        });
        describe("#translit", function(){
           it("Cha sha",function(){
               $.translit("Ча ща").should.eq("Cha sha");
           }) ;
        });
        describe("#orgSysName", function () {
            it("superstroy", function () {
                $.orgSysName(" ООО \"Суперстрой\"").should.eq("superstroy");
            });
        });
    });
});