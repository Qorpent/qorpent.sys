define([
    'angular'
], function(angular) {
    var extendAttrs = function(options, attrs) {
        Object.keys(attrs).forEach(function(a) {
            options[a] = attrs[a];
        });
        return options;
    };

    angular.module('Layout', [])
        .controller('LayoutController', function($scope) {
            $scope.order = {};
            $scope.swapOrder = function(a, b) {
                $scope.$apply(function() {
                    $scope.order[a.id].order = b.order;
                    $scope.order[a.id].el.attr('order', b.order);
                    $scope.order[a.id].el.css('order', b.order);

                    $scope.order[b.id].order = a.order;
                    $scope.order[b.id].el.attr('order', a.order);
                    $scope.order[b.id].el.css('order', a.order);
                });
                /*localStorage.setItem('layout__position-' + a.id, b.order);
                localStorage.setItem('layout__position-' + b.id, a.order);*/
            }
        })
        .directive('layoutItem', function() {
            return {
                restrict: 'A',
                link: function($scope, $el, $attrs) {
                    var id = $attrs.id;
                    var order;
                    if (!!id) {/*
                        order = localStorage.getItem('layout__position-' + id);
                        if (!order) {
                            order = localStorage.setItem('layout__position-' + id, $attrs.order || 0);
                        }*/
                        $scope.order[id] = { el: $el, order: order || $attrs.order };
                    }
                    var css = {};
                    if ($attrs.width) {
                        css.width = $attrs.width;
                    }
                    if ($attrs.height) {
                        css.height = $attrs.height;
                    }
                    if ($attrs.grow) {
                        css["flex-grow"] = $attrs.grow;
                    }
                    if ($attrs.order) {
                        css.order = $attrs.order;
                    }
                    $el.css(css);

                    /*if (!!$attrs.root) {
                        $el.on('mousedown', 'widget', function(e) {
                            var $e = $(e.target);
                            $scope.dragW = { id: $e.attr('id'), order: $e.attr('order') };
                        });
                        $el.on('mouseup', 'widget', function(e) {
                            var $e = $(e.target);
                            $scope.dropW = { id: $e.attr('id'), order: $e.attr('order') };
                            $scope.swapOrder($scope.dragW, $scope.dropW);
                        });
                    }*/
                }
            }
        })
        /*.directive('widget', ['localSettings', function() {
            return {
                restrict: 'E',
                link: function($scope, $el, $attrs) {
                    var id = $attrs.id;
                    var order;
                    if (!!id) {
                        order = localStorage.getItem('layout__position-' + id);
                        if (!order) {
                            order = localStorage.setItem('layout__position-' + id, $attrs.order || 0);
                        }
                        $scope.order[id] = { el: $el, order: order };
                    }
                    $el.css({
                        width: $attrs.width,
                        height: $attrs.height,
                        order: order*//*,
                        'background-color': '#'+(Math.random()*0xFFFFFF<<0).toString(16)*//*
                    });
                }
            }
        }])*/;
});