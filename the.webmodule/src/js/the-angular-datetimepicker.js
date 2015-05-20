/**
 * Created by comdiv on 04.11.2014.
 */
define(["the-angular"], function ($the) {
    return $the(function (root) {
        if (null == root.modules)return;
        root.modules.all.directive("datetimepicker",function(){
            return {
                compile:function(e,attr){
                    $(e[0]).datetimepicker(
                        {
                            lang:'ru',
                            format:'d.m.Y H:i',
                            step:15,
                            onChangeDateTime : function(){
                                var ngchange = attr["ngChange"];
                                if(!!ngchange) {
                                    setTimeout(function(){
                                        angular.element(e[0]).scope().$eval(ngchange);
                                    },20);

                                }
                            }

                        });
                }
            }
        });
        root.modules.all.directive("datepicker",function(){
            return {
                compile:function(e,attr){
                    $(e[0]).datetimepicker(
                        {
                            lang:'ru',
                            format:'d.m.Y',
                            timepicker:false,
                            onChangeDateTime : function(){
                                var ngchange = attr["ngChange"];
                                if(!!ngchange) {
                                    setTimeout(function(){
                                        angular.element(e[0]).scope().$eval(ngchange);
                                    },20);

                                }
                            }

                        });
                }
            }
        });


    });
});