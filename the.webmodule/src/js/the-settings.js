define(["the-root"
], function($the) {
    if(typeof angular == "undefined")return;
    function Settings() {
        var self = this;
        Object.keys(localStorage).forEach(function(k) {
            var i = localStorage.getItem(k);
            self[k] = !self.isJson(i) ? i : JSON.parse(i);
        });
    }

    Settings.prototype.set = function(k, v) {
        if (typeof k == 'object') {
            var self = this;
            Object.keys(k).forEach(function(key) {
                self.set(key, k[key]);
            });
        }
        else if (typeof k == 'string') {
            if (typeof v == 'object') {
                this[k] = v;
                v = JSON.stringify(v);
            } else {
                this[k] = !this.isJson(v) ? v : JSON.parse(v);
            }
            localStorage.setItem(k, v);
            return v;
        }
    };

    Settings.prototype.isJson = function(s) {
        return (typeof s == 'string' && s != '' && (s.trim().indexOf('{') == 0 || s.trim().indexOf('[') == 0  || s.trim() == 'true' || s.trim() == 'false'));
    };

    Settings.prototype.get = function(k) {
        if (!this[k]) {
            this[k] = false;
            localStorage.setItem(k, 'false');
        }
        return this[k];
    };

    angular.module('settings', [])
        .factory('settings', ["$rootScope",function($rs) {
            var settings = new Settings();
            var result = function($scope) {
                $scope.settings = settings;
                $scope.$watch('settings', function(n, o) {
                    Object.keys(n).forEach(function(k) {
                        if (n[k] != o[k]) {
                            $scope.settings.set(k, n[k]);
                        }
                    });
                }, true);
            };
            result.set = function(n, v) {
                return settings.set(n, v);
            };
            result.get = function(n) {
                return  settings.get(n);
            }
            result.setup = function(options){
                var _ =  $the.extend(options.default ||{}, result.get(options.name));
                $rs.$watch(function(){return _;},function(){
                    result.set(options.name,_);
                },true);
                if(!!options.init){
                    options.init(_);
                }
                if(!!_.initialize){
                    _.initialize();
                }
                return _;
            }
            return result;

        }]);
});