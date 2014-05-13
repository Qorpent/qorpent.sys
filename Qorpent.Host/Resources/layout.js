define([
    'angular',
    'settings'
], function(angular) {
    var extendAttrs = function(options, attrs) {
        Object.keys(attrs).forEach(function(a) {
            options[a] = attrs[a];
        });
        return options;
    };

    angular.module('Layout', ['settings'])
        .controller('LayoutController', function($scope, settings) {
            settings($scope);
            $scope.splitting = false;
            $scope.splittingmode = '';
            $(document).on('mousemove', function() {
                if (!$scope.splitting) return;
                if (window.getSelection().empty) {  // Chrome
                    window.getSelection().empty();
                } else if (window.getSelection().removeAllRanges) {  // Firefox
                    window.getSelection().removeAllRanges();
                }
            });
            $(document).on('mouseup', function(e) {
                if (!$scope.splitting) return;
                $scope.$apply(function() {
                    $scope.splitting = false;
                    $scope.splittingmode = '';
                });
                var s = $scope.splitter;
                s.data({
                    xend: e.screenX,
                    yend: e.screenY
                });
                var d;
                var data = s.data();
                if (data.mode == 'horizontal') {
                    d = Math.round((data.xstart - data.xend)*100/(data.parent.width()));
                } else {
                    d = Math.round((data.ystart - data.yend)*100/(data.parent.height()));
                }
                var pos = {};
                pos.a = (parseInt(data.prev.css('flex-grow')) - d).toString();
                pos.b = (parseInt(data.next.css('flex-grow')) + d).toString();
                data.prev.css('flex-grow', pos.a);
                data.next.css('flex-grow', pos.b);
                s.data('pos', pos);
                $scope.settings.set(data.prev.attr('id'), pos.a);
                $scope.settings.set(data.next.attr('id'), pos.b);
            });
        })
        .directive('layoutItem', function() {
            return {
                restrict: 'A',
                link: function($scope, $el, $attrs) {
                    $el = $($el);
                    var id = $attrs.id;
                    var css = {};
                    if ($attrs.width) {
                        css.width = $attrs.width;
                    }
                    if ($attrs.height) {
                        css.height = $attrs.height;
                        if (css.height.indexOf('px') != -1) {
                            css['flex-basis'] = css.height;
                        }
                    }
                    if ($attrs.order) {
                        css.order = $attrs.order;
                    }
                    if (null != $attrs.split) {
                        var ch = $el.children().filter(function(i,c){return c.tagName!='SPLITTER'});
                        $.each(ch, function(i, c) {
                            if (!$scope.settings.get($(c).attr('id'))) {
                                $(c).css('flex-grow', Math.round(100/ch.length).toString());
                            }
                        });
                    }
                    $el.css(css);
                }
            }
        })
        .directive('splitter', function() {
            return {
                restrict: 'E',
                link: function($scope, $el) {
                    $el = $($el);
                    var parent = $el.parent();
                    var prevGrow = $scope.settings.get($el.prev().attr('id'));
                    var nextGrow = $scope.settings.get($el.next().attr('id'));
                    $el.data({
                        parent: parent,
                        mode: parent.attr('orientation') || 'horizontal',
                        prev: $el.prev(),
                        next: $el.next()
                    });
                    if (!!prevGrow) {
                        $el.data('prev').css('flex-grow', prevGrow);
                    }
                    if (!!nextGrow) {
                        $el.data('next').css('flex-grow', nextGrow);
                    }
                    $el.on('mousedown', function(e) {
                        $scope.$apply(function() {
                            $scope.splitting = true;
                            $scope.splittingmode = parent.attr('orientation') || 'horizontal';
                            $scope.splitter = $el;
                        });
                        $el.data({
                            xstart: e.screenX,
                            ystart: e.screenY
                        });
                    });

                }
            };
        });
});