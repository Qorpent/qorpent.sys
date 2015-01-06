/**
 * Created by comdiv on 26.09.14.
 */
  define(["the-collections-core"], function ($the) {
        return $the(function ($root) {

            var $digest = $root.object.digest;
            var ls = typeof( localStorage ) == "undefined" ?  $the.localStorage : localStorage;
            var HistoryOptions = function(){
                this.size = 20;
                this.lskey = "";
                this.fixedOrder = true;
                this.jsonify = true;
                this.jsonifyOptions = {};
            };
            var History =  function(options){
                this.options = $the.cast(HistoryOptions, options);
                this.items = [];
                this.index = {};
                this.version = 0;
                if(!!this.options.lskey){
                    var items = ls.getItem(this.options.lskey);
                    if(!!items){
                        items  = JSON.parse(items);
                        for(var i=items.length-1;i>=0;i--){
                            this.add(items[i.item],true);
                        }
                    }
                }
            };

            $root.collections = $root.collections || {};

            $root.collections.History = History;
            $root.collections.HistoryOptions = HistoryOptions;

            History.prototype = Object.create($root.collections.Enumeration.prototype);

            History.prototype.baseNext = function () {
                this._index++;
                if (this._index < this.items.length) {
                    return this.items[this._index].item;
                }
                return $root.collections.EndOfEnumeration;
            };

            History.prototype.getItems = function(){
                var result = [];
                this.items.forEach(function(_){
                   result.push(_.item);
                });
                return result;
            };


            History.prototype.add = function(object, nowritels){
                if(this.options.fixedOrder){
                    object = $the.object.fixedOrder(object);
                }
                if(this.options.jsonify){
                    object = $the.jsonify(object,this.options.jsonifyOptions);
                }
                var digest = $digest(object);
                var handler = this.index[digest];
                if(!handler){
                    handler = this.index[digest] =  {item:object};
                    if(this.items.length==this.options.size){
                        var todelete = this.items[this.options.size-1];
                        delete this.items[this.options.size-1];
                        var digest = $digest(todelete.item);
                        delete this.index[digest];
                    }
                    this.items.push(handler);
                }
                handler.version = this.version++;
                this.items.sort(function(a,b){
                    if(a.version> b.version){
                        return -1;
                    }
                    if(a.version < b.version){
                        return 1;
                    }
                    return 0;
                });
                if(!!this.options.lskey && !nowritels){
                    ls.setItem(this.options.lskey,JSON.stringify(this.items));
                }
            }

            History.prototype.clear = function(){
                this.index = {};
                this.items = [];
                this.reset();
            }


        });
    });