/**
 * Created by comdiv on 14.05.2015.
 */
/**
 * Created by comdiv on 04.11.2014.
 */
define(["the-angular", "the-action"], function ($the, template) {
    return $the(function (root, privates) {
        if (null == root.modules)return;
        var loginResult = function (result, error, redirect) {
            this.result = result || false;
            this.error = result || "";
            this.redirect = redirect || "";
        };
        var loginRequest = function(login,pass,resource,redirect){
            this.login = login;
            this.pass = pass;
            this.resource = resource || "";
            this.redirect = redirect || "";
        };
        var logonAction = new $the.action({url: "/logon", result: loginResult, arguments:loginRequest});
        var logoutAction = new $the.action({url: "/logout"});
        var isauthAction = new $the.action({url: "/isauth"});
        var login = $the.login || ($the.login = {});
        var loginActions = login.actions || (login.actions = {});
        loginActions.login = logonAction;
        loginActions.logout = logoutAction;
        loginActions.isauth = isauthAction;

        login.isauth = function (callback) { //callback(result, error)
            login.actions.isauth({
                success: function (result) {
                    if (!!callback) {
                        callback(result);
                    }
                },
                error: function (error) {
                    if (!!callback) {
                        callback(false, error);
                    }
                }
            });
        }

        login.logon = function(req,callback){ //
            login.actions.login(
                req,
                {
                    success : function(data){
                        if(!!callback){
                            callback(data);
                        }
                    },
                    error : function(e){
                        if(!!callback){
                            callback(new loginResult(false,e));
                        }
                    }
                }
            );
        }


    });
});