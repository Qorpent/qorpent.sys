(function (define) {
    define(["./the-collections-core"], function ($the) {
        return $the(function (root, privates) {
            var $ = root.collections;
            /**
             *
             * @param value
             * @constructor
             */
            var LinkedItem = function (value) {
                this.value = value;
                this.previous = null;
                this.next = null;
                this.key = null;
            };

            var LinkedList = function (enumeration, options) {
                this.first = null;
                this.last = null;
                if (!!options) {
                    if (!!options.onKey) {
                        this.onKey = ($the.expression(options.onKey)).bind(this);
                    }
                }
                if (enumeration) {
                    enumeration = $(enumeration);
                    while (enumeration.next()) {
                        this.append(enumeration.current);
                    }
                }
            };
            LinkedList.prototype = Object.create($.Enumeration.prototype);

            LinkedList.prototype.merge = function (other, options) {
                var result = {removed: [], created: [], updated: []};
                var self = this;
                var doremove = !!options && !!options.remove;
                var doreplace = !!options && !!options.replace;
                var others = {};
                $(other).each(function (_) {
                    others[self.getKey(_)] = _;
                });
                if (doremove) {
                    var todelete = [];
                    this.reset();
                    while (this.next()) {
                        var val = this.currentItem;
                        var thiskey = this.getKey(val.value);
                        if (!others.hasOwnProperty(thiskey)) {
                            todelete.push(this.currentItem);
                        }
                    }
                    $(todelete).each(function (_) {
                        result.removed.push(_.value);
                        self.remove(_);
                    });
                }
                var currents = {};
                this.each(function (_) {
                    currents[self.getKey(_)] = _;
                });
                $(other).each(function (_) {
                    var newkey = self.getKey(_);
                    if (!currents.hasOwnProperty(newkey)) {
                        result.created.push(_);
                        if (!!this.push) {
                            self.push(_);
                        } else {
                            self.append(_);
                        }
                    } else if (doreplace) {
                        result.updated.push(_);
                        self.replace(_, newkey);
                    }
                });
                return result;
            };

            LinkedList.prototype.onGetValue = function (i) {
                if (i instanceof LinkedItem) return i.value;
                return i;
            };
            LinkedList.prototype.baseNext = function () {
                if ($.StartOfEnumeration === this.current) {
                    if (!this.first)return $.EndOfEnumeration;
                    return this.first;
                }
                if (!this.currentItem) return this.first;
                if (!this.currentItem.next)return  $.EndOfEnumeration;
                return this.currentItem.next;
            };
            LinkedList.prototype.find = function (condition) {
                condition = $the.expression(condition);
                var current = this.first;
                while (current) {
                    if (typeof condition == "function") {

                        if (condition(current.key))return current;
                        if (condition(current.value))return current;
                        if (condition(current))return current;
                    }
                    if (condition == current.key)return current;
                    if (condition == current.value)return current;
                    current = current.next;
                }
                return null;
            };

            LinkedList.prototype.append = function (any) {
                any = this.setupItem(any);
                any.previous = this.last;
                if (this.last) {
                    this.last.next = any;
                }
                this.last = any;
                if (!this.first)this.first = any;
                return this;
            };
            LinkedList.prototype.getKey = function (any) {
                if (!!this.onKey) {
                    return this.onKey(any);
                }
                return any;
            };
            LinkedList.prototype.setupItem = function (any) {
                if (!(any instanceof LinkedItem)) {
                    any = new LinkedItem(any);
                }
                if (!!this.onKey) {
                    any.key = this.onKey(any.value);
                }
                return any;
            };
            LinkedList.prototype.prepend = function (any) {
                any = this.setupItem(any);
                any.next = this.first;
                if (this.first) {
                    this.first.previous = any;
                }
                this.first = any;
                if (!this.last)this.last = any;
                return this;
            };

            LinkedList.prototype.insertBefore = function (any, key) {
                var target = this.__resolveTarget(key);
                if (!target) {
                    throw "cannot find target for insert"
                }
                any = this.setupItem(any);
                any.next = target;
                any.previous = target.previous;
                if (target.previous) {
                    target.previous.next = any;
                }
                target.previous = any;
                if (this.first == target) {
                    this.first = any;
                }
                return this;
            };
            LinkedList.prototype.insertAfter = function (any, key) {
                var target = this.__resolveTarget(key);
                if (!target) {
                    throw "cannot find target for insert"
                }
                any = this.setupItem(any);
                any.previous = target;
                any.next = target.next;
                if (target.next) {
                    target.next.previous = any;
                }
                target.next = any;
                if (this.last == target) {
                    this.last = any;
                }
                return this;
            };
            LinkedList.prototype.replace = function (any, key) {
                var target = this.__resolveTarget(key);
                if (!target) {
                    throw "cannot getByKey target for insert"
                }
                any = this.setupItem(any);
                any.previous = target.previous;
                any.next = target.next;
                if (any.previous)any.previous.next = any;
                if (any.next)any.next.previous = any;
                if (this.first == target) {
                    this.first = any;
                }
                if (this.last == target) {
                    this.last = any;
                }
                return this;
            };
            LinkedList.prototype.__resolveTarget = function (keyOrTarget) {
                if (!keyOrTarget)return this.currentItem || this.first;
                if (keyOrTarget instanceof LinkedItem)return keyOrTarget;
                return this.find(keyOrTarget);
            };
            LinkedList.prototype.remove = function (key) {
                var target = this.__resolveTarget(key);
                if (!target) {
                    throw "cannot getByKey target for insert"
                }
                if (target.previous) {
                    target.previous.next = target.next;
                }
                if (target.next) {
                    target.next.previous = target.previous;
                }
                if (this.first == target) {
                    this.first = target.next;
                }
                if (this.last == target) {
                    this.last = target.previous;
                    if (!this.last) {
                        this.last = this.first;
                    }
                }
                if (this.currentItem === target) {
                    this.currentItem = target.next | target.previous | this.first;
                    this.current = this.currentItem.value;
                }
                return this;
            };

            var CycleList = function (source) {
                LinkedList.call(this, source);
            };
            CycleList.prototype = Object.create(LinkedList.prototype);
            CycleList.prototype.pop = function () {
                if (null == this.first) {
                    return null;
                }
                if (this.next()) {
                    return this.current;
                }
                this.reset();
                this.next();
                return this.current;
            };
            CycleList.prototype.push = function (any) {
                this.insertAfter(any);
            };
            CycleList.prototype.goto = function (key, offset) {
                var basis = this.find(key);
                if (null == basis)return;
                var i;
                if (offset > 0) {
                    for (i = 0; i < offset; i++) {
                        if (basis.next)basis = basis.next;
                        else basis = this.first;
                    }
                } else if (offset < 0) {
                    for (i = 0; i > offset; i--) {
                        if (basis.previous)basis = basis.previous;
                        else basis = this.last;
                    }
                }
                this.currentItem = basis;
                this.current = basis.value;
            };

            var Stack = function (enumeration) {
                LinkedList.call(this);
                if (enumeration) {
                    var self = this;
                    $(enumeration).each(function (_) {
                        self.push(_);
                    });
                }
            };
            Stack.prototype = Object.create(LinkedList.prototype);
            Stack.prototype.push = function (any) {
                this.prepend(any);
            };
            Stack.prototype.pop = function () {
                if (null == this.first)return null;
                var result = this.first.value;
                this.remove(this.first);
                this.reset();
                return result;
            };

            var Queue = function (enumeration) {
                LinkedList.call(this, enumeration);

            };
            Queue.prototype = Object.create(LinkedList.prototype);
            Queue.prototype.push = function (any) {
                this.append(any);
            };
            Queue.prototype.pop = Stack.prototype.pop;

            $.Enumeration.prototype.toList = function () {
                return new LinkedList(this);
            };
            $.Enumeration.prototype.toStack = function () {
                return new Stack(this);
            };
            $.Enumeration.prototype.toQueue = function () {
                return new Queue(this);
            };
            $.Enumeration.prototype.toCycle = function () {
                return new CycleList(this);
            };

            $.LinkedList = LinkedList;
            $.LinkedItem = LinkedItem;
            $.CycleList = CycleList;
            $.Stack = Stack;
            $.Queue = Queue;
        });
    });
})(typeof define === "function" ? define : require('amdefine')(module))