/**
 * Created by comdiv on 26.09.14.
 */
(function (define) {
    define(["./the-object"], function ($the) {
        return $the(function (root) {

            var jsonify = root.jsonify = function (obj, options) {
                var result;
                var i;
                if (typeof obj === "undefined" || null == obj)return null;
                if (typeof obj === "function") {
                    return obj.toString();
                }
                options = $the.cast(JsonifyOptions, options);
                if (!!options.filter && $the.expression) {
                    options.filter = $the.expression(options.filter);
                }
                if (Array.isArray(obj)) {
                    result = [];
                    for (i = 0; i < obj.length; i++) {
                        result.push(jsonify(obj[i], options))
                    }
                    return result;
                }
                else if ($the.isUserObject(obj)) {
                    result = {};
                    var realobj = obj;
                    if (options.interpolate) {
                        if (!$the.interpolate)throw "Interpolation requested, but no the-interpolate loaded";
                        realobj = $the.interpolate(obj);
                    }
                    for (var i in realobj) {
                        if (!realobj.hasOwnProperty(i))continue;
                        if (!!options.filter && !options.filter(i, realobj[i], realobj))continue;
                        var newval = getValue(i, realobj[i], realobj, options);
                        if (typeof newval !== "undefined") {
                            result[i] = newval;
                        }
                    }
                    return result;
                } else if (typeof obj === "object") {
                    return obj.toString();
                } else {
                    return obj;
                }

            };

            var getValue = jsonify.getValue = function (name, obj, context, options) {
                if (typeof obj === "undefined")return undefined;
                if (null === obj)return (options.nulls && options.defaults) ? obj : undefined;
                if (!options.defaults && root.isDefaultValue(obj))return undefined;
                if (typeof obj === "function") {
                    if (options.evalfunctions) {
                        return jsonify(obj.apply(context, []))
                    }
                    if (!options.functions)return undefined;
                    if (options.stringify)return obj.toString();
                    return obj;
                }
                if (Array.isArray(obj)) {
                    return jsonify(obj);
                }
                else if ($the.isUserObject(obj)) {
                    var newvalue = jsonify(obj);
                    if (options.stringify) {
                        newvalue = JSON.stringify(newvalue);
                    }
                    return newvalue;
                }
                if (typeof obj === "object") {
                    if (options.stringify)return obj.toString();
                    return obj;
                }
                return obj;
            };

            var JsonifyOptions = jsonify.JsonifyOptions = function(){
                this.functions = true;
                this.interpolate = false;
                this.stringify = false;
                this.nulls =true;
                this.defaults =true;
                this.filter = null;
                this.evalfunctions = false;
            };
        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));