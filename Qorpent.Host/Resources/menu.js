define([
    'angular',
    'jquery'
], function(angular, $) {
    $(document).on('click', function() {
         $('.menu__item.hover').removeClass('hover');
    });

    var getRandomHsl = function() {
        var h = Math.floor(Math.random() * 360);
        var s = Math.floor(Math.random() * 20) + 50;
        var l = Math.floor(Math.random() * 20) + 50;
        return {h:h,s:s,l:l};
    };

    angular.module('Menu', [])
        .directive('menu', function() {
            var activateMenuGroup = function(menu, group) {
                var items = menu.find('div:last-child>.menu__item');
                var groups = menu.find('div:first-child>.menu__item');
                groups.removeClass('active');
                groups.filter('[code="' + group + '"]').first().addClass('active');
                items.hide();
                items.filter('[group="' + group + '"]').show();
            };

            return {
                restrict: 'A',
                link: function(scope, el, attrs) {
                    scope.click = function() {

                    };

                    el = $(el);
                    var settings = scope.settings.get(attrs.id);
                    var initRibbonMenu = function() {
                        el.on('click', 'div:first-child>.menu__item', function(e) {
                            var g = $(e.currentTarget.className == 'menu__item' ? e.currentTarget : e.currentTarget.parentElement).attr('code');
                            scope.settings.set(attrs.id, { activegroup: g });
                            activateMenuGroup(el, g);
                        });
                        if (!settings.activegroup) {
                            var defaultGroup = el.find('div:first-child>.menu__item[default]').first();
                            if (defaultGroup.length == 0) {
                                defaultGroup = el.find('div:first-child>.menu__item').first();
                            }
                            var groupCode = defaultGroup.attr('code');
                            scope.settings.set(attrs.id, { activegroup: groupCode });
                            activateMenuGroup(el, groupCode);
                        } else {
                            activateMenuGroup(el, settings.activegroup);
                        }
                    };
                    var initDropdownMenu = function() {
                        el.on('mouseenter', '.menu__item>.menu__item-element', function(e) {
                            $('.menu__item.hover').removeClass('hover');
                            var p = $(e.target).parents('.menu__item[menu-group]');
                            p.addClass('hover');
                            if ($(window).width() - (p.first().offset().left + p.first().outerWidth()) < 200) {
                                p.first().addClass('open-left').removeClass('open-right');
                            }
                            else {
                                p.first().addClass('open-right').removeClass('open-left');
                            }
                        });
                        el.on('click', function(e) {
                            e.stopPropagation();
                        });
                    };
                    if (attrs.menu == 'ribbon') {
                        initRibbonMenu();
                    }
                    else if (attrs.menu == 'inline') {

                    }
                    else if (attrs.menu == 'dropdown') {
                        initDropdownMenu();
                    }
                }
            }
        })
        .directive('menuItem', function() {
            var wrapMenuItem = function(m) {
                if (!m.Type && !m.type) m.type = 'text';
            };

            var generateSubmenu = function(data) {
                var submenu = $('<div class="submenu"/>');
                $.each(data, function(i, m) {
                    var menuItem = generateMenuItem(m);
                    submenu.append(menuItem);
                });
                return submenu;
            };

            var generateSubmenuFromModel = function(options) {
                var submenu = $('<div class="submenu"/>');
                var expression = options.modelName;
                if (!options.wrapperName) {
                    expression =  options.wrapperName + '(' + expression + ')';
                }
                if (!options.filter) {
                    expression += ' | ' + options.filter;
                }
                var html = '<div class="menu__item menu__item-element" menu-item ng-repeat="m in ' + expression + '">\
    <div ng-if="(m.type || m.Type) == \'text\' || (m.type || m.Type) == null" class="menu__item-title menu__item-element" ng-bind="(m.name || m.Name)">\
    <div ng-if="(m.type || m.Type) == \'view\'" class="menu__item-view menu__item-element" ng-bind="(m.name || m.Name)">\
        <ng-include ng-src="\' + (m.view.indexOf(\'.html\') != -1 ? m.view : m.view + \'.html\')\'" />\
    </div>\
    <div ng-if="(m.type || m.Type) == \'icon_with_text\'" class="icon menu__item-icon menu__item-element">\
        <img ng-if="(m.icon || m.iconclass).indexOf(\'/\') != -1" ng-src="(m.icon || m.iconclass)" />\
        <i ng-if="(m.icon || m.iconclass).indexOf(\'/\') == -1" ng-class="(m.icon || m.iconclass)"></i>\
        <div ng-if="(m.type || m.Type) == \'icon_with_text\'" class="menu__item-title menu__item-element" ng-bind="(m.name || m.Name)">\
    </div>\
</div>';
                return submenu.html(html);
            };

            var generateMenuItem = function(m) {
                m.type = m.type || m.Type || 'text';
                switch (m.type) {
                    case 'text' : return generateTextItem(m);
                    case 'icon_with_text' : return generateTextWithIconItem(m);
                }
            };

            var generateTextItem = function(m) {
                var result = generateMenuItemBase(m);
                result.append($('<div class="menu__item-title menu__item-element" />').text(m.Name));
                return result;
            };

            var generateViewItem = function(m) {
                var result = $('<div class="menu__item-view menu__item-element" />');
                var attrView = m.attr('view');
                result.append('<ng-include src="\'' + (attrView.indexOf('.html') != -1 ? attrView : attrView + '.html') + '\'" />');
                return result;
            };

            var generateTextWithIconItem = function(m) {
                var result = generateMenuItemBase(m);
                result.append(generateIconItem(m));
                result.append(generateTextItem(m));
                return result;
            };

            var generateIconItem = function(m) {
                var result = $('<div class="icon menu__item-icon menu__item-element" />');
                var iconAttr = m.icon || m.iconclass;
                if (!!iconAttr && iconAttr != '') {
                    if (iconAttr.indexOf('/') != -1) {
                        result.append($('<img />').attr('src', iconAttr));
                    }
                    else {
                        result.append($('<i/>').addClass(iconAttr));
                    }
                }
            };

            var generateMenuItemBase = function(m) {
                var result = $('<div class="menu__item menu__item-element" menu-item />')
                    .attr('type', m.type);
                m.action = m.action || m.Action || null;
                if (!!m.action) {
                    result.attr('action', m.action);
                }
                return result;
            };

            var getMenuOptions = function(attrs) {
                return {
                    modelName: attrs.model,
                    wrapperName: attrs.wrapper || '',
                    filterName: attrs.filter || ''
                }
            };

            return {
                restrict: 'A',
                compile: function (el, attrs) {
                    if (!!attrs.view) {
                        var viewItem = generateViewItem(el);
                        el.empty().append(viewItem);
                    }
                    else if (!!attrs.model && !attrs.url) {
                        var submenu = el.children('.submenu').first();
                        var options = getMenuOptions(attrs);
                        if (submenu.length == 0) {
                            submenu = generateSubmenuFromModel(options);
                            el.append(submenu);
                        }
                        el.append(submenu);
                        el.removeAttr('menu-item').attr('menu-group', 1);
                    }
                    if (!!attrs.url) { // Это можно использовать для статического JSON'а
                        if (!!attrs.model) {
                            var options = getMenuOptions(attrs);
                            var submenu = generateSubmenuFromModel(options);
                            el.append(submenu);
                            el.removeAttr('menu-item').attr('menu-group', 1);
                        } else {
                            $.getJSON(attrs.url, {}, function(data) {
                                var submenu = generateSubmenu(data);
                                el.append(submenu);
                                el.removeAttr('menu-item').attr('menu-group', 1);
                                el.on('click', '.menu__item', function(e) {
                                    var action = e.target.classList.contains('menu__item') ?
                                        e.target.getAttribute('action') :
                                        e.target.parentNode.getAttribute('action');
                                    eval(action);
                                });
                            });
                        }
                    }

                    return function (scope, el, attrs) {
                        el = $(el);
                        if (attrs.type == 'icon_with_text' || attrs.type == 'icon') {
                            var c = getRandomHsl();
                            el.find('.icon').first().css('background',
                                    'linear-gradient(to top, hsl(' + c.h + ',' + c.s + '%,' + c.l + '%),hsl(' + (c.h + 10) + ',' + c.s + '%,' + (c.l - 20) + '%))');
                        }
                        if (!!attrs.url && !!attrs.model) {
                            $.getJSON(attrs.action, {}, function(data) {
                                scope[attrs.model] = data;
                            });
                        }
                    }
                }
            }
        })
        .directive('menuGroup', function() {
            return {
                restrict: 'A',
                link: function(scope, el, attrs) {
                    el = $(el);
                    if (attrs.type == 'icon_with_text' || attrs.type == 'icon') {
                        var c = getRandomHsl();
                        el.find('.icon').first().css('background',
                                'linear-gradient(to top, hsl(' + c.h + ',' + c.s + '%,' + c.l + '%),hsl(' + (c.h + 10) + ',' + c.s + '%,' + (c.l - 20) + '%))');
                    }
                }
            }
        });
});