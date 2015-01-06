/**
 * Created by comdiv on 26.09.14.
 */
  define(["the-collections-core"], function ($the) {
        return $the(function ($root) {

            var $digest = $root.object.digest;
            var ls = typeof( localStorage ) == "undefined" ?  $the.localStorage : localStorage;
            var History =  function(size, lskey){
                this.size = size || 20;
                this.items = [];
                this.index = {};
                this.version = 0;
                this.lskey = lskey;
                if(!!lskey){
                    var items = ls.getItem(this.lskey);
                    if(!!items){
                        items  = JSON.parse(items);
                        for(var i=items.length-1;i>=0;i--){
                            this.add(items[i.item]);
                        }
                    }
                }
            };

            $root.collections = $root.collections || {};

            $root.collections.History = History;

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


            History.prototype.add = function(object){
                var digest = $digest(object);
                var handler = this.index[digest];
                if(!handler){
                    handler = this.index[digest] =  {item:object};
                    if(this.items.length==this.size){
                        var todelete = this.items[this.size-1];
                        delete this.items[this.size-1];
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
                if(!!this.lskey){
                    ls.setItem(this.lskey,JSON.stringify(this.items));
                }
            }

            History.prototype.clear = function(){
                this.index = {};
                this.items = [];
                this.reset();
            }


        });
    });