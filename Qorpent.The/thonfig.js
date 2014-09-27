/**
 * Created by comdiv on 26.09.14.
 */
(function (define) {
    define(["./the"], function ($the) {
        return $the(function ($root) {
            var config = function () {
                if(typeof this === "undefined" || !(this instanceof config)){
                    return config.build.apply(null,arguments);
                }
                this.index = {};
                for (var i = 0; i < arguments.length; i++) {
                    this.apply(arguments[i]);
                }
            };
            /**
             * Родительская конфигурация
             * @type {config}
             */
            config.prototype.parent = null;
            config.prototype.index = null;
            /**
             * Применяет объект к конфигурации, конфигурация выставляется как родительская по стеку
             * @param obj другая конфигурация или объект с настройками
             * @returns {config} this (fluent)
             */
            config.prototype.apply = function (obj) {
                if (obj instanceof config) {
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
                return this;
            };
            /**
             * Устанавливает значение на уровне текущей конфигурации, если значение не переано
             * или является null то удаляет значение
             * @param {String} name имя параметра
             * @param [val] значение параметра
             * @return {config} this (fluent)
             */
            config.prototype.set = function (name, val) {
                if (typeof name != "string" || !name)throw  "name must be not empty";
                if (typeof val !== "undefined" && null != val) {
                    this.index[name] = val;
                }
                else {
                    delete this.index[name];
                }
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
             * @param {Number} [mindepth] минимальная глубина поиска по стеку
             * @param {Number} [maxdepth] максимальная глубина поиска по стеку
             * @param {Function} selector функция для возврата значения
             */
            config.prototype.select = function (name, selector, mindepth, maxdepth) {
                selector = selector || proxy;
                if (typeof name != "string" || !name)throw  "name must be not empty";

                if (null == mindepth)mindepth = 0;
                if (null == maxdepth)maxdepth = 1 << 30;
                mindepth = Math.max(Number(mindepth), 0) || 0;
                maxdepth = Math.max(Number(maxdepth), 0) || 0;
                maxdepth = Math.max(mindepth, maxdepth);

                if (mindepth > 0) {
                    if (null == this.parent)return selector(null);
                    return this.parent.select(name, selector, mindepth - 1, maxdepth - 1);
                }

                if (this.index.hasOwnProperty(name)) {
                    return selector(this.index[name]);
                }

                if (maxdepth == 0) return selector(null);
                if (null == this.parent)return selector(null);
                return this.parent.select(name, selector, mindepth - 1, maxdepth - 1);
            };

            /**
             * Возвращает true при наличии значения в конфигурации
             * @param {String} name имя параметра
             * @param {Number} [mindepth] минимальная глубина поиска по стеку
             * @param {Number} [maxdepth] максимальная глубина поиска по стеку
             */
            config.prototype.exists = function (name, mindepth, maxdepth) {
                return this.select(name, exists, mindepth, maxdepth);
            };
            /**
             * Возвращает значение параметра
             * @param {String} name имя параметра
             * @param {Number} [mindepth] минимальная глубина поиска по стеку
             * @param {Number} [maxdepth] максимальная глубина поиска по стеку
             */
            config.prototype.get = function (name, mindepth, maxdepth) {
                return this.select(name, proxy, mindepth, maxdepth);
            };


            /**
             * Процедура посещения всех узлов конфигурации с вызовом колбэка с учетом глубины
             * @param {Function} callback
             * @param [mindepth]
             * @param [maxdepth]
             * @param index
             * @returns {*}
             */
            config.prototype.visit = function (callback, mindepth, maxdepth, index) {

                if (null == mindepth)mindepth = 0;
                if (null == maxdepth)maxdepth = 1 << 30;
                mindepth = Math.max(Number(mindepth), 0) || 0;
                maxdepth = Math.max(Number(maxdepth), 0) || 0;
                maxdepth = Math.max(mindepth, maxdepth);

                index = index || {};

                if (mindepth > 0) {
                    if (!this.parent)return index;
                    return this.parent.visit(callback, mindepth - 1, maxdepth - 1, index);
                }

                for (var i in this.index) {
                    if (this.index.hasOwnProperty(i) && !index.hasOwnProperty(i)) {
                        index[i] = true;
                        if (!!callback) {
                            callback(this.index[i], i, this);
                        }
                    }
                }
                if (maxdepth > 0 && this.parent) {
                    this.parent.visit(callback, mindepth - 1, maxdepth - 1, index);
                }
                return index;
            };
            /**
             * Возвращает перечень всех ключей конфигурации с учетом глубины
             * @returns {Array}
             * @param {Number} [mindepth]
             * @param {Number} [maxdepth]
             */
            config.prototype.getKeys = function (mindepth, maxdepth) {
                var result = [];
                this.visit(function (val, name) {
                    result.push(name);
                }, mindepth, maxdepth);
                return result;
            };
            /**
             * Возвращает всю конфигурацию заданной глубины в виде объекта
             * @param {Number} [mindepth]
             * @param {Number} [maxdepth]
             * @returns {{}}
             */
            config.prototype.toObject = function (mindepth, maxdepth) {
                var result = {};
                this.visit(function (val, name) {
                    result[name] = val;
                }, mindepth, maxdepth);
                return result;
            };
            $root.collections = $root.collections || {};
            $root.collections.Config = config;
            config.build = function(){
                var result = new config();
                for(var i =0;i<arguments.length;i++){
                    var current = arguments[i];
                    if(current instanceof config){
                       current.parent = result;
                        result = current;
                    }else{
                        current = new config(current);
                        current.parent = result;
                        result = current;
                    }
                }
                return result;
            }

        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));