/**
 * Created by comdiv on 04.11.2014.
 */
define(["the-angular","the-rest2"], function ($the) {
    return $the(function (root, privates) {
        root.modules.all.factory('the-initclient', [
            "$http","the-rest2",
            function($http,$rest2) {
                var result = {

                }
                result.clientRequest = function(options){
                    this.name = null;
                    this.sysname = null;
                    this.username = null;
                    this.useremail = null;
                    this.isdemo = false;
                    this.password = null;
                    this.success = null;
                    this.operation = "init";
                    this.expire = null;
                    $the.extend(this,options);
                    var self = this;
                }
                result.clientRequest.prototype.getHttpOptions = function(){
                    return {
                        url : "/client",
                        method: "POST",
                        data : $the.jsonify(this,{nulls:false,defaults:false,functions:false})
                    }
                }

                result.initClient = function(options){
                    return $rest2.executeCommand(options,"initclient",result.clientRequest);
                }

                result.toWork = function(name){
                    var options = {sysname:name, operation:"towork"};
                    return $rest2.executeCommand(options,"towork", result.clientRequest)
                }

                result.toDemo = function(name){
                    var options = {sysname:name, operation:"todemo"};
                    return $rest2.executeCommand(options,"todemo", result.clientRequest)
                }

                result.setExpire = function(name, expire){
                    var options = {sysname:name, operation:"setexpire", expire:expire};
                    return $rest2.executeCommand(options,"setexpire", result.clientRequest)
                }

                return result;

            }
        ]);

    });
});