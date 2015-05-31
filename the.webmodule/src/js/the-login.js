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
            this.error = error || "";
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
        var myinfoAction = new $the.action({url: "/myinfo"});
        var requestKeyAction = new $the.action({url: "/resetpwdreq"});
        var resetPassAction = new $the.action({url: "/resetpwd"});
        var infoApp = new $the.action({url: "/info/app"});
        var login = $the.login || ($the.login = {});
        var loginActions = login.actions || (login.actions = {});
        loginActions.login = logonAction;
        loginActions.logout = logoutAction;
        loginActions.isauth = isauthAction;
        loginActions.appinfo = infoApp;
        loginActions.myinfo = myinfoAction;
        loginActions.requestkey = requestKeyAction;
        loginActions.resetpass = resetPassAction;

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


        login.requestkey = function (req,callback) { //callback(result, error)
            login.actions.requestkey(
                req,
                {
                    method:"POST",
                success: function (result) {
                    if (!!callback) {
                        callback(result);
                    }
                },
                error: function (error,response) {

                    if (!!callback) {
                        callback(false, error, response.nativeResult.responseJSON);
                    }
                }
            });
        }

        login.resetpass = function (req,callback) { //callback(result, error)
            login.actions.resetpass(
                req,
                {
                    method:"POST",
                    success: function (result) {
                        if (!!callback) {
                            callback(result);
                        }
                    },
                    error: function (error,response) {
                        if (!!callback) {
                            callback(false, error,response.nativeResult.responseJSON);
                        }
                    }
                });
        }



        login.logon = function(req,callback){ //
            login.actions.login(
                req,
                {
                    method:"POST",
                    success : function(data){
                        var result = new loginResult(data===true,data===true?"":data);
                        if(!!callback){
                            callback(result);
                        }
                    },
                    error : function(e,native){
                        if(!!callback){
                            callback(new loginResult(false,e));
                        }
                    }
                }
            );
        }

        login.logout = function(callback){
            login.actions.logout(
                {
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
                }
            )
        }

        login.appinfo = function(callback){
            login.actions.appinfo(
                {
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
                }
            )
        }

        login.myinfo = function(callback){
            login.actions.myinfo(
                {
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
                }
            )
        }


    });
});