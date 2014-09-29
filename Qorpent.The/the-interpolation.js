/**
 * Created by comdiv on 26.09.14.
 */
(function (define) {
    define(["./the-object"], function ($the) {
        return $the(function (root, privates) {
            var __interpolateString = privates.__interpolateString = function (str, source, options) {
                return str.replace(options.getRegex(), function (_) {
                    var reference = _.substring(2, _.length - 1);
                    var value = options.isLayeredDictionary ? source.get(reference) : source[reference];
                    if (typeof value === "undefined") {
                        return _;
                    }
                    if (null === value) {
                        value = "";
                    } else if (typeof value === "function") {
                        value = value.apply(source, []);
                    } else if (typeof value === "object") {
                        value = JSON.stringify(value);
                    }
                    return value;
                });
            };

            var __interpolateObject = privates.__interpolateObject = function (obj, source, options) {
                var realTarget = $the.clone(obj);
                var realSource = __prepareSource(realTarget, source, options);
                var recursive = 3;
                var i;
                while (true) {
                    if (0 == (recursive -= 1))break;
                    __updateSource(realTarget, realSource, options);
                    var targets = __collectInterpolables(realTarget, options);
                    if (0 == targets.length)break;
                    for (i = 0; i < targets.length; i++) {
                        realTarget[targets[i][0]] = __interpolateString(targets[i][1], realSource, options);
                    }
                }
                for (i in realTarget) {
                    if (!realTarget.hasOwnProperty(i))continue;
                    if ($the.isUserObject(realTarget[i])) {
                        realTarget[i] = __interpolateObject(realTarget[i], realSource, options);
                    }
                }

                return realTarget;
            };

            var __collectInterpolables = function (obj, options) {
                var result = [];
                for (var i in obj) {
                    if (!obj.hasOwnProperty(i))continue;
                    if (typeof obj[i] !== "string")continue;
                    var val = obj[i];
                    if (!val.match(options.getRegex()))continue;
                    result.push([i, val]);
                }
                return result;
            };

            var __updateSource = function (obj, source, options) {
                if (options.isLayeredDictionary) {
                    source.apply(obj);
                } else {
                    $the.extend(source, obj);
                }
            };

            var __prepareSource = function (obj, source, options) {
                var result = null;
                if (options.isLayeredDictionary) {
                    result = new options.layeredDictionary(source);
                } else {
                    result = $the.clone(source);
                }
                return result;
            };

            root.interpolate = function (target, source, options) {
                if (typeof target == "undefined" || null == target)return null;
                options = $the.cast(opts, options);
                source = source || {};
                if (typeof source !== "object")return target;
                if (!!$the.collections && !!$the.collections.LayeredDictionary && (source  instanceof $the.collections.LayeredDictionary )) {
                    options.isLayeredDictionary = true;
                    options.layeredDictionary = $the.collections.LayeredDictionary;
                }
                if (typeof target === "string") {
                    return __interpolateString(target, source, options);
                }
                if (!$the.isUserObject(target))return target;
                return __interpolateObject(target, source, options);
            };
            var opts = root.interpolate.InterpolationOptions = function () {
                this.anchor = "$";
                this.start = "{";
                this.finish = "}";
                this.isLayeredDictionary = false;
                this.regex = null;
                this.getRegex = function () {
                    return this.regex || (this.regex = new RegExp("\\" + this.anchor + "\\" + this.start + "[^\\" + this.finish + "]+" + "\\" + this.finish, "g"));
                }
            };
        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));