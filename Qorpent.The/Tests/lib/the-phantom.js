var page = require('webpage').create();
page.onConsoleMessage = function(msg) {console.log(msg);};
page.onCallback =  function(){phantom.exit();}
page.open('../the.html');