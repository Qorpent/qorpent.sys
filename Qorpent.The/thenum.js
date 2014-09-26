var Iterator = Iterator || undefined;
var StopIteration = StopIteration || undefined;
var module = module || undefined;
var define = define || undefined;
var window = window || {};
if (typeof define !== 'function'){
    try{
        define = require('amdefine')(module);
    }catch(e){
        define = function(dep,mymodule){
            window.$thenum = mymodule(dep);
        }
    }
}
define ([],function(){
    var EndOfEnumeration = {};
    var StartOfEnumeration = {};
    var NUMBERCOUNTER = 1;
    var CHECKCOMPARER = 2;
    /**
     *
     * @param funcOrExpression
     * @param [hint]
     * @returns {*}
     */
    var getExpression = function (funcOrExpression, hint) {
        if (!funcOrExpression)return null;
        if (typeof funcOrExpression == "number") {
            if (!hint)return funcOrExpression;
            if (hint === NUMBERCOUNTER) {
                var count = funcOrExpression;
                return function () {
                    if (count == 0)return false;
                    count--;
                    return true;
                }
            } else {
                return funcOrExpression;
            }
        }
        if (typeof funcOrExpression == "function") {
            if (hint == CHECKCOMPARER) {
                if (funcOrExpression.toString().match(/^function\s*\(a\s*,b\s*\)/)) {
                    funcOrExpression.__iscomparer__ = true;
                }
            }
            return funcOrExpression;
        }
        if (!funcOrExpression.match(/[\.=\-+<>&|*\/!()]/)) {
            return funcOrExpression;
        }
        if (!funcOrExpression.match(/_/)) {
            funcOrExpression = "_" + funcOrExpression;
        }
        funcOrExpression = "x = function(_,idx){ try{return " + funcOrExpression + ";}catch(e){return false;}}";
        return eval(funcOrExpression);
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
                this._referencedEnum = thenum(options.referencedEnum);
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
            }
            else {
                this._object = target;
                this._nameindex = [];
                for (var i in this._object) {
                    if(this._object.hasOwnProperty(i)){
                        this._nameindex.push(i);
                    }
                }
            }
        }
    };

    Enumeration.prototype.any = function (expr) {
        return this.reset().next(getExpression(expr));
    };

    Enumeration.prototype.toArray = function (expr) {
        this.reset();
        var result = [];
        var e = getExpression(expr);
        while (this.next(e)) {
            result.push(this.current);
        }
        return result;
    };

    Enumeration.prototype.count = function (expr) {
        this.reset();
        var result = 0;
        var e = getExpression(expr);
        while (this.next(e)) {
            result++;
        }
        return result;
    };

    Enumeration.prototype.first = function (condition) {
        this.reset();
        condition = getExpression(condition);
        if (this.next(condition)) {
            return this.current;
        }
        throw "first element for this conditions not found";
    };
    Enumeration.prototype.firstOrDefault = function (condition, defaultValue) {
        this.reset();
        condition = getExpression(condition);
        if (this.next(condition)) {
            return this.current;
        }
        return defaultValue;
    };
    Enumeration.prototype.where = function (condition) {
        this.reset();
        condition = getExpression(condition);
        return new Enumeration(this, function (e) {
            e._baseEnumeration.next(condition);
            return e._baseEnumeration.current;
        });
    };
    Enumeration.prototype.union = function (other) {
        other = thenum(other);
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
        projection = getExpression(projection);
        return new Enumeration(this, function (e, eof) {
            if (e._baseEnumeration.next()) {
                var val = e._baseEnumeration.current;
                if (!!projection)val = projection(val);
                return val;
            }
            return eof;
        },null);
    };

    Enumeration.prototype.distinct = function (keyfunc) {
        keyfunc = getExpression(keyfunc);
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
        keyOrComparer = getExpression(keyOrComparer, CHECKCOMPARER);
        if (keyOrComparer && !keyOrComparer.__iscomparer__) {
            var keyFunc = keyOrComparer;
            keyOrComparer = function (a, b) {
                return keyFunc(a) - keyFunc(b);
            }
        }
        return new Enumeration(this, function (e, eof) {
            var self= this;
            if (self._bufferIter.next()) {
                return self._bufferIter.current;
            }
            return eof;
        }, {buffered: true, onBuffer: function () {
            var self = this;
            if (!!keyOrComparer) {
                self._buffer.sort(keyOrComparer);
            } else {
                self._buffer.sort();
            }

        }});
    };

    Enumeration.prototype.except = function (other, keyFunc) {
        other = thenum(other);
        keyFunc = getExpression(keyFunc);

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
        var skipper = getExpression(numOrFunc, NUMBERCOUNTER);
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
            skipper = getExpression(numOrFunc, NUMBERCOUNTER);
            skipped = false;
        }});
    };

    Enumeration.prototype.take = function (numOrFunc) {
        var skipper = getExpression(numOrFunc, NUMBERCOUNTER);
        return new Enumeration(this, function (e, eof) {
            var self = this;
            var next = self._baseEnumeration.next();
            if (!next)return eof;

            if (!skipper(self._baseEnumeration.current)) {
                return eof;
            }
            return self._baseEnumeration.current;
        }, {onReset: function () {
            skipper = getExpression(numOrFunc, NUMBERCOUNTER);
        }});
    };

    var KeyValuePair = function(key,value){
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

    Enumeration.prototype.each = function(action){
        this.reset();
        while(this.next()){
            var result = action(this.current,this.index,this.currentItem);
            if(result===false)break;
        }
    };

    Enumeration.prototype.baseNext = function(){
        if(this.current===EndOfEnumeration)return EndOfEnumeration;
        if(!!this._baseEnumeration) {
            if( this._baseEnumeration.next()){
                return this._baseEnumeration.current;
            }
            return EndOfEnumeration;
        }else if(!!this._array){
            this._index++;
            if(this._index>=this._array.length){
                return EndOfEnumeration;
            }
            return this._array[this._index];
        } else if(!!this._object){
            this._index++;
            if(this._index>=this._nameindex.length){
                return EndOfEnumeration;
            }
            var name = this._nameindex[this._index];
            return new KeyValuePair(name,this._object[name]);
        }else if(!!this._number){
            this._index++;
            if(this._index>=this._number){
                return EndOfEnumeration;
            }
            return this._index;
        }
        else{
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
        this._bufferIter = thenum(this._buffer);
        if (!!this._referencedEnum) {
            this._refbuffer = this._referencedEnum.toArray();
            this._refbufferIter = thenum(this._refbuffer);
        }
    };

    if (typeof Iterator !== "undefined" && typeof  StopIteration != "undefined"){
        var v8Iterator = function(enumeration){
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


    var thenum = function (target) {
        if (target instanceof Enumeration)return target;
        return new Enumeration(target);
    };


    thenum.Enumeration = Enumeration;
    thenum.KeyValuePair = KeyValuePair;
    thenum.EndOfEnumeration = EndOfEnumeration;
    thenum.StartOfEnumeration = StartOfEnumeration;
    thenum.getExpression = getExpression;

    return thenum;
});