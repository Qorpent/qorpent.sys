define([], function () {
    if (typeof document == "undefined") return;
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

    function MenuGenerator(el, attrs) {
        var self = this;
        this.rootElement = el;
        this.modelName = attrs.model || attrs.Model || null;
        this.wrapperName = attrs.wrapper || attrs.Wrapper || '';
        this.filterName = attrs.filter || attrs.Filter || '';

        this.generateSubmenu = function (m) {
            m.items = m.Items || null;
            if (!m.items) return;
            var submenu = $('<div class="submenu"/>');
            m.items.forEach(function(i) {
                var menuItem = self.generateMenuItem(i);
                submenu.append(menuItem);
            });
            return submenu;
        };

        this.generateMenuItem = function (m) {
            m.type = m.type || m.Type || 'text';
            m.name = m.name || m.Name;
            var result = this.generateMenuItemBase(m);
            if (m.name == 'divider') {
                return $('<div class="menu__item menu__item-divider" type="divider"/>');
            }
            switch (m.type) {
                case 'text' :
                    result.append(this.generateTextItem(m)); break;
                case 'icon' :
                    result.append(this.generateIconItem(m)); break;
                case 'icon_with_text' :
                    result.append(this.generateIconItem(m)).append(this.generateTextItem(m)); break;
                case 'view' :
                    result.append(this.generateViewItem(m)); break;
            }

            m.items = m.Items || null;
            if (!!m.items) {
                var submenu = this.generateSubmenu(m);
                result.append(submenu);
            }
            return result;
        };

        this.generateTextItem = function (m) {
            return $('<div class="menu__item-title menu__item-element" />').text(m.Name);
        };

        this.generateViewItem = function (m) {
            var result = $('<div class="menu__item-view menu__item-element" />');
            var attrView = m.attr('view');
            result.append('<ng-include src="\'' + (attrView.indexOf('.html') != -1 ? attrView : attrView + '.html') + '\'" />');
            return result;
        };

        this.generateIconItem = function (m) {
            var result = $('<div class="icon menu__item-icon menu__item-element" menu-icon="1" />');
            var iconAttr = m.icon || m.Icon || m.iconclass;
            if (!!iconAttr && iconAttr != '') {
                if (iconAttr.indexOf('/') != -1) {
                    result.append($('<img />').attr('src', iconAttr));
                }
                else {
                    result.append($('<i/>').addClass(iconAttr));
                }
            }
            return result;
        };

        this.generateMenuItemBase = function (m) {
            var result = $('<div class="menu__item menu__item-element" />')
                .attr('type', m.type);
            result.addClass('menu__item-' + (!!m.size ? m.size : 'small'));
            m.items = m.items || m.Items || null;
            m.model = m.model || m.Model || null;
            m.code = m.code || m.Code || null;
            if (!!m.items || !!m.model) {
                result.attr('menu-group', '1');
                result.addClass('open-right');
            } else {
                result.attr('menu-item', '1');
            }
            m.action = m.action || m.Action || null;
            if (!!m.action && !!this.modelName && !m.items) {
                result.attr('ng-click', "exec(" + this.modelName + ", '" + m.code + "')");
            }
            /*else if (!!m.action) {
                result.attr('ng-click', m.action);
            }*/
            return result;
        };

        this.copyAttrs = function (m) {
            var attrsToCopy = ['ng-if'];
            Object.keys(m).forEach(function (a) {
                if ($.inArray(attrsToCopy) != -1) {

                }
            });
        };

        this.isItemFiltered = function(item, filterAttr) {
            var filter = eval("({" + filterAttr + "})"), result = false;
            if (!!filter.filter) filter = filter.filter;
            Object.keys(filter).forEach(function(f) {
                if (!!item[f]) {
                    result = (item[f] != filter[f]);
                }
            });
            return result;
        };

        this.generateMenu = function (model) {
            var result = $('<div/>');
            if (model instanceof Array) {
                model.forEach(function(m) {
                    if (!!self.filterName && self.filterName != '') {
                        !self.isItemFiltered(m, self.filterName) && result.append(self.generateMenuItem(m));
                    } else {
                        result.append(self.generateMenuItem(m));
                    }
                });
            }
            return result;
        };
    }

    angular.module('Menu', [])
        .directive('menu', function ($compile) {
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
                link: function (scope, el, attrs) {
                    el = $(el);
                    var settings = scope.settings.get(attrs.id);
                    if (!!attrs.model) {
                        scope.$watch(attrs.model, function() {
                            var menuGenerator = new MenuGenerator(el, attrs);
                            var template = angular.element(menuGenerator.generateMenu(scope[attrs.model]).html());
                            var menu = $compile(template)(scope);
                            el.find('.menu__horizontal').empty().append(menu);
                        });
                    }
                    var initRibbonMenu = function () {
                        el.on('click', 'div:first-child>.menu__item', function (e) {
                            var g = $(e.currentTarget.classList.contains('menu__item') ? e.currentTarget : e.currentTarget.parentElement).attr('code');
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
        })
        .directive('menuItem', function ($compile) {
            var wrapMenuItem = function (m) {
                if (!m.Type && !m.type) m.type = 'text';
            };

            return {
                restrict: 'A',
                link: function (scope, el, attrs) {
                    el = $(el);

                    if (!!attrs.model) {
                        scope.$watch(attrs.model, function() {
                            var menuGenerator = new MenuGenerator(el, attrs);
                            var template = angular.element(menuGenerator.generateMenu(scope[attrs.model]).html());
                            var menu = $compile(template)(scope);
                            el.find('.submenu').first().empty().append(menu);
                        });
                    }
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
					var color = attrs.color;
					if(!color && el.parent()) {
						color = el.parent().attr('color');
					}
                    if (!!color) {
                        startcolor = color.indexOf('#') == 0 ? color : '#' + color;
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
        .directive('menuGroup', function ($compile) {
            return {
                restrict: 'A',
                link: function (scope, el, attrs) {
                    el = $(el);
                    if (!!attrs.model) {
                        var menuGenerator = new MenuGenerator(el, attrs);
                        scope.$watch(attrs.model, function() {
                            var template = angular.element(menuGenerator.generateMenu(scope[attrs.model]).html());
                            var menu = $compile(template)(scope);
                            el.find('.submenu').first().empty().append(menu);
                        });
                    }
                    if (attrs.type == 'icon_with_text' || attrs.type == 'icon') {
                    	var color = attrs.color;
                    	if (!color && el.parent()) {
                    		color = el.parent().attr('color');
                    	}
                    	if (!!color) {
                    		var startcolor, endcolor;
                    		startcolor = color.indexOf('#') == 0 ? color : '#' + color;
                    		endcolor = colorLuminance(startcolor, -0.5);
                    		el.find('.icon').first().css('background', 'linear-gradient(to top, ' + startcolor + ',' + endcolor + ')');
                    	} else {
                    		var c = getRandomHsl();
                    		el.find('.icon').first().css('background',
									'linear-gradient(to top, hsl(' + c.h + ',' + c.s + '%,' + c.l + '%),hsl(' + (c.h + 10) + ',' + c.s + '%,' + (c.l - 20) + '%))');
                    	}
                    }
                }
            }
        });
});