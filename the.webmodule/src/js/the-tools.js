/**
 * Created by comdiv on 26.02.2015.
 */
define([
    "the-root"
], function (the, ddutils) {

    var $ = the.$jQuery;
    var root = the;

    if (!$) {
        console.error('JQuery required');
        return the;
    }


    if (null == root.modules)return;

    var module = root.modules.all;
    var uistate = root.uistate;

    module.directive('theLeft', [
        "dropdownService",
        "$compile",
        function (dropdown,$compile) {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    element = $(element[0]);
                    element.addClass('left');

                    var label = element.children('label');



                    var labelInternal = null;
                    var customLabel = false;
                    if (label.length == 0) {
                        customLabel = true;
                        label = $('<label class="title primary bordered"></label>').prependTo(element);

                    }


                    labelInternal = label.children('i');
                    if(labelInternal.length==0){
                        customLabel = true;
                        labelInternal = $('<i/>').prependTo(label);
                    }


                    label.on('click',function(e){
                        dropdown(e,{menu:true});
                    })

                    label.find('.fa-remove').on('click', function () {
                        scope.$apply(function () {
                            uistate.left.visible = false;
                        })
                    })

                    var tbt = label.find('.tbt');
                    if (tbt.length == 0) {
                        tbt = $('<div class="tbt"></div>').appendTo(label);
                    }

                    var remover = tbt.children('.fa-remove');
                    if (remover.length == 0) {
                        remover = $('<i title="Скрыть панель" class="fa fa-remove"></i>').appendTo(tbt);
                        remover.on('click', function (e) {
                            e.stopPropagation();
                            e.preventDefault();
                            scope.$apply(function () {
                                uistate.left.visible = false;
                            })
                        })
                    }

                    var collapser = element.children('.collapser');
                    var customCollapser = false;
                    if (collapser.length == 0) {
                        customCollapser = true;
                        collapser = $('<div class="collapser"/>').prependTo(element);
                        collapser.on('click', function () {
                            scope.$apply(function () {
                                uistate.left.visible = true;
                            })
                        });
                    }


                    the.uistate.getGroup('left').get('log').title = "Журнал ошибок";
                    if (the.uistate.left.activeObject == 'default'||the.uistate.left.activeObject=='null'||the.uistate.left.activeObject=='none'){
                        the.uistate.left.activate('log');
                    }

                    var menu = element.children('ul');
                    if(menu.length==0){
                        menu = $('<ul class="hidden" the-menu >\
                        <li  ng-click="uistate.left.activate(o.code)" ng-repeat="o in uistate.left.objects | activeobjects ">\
                        {{o.title}}\
                    </li>\
                    </ul>');
                        menu.appendTo(label);
                       var compiled = $compile(menu);
                        compiled(scope);
                    }



                    logtool = $('<div tool="log" style="max-width: 400px;overflow-y: auto">\
                    <label class="info bordered">В случае необходимости скопируйте текст из поля внизу для пересылки администраторам</label>\
                <textarea style="width: 90%;height: 50px">{{getlogjson()}}</textarea>\
            <div class="message bordered soft"  ng-class="{warn:m.level==\'error\',info:m.level==\'info\'}" ng-repeat="m in log.messages">\
            Время: {{m.time}}, Действие: {{m.action.url}}, Ошибка: {{m.text}}\
            </div>\
            </div>');
                    logtool.appendTo(element);
                    $compile(logtool)(scope);



                    scope.$watch(function () {
                        return uistate.left.visible;
                    }, function (value) {
                        element.toggleClass('collapsed', !value);
                        if (customCollapser) {
                            collapser.toggle(!value);
                        }
                    });


                    scope.$watch(function () {
                        return uistate.left.getActive();
                    }, function (value) {
                        if (customCollapser) {
                            collapser.text(value.title);
                        }
                        if (customLabel) {
                            labelInternal.text(value.title);
                        }
                        setTimeout(function(){
                            element.children('[tool]').each(function(idx,e){
                                $(e).toggle($(e).attr('tool')==value.code);
                            });
                        },100);
                    });

                }
            }
        }
    ]);

    module.directive('theErrors',[function(){
        return {
            restrict: 'A',
            template : '<div class="messages">\
        <div class="message soft  bordered info" style="min-height: 40px" ng-click="log.hideAll()" ng-if="log.activemessages.length > 1">\
        <i  class="title success" >Скрыть все сообщения</i>\
        <i class=" title success fa fa-remove"></i>\
        </div>\
        <div ng-click="m.expanded = !!!m.expanded" class="message bordered soft" ng-class="{expanded:m.expanded,warn:m.level==\'error\',info:m.level==\'info\'}" ng-repeat="m in log.activemessages">\
        Время: {{m.time}}, Действие: {{m.action.url}}, Ошибка: {{m.text}}\
        <i class="closer" ng-click="m.hide()"></i>\
        </div>\
        </div>'
        }
    }])

    module.directive('theTab', [function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var tabname = attrs["theTab"];
                element.addClass('maintoolbar');
                element.addClass('tab');
                scope.$watch(function () {
                    return the.uistate.toolbar.isActive(tabname)
                }, function (v) {
                    if (v) {
                        $(element).show();
                    } else {
                        $(element).hide();
                    }
                });
            }
        };
    }]);

    module.directive('theMenu', ["dropdownService", function (dd) {
            function setupItem(e, level) {
                e.addClass('menuitem');
                if (e.children('ul').exists()) {
                    e.addClass('submenu');
                    e.on('mouseenter', function (event) {
                        var options = {timeout: level == 200, menu: true, positions: ['RD', 'RU', 'LD', 'LU']};
                        dd(e, options);
                    });
                    e.children('ul').each(function (idx, sm) {
                        setupMenu($(sm), level + 1);
                    });
                }
            }

            function setupMenu(e, level) {
                e = $(e);
                if (e[0].tagName.toUpperCase() == 'UL') {
                    e.addClass('dropdown');
                    e.addClass('menu');
                } else {
                    e.on('mouseenter', function (event) {
                        var options = {menu: true};
                        dd(e, options);
                    });
                }
                $(e).children('li').each(function (idx, i) {
                    setupItem($(i), level);
                });
                $(e).children('ul').each(function (idx, i) {
                    setupMenu($(i), level);
                });
                if (e[0].tagName.toUpperCase() == 'UL') {
                    e.removeClass('hidden');
                }
            }

            return {
                restrict: 'A',
                compile: function (element, attributes) {

                    return {
                        pre: function (scope, element, attributes, controller, transcludeFn) {

                        },
                        post: function (scope, element, attributes, controller, transcludeFn) {
                            if (element[0].tagName.toUpperCase() == 'UL') {
                                element.addClass('hidden');
                            }
                            setTimeout(function () {
                                setupMenu(element, 0);
                            }, 100);
                        }
                    }
                }
            }
        }]
    );

    module.directive('theTabButton', [function () {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var tabname = attrs["theTabButton"];
                element.addClass('button');
                element.addClass('tab');
                element.addClass('primary');
                scope.$watch(function () {
                    return the.uistate.toolbar.isActive(tabname)
                }, function (v) {
                    if (v) {
                        element.addClass("checked");
                    } else {
                        element.removeClass("checked");
                    }
                });
                element.on('click', function () {
                    scope.$apply(function () {
                        the.uistate.toolbar.activate(tabname);
                    });
                });

            }
        };
    }]);

})