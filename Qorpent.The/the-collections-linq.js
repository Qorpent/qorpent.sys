(function (define) {
    define(["./the-collections-core","./the-expression"], function ($the) {
        return $the(function (root,privates) {
            var Enumeration = $the.collections.Enumeration;
            var $ex = privates._collectionEx;
            var collections = $the.collections;

            Enumeration.prototype.any = function (expr) {
                return this.reset().next($ex(expr));
            };

            Enumeration.prototype.count = function (expr) {
                this.reset();
                var result = 0;
                var e = $ex(expr);
                while (this.next(e)) {
                    result++;
                }
                return result;
            };

            Enumeration.prototype.first = function (condition) {
                this.reset();
                condition = $ex(condition);
                if (this.next(condition)) {
                    return this.current;
                }
                throw "first element for this conditions not found";
            };
            Enumeration.prototype.firstOrDefault = function (condition, defaultValue) {
                this.reset();
                condition = $ex(condition);
                if (this.next(condition)) {
                    return this.current;
                }
                return defaultValue;
            };
            Enumeration.prototype.where = function (condition) {
                this.reset();
                condition = $ex(condition);
                return new Enumeration(this, function (e) {
                    e._baseEnumeration.next(condition);
                    return e._baseEnumeration.current;
                });
            };
            Enumeration.prototype.union = function (other) {
                other = collections(other);
                this.reset();
                other.reset();
                return new Enumeration(this, function (e, eof) {
                    if (e._baseEnumeration.next()) {
                        return e._baseEnumeration.current;
                    }
                    if (other.next()) {
                        return other.current;
                    }
                    return eof;
                }, {referencedEnum: other});
            };

            Enumeration.prototype.select = function (projection) {
                projection = $ex(projection);
                return new Enumeration(this, function (e, eof) {
                    if (e._baseEnumeration.next()) {
                        var val = e._baseEnumeration.current;
                        if (!!projection)val = projection(val);
                        return val;
                    }
                    return eof;
                }, null);
            };

            Enumeration.prototype.distinct = function (keyfunc) {
                keyfunc = $ex(keyfunc);
                var cache = {};
                return new Enumeration(this, function (e, eof) {
                    while (e._baseEnumeration.next()) {
                        var val = e._baseEnumeration.current;
                        var key = val;
                        if (!!keyfunc) {
                            key = keyfunc(val);
                        }
                        if (!cache.hasOwnProperty(key)) {
                            cache[key] = val;
                            return val;
                        }
                    }
                    return eof;
                }, {onReset: function () {
                    cache = {}
                }});
            };

            Enumeration.prototype.reverse = function () {
                return new Enumeration(this, function (e, eof) {
                    var self = this;
                    if (self._index == -1)return eof;
                    return self._buffer[self._index--];
                }, {buffered: true, onBuffer: function () {
                    var self = this;
                    this._index = self._buffer.length - 1;
                }});
            };

            Enumeration.prototype.order = function (keyOrComparer) {
                var e = $ex(keyOrComparer, {annotate:true});
                if (keyOrComparer && !e.annotation.comparer) {
                    var keyFunc = e;
                    e = function (a, b) {
                        return keyFunc(a) - keyFunc(b);
                    }
                }
                return new Enumeration(this, function (e, eof) {
                    var self = this;
                    if (self._bufferIter.next()) {
                        return self._bufferIter.current;
                    }
                    return eof;
                }, {buffered: true, onBuffer: function () {
                    var self = this;
                    if (!!e) {
                        self._buffer.sort(e);
                    } else {
                        self._buffer.sort();
                    }

                }});
            };

            Enumeration.prototype.except = function (other, keyFunc) {
                other = collections(other);
                keyFunc = $ex(keyFunc);

                return new Enumeration(this, function (e, eof) {

                    while (e._bufferIter.next()) {
                        if (!!keyFunc) {
                            if (!(function (key) {
                                return e._refbufferIter.any(function (_) {
                                    return keyFunc(_) == key;
                                });
                            })(keyFunc(e._bufferIter.current)))return e._bufferIter.current;
                        } else {
                            var exists = e._refbufferIter.any(function (_) {
                                return _ == e._bufferIter.current;
                            });
                            if (!exists)return e._bufferIter.current;
                        }
                    }
                    return eof;
                }, {buffered: true, referencedEnum: other});
            };


            Enumeration.prototype.skip = function (numOrFunc) {
                var skipper = $ex(numOrFunc,  {counter:true});
                var skipped = false;
                return new Enumeration(this, function (e, eof) {
                    var self = this;
                    while (!skipped) {
                        var next = self._baseEnumeration.next();
                        if (!next)return eof;
                        if (!skipper(self._baseEnumeration.current)) {
                            skipped = true;
                            return self._baseEnumeration.current;
                        }
                    }
                    if (self._baseEnumeration.next()) {
                        return self._baseEnumeration.current;
                    }
                    return eof;
                }, {onReset: function () {
                    skipper = $ex(numOrFunc,  {counter:true});
                    skipped = false;
                }});
            };

            Enumeration.prototype.take = function (numOrFunc) {
                var skipper = $ex(numOrFunc, {counter:true});
                return new Enumeration(this, function (e, eof) {
                    var self = this;
                    var next = self._baseEnumeration.next();
                    if (!next)return eof;

                    if (!skipper(self._baseEnumeration.current)) {
                        return eof;
                    }
                    return self._baseEnumeration.current;
                }, {onReset: function () {
                    skipper = $ex(numOrFunc,  {counter:true});
                }});
            };


        })

    });
})(typeof define === "function" ? define : require('amdefine')(module));