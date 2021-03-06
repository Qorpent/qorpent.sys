/**
 * Created by comdiv on 26.09.14.
 * @description Provides utitlity for extending, casting object and types
 */
define(["the-root"], function ($the) {
    return $the(function ($root) {
        var ExtendOptions = function (options) {
            if (!(this instanceof ExtendOptions)) {
                if (options instanceof ExtendOptions)return options;
                return new ExtendOptions(options);
            }
            this.defaultOnNull = false;
            this.ignoreCase = false;
            this.extensions = true;
            this.deep = false;
            this.clone = false;
            this.cloneInternals = false;
            this.functions = true;
            this.preservefalse = false;
            this.filter = function (_) {
                return !!_
            };
            if (!!options) {
                $root.object.extend(this, options);
            }
            return this;
        };
        ExtendOptions.Default = new ExtendOptions();
        ExtendOptions.DefaultCreate = new ExtendOptions();
        ExtendOptions.DefaultCreate.extensions = false;
        ExtendOptions.DefaultClone = new ExtendOptions();
        ExtendOptions.DefaultClone.cloneInternals = true;
        ExtendOptions.DefaultCast = new ExtendOptions();
        ExtendOptions.DefaultCast.extensions = false;
        ExtendOptions.DefaultCast.defaultOnNull = true;
        ExtendOptions.DefaultCast.ignoreCase = true;
        ExtendOptions.DefaultCast.cloneInternals = true;
        ExtendOptions.ExtendedCast = new ExtendOptions();
        ExtendOptions.ExtendedCast.extensions = true;
        ExtendOptions.ExtendedCast.deep = true;
        ExtendOptions.ExtendedCast.defaultOnNull = true;
        ExtendOptions.ExtendedCast.ignoreCase = true;
        ExtendOptions.ExtendedCast.cloneInternals = true;

        $root.object = $root.object || {};
        $root.object.ExtendOptions = ExtendOptions;

        var toHex = $root.object.toHex = function (i) {
            var ret = ((i<0?0x8:0)+((i >> 28) & 0x7)).toString(16) + (i & 0xfffffff).toString(16);
            while (ret.length < 8) ret = '0'+ret;
            return ret;
        };

        var fixedOrder = $root.object.fixedOrder = function(object){
            if(null==object)return null;
            if(Array.isArray(object))return object;
            if(typeof(object)=="function")return object;
            if(typeof(object)!="object")return object;
            var result = {};
            var props = [];
            for(var i in object){
                if(object.hasOwnProperty(i)){
                    props.push(i);
                }
            }
            props.sort();
            for(var i=0;i<props.length;i++){
                result[props[i]] = object[props[i]];
            }
            return result;
        }

        var equal =$root.object.equal = function(obj1,obj2){
            return $root.object.digest(obj1)==$root.object.digest(obj2);
        }

        $root.object.digest = function(object){

            var r = [];
            for (var i=0; i<2; i++)
                r.push(i*268803292);
            var o = JSON.stringify(fixedOrder(object));
            for (i=0; i<o.length; i++) {
                for (c=0; c<r.length; c++) {
                    r[c] = (r[c] << 13)-(r[c] >> 19);
                    r[c] += o.charCodeAt(i) << (r[c] % 24);
                    r[c] = r[c] & r[c];
                }
            }
            for (i=0; i<r.length; i++) {
                r[i] = toHex(r[i]);
            }
            return r.join('');
        }

        $root.object.extend = function (target, source, options) {
            options = ExtendOptions(options) || ExtendOptions.Default;
            if (options.clone) {
                target = self.extend({}, target, ExtendOptions.DefaultClone);
            }
            if (typeof source === "undefined" || null === source)return target;
            var trgKeys = {};
            var srcKeys = {};
            var i;
            for (i in target) {
                //works only on own properties
                if (!target.hasOwnProperty(i))continue;
                trgKeys[i] = i;
                if (options.ignoreCase) {
                    trgKeys[i.toLowerCase()] = i;
                }
            }
            for (i in source) {
                //works only on own properties
                if (!source.hasOwnProperty(i))continue;
                if (!options.filter(i, source[i], source))continue;
                if (!options.functions && typeof  source[i] === "function")continue;
                srcKeys[i] = i.toLowerCase();

            }
            for (i in srcKeys) {
                if (!srcKeys.hasOwnProperty(i))continue;
                var exists = false;
                var trg = trgKeys[i];
                if (typeof trg == "undefined")trg = trgKeys[i.toLowerCase()];
                if (typeof trg !== "undefined")exists = true;
                if (!exists && options.extensions)trg = i;
                if (typeof trg !== "undefined") {
                    var src = source[i];
                    if (options.cloneInternals && $isusr(target[trg])) {
                        target[trg] = self.clone(target[trg]);
                    }
                    if (options.cloneInternals && $isusr(src)) {
                        src = self.clone(src);
                    }
                    if (exists && options.deep && $isusr(target[trg]) && $isusr(src)) {
                        self.extend(target[trg], src, options);
                    } else {
                        if(target[trg]===false && src==true && options.preservefalse){

                            continue;
                        }
                        target[trg] = src;
                    }
                }
            }
            return target;
        };
        $root.isDefaultValue = function (obj) {
            if (null === obj)return true;
            if ("" === obj)return true;
            if (0 === obj)return true;
            if (false === obj)return true;
            if (Array.isArray(obj) && 0 == obj.length)return true;
            if ($root.isUserObject(obj)) {
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
        };
        $root.isUserObject = $root.object.isUserObject = function (obj) {
            if (typeof obj === "undefined" || null === obj)return false;
            if (typeof obj !== "object") return false;
            if (obj instanceof RegExp)return false;
            return !(obj instanceof Date);

        };
        var $isusr = $root.isUserObject;
        var self = $root.object;
        /**
         * Создает новый объект указанного типа с возможностями гибкого слияния
         * @param {Function} ctor Конструктор объекта
         * @param {*} obj параметр, источник каста
         * @param {ExtendOptions} [options] опции слияния
         */
        $root.object.create = function (ctor, obj, options) {
            options = options || ExtendOptions.DefaultCreate;
            var result;
            if (Array.isArray(obj)) {
                result = Object.create(ctor.prototype);
                var _parse = ctor.__parse__ || ctor.prototype.__parse__;
                if (!!_parse && obj.length == 1 && typeof obj[0] === "string") {
                    ctor.apply(result, []);
                    _parse.apply(result, [obj[0]]);
                } else {
                    ctor.apply(result, obj);
                }
                return result;
            } else if (typeof obj == "function") {
                //treat them as factories
                result = obj(ctor);
                return self.cast(ctor, result);
            } else if (typeof obj != "object" || obj instanceof RegExp) {
                return self.cast(ctor, [obj]);
            } else {
                result = Object.create(ctor.prototype);
                ctor.apply(result, []);
                self.extend(result, obj, options);
                return result;

            }
        };

        /**
         * Приводит объект к указанному типу с опциональным использованием ExtendOptions
         * @param {Function} ctor Конструктор объекта
         * @param {*} obj параметр, источник каста
         * @param {ExtendOptions} [options] опции слияния
         * @returns {*} целевой объекта указанного типа
         */
        $root.object.cast = function (ctor, obj, options) {
            if (obj instanceof ctor)return obj;
            options = options || ExtendOptions.DefaultCast;
            if (null == obj || typeof obj == "undefined") {
                if (options.defaultOnNull)return self.create(ctor, []);
                return null;
            }
            return $root.object.create(ctor, obj, options);
        };

        $root.object.propertise = function(obj){
            for(var i in obj){
                if(obj.hasOwnProperty(i) &&(typeof obj[i] == "object") && ("get" in  obj[i])){
                    Object.defineProperty(obj,i,obj[i]);
                }
            }
            return obj;
        }

        $root.object.clone = function (obj) {

            if (Array.isArray(obj)) {
                var result = [];
                for (var i = 0; i < obj.length; i++) {
                    result.push($root.object.clone(obj[i]));
                }
                return result;
            }
            if (typeof undefined == obj || null == obj || !$the.isUserObject(obj))return obj;

            return self.extend({}, obj, ExtendOptions.DefaultClone);
        };
        $root.cast = $root.object.cast;
        $root.extend = $root.object.extend;
        $root.create = $root.object.create;
        $root.clone = $root.object.clone;
        $root.digest = $root.object.digest;
    });
});
