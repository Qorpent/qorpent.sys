/**
 * Created by comdiv on 15.11.2014.
 */
define(["totalleaflet","chai"],function($l,$c){
    var should = $c.Should();
   describe("Simple map test",function(){
       it("can be rendered",function(){
           var maproot = $("<div style='width: 300px;height: 300px; display: flex;'></div>");
           maproot.appendTo(document.body);
           var map = $('<div style="width:100%;height:100%;" ng-include="\'../tests/simplemap.html\'"></div>');
           map.appendTo(maproot);
           angular.bootstrap(maproot,["totalleaflet"])
       });
   }) ;
});