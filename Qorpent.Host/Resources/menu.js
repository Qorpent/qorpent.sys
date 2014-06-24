define([
    'angular',
    'jquery'
], function (angular, $) {
    $(document).on('click', function () {
        $('.menu__item.hover').removeClass('hover');
    });

    var getRandomHsl = function () {
        var h = Math.floor(Math.random() * 360);
        var s = Math.floor(Math.random() * 20) + 50;
        var l = Math.floor(Math.random() * 20) + 50;
        return {h: h, s: s, l: l};
    };

    var colorLuminance = function (hex, lum) {
        hex = String(hex).replace(/[^0-9a-f]/gi, '');
        if (hex.length < 6) {
            hex = hex[0] + hex[0] + hex[1] + hex[1] + hex[2] + hex[2];
        }
        lum = lum || 0;
        var rgb = "#", c, i;
        for (i = 0; i < 3; i++) {
            c = parseInt(hex.substr(i * 2, 2), 16);
            c = Math.round(Math.min(Math.max(0, c + (c * lum)), 255)).toString(16);
            rgb += ("00" + c).substr(c.length);
        }
        return rgb;
    };

    function MenuGenerator() {
        var that = this;

        this.generateSubmenu = function (data) {
            var submenu = $('<div class="submenu"/>');
            $.each(data, function (i, m) {
                var menuItem = generateMenuItem(m);
                submenu.append(menuItem);
            });
            return submenu;
        };

        this.generateMenuFromModel = function (options) {
            var result = $('<div />');
            var expression = options.modelName;
            if (!!options.wrapperName) {
                expression = options.wrapperName + '(' + expression + ')';
            }
            if (!!options.filterName) {
                expression += ' | ' + options.filterName;
            }
            // Если меню динамическое, то нужно позаботиться о методе exec в текущем скоупе!
            var html = '<div class="menu__item" type="text" menu-item ng-repeat="m in ' + expression + '" ng-click="exec(m)" ng-class="\'menu__item-\' + (!!m.size ? m.size : \'small\')">\
    <div ng-if="(m.type || m.Type) == \'text\' || (m.type || m.Type) == null" class="menu__item-title menu__item-element" ng-bind="(m.name || m.Name)">\
    <div ng-if="(m.type || m.Type) == \'view\'" class="menu__item-view menu__item-element" ng-bind="(m.name || m.Name)">\
        <ng-include ng-src="\' + (m.view.indexOf(\'.html\') != -1 ? m.view : m.view + \'.html\')\'" />\
    </div>\
    <div ng-if="(m.type || m.Type) == \'icon_with_text\'" class="icon menu__item-icon menu__item-element">\
        <img ng-if="(m.icon || m.Icon).indexOf(\'/\') != -1" ng-src="(m.icon || m.Icon)" />\
        <i ng-if="(m.icon || m.Icon).indexOf(\'/\') == -1" ng-class="(m.icon || m.Icon)"></i>\
        <div ng-if="(m.type || m.Type) == \'icon_with_text\'" class="menu__item-title menu__item-element" ng-bind="(m.name || m.Name)">\
    </div>\
</div>';
            return result.html(html);
        };

        this.generateSubmenuFromModel = function (options) {
            var submenu = $('<div class="submenu"/>');
            var menu = that.generateMenuFromModel(options);
            return submenu.html(menu.html());
        };

        this.generateMenuItem = function (m) {
            m.type = m.type || m.Type || 'text';
            switch (m.type) {
                case 'text' :
                    return generateTextItem(m);
                case 'icon_with_text' :
                    return generateTextWithIconItem(m);
            }
        };

        this.generateTextItem = function (m) {
            var result = generateMenuItemBase(m);
            result.append($('<div class="menu__item-title menu__item-element" />').text(m.Name));
            return result;
        };

        this.generateViewItem = function (m) {
            var result = $('<div class="menu__item-view menu__item-element" />');
            var attrView = m.attr('view');
            result.append('<ng-include src="\'' + (attrView.indexOf('.html') != -1 ? attrView : attrView + '.html') + '\'" />');
            return result;
        };

        this.generateTextWithIconItem = function (m) {
            var result = generateMenuItemBase(m);
            result.append(generateIconItem(m));
            result.append(generateTextItem(m));
            return result;
        };

        this.generateIconItem = function (m) {
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

        this.generateMenuItemBase = function (m) {
            var result = $('<div class="menu__item menu__item-element" menu-item />')
                .attr('type', m.type);
            m.action = m.action || m.Action || null;
            if (!!m.action) {
                result.attr('action', m.action);
            }
            return result;
        };

        this.getMenuOptions = function (attrs) {
            return {
                modelName: attrs.model,
                wrapperName: attrs.wrapper || '',
                filterName: attrs.filter || ''
            }
        };
    }

    angular.module('Menu', [])
        .directive('menu', function () {
            var menuGenerator = new MenuGenerator();

            var activateMenuGroup = function (menu, group) {
                var items = menu.find('div:last-child>.menu__item');
                var groups = menu.find('div:first-child>.menu__item');
                groups.removeClass('active');
                groups.filter('[code="' + group + '"]').first().addClass('active');
                items.hide();
                items.filter('[group="' + group + '"]').show();
            };

            return {
                restrict: 'A',
                compile: function (el, attrs) {
                    el = $(el);
                    if (!!el.attr('model')) {
                        var options = menuGenerator.getMenuOptions(attrs);
                        el.find('[items]').append(menuGenerator.generateMenuFromModel(options).html());
                    }

                    return function (scope, el, attrs) {
                        el = $(el);
                        var settings = scope.settings.get(attrs.id);
                        var initRibbonMenu = function () {
                            el.on('click', 'div:first-child>.menu__item', function (e) {
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
                        var initDropdownMenu = function () {
                            el.on('mouseenter', '.menu__item>.menu__item-element', function (e) {
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
                        };
                        if (attrs.menu == 'ribbon') {
                            initRibbonMenu();
                        }
                        else if (attrs.menu == 'inline') {

                        }
                        else if (attrs.menu == 'dropdown') {
                            initDropdownMenu();
                        }
                        el.on('click', '.menu__item', function (e) {
                            var t = $(e.target);
                            var item = t.hasClass('.menu__item') ? t : t.parents('.menu__item').first();
                            if (!!item.attr('url')) {
                                window.open(item.attr('url'), '_blank');
                            }
                        });
                    }
                }
            }
        })
        .directive('menuItem', function () {
            var wrapMenuItem = function (m) {
                if (!m.Type && !m.type) m.type = 'text';
            };

            return {
                restrict: 'A',
                link: function (scope, el, attrs) {
                    el = $(el);
                    if (!!attrs.url && !!attrs.model) {
                        $.getJSON(attrs.action, {}, function (data) {
                            scope[attrs.model] = data;
                        });
                    }
                }
            }
        })
        .directive('menuIcon', function () {
            return {
                restrict: 'A',
                link: function (scope, el, attrs) {
                    el = $(el);
                    var startcolor, endcolor;
                    if (!!attrs.color) {
                        startcolor = attrs.color.indexOf('#') == 0 ? attrs.color : '#' + attrs.color;
                        endcolor = colorLuminance(startcolor, -0.5);
                    } else {
                        var c = getRandomHsl();
                        startcolor = 'hsl(' + c.h + ',' + c.s + '%,' + c.l + '%)';
                        endcolor = 'hsl(' + (c.h + 10) + ',' + c.s + '%,' + (c.l - 20) + '%)';
                    }
                    el.css('background', 'linear-gradient(to top, ' + startcolor + ',' + endcolor + ')');
                }
            }
        })
        .directive('menuGroup', function () {
            return {
                restrict: 'A',
                link: function (scope, el, attrs) {
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