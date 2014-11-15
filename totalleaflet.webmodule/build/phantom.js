/**
 * Created by comdiv on 12.11.2014.
 */
var page = require('webpage').create();
page.onConsoleMessage = function(msg) {console.log(msg);};
page.onCallback =  function(){
    phantom.exit();
}
page.open('browser.html');