define(["require", "exports"], function (require, exports) {
    /**
     * Created by comdiv on 14.11.2014.
     */
    ///<reference path="headers.d.ts"/>
    var Extender;
    (function (Extender) {
        var CastOptions = (function () {
            function CastOptions(options) {
                this.defaultOnNull = false;
                this.ignoreCase = false;
                this.extensions = true;
                this.deep = false;
                this.clone = false;
                this.cloneInternals = false;
                this.functions = true;
                this.filter = function (name, value, source) { return true; };
                setValues(this, options);
            }
            CastOptions.Default = new CastOptions();
            CastOptions.Cast = new CastOptions({ extensions: false, defaultOnNull: true, ignoreCase: true, cloneInternals: true });
            CastOptions.Create = new CastOptions({ extensions: false });
            CastOptions.Extend = new CastOptions({ defaultOnNull: true, ignoreCase: true, cloneInternals: true, deep: true });
            CastOptions.Clone = new CastOptions({ cloneInternals: true });
            return CastOptions;
        })();
        Extender.CastOptions = CastOptions;
        function setValues(target) {
            var source = [];
            for (var _i = 1; _i < arguments.length; _i++) {
                source[_i - 1] = arguments[_i];
            }
            source.forEach(function (_) {
                for (var i in _) {
                    if (target.hasOwnProperty(i)) {
                        target[i] = _[i];
                    }
                }
            });
            return target;
        }
        Extender.setValues = setValues;
        function propertise(obj) {
            for (var i in obj) {
                if (obj.hasOwnProperty(i) && (typeof obj[i] == "object") && ("get" in obj[i])) {
                    Object.defineProperty(obj, i, obj[i]);
                }
            }
            return obj;
        }
        Extender.propertise = propertise;
        function extend(target, source, options) {
            if (options === void 0) { options = CastOptions.Default; }
            if (!(options instanceof CastOptions)) {
                options = setValues(new CastOptions(), CastOptions.Default, options);
            }
            if (options.clone) {
                var copy = target.constructor ? new target.constructor() : {};
                target = extend(copy, target, CastOptions.Clone);
            }
            if (typeof source === "undefined" || null === source)
                return target;
            var trgKeys = {};
            var srcKeys = {};
            var i;
            for (i in target) {
                //works only on own properties
                if (!target.hasOwnProperty(i))
                    continue;
                trgKeys[i] = i;
                if (options.ignoreCase) {
                    trgKeys[i.toLowerCase()] = i;
                }
            }
            for (i in source) {
                //works only on own properties
                if (!source.hasOwnProperty(i))
                    continue;
                if (!options.filter(i, source[i], source))
                    continue;
                if (!options.functions && typeof source[i] === "function")
                    continue;
                srcKeys[i] = i.toLowerCase();
            }
            for (i in srcKeys) {
                if (!srcKeys.hasOwnProperty(i))
                    continue;
                var exists = false;
                var trg = trgKeys[i];
                if (typeof trg == "undefined")
                    trg = trgKeys[i.toLowerCase()];
                if (typeof trg !== "undefined")
                    exists = true;
                if (!exists && options.extensions)
                    trg = i;
                if (typeof trg !== "undefined") {
                    var src = source[i];
                    if (options.cloneInternals && isUserDefined(target[trg])) {
                        target[trg] = clone(target[trg]);
                    }
                    if (options.cloneInternals && isUserDefined(src)) {
                        src = clone(src);
                    }
                    if (exists && options.deep && isUserDefined(target[trg]) && isUserDefined(src)) {
                        extend(target[trg], src, options);
                    }
                    else {
                        target[trg] = src;
                    }
                }
            }
            return target;
        }
        Extender.extend = extend;
        function create(ctor, source, options) {
            if (options === void 0) { options = CastOptions.Create; }
            if (!(options instanceof CastOptions)) {
                options = setValues(new CastOptions(), CastOptions.Create, options);
            }
            var result;
            if (Array.isArray(source)) {
                result = Object.create(ctor.prototype);
                var _parse = ctor.__parse__ || ctor.prototype.__parse__;
                if (!!_parse && source.length == 1 && typeof source[0] === "string") {
                    ctor.apply(result, []);
                    _parse.apply(result, [source[0]]);
                }
                else {
                    ctor.apply(result, source);
                }
                return result;
            }
            else if (typeof source == "function") {
                //treat them as factories
                result = source(ctor);
                return cast(ctor, result);
            }
            else if (typeof source != "object" || source instanceof RegExp) {
                return cast(ctor, [source]);
            }
            else {
                result = Object.create(ctor.prototype);
                ctor.apply(result, []);
                extend(result, source, options);
                return result;
            }
        }
        Extender.create = create;
        function cast(ctor, obj, options) {
            if (options === void 0) { options = CastOptions.Cast; }
            if (!(options instanceof CastOptions)) {
                options = setValues(new CastOptions(), CastOptions.Cast, options);
            }
            if (obj instanceof ctor)
                return obj;
            if (null == obj || typeof obj == "undefined") {
                if (options.defaultOnNull)
                    return create(ctor, []);
                return null;
            }
            return create(ctor, obj, options);
        }
        Extender.cast = cast;
        function isDefaultValue(obj) {
            if (null === obj)
                return true;
            if ("" === obj)
                return true;
            if (0 === obj)
                return true;
            if (false === obj)
                return true;
            if (Array.isArray(obj) && 0 == obj.length)
                return true;
            if (isUserDefined(obj)) {
                var hasown = false;
                for (var i in obj) {
                    if (obj.hasOwnProperty(i)) {
                        hasown = true;
                        break;
                    }
                }
                return !hasown;
            }
            return false;
        }
        Extender.isDefaultValue = isDefaultValue;
        function isUserDefined(obj) {
            if (typeof obj === "undefined" || null === obj)
                return false;
            if (typeof obj !== "object")
                return false;
            if (obj instanceof RegExp)
                return false;
            return !(obj instanceof Date);
        }
        Extender.isUserDefined = isUserDefined;
        function clone(source, options) {
            if (options === void 0) { options = CastOptions.Clone; }
            if (!(options instanceof CastOptions)) {
                options = setValues(new CastOptions(), CastOptions.Clone, options);
            }
            if (Array.isArray(source)) {
                var result = [];
                for (var i = 0; i < source.length; i++) {
                    result.push(clone(source[i], options));
                }
                return result;
            }
            if (typeof undefined == source || null == source || !isUserDefined(source))
                return source;
            return extend({}, source, options);
        }
        Extender.clone = clone;
    })(Extender = exports.Extender || (exports.Extender = {}));
});
//# sourceMappingURL=the2-object.js.map