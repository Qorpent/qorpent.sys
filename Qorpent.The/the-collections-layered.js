/**
 * Created by comdiv on 26.09.14.
 */
(function (define) {
    define(["./the-collections-core"], function ($the) {
        return $the(function ($root) {
            var $ex = $root.object.extend;
            var $cast = $root.object.cast;

            var LayeredDictionaryContext = function(mindepth, maxdepth,skip){
                if (null == mindepth)mindepth = 0;
                if (null == maxdepth)maxdepth = 1 << 30;
                if (null == skip)skip = 0;
                this.mindepth = Math.max(Number(mindepth), 0) || 0;
                this.maxdepth = Math.max(Number(maxdepth), 0) || 0;
                this.skip = Math.max(Number(skip), 0) || 0;
                this.maxdepth = Math.max(mindepth, maxdepth);
                this.next =  function(skip){
                    return new LayeredDictionaryContext(this.mindepth-1,this.maxdepth-1,skip?this.skip-1:this.skip);
                }
            }
            var LayeredDictionary = function () {
                if(typeof this === "undefined" || !(this instanceof LayeredDictionary)){
                    return LayeredDictionary.build.apply(null,arguments);
                }
                this.internalIndex = {};
                for (var i = 0; i < arguments.length; i++) {
                    this.apply(arguments[i]);
                }
                if(!!this.reset)this.reset();
            };
            $root.collections = $root.collections || {};
            $root.collections.LayeredDictionary = LayeredDictionary;
            $root.collections.LayeredDictionaryContext = LayeredDictionaryContext;

                LayeredDictionary.prototype = Object.create($root.collections.Enumeration.prototype);

                LayeredDictionary.prototype.reset  = function(){
                    $root.collections.Enumeration.prototype.reset.call(this);
                    this.keys = this.getKeys();
                }
                LayeredDictionary.prototype.baseNext  = function(){
                    this._index++;
                    if(this._index < this.keys.length){
                        var key = this.keys[this._index];
                        return new $root.collections.KeyValuePair(key,this.get(key));
                    }
                    return $root.collections.EndOfEnumeration;
                }



            /**
             * Родительская конфигурация
             * @type {LayeredDictionary}
             */
            LayeredDictionary.prototype.parent = null;
            LayeredDictionary.prototype.internalIndex = null;
            /**
             * Применяет объект к конфигурации, конфигурация выставляется как родительская по стеку
             * @param obj другая конфигурация или объект с настройками
             * @returns {LayeredDictionary} this (fluent)
             */
            LayeredDictionary.prototype.apply = function (obj) {
                if (obj instanceof LayeredDictionary) {
                    if (this.parent) {
                        obj.parent = this.parent;
                    }
                    this.parent = obj;
                } else if (typeof obj === "function") { //custom setup
                    obj(this);
                } else if (typeof obj === "object" && !(obj instanceof Array) && !(obj instanceof RegExp)) {
                    for (var i in obj) {
                        if (obj.hasOwnProperty(i)) {
                            this.set(i, obj[i]);
                        }
                    }
                } else {
                    throw "invalid object to apply";
                }
                if(!!this.reset)this.reset();
                return this;
            };
            /**
             * Устанавливает значение на уровне текущей конфигурации, если значение не переано
             * или является null то удаляет значение
             * @param {String} name имя параметра
             * @param [val] значение параметра
             * @return {LayeredDictionary} this (fluent)
             */
            LayeredDictionary.prototype.set = function (name, val) {
                if (typeof name != "string" || !name)throw  "name must be not empty";
                if (typeof val !== "undefined" && null != val) {
                    this.internalIndex[name] = val;
                }
                else {
                    delete this.internalIndex[name];
                }
                if(!!this.reset)this.reset();
                return this;
            };
            var proxy = function (_) {
                return _;
            };
            var exists = function (_) {
                return (!(typeof _ == "undefined" || null == _));
            };
            /**
             * Обходит конфигурацию и вызывает переданную функцию при нахождении элемента в колбэке
             * @param {String} name имя параметра
             * @param {Function} [selector] функция для возврата значения
             * @param {LayeredDictionaryContext} [hint] минимальная глубина поиска по стеку
             */
            LayeredDictionary.prototype.select = function (name, selector, hint) {
                selector = selector || proxy;
                if (typeof name != "string" || !name)throw  "name must be not empty";

                hint = $cast(LayeredDictionaryContext,hint);

                if (hint.mindepth > 0) {
                    if (null == this.parent)return selector(null);
                    return this.parent.select(name, selector, hint.next(false));
                }

                if (this.internalIndex.hasOwnProperty(name)) {
                    var result = this.internalIndex[name];
                    if(hint.skip!=0){
                        var skipresult = null;
                        if(this.parent)skipresult = this.parent.select(name, selector, hint.next(true));
                        if(null!=skipresult)result = skipresult;
                    }
                    return selector(result);
                }

                if (hint.maxdepth == 0) return selector(null);
                if (null == this.parent)return selector(null);
                return this.parent.select(name, selector, hint.next(false));
            };

            /**
             * Возвращает true при наличии значения в конфигурации
             * @param {String} name имя параметра
             * @param {LayeredDictionaryContext} [hint] подсказка к поиску
             */
            LayeredDictionary.prototype.exists = function (name, hint) {
                return this.select(name, exists,hint);
            };
            /**
             * Возвращает значение параметра
             * @param {String} name имя параметра
             * @param {LayeredDictionaryContext} [hint]
             */
            LayeredDictionary.prototype.get = function (name,hint) {
                return this.select(name, proxy, hint);
            };


            /**
             * Процедура посещения всех узлов конфигурации с вызовом колбэка с учетом глубины
             * @param {Function} callback
             * @param internalIndex
             * @returns {*}
             * @param {LayeredDictionaryContext} [hint]
             */
            LayeredDictionary.prototype.visit = function (callback, hint, internalIndex) {

                hint = $cast(LayeredDictionaryContext,hint);
                internalIndex = internalIndex || {};

                if (hint.mindepth > 0) {
                    if (!this.parent)return internalIndex;
                    return this.parent.visit(callback, hint.next(false), internalIndex);
                }

                for (var i in this.internalIndex) {
                    if (this.internalIndex.hasOwnProperty(i) && !internalIndex.hasOwnProperty(i)) {
                        internalIndex[i] = true;
                        if (!!callback) {
                            callback(this.internalIndex[i], i, this);
                        }
                    }
                }
                if (hint.maxdepth > 0 && this.parent) {
                    this.parent.visit(callback, hint.next(false), internalIndex);
                }
                return internalIndex;
            };
            /**
             * Возвращает перечень всех ключей конфигурации с учетом глубины
             * @returns {Array}
             * @param {LayeredDictionaryContext} [hint]
             */
            LayeredDictionary.prototype.getKeys = function (hint) {
                var result = [];
                this.visit(function (val, name) {
                    result.push(name);
                }, hint);
                return result;
            };
            /**
             * Возвращает всю конфигурацию заданной глубины в виде объекта
             * @returns {{}}
             * @param {LayeredDictionaryContext} [hint]
             */
            LayeredDictionary.prototype.toObject = function (hint) {
                var result = {};
                this.visit(function (val, name) {
                    result[name] = val;
                }, hint);
                return result;
            };
            LayeredDictionary.build = function(){
                var result = new LayeredDictionary();
                for(var i =0;i<arguments.length;i++){
                    var current = arguments[i];
                    if(current instanceof LayeredDictionary){
                        current.parent = result;
                        result = current;
                    }else{
                        current = new LayeredDictionary(current);
                        current.parent = result;
                        result = current;
                    }
                }
                return result;
            }





        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));