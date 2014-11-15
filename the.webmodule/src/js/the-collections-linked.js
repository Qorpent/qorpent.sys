
    define(["the-collections-core"], function ($the) {
        return $the(function (root, privates) {
            var $ = root.collections;
            var $ex = privates._collectionEx;
            var cast = $the.cast;
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
            Object.defineProperty(LinkedItem.prototype,"index",{
                writeable: false,
                get: function(){
                    if(!this.previous)return 0;
                    return this.previous.index + 1;
                }
            });
            var LinkedListMergeOptions = function(){
                this.replace = null;
                this.remove = null;
            }

            var LinkedList = function (enumeration, options) {
                this.first = null;
                this.last = null;
                this.replaceOnMerge = false;
                this.removeOnMerge = false;
                if (!!options) {
                    $the.extend(this,options);
                    if (!!options.onKey) {
                        this.onKey = ($ex(options.onKey)).bind(this);
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
                options = $the.cast(LinkedListMergeOptions,options);
                var result = {removed: [], created: [], updated: []};
                var self = this;
                var doremove = options.remove == null?this.removeOnMerge:options.remove;
                var doreplace = options.replace == null?this.replaceOnMerge:options.replace;
                var others = {};
                $(other).each(function (_) {
                    others[self.getKey(_)] = _;
                });
                if (doremove) {
                    var todelete = [];
                    this.eachItem(function(item){
                        var thiskey = self.getKey(item.value);
                        if (!others.hasOwnProperty(thiskey)) {
                            todelete.push(item);
                        }
                    })
                    todelete.forEach(function (_) {
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

            LinkedList.prototype.trigger = function (op,n,o) {
                if(!!this.onItemTrigger){
                    this.onItemTrigger(op,n||null,o||null);
                }
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
                condition = $ex(condition);
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

            LinkedList.prototype.itemByIndex = function(idx){
                if(idx<0)return undefined;
                var current = this.first || undefined;
                if(0==idx)return current;
                for(var i=0;i<idx;i++){
                    if(!current.next)return undefined;
                    current = current.next;
                }
                return current;
            };
            LinkedList.prototype.byIndex = function(idx){
                var item = this.itemByIndex(idx);
                if(typeof item === "undefined")return undefined;
                return item.value;
            };
            Object.defineProperty(LinkedList.prototype,"currentIndex",{
                get:function(){
                    if(!this.currentItem)return -1;
                    return this.currentItem.index;
                },
                set:function(value){
                    this.gotoIndex(value);
                }
            });


            LinkedList.prototype.indexOf = function(any){
                var target = this.__resolveTarget(any);
                if(!target)return -1;
                return target.index;
            };
            LinkedList.prototype.gotoIndex = function (index,offset) {
                var basis = this.itemByIndex(index);
                if (null == basis)return;
                var i;
                if (offset > 0) {
                    for (i = 0; i < offset; i++) {
                        if (basis.next)basis = basis.next; else {
                            basis = undefined;
                            break;
                        }
                    }
                } else if (offset < 0) {
                    for (i = 0; i > offset; i--) {
                        if (basis.previous)basis = basis.previous; else {
                            basis = undefined;
                            break;
                        }
                    }
                }
                if(basis===undefined)this.reset();
                else{
                    this.currentItem = basis;
                    this.current = basis.value;
                    this._index = basis.index;
                }

            };
            LinkedList.prototype.goto = function (key, offset) {
                var basis = this.find(key);
                if (null == basis)return;
                var i;
                if (offset > 0) {
                    for (i = 0; i < offset; i++) {
                        if (basis.next)basis = basis.next; else {
                            basis = undefined;
                            break;
                        }
                    }
                } else if (offset < 0) {
                    for (i = 0; i > offset; i--) {
                        if (basis.previous)basis = basis.previous; else {
                            basis = undefined;
                            break;
                        }
                    }
                }
                if(basis===undefined){
                    this.reset();
                }
                else{
                    this.currentItem = basis;
                    this.current = basis.value;
                    this._index = basis.index;
                }

            };

            LinkedList.prototype.each = function (action) {
                var current = this.first;
                while(current){
                    action(current.value);
                    current = current.next;
                }
            };
            LinkedList.prototype.eachItem = function (action) {
                var current = this.first;
                while(current){
                    action(current);
                    current = current.next;
                }
            };


            LinkedList.prototype.append = function (any) {
                any = this.setupItem(any);
                any.previous = this.last;
                if (this.last) {
                    this.last.next = any;
                }
                this.last = any;
                if (!this.first)this.first = any;
                this.trigger("insert",any);
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
                this.trigger("insert",any);
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
                this.trigger("insert",any);
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
                this.trigger("insert",any);
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
                this.trigger("update",any,target);
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
                this.trigger("delete",null,target);
                return this;
            };

            var CycleList = function (source,options) {
                LinkedList.call(this, source,options);
            };
            CycleList.prototype = Object.create(LinkedList.prototype);
            CycleList.prototype.pop = function () {
                /*if (null == this.first) {
                    return null;
                }*/
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
                        if (basis.next)basis = basis.next; else basis = this.first;
                    }
                } else if (offset < 0) {
                    for (i = 0; i > offset; i--) {
                        if (basis.previous)basis = basis.previous; else basis = this.last;
                    }
                }
                this.currentItem = basis;
                this.current = basis.value;
                this._index = basis.index;
            };


            var Stack = function (enumeration,options) {
                LinkedList.call(this,null,options);
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

            var Queue = function (enumeration,options) {
                LinkedList.call(this, enumeration,options);

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
