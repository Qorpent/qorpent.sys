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
                result.initClientRequest = function(options){
                    this.name = null;
                    this.sysname = null;
                    this.username = null;
                    this.useremail = null;
                    this.isdemo = false;
                    this.password = null;
                    this.success = null;
                    $the.extend(this,options);
                    var self = this;
                }
                result.initClientRequest.prototype.getHttpOptions = function(){
                    return {
                        url : "/initclient",
                        method: "POST",
                        data : $the.jsonify(this,{nulls:false,defaults:false,functions:false})
                    }
                }

                result.initClient = function(options){
                    return $rest2.executeCommand(options,"initclient",result.initClientRequest);
                }

                return result;

            }
        ]);

    });
});