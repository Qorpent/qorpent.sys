/**
 * Created by comdiv on 04.11.2014.
 */
define(["the-angular"], function ($the) {
    return $the(function (root) {
        if (null == root.modules)return;
        var compareTo = function() {
            return {
                require: "ngModel",
                link: function(scope, element, attributes, ngModel) {

                    ngModel.$validators.compareTo = function(modelValue) {
                        var othervalue = scope.$eval(attributes["compareTo"]);
                        return (modelValue == othervalue) || (!modelValue && !othervalue);
                    };

                    scope.$watch(function(){
                        return scope.$eval(attributes["compareTo"]);
                    }, function() {
                        ngModel.$validate();
                    });

                }
            };
        };
        var passwordPolicy = function() {
            return {
                require: "ngModel",

                link: function(scope, element, attributes, ngModel) {

                    ngModel.$validators.passwordPolicy = function(modelValue) {
                        if(!attributes["ngRequired"] && !modelValue){
                            return true;
                        }
                        if(modelValue==null)return false;
                        if(modelValue.length<8)return false;
                        var chars = {

                        }
                        var distinct = 0;
                        for(var i=0;i<modelValue.length;i++){
                            var name = modelValue[i];
                            if(!(name in chars)){
                                distinct++;
                                chars[modelValue[i]] =true;
                            }
                        }
                        if(distinct<modelValue.length-2)return false;
                        var hasupper = false;
                        var haslower = false;
                        var hasdigit = false;
                        var hassign = false;
                        var dist = false;
                        for(var i=0;i<modelValue.length;i++){

                            var c = modelValue[i];
                            if(c.match(/\d/)){
                                hasdigit=true;
                            }else if(c.toLowerCase()!= c.toUpperCase()){
                                if(c.toLowerCase()==c){
                                    haslower = true;
                                }else {
                                    hasupper = true;
                                }
                            }else{
                                hassign = true;
                            }
                        }
                        if(haslower && hasupper && hasdigit && hassign){
                            dist= true;

                        }
                        if(dist)return true;
                        return false;

                    };

                }
            };
        };
        root.modules.all.directive("compareTo", compareTo);
        root.modules.all.directive("passwordPolicy", passwordPolicy);

    });
});