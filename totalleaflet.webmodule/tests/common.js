/**
 * Created by comdiv on 15.11.2014.
 */
define(["totalleaflet","chai"],function($l,$c){
    var should = $c.Should();
   describe("Common leaflet accessibility",function(){
       it("Leaflet with global window.L",function(){
           should.exist(window.L);
           should.exist(L);
       });
       it("Leaflet with AMD return value", function () {
           should.exist($l);
       });
       it("Leaflet.awesome icons accessible",function(){
          should.exist(L.AwesomeMarkers);
       });
       it("Leaflet.draw accessible",function(){
           should.exist(L.Draw);
       });
       it("Leaflet.markercluster accessible",function(){
           should.exist(L.MarkerClusterGroup);
       });
   }) ;
});