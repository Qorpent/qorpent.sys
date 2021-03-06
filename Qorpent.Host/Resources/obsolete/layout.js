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
               var delta;
                var data = s.data();
				var minsize = 10;
				var maxsize = 90;
				var emaxsize = 100;
				var eminsize = 0;
				var emaxsize2 = 0;
				var eminsize2 = 100;
                if (data.mode == 'horizontal') {
					eminsize = Math.round(parseInt((data.prev.css('min-width')||'0px').replace('px',''))*100/data.parent.width());
					emaxsize = Math.round(parseInt((data.prev.css('max-width')||'5000px').replace('px',''))*100/data.parent.width());
					eminsize2 = 100-Math.round(parseInt((data.next.css('min-width')||'0px').replace('px',''))*100/data.parent.width());
					emaxsize2 = 100-Math.round(parseInt((data.next.css('max-width')||'5000px').replace('px',''))*100/data.parent.width());
                    delta = (data.xend - data.xstart)*100.0/data.parent.width();
                } else {
					eminsize = Math.round(parseInt((data.prev.css('min-height')||'0px').replace('px',''))*100/data.parent.height());
					emaxsize = Math.round(parseInt((data.prev.css('max-height')||'5000px').replace('px',''))*100/data.parent.height());
					eminsize2 =100-Math.round(parseInt((data.next.css('min-height')||'0px').replace('px',''))*100/data.parent.height());
					emaxsize2 =100- Math.round(parseInt((data.next.css('max-height')||'5000px').replace('px',''))*100/data.parent.height());
                    delta = (data.yend - data.ystart)*100.0/data.parent.height();
                }
				if (eminsize > minsize){
					minsize = eminsize;
				}
				if(emaxsize2 < minsize){
					minsize -= (minsize - eminsize2)/2;
				}
				if(emaxsize < maxsize){
					maxsize = emaxsize;
				}
				if(eminsize2!=100 && eminsize2 > maxsize){
					maxsize += (eminsize2 - maxsize)/2;
				}
                var pos = {};
                pos.a = (parseFloat(data.prev.css('flex-grow')) );
                pos.b = (parseFloat(data.next.css('flex-grow')) );
				var total = Math.round(pos.a+pos.b);
				pos.a += delta;
				if(pos.a < minsize){
					pos.a = minsize;
				}
				if(pos.a > maxsize){
					pos.a = maxsize;
				}
				pos.b = total - pos.a;
				
				
				data.prev.css('flex-grow', pos.a.toString());
                data.next.css('flex-grow', pos.b.toString());

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
                    if (!!$attrs.root && !!$attrs.name) {
                        document.title = $attrs.name;
                    }
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
                    var ch = $el.children().filter(function (i, c) { return c.tagName != 'SPLITTER' });
                    if (null != $attrs.split) {
                        
                        $.each(ch, function(i, c) {
                            if (!$scope.settings.get($(c).attr('id'))) {
                                if ($attrs.grow) {
                                    $(c).css('flex-grow', $attr.grow);
                                } else {
                                    $(c).css('flex-grow', Math.round(100 / ch.length).toString());
                                }
                            }
                        });
                    } else {
                        
                        if ($attrs.grow) {
                            css['flex-grow'] = $attrs.grow;
                        } 
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