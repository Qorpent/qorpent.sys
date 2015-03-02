/**
 * Created by comdiv on 26.02.2015.
 */
define([
    "the-root"
], function (the,ddutils) {

    var $ = the.$jQuery;
    var root = the;

    if (!$) {
        console.error('JQuery required');
        return the;
    }


    if(null==root.modules)return;

    var module = root.modules.all;

    module.directive('theTab', [function() {
        return {
            restrict: 'A',
            link: function(scope, element, attrs) {
                var tabname = attrs["theTab"];
                element.addClass('maintoolbar');
                element.addClass('tab');
                scope.$watch(function(){
                    return the.uistate.toolbar.isActive(tabname)
                },function(v){
                    if(v){
                        $(element).show();
                    }else{
                        $(element).hide();
                    }
                });
            }
        };
    }]);

    module.directive('theMenu',["dropdownService",function(dd){
            function setupItem(e,level){
                e.addClass('menuitem');
                if(e.children('ul').exists()){
                    e.addClass('submenu');
                    e.on('mouseenter',function(event){
                        var options ={timeout:level==200,menu:true,positions: ['RD','RU','LD','LU']};
                        dd(e,options);
                    });
                    e.children('ul').each(function(idx,sm){
                        setupMenu($(sm),level+1);
                    });
                }
            }
            function setupMenu(e,level){
                e = $(e);
                if(e[0].tagName.toUpperCase()=='UL') {
                    e.addClass('dropdown');
                    e.addClass('menu');
                }else{
                    e.on('mouseenter',function(event){
                        var options ={menu:true};
                        dd(e,options);
                    });
                }
                $(e).children('li').each(function(idx,i){
                    setupItem($(i),level);
                });
                $(e).children('ul').each(function(idx,i){
                    setupMenu($(i),level);
                });
                if(e[0].tagName.toUpperCase()=='UL') {
                    e.removeClass('hidden');
                }
            }
            return {
                restrict: 'A',
                compile: function(element, attributes){

                    return {
                        pre: function(scope, element, attributes, controller, transcludeFn){

                        },
                        post: function(scope, element, attributes, controller, transcludeFn){
                            if(element[0].tagName.toUpperCase()=='UL') {
                                element.addClass('hidden');
                            }
                            setTimeout( function(){setupMenu(element,0);},100);
                        }
                    }
                }
            }
        }]
    );

    module.directive('theTabButton',  [function() {
        return {
            restrict: 'A',
            link: function(scope, element, attrs) {
                var tabname = attrs["theTabButton"];
                element.addClass('button');
                element.addClass('tab');
                element.addClass('primary');
                scope.$watch(function(){
                    return the.uistate.toolbar.isActive(tabname)
                },function(v){
                    if(v){
                        element.addClass("checked");
                    }else{
                        element.removeClass("checked");
                    }
                });
                element.on('click',function(){
                    scope.$apply(function(){
                        the.uistate.toolbar.activate(tabname);
                    });
                });

            }
        };
    }]);

})