/**
 * Created by comdiv on 04.11.2014.
 */
define(["the-angular"], function ($the) {
    return $the(function (root) {
        if (null == root.modules)return;
        root.modules.all.directive("datetimepicker",function(){
            return {
                link:function(scope,e,attr){
                    var ngchange = attr["ngChange"];
                    var ngmodel = attr["ngModel"];
                    $(e[0]).datetimepicker(
                        {
                            lang:'ru',
                            format:'d.m.Y H:i',
                            step:15,
                            /* cool firefox hack avoid of it's lag in events */
                            onChangeDateTime : function(c,i){

                                if(!!ngmodel) {
                                    setTimeout(function(){
                                        if(!!ngmodel){
                                            scope.$eval(ngmodel+"='"+ $(i).val()+"'");
                                        }
                                        if(!!ngchange) {
                                            scope.$eval(ngchange);
                                        }
                                    },4);

                                }
                            }
                        });
                }
            }
        });
        root.modules.all.directive("datepicker",function(){
            return {
                link:function(scope,e,attr){
                    var ngchange = attr["ngChange"];
                    var ngmodel = attr["ngModel"];
                    if(e[0].__dtsetup)return;
                    e[0].__dtsetup = true;

                    $(e[0]).datetimepicker(
                        {
                            lang:'ru',
                            format:'d.m.Y',
                            timepicker:false,
                            closeOnDateSelect : true,
                            scrollInput : false,
                            onChangeDateTime : function(c,i){

                                if(!!ngmodel) {
                                   setTimeout(function(){
                                       if(!!ngmodel){
                                           scope.$eval(ngmodel+"='"+ $(i).val()+"'");
                                       }
                                       if(!!ngchange) {
                                           scope.$eval(ngchange);
                                       }
                                    },4);

                                }
                            }

                        });}

            }
        });


    });
});