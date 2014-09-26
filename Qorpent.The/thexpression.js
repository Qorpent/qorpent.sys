/**
 * Created by comdiv on 26.09.14.
 */
(function (define) {
    define(["./the"], function ($the) {
        return $the(function (root) {

            var e = function (expr, options) {
                if (typeof  expr === "undefined" ||null==expr) return null;
                if (typeof  expr === "function"){
                    expr.annotation = {};
                    if(!!options && !!options.annotate){
                        e.annotateFunction(expr,options);
                    }
                    return expr;
                }
                if(typeof  options ==="string"){
                    options = {type:options};
                }
                options = options || {type:"check"};
                if (!options.type) {
                    options.type = "check";
                }

                if (!e.prototypes.hasOwnProperty(options.type))throw "type not registered " + options.type;
                if (!options.hasOwnProperty("hasExpressionSymbols")) {
                    options.hasExpressionSymbols = false;
                    if (typeof  expr === "string") {
                        options.hasExpressionSymbols = !!expr.match(/[\.=\-+<>&|*\/!()]/);
                    }
                }
                var result =  e.prototypes[options.type](expr, options);
                result.annotation = {};
                return result;

            };

            e.nullFunction = function () {
                return undefined;
            };

            e.annotateFunction =  function(f,options){
                f.annotation = f.annotation || {};
                if (f.toString().match(/^function\s*\(a\s*,b\s*\)/)) {
                    f.annotation.comparer = true;
                }
            };

            e.prototypes = {};

            e.prototypes.check = function (expr, options) {
                var result = null;
                if (typeof expr === "number" || (typeof expr === "string" && !options.hasExpressionSymbols)) {
                    if(!options.counter){
                        result = function (_) {
                            return _ == expr;
                        }
                    }else{
                        var current = expr;
                        result = function(_){
                            if (current == 0)return false;
                            current--;
                            return true;
                        }
                    }
                } else if (typeof  expr === "string") {
                    var realExpr = expr;
                    if (!expr.match(/_/))realExpr = "_" + realExpr;
                    realExpr = "_=function(_,idx){try{return  (" + realExpr + ");}catch(e){return false;}}";
                    result = eval(realExpr);
                }else if(expr instanceof RegExp){
                    result = function(_){
                        return !!_.match(expr);
                    }
                }else if (Array.isArray(expr)) { //array support in "in" notation by default
                    if (!!options.and) {
                        result = function (_) {
                            var match = true;
                            for (var i = 0; i < expr.length; i++) {
                                if (!e(expr[i])(_)) {
                                    match = false;
                                    break;
                                }
                            }
                            return match;
                        }
                    } else {
                        result = function (_) {
                            for (var i = 0; i < expr.length; i++) {
                                if (e(expr[i])(_))return true;
                            }
                            return false;
                        }
                    }
                } else { //object pattern matching
                    if (!!options.or) {
                        result = function (_) {
                            for (var i in expr) {
                                if (expr.hasOwnProperty(i)) {
                                    var subExpr = e(expr[i]);
                                    if (i[0] == "$") { //global test
                                        if (subExpr(_))return true;
                                    } else {
                                        if (_.hasOwnProperty(i)) {
                                            if (subExpr(_[i]))return true;
                                        }
                                    }
                                }
                            }
                            return false;
                        }
                    } else {
                        result = function (_) {
                            var match = true;
                            for (var i in expr) {
                                if (expr.hasOwnProperty(i)) {
                                    var subExpr = e(expr[i]);
                                    if (i[0] == "$") { //global test
                                        if (!subExpr(_)) {
                                            match = false;
                                            break;
                                        }
                                    } else {
                                        if (_.hasOwnProperty(i)) {
                                            if (!subExpr(_[i])) {
                                                match = false;
                                                break;
                                            }
                                        } else {
                                            match = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            return match;

                            return false;
                        }
                    }

                }
                result.expressionType = "equal";
                return result;
            };

            root.expression = e;
            root.expression.define = function (proto, processfuntion) {
                e.prototypes[proto] = processfuntion;
            }
        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));