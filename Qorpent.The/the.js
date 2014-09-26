/**
 * Created by comdiv on 26.09.14.
 */
(function(define){
   define([],function(){
    //incapsulation of internal $the manager
    var privates = {};
    var the = function(plugin){
        if(!!plugin){
            plugin(the,privates);
        }
        return the;
    }
    return the;
   });
})(typeof define === "function" ? define : require('amdefine')(module))