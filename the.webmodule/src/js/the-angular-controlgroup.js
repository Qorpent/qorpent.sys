/**
 * Created by comdiv on 04.11.2014.
 */
    define(["the-angular"], function ($the) {
        return $the(function(root, privates)
        {
            if(null==root.modules)return;
            root.modules.all.directive("theControlGroup",["ngClassDirective",function(ngc,compile){
                var ngclass= ngc[0];
                return {
                    restrict: 'A',
                    compile : function(element,attr){
                        var el =$(element);
                        el.addClass("control-group");
                        var input = el.find('input, textarea');
                        input.addClass("form-control");
                        var formname = el.parent().attr("name");
                        var name = input.attr("name");
                        attr["ngClass"]="{'has-error':"+formname+"."+name+".$invalid}";
                        el.attr("ng-class","{'has-error':"+formname+"."+name+".$invalid}");
                        var label= $('<label class="control-label">'+attr["label"]+'</label>').prependTo(el);


                        if (input.attr('required')){
                            $('<span class="validate" ng-show="'+formname+'.'+name+'.$error.required" class="help-inline">&nbsp;(обязательно)</span>').appendTo(label);
                        }
                        if (!!input.attr("ng-pattern")){
                            $('<span class="validate" ng-show="'+formname+'.'+name+'.$error.pattern" class="help-inline">&nbsp;('+attr["patternDesc"]+')</span>').appendTo(label);
                        }
                        if (!!input.attr("min")){
                            $('<span class="validate" ng-show="'+formname+'.'+name+'.$error.min" class="help-inline">&nbsp;(не менее '+input.attr("min")+')</span>').appendTo(label);
                        }

                        return {
                            pre : function(s, e,a){
                                ngclass.link(s,e,a);
                            }
                        }
                    }
                }
            }]);
        });
    });