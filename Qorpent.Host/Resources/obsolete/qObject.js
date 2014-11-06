/*
	QObject utility mode for smart object $http-friendly serilizing and "strong-typing"
		
	

	
	
*/
define([], function () {
    function planalizeOptions(args) {
        this.__planalizeOptions = true;
        this.emptyfornulls = true;
        this.evalfunctions = true;
        this.stringifyobjects = true;
        this.skipprivate = true;
        this.privatemask = /^__/;
        this.skipempty = true;
        this.skipzeroes = true;
        this.skipfalses = true;
        extend(this, args);
    }

    function planalize(object, args, options) {
        var config = (!!options && options.__planalizeOptions) ? options : new planalizeOptions(options);
        if (null == object) return config.emptyfornulls ? "" : null;
        if (typeof (object) == 'function') return planalize(object(args), args, config);
        if (typeof (object) != 'object') return object;

        var result = {};
        for (var i in object) {
            var val = object[i];
            if (config.skipprivate && config.privatemask && i.match(config.privatemask)) {
                continue;
            }
            if (null == val) {
                if (config.emptyfornulls && !config.skipempty) {
                    result[i] = "";
                }
            } else if ("" == val && config.skipempty) {
                continue;
            } else if (0 == val && config.skipzeroes) {
                continue;
            }
            else if (false == val && config.skipfalses) {
                continue;
            }
            else if (typeof (val) == 'object') {
                var subconfig = new planalizeOptions(config);
                subconfig.stringifyobjects = false;
                var subresult = planalize(val, args, subconfig); //nested objects must always keep as object
                if (config.stringifyobjects) {
                    result[i] = JSON.stringify(subresult);
                } else {
                    result[i] = subresult;
                }
            } else if (typeof (val) == 'function') {
               
                if (config.evalfunctions) {
                    var sresult = planalize(val(args), args, config);
                    if ("" == sresult && config.skipempty) {
                        continue;
                    } else if (0 == sresult && config.skipzeroes) {
                        continue;
                    }
                    else if (false == sresult && config.skipfalses) {
                        continue;
                    }
                    result[i] = sresult;
                }
            } else {
                result[i] = val;
            }
        }
        return result;
    }



    function extend(target, source, extendprefix, excluderegex) {
        if (!source) return target;
        var prefix = extendprefix;
        if (extendprefix === true || extendprefix == null) {
            prefix = "";
        }
        var dict = {};
        for (var t in target) {
            dict[t.toUpperCase()] = t;
        }
        for (var s in source) {
            if (!!excluderegex) {
                if (!!s.match(excluderegex)) continue;
            }
            if (dict.hasOwnProperty(s.toUpperCase())) {
                var realname = dict[s.toUpperCase()];
                target[realname] = source[s];
            } else {
                if (extendprefix === false) {
                    continue;
                }
                var newname = prefix + s;
                target[newname] = source[s];
            }
        }

        return target;
    }

    var extensionsRegex = /h[qd]_/;

    function applyHttpExtensions(target, source) {
        for (var s in source) {
            if (!!s.match(extensionsRegex)) {
                var val = source[s];
                var type = s.substring(1, 2);
                var name = s.substring(3);
                if (type == 'q') { //request extension 
                    if (typeof (val) == 'object') {
                        target[name] = target[name] || {};
                        smartExtend(target[name], val);
                    } else {
                        target[name] = val;
                    }
                } else if (type == 'd') { //named header
                    target.headers = target.headers || {};
                    target.headers[name] = val;
                }
            }
        }

        return target;
    }

    return {
        planalize: planalize,
        extend: extend,
        extensionsRegex: extensionsRegex,
        applyHttpExtensions: applyHttpExtensions,
        planalizeOptions : planalizeOptions
    };
});