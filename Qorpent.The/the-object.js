/**
 * Created by comdiv on 26.09.14.
 * @description Provides utitlity for extending, casting object and types
 */
(function (define) {
    define(["./the"], function ($the) {
        return $the(function ($root,$privates) {
            var ExtendOptions =function(options){
                if (!(this instanceof ExtendOptions)){
                    if(options instanceof ExtendOptions)return options;
                    return new ExtendOptions(options);
                }
                this.defaultOnNull = false;
                this.ignoreCase  = false;
                this.extensions  = true;
                this.deep = false;
                this.clone = false;
                this.cloneInternals = false;
                this.functions = true;
                this.filter = function(_){return !!_};
                if(!!options){
                    $root.object.extend(this,options);
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
            ExtendOptions.DefaultCast.defaultOnNull = true;
            ExtendOptions.ExtendedCast.ignoreCase = true;
            ExtendOptions.ExtendedCast.cloneInternals = true;

            $root.object = $root.object || {};
            $root.object.ExtendOptions = ExtendOptions;

            $root.object.extend = function(target,source,options){
                options = ExtendOptions(options) || ExtendOptions.Default;
                if(options.clone){
                    target = self.extend({},target, ExtendOptions.DefaultClone);
                }
                var trgKeys = {};
                var srcKeys = {};
                var i;
                for(i in target){
                    //works only on own properties
                    if(!target.hasOwnProperty(i))continue;
                    trgKeys[i] = i;
                    if(options.ignoreCase){
                        trgKeys[i.toLowerCase()] = i;
                    }
                }
                for(i in source){
                    //works only on own properties
                    if(!source.hasOwnProperty(i))continue;
                    if(!options.filter(i))continue;
                    if(!options.functions && typeof  source[i]==="function")continue;
                    srcKeys[i] = i.toLowerCase();

                }
                for(i in srcKeys){
                    if(!srcKeys.hasOwnProperty(i))continue;
                    var exists = false;
                    var trg = trgKeys[i];
                    if(typeof trg == "undefined")trg = trgKeys[i.toLowerCase()];
                    if(typeof trg!=="undefined")exists = true;
                    if(!exists && options.extensions)trg = i;



                    if(typeof trg !== "undefined"){
                        var src = source[i];
                        if(options.cloneInternals && $iscloneable(target[trg])){
                            target[trg] = self.extend({},target[trg],ExtendOptions.DefaultClone);
                        }
                        if(options.cloneInternals && $iscloneable(src)){
                            src = self.extend({},src,ExtendOptions.DefaultClone);
                        }
                        if(exists
                            && options.deep
                            && $iscloneable(target[trg])
                            && $iscloneable(src)
                            ){
                            self.extend(target[trg],src,options);
                        }else{

                            target[trg] = src;
                        }
                    }
                }
                return target;
            };

            $privates._isCloneable = function (obj) {
                if (typeof obj !== "object") return false;
                if (obj instanceof RegExp)return false;
                return !(obj instanceof Date);

            };
            var $iscloneable = $privates._isCloneable;
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
                    if(!!_parse && obj.length==1 && typeof obj[0] === "string" ){
                        ctor.apply(result,[]);
                        _parse.apply(result,[obj[0]]);
                    }else{
                        ctor.apply(result, obj);
                    }
                    return result;
                } else if (typeof obj == "function") {
                    //treat them as factories
                    result = obj(ctor);
                    return self.cast(ctor, result);
                } else if (typeof obj != "object" || obj instanceof RegExp) {
                    return self.cast(ctor, [obj]);
                }
                else {
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

            $root.object.clone = function (obj) {
                return self.extend({}, obj, ExtendOptions.DefaultClone);
            };
            $root.cast =  $root.object.cast;
            $root.extend = $root.object.extend;
            $root.create = $root.object.create;
            $root.clone = $root.object.clone;
        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));