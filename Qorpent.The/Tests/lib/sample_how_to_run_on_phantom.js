var page = require('webpage').create();
console.log('The default user agent is ' + page.settings.userAgent);
page.onConsoleMessage = function(msg, lineNum, sourceId) {
  console.log(msg);
};
page.open('file:///G:/repos/qorpent.sys/Qorpent.The/Tests/the.html', function(status) {
  if (status !== 'success') {
    console.log('Unable to access network');
	phantom.exit();
  } else {
	setTimeout(function(){
		phantom.exit();
	},5000);
  } 
});