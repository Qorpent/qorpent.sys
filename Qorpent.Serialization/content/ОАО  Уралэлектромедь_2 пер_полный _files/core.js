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
// Component        : QJS Core
// Description      : core functionality of QJS - json queries to QWeb wrap, dynamic script/css loading, 
//                    main abstracts setup
// Usage            : you may, but not intended to use core objects directly from DOM,
//                    core designed to be down-level for JSON APIs and base application framework
/////////////////////////////////////////////////////////////////////////////////////////////////////////////

(function () {
    //we must ensure that prototype library is loaded, that is valid way to test root object of
    //prototype lib
    if (!window.Prototype) {
        throw 'prototype was not laoded!!!!';
    }

    //defineGlobal - safe method to define namespaces/objects in global (window) scope
    // name is path in form : "ns1.ns2.obj1.obj2..." method will create all of them if
    // they are not existed returns LAST object in ierarchy
    window.qweb.defineGlobal = function (name) {
        if (!name) throw 'must provide name of global object';
        if (typeof (name) != "string") throw 'name of object must be string';
        if (!name.match(/^\w+((\.\w+)+)?$/)) throw 'syntax error in name ' + name;

        //collect all words splited by dot
        parts = name.strip().match(/[^\.]+/g);
        //set windows as first global root
        root = window;
        for (i = 0; i < parts.length; i++) {
            part = parts[i];
            //if current root not contains given name create empty object for it
            if (!root[part]) {
                root[part] = {};
            }
            //set current root for object with given name
            root = root[part];
        }
        return root;
    }

    //qweb.ajax - is a object that wraps and dispatches calls to QWeb AJAX Api
    qweb.defineGlobal('qweb.ajax');
    //resetup scriploader with full implementattion
    qweb.defineGlobal('qweb.scriptLoader');

    Object.extend(qweb.scriptLoader, {

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*
         PUBLIC METHOD qweb.scriptLoader.load (qweb.load) - method for dynamic script loading
         uses internal cache to prevent double script loading,
         supports two methods - adding script tags to head (only if called from head),
         ajax method which dynamicaly loads script and treat it as JSON
         to simplify script's location scriptLoader uses resolveUrl method to
         convert short names to correct full url of JS

         notation : a) load(url) b) load(opts)
         where:
         url (for full info see resolveUrl method)  = absurl | relurl | qwebjsname | qwebdynjsurl
         absurl = full url, which is
         a) an url started with http.. - not transformed in resolveUrl
         b) an url ENDED with .js (with extension but not from http://)
         - converted to url, relative to siteroot
         relurl = path/name of script without extension is parsed as relative to siteroot/scripts
         qwebjsname = #path/name - transformed to siteroot/scripts/qweb/path/name
         or to siteroot/scripts/qweb/compiledpath/name - due to startup option
         qwebdynjsurl = ~path/name - is not resolved by QJS itself - instead
         of this standard server ajax qweb command qweb.getresourceurl(url,type="js") used
         opts = {name : url, type : 'ajax'|'tag' ,ajax : { [prototype ajax options]} }
         type==ajax by default
         */
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        load: function (loadoptions) {
            if (!loadoptions) throw 'must provide load info for window.qweb.scriptLoader.load';
            if (!(typeof (loadoptions) == "string" || typeof (loadoptions) == "object")) throw 'load info must be string or special object';

            //first - setup load options
            defaultType = 'ajax';
            if (!document.body) defaultType = 'tag'; //if no body exists yet we are in header and it's much beter to use <SCRIPT/> tags rather
            //than AJAX due to debug/caching issues
            options = { name: "", type: defaultType };
            if (typeof (loadoptions) == "string") {
                options.name = loadoptions;
            } else {
                Object.extend(options, loadoptions);
            }
            if (!options.name) throw 'name of script not provided';
            ////////////////////////////

            result = null;

            // if JS was already requested and loaded got it from cache
            if (this.loadcache[options.name]) {
                result = this.loadcache[options.name];
                if (result.loaded && !result.error) { //but we use cached result ONLY if it was SUCCESS
                    return result;
                }
            }

            //if script is requested by first time or it was not successfull last time we try to load JS from net

            //prepare empty result
            var result = { name: options.name, url: null, type: options.type, loaded: false, error: null, json: null };

            try {
                // try to resolve url due to options or by using Url resolution
                result.url = (loadoptions.url || this._resolveUrl(options.name));
                if (!result.url) { //but if it was not sucessfull - it's exception
                    throw 'no url was resolved for script ' + options.name;
                }

                // if 'tag' mode choosed we create <SCRIPT src=URL/> tag
                if (options.type == "tag") {
                    options.noThrowError = true;
                    document.writeln("<script type='text/javascript' src='" + result.url + "'></script>");

                    // otherwise if 'ajax' mode choosed - we load JS by AJAX end eval it as JSON
                } else if (options.type == "ajax") {
                    this.__loadByAjax(options, result);
                    // otherwise - it's unsupported mode
                } else {
                    throw 'unknown type - ' + options.type;
                }

                //if all was OK mark result as loaded
                result.loaded = true;

            } catch (error) {
                // otherwise if it's error we CONTINUE, but marks result as not successfull
                result.loaded = false;
                result.error = error;
            }

            //store result to cache
            this.loadcache[options.name] = result;

            //if result was not successfull and exceptions was not suppressed by caller
            //we throws exception here
            if (result.error && !options.noThrowError) {
                throw result.error;
            }
            return result;
        },

        //-- helper private method for 'load'
        __loadByAjax: function (options, result) {
            requestOptions = {
                asynchronous: false,
                method: 'GET',
                onSuccess: function (r) {
                    result.json = eval("__stub = " + r.responseText);
                },
                onFailure: function (r) {
                    result.error = 'some error during ajax request';
                }
            };
            if (options.ajax) {
                requestOptions = Object.extend(requestOptions, options.ajax);
            }
            new Ajax.Request(result.url, requestOptions);
        },

        ////////////////////////////////////////////////////////////////////////////////////////////
        // PROTECTED METHOD _resolveUrl(name) - converts JS urls/names to full resource URL
        //    behaviour:
        /*
         1)   full urls : http://... not changed and returned as given
         2)   not rooted urls but full with EXTENSION '.js' returned as siteroot+/+url
         3)   not rooted and without extensions (myscripts/first) are resolved as
         /siteroot/scripts/myscripts/first.js
         4)   # prefixed names (ex.: #my/first) are treated as QJS scripts and resolved as :
         /siteroot/scripts/qweb/my/first.js  or as
         /siteroot/scripts/qweb/compiled/my/first.js if 'compiled' option
         was used on qweb/start.js script
         5)   ~ prefixed names (ex.: ~my/first) are resolved BY SERVER,
         with AJAX call to QWeb standard action "qweb.getresourceurl"
         which performs  server-determined URL resolution, it's
         usefull if we use some complex logic of JS file allocations
         (with culture/environment/browser/user dependency, or
         dynamic JS creation), but want to use simple names on client.
         By default   "qweb.getresourceurl" has ability to quick decide
         of 304 status posibility or long cache header so server resolution is not
         big overheat but it's far more better rather than using MVC-like
         server JSURL resolving during VIEW-rendering time, because of
         undetermineable cache state of common page. Instead of this,  "qweb.getresourceurl"
         can be used on  huge static HTML page, which will be cached without any problems
         override :
         _resolveUrl method may be overrided for custom url resolving, but you must override
         it correctly with saving default behaviour where it's not to be overriden
         */
        ////////////////////////////////////////////////////////////////////////////
        _resolveUrl: function (name) {
            if (name.match(/^\#/)) {
                compiled = "";
                if (this.calloptions.compiled) {
                    compiled = "compiled/";
                }
                return siteroot + '/scripts/qweb/' + compiled + name.replace(/^\#/, '') + '.js';
            }
            if (name.match(/\.js$/) || name.match(/^(https?:\/)?\//)) {
                if (name.match(/^https?:\//)) {
                    return name;
                }
                if (name.match(/^\//)) {
                    return name;
                }
                return siteroot + '/' + name;
            }
            if (name.match(/^[\w\.\d\_]+/)) {
                return siteroot + '/scripts/' + name + '.js';
            }
            if (name.match(/^~/)) {
                return qweb.ajax.call({ name: 'resource.geturl', ajax: { requestHeaders: { "If-Modified-Since": this.__resolve_version}} }, { name: name, type: 'js' });
            }
            throw "illegal script name : " + name;
        },

        __resolve_version: ""

    });

    Object.extend(String.prototype, {
        embed: function (obj) {
            var copy = this;
            var match = this.match(/\$\{[^\}]+\}/g);
            if (match) {
                for (i = 0; i < match.length; i++) {
                    m = match[i];
                    p = m.match(/\$\{([^\}]+)\}/)[1];
                    v = "";
                    try {
                        v = eval('obj.' + p);
                    } catch (e) {
                        v = e;
                    }
                    v = v || "";
                    copy = copy.replace(m, v);
                }
            }
            return copy;
        }
    });
    Element.addMethods({
        embed: function (element, obj) {
            element.innerHTML = element.innerHTML.embed(obj);
        }
    });

    Object.extend(window.qweb.ajax, {
        errorHandlers: [],
        call: function (command, query, callback, errorcallback) {
            if ("string" != typeof (query)) {
                query = Object.extend({}, query || {});
            } else {
                query = query.replace(/^\?/, '');
            }
            cmd = { type: "json" };
            if (typeof (command) == "object") {
                Object.extend(cmd, command);
            } else {
                cmd.name = command;
            }
            if ("string" != typeof (query)) {
                if (query._jdata) {
                    query._jdata = JSON.stringify(query._jdata);
                }
            }
            result = null;
            error = null;
			if (cmd.name.startsWith('http')) {
				url = cmd.name;
			} else {
				url = siteroot + '/' + cmd.name.replace(/\./, '/') + '.' + cmd.type + '.qweb';
			}
            ajaxoptions = {
                asynchronous: !!callback,
                method: 'POST',
                parameters: query,
                onSuccess: function (r) {
                    key = JSON.stringify({ url: url, parameters: query });
                    if (r.status == 304) {
                        result = qweb.ajax.__ajax_cache[key].result;
                    } else {
                        if (cmd.type == "js") {
                            result = eval("__stub = " + r.responseText);
                        } else if (cmd.type == "json") {
                            result = JSON.parse(r.responseText)
                        } else if (cmd.type == "xml") {
                            result = r.responseXML;
                        } else {
                            result = r.responseText;
                        }
                        lm = r.getHeader('Last-Modified');
                        etag = r.getHeader('Etag');
                        if (lm) {
                            qweb.ajax.__ajax_cache[key] = { result: result, etag: etag, lastmod: lm };

                        }
                    }
                    if (callback) {
                        callback(result, cmd, query);
                    }

                }.bind(this),
                onFailure: function (r) {
                    if (errorcallback) {
                        errorcallback(r, cmd, query, r.responseText);
                    }
                    for (i = 0; i < this.errorHandlers.length; i++) {
                        this.errorHandlers[i]({ type: 'js', command: command, query: query }, r);
                    }
                    error = r.status + " : " + r.responseText;
                }.bind(this)
            };
            if (cmd.ajax) {
                Object.extend(ajaxoptions, cmd.ajax);
            }

            key = JSON.stringify({ url: url, parameters: ajaxoptions.parameters });
            if (this.__ajax_cache[key]) {
                cached = this.__ajax_cache[key];
                ajaxoptions.requestHeaders = ajaxoptions.requestHeaders || {};
                ajaxoptions.requestHeaders.Etag = cached.etag;
                ajaxoptions.requestHeaders["If-Modified-Since"] = cached.lastmod;
            }
            new Ajax.Request(url, ajaxoptions);
            if (error) {
                throw error;
            }
            return result;
        },

        __ajax_cache: {

        }
    });

    qweb.load = qweb.scriptLoader.load.bind(qweb.scriptLoader);
    qweb.call = qweb.ajax.call.bind(qweb.ajax);

    Object.extend(qweb, {
        onWindowLoad : function (func) {
            Event.observe(window, 'load', func);
        }
    });

})();