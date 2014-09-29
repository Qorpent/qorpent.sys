(function (define) {
    define(["./the-object"], function ($the) {
        return $the(function (root, privates) {
            var EndOfEnumeration = {};
            var StartOfEnumeration = {};


            var $ex = privates._collectionEx = function (valOrFunc, options) {
                if (!!$the.expression) {
                    return $the.expression(valOrFunc, options);
                } else {
                    if (typeof valOrFunc === "undefined" || null == valOrFunc) {
                        return function () {
                            return true;
                        }
                    } else if (typeof valOrFunc === "function") {
                        return valOrFunc;
                    } else {
                        return  function (_) {
                            return _ === valOrFunc;
                        }
                    }
                }
            };

            /**
             *
             * @param target target object of any type to be wrapped as enumeration
             * @param [onnext] delegate funciton tha will be used as special onNext
             * @param [options]
             * @constructor
             */
            var Enumeration = function (target, onnext, options) {
                this.current = StartOfEnumeration;
                this.onNext = onnext;
                this._index = -1;
                if (!!options) {
                    if (!!options.referencedEnum) {
                        this._referencedEnum = collections(options.referencedEnum);
                    }
                    if (!!options.buffered) {
                        this._buffered = true;
                    }
                    if (!!options.onBuffer) {
                        this.onBuffer = options.onBuffer.bind(this);
                    }
                    if (!!options.onReset) {
                        this.onReset = options.onReset.bind(this);
                    }
                }
                if (!!target) {
                    if (target instanceof Enumeration) {
                        this._baseEnumeration = target;
                    } else if (Array.isArray(target) || typeof target == "string") {
                        this._array = target;
                    } else if (typeof target == "number") {
                        this._number = target;
                    } else {
                        this._object = target;
                        this._nameindex = [];
                        for (var i in this._object) {
                            if (this._object.hasOwnProperty(i)) {
                                this._nameindex.push(i);
                            }
                        }
                    }
                }
            };


            Enumeration.prototype.toArray = function (expr) {
                this.reset();
                var result = [];
                var e = $ex(expr);
                while (this.next(e)) {
                    result.push(this.current);
                }
                return result;
            };


            var KeyValuePair = function (key, value) {
                this.key = key;
                this.value = value;
            };


            Enumeration.prototype.next = function (condition) {
                if (this.current === EndOfEnumeration)return false;
                if (this.current === StartOfEnumeration && this._buffered) {
                    this._loadBuffer();
                    if (!!this.onBuffer) {
                        this.onBuffer(this);
                    }
                }
                var newvalue = null;
                while (true) {
                    if (this.onNext) {
                        newvalue = this.onNext(this, EndOfEnumeration);
                    } else {
                        newvalue = this.baseNext();
                    }
                    this.currentItem = newvalue;
                    var realvalue = newvalue;
                    if (!!this.onGetValue) {
                        realvalue = this.onGetValue(realvalue);
                    }
                    this.current = realvalue;
                    this.index++;
                    if (newvalue === EndOfEnumeration)break;
                    if (!!condition) {
                        if (condition(this.current, this.index)) {
                            break;
                        }
                    } else {
                        break;
                    }
                }

                return newvalue !== EndOfEnumeration;


            };

            Enumeration.prototype.each = function (action) {
                this.reset();
                while (this.next()) {
                    var result = action(this.current, this.index, this.currentItem);
                    if (result === false)break;
                }
            };

            Enumeration.prototype.baseNext = function () {
                if (this.current === EndOfEnumeration)return EndOfEnumeration;
                if (!!this._baseEnumeration) {
                    if (this._baseEnumeration.next()) {
                        return this._baseEnumeration.current;
                    }
                    return EndOfEnumeration;
                } else if (!!this._array) {
                    this._index++;
                    if (this._index >= this._array.length) {
                        return EndOfEnumeration;
                    }
                    return this._array[this._index];
                } else if (!!this._object) {
                    this._index++;
                    if (this._index >= this._nameindex.length) {
                        return EndOfEnumeration;
                    }
                    var name = this._nameindex[this._index];
                    return new KeyValuePair(name, this._object[name]);
                } else if (!!this._number) {
                    this._index++;
                    if (this._index >= this._number) {
                        return EndOfEnumeration;
                    }
                    return this._index;
                } else {
                    return EndOfEnumeration;
                }
            };


            Enumeration.prototype.reset = function () {
                this._index = -1;
                this.index = -1;
                this.current = StartOfEnumeration;
                if (!!this._baseEnumeration) {
                    this._baseEnumeration.reset();
                }
                if (!!this._referencedEnum) {
                    this._referencedEnum.reset();
                }
                if (!!this._buffered) {
                    this._buffer = null;
                    this._refbuffer = null;
                    this._bufferIter = null;
                    this._refbufferIter = null;
                }
                if (!!this.onReset) {
                    this.onReset();
                }
                return this;
            };

            Enumeration.prototype._loadBuffer = function () {
                this._buffer = this._baseEnumeration.toArray();
                this._bufferIter = collections(this._buffer);
                if (!!this._referencedEnum) {
                    this._refbuffer = this._referencedEnum.toArray();
                    this._refbufferIter = collections(this._refbuffer);
                }
            };

            if (typeof Iterator !== "undefined" && typeof  StopIteration != "undefined") {
                var v8Iterator = function (enumeration) {
                    this._e = enumeration;
                };
                v8Iterator.prototype.next = function () {
                    if (this._e.next())return this._e.current;
                    throw StopIteration;
                };
                //noinspection JSUnusedGlobalSymbols
                Enumeration.prototype.__iterator__ = function () {
                    return new v8Iterator(this);
                };
            }

            var collections = function (target) {
                if (target instanceof Enumeration)return target;
                return new Enumeration(target);
            };
            collections.Enumeration = Enumeration;
            collections.KeyValuePair = KeyValuePair;
            collections.EndOfEnumeration = EndOfEnumeration;
            collections.StartOfEnumeration = StartOfEnumeration;

            if (root.collections) {
                var oldcollections = root.collections;

                for (var i in oldcollections) {
                    if (oldcollections.hasOwnProperty(i)) {
                        collections[i] = oldcollections[i];
                    }
                }
            }
            root.collections = collections;


        })

    });
})(typeof define === "function" ? define : require('amdefine')(module));