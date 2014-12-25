


/**
 * Created by comdiv on 04.11.2014.
 */
    define(["the-angular"], function ($the) {
        return $the(function(root, privates)
        {
            if(null==root.modules)return;
            root.modules.all.factory('theViewContextHelper', [
                    "$rootScope",
                    function($rs){
                        return {
                            addEvents : function (target, events){
                                if(!(events && target))return;
                                events  = Array.isArray(events) ? events : [events];
                                events.forEach (function(_){
                                    var eventname = _.toUpperCase()+"EVENT";
                                    target[eventname] = eventname;
                                    var subscribeName = "on"+_[0].toUpperCase()+_.substring(1);
                                    target[subscribeName] = (function(handler){
                                        $rs.$on(eventname,handler);
                                    });
                                    var callName = _;
                                    target[callName] = function(data, context){
                                        $rs.$broadcast(eventname, data, context);
                                    }
                                });
                            }
                        }
                    }
                ]);

        });
    });