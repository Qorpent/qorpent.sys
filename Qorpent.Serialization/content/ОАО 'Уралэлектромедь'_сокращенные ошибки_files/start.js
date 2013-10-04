//   Copyright 2007-2011 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
//   Supported by Media Technology LTD 
//    
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//    
//        http://www.apache.org/licenses/LICENSE-2.0
//    
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
//   MODIFICATIONS HAVE BEEN MADE TO THIS FILE
//
////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Product          : QWeb JS library (QJS)
// Component        : QJS Starter
// Description      : prepares main environment variables, basic script loader, loads prototype and QJS Core
// Usage            : intended to be first and if possible single <script/> tag in page, following scripts
//                    can be loaded thurther by needings through qweb.load method
/////////////////////////////////////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////////////////////////////////
// DEFINE siteroot variable to point to 'applicationname' part of url
// siteroot allow us to prepare absolute urls without schema/host part
// and without thinking about 'current position' and urls
//////////////////////////////////////////////////////////////////////////////////////////////////////////
window.siteroot = document.location.pathname.match(/\/[^\/]+/)[0];
if (window.siteroot.match(/\./)) siteroot = "";


///////////////////////////////////////////////////////////////////////////////////////////////////////////
// DEFINE core QJS objects which are required in any cases
///////////////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////////////
// qweb is main root object
//////////////////////////////////////////////////////////////////////////////////////////////////////////
window.qweb = window.qweb || {};

///////////////////////////////////////////////////////////////////////////////////////////////////////////
// qweb.scriptloader contains logic to load scripts at runtime
// minimalistic qweb.scriptLoader implementation contains loadcache which keeps info about all loaded scripts
// and temporary implementation of qweb.scriptLoader.load which allow us to load 
// main scripts into header of page
// calloptions - options given in SCRIPT tag in query part of url as comma-separated list
/////////////////////////////////////////////////////////////////////////////////////////////////////////
window.qweb.scriptLoader = window.qweb.scriptLoader || {};
window.qweb.scriptLoader.calloptions = {};
(function () {
    if(document.location.href.match(/\?_qweb_compiled/)){
          window.qweb.scriptLoader.calloptions.compiled = true;
    }
    headelements = document.head.children;
    for (i=0;i<headelements.length;i++){
        e = headelements[i];
        if(e.tagName.toUpperCase()=="SCRIPT"){
			src = e.getAttribute("src");
            if(!src) continue;
            if(src.match(/qweb\/start.js\?/i)){
                options = src.match(/\?.*$/)[0].match(/\w+/);
                for(j=0;j<options.length;j++){
                    window.qweb.scriptLoader.calloptions[options[j]] = true;
                }
                if(src.match(/compiled/)){
                    window.qweb.scriptLoader.calloptions.compiled = true;
                }
                return;
            }
        }
    }
})();
window.qweb.scriptLoader.loadcache = {};
window.qweb.scriptLoader.load = function (options) {
    var result = { name: options.name, url: null, type: options.type, loaded: false, error: null, json: null };
    var compiled = !!window.qweb.scriptLoader.calloptions.compiled;
    if(compiled){
        compiled = "compiled/";
    }else{
        compiled = "";
    }
    result.url = window.siteroot + '/scripts/qweb/' + compiled + options.name + '.js';
    document.writeln("<script type='text/javascript' src='" + result.url + "'></script>");
    result.loaded = true;
    return result;
};
//-----------------------------------------------------------------------------------------------------
// Now we call scriptLoader to setup prototype.js which is basic script for QJS, and qweb/core.js 
// which contains core classes to work with QJS framework
//-----------------------------------------------------------------------------------------------------
window.qweb.scriptLoader.load({ name: 'prototype', type: 'tag' });
window.qweb.scriptLoader.load({ name: 'core', type: 'tag' });





