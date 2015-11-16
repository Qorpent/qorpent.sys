/**
 * Created by comdiv on 04.11.2014.
 */
define(["the-angular"], function ($the) {
    return $the(function (root, privates) {
        root.modules.all.factory('thereports', [
            "$http","$compile",
            function($http,$compile){
                function prepare(target, d,$scope){
                    d.items.forEach(function(_){
                        target.reports.push(_);
                        target.ireports[_._id] = _;
                        if(!!_.onform) {
                            _.onform = eval("___=" + _.onform);
                        }
                        if(!!_.onquery) {
                            _.onquery = eval("___=" + _.onquery);
                        }
                        _.toolbar = function(){
                            if(!!_.toolview){
                                return _.toolview;
                            }
                            return "views/the/report-toolbar.html";
                        }
                        _.params = {
                            if_has_level : function(lvl, vals, levelname) {
                                var l = lvl;
                                var lb = levelname || "level";
                                while(true){
                                    l--;
                                    if(l==0)return false;
                                    var name = lb+l;
                                    if(!this.hasOwnProperty(name))return false;
                                    var val = this[name];
                                    if(-1!=vals.indexOf(val))return true;
                                }
                                return false;
                            },
                            if_no_level : function (lvl, vals, levelname) {
                                var l = lvl;
                                var lb = levelname || "level";
                                while(true){
                                    l--;
                                    if(l==0)return true;
                                    var name = lb+l;
                                    if(!this.hasOwnProperty(name))return true;
                                    var val = this[name];
                                    if(-1!=vals.indexOf(val))return false;
                                }
                                return true;
                            },
                            level_change :  function(lvl, levelname){
                                //console.log(['change',lvl,levelname]);
                                var lb = levelname || "level";
                                for(var i=lvl+1;i<=10;i++){
                                    var name = lb + i;
                                    if(!this.hasOwnProperty(name))return;
                                    this[name] = 'none';
                                }
                            },
                            level_visible : function(lvl, levelname){
                                //console.log(['visible',lvl,levelname]);
                                if(lvl == 1)return true;
                                var lb = levelname || "level";
                                var name = lb + (lvl-1);
                                if (!this.hasOwnProperty(name))return true; //logically first
                                return this[name]!='none';
                            }
                        };
                        _.lists = {};
                        //console.log(_.parameters);
                        _.parameters.forEach(function(__){
                            __.show = function(){
                                console.log(__);
                                if(__.hidden)return false;
                                if(!!__.ngif){
                                    return $scope.$eval(__.ngif,{report:_,params: _.params});
                                }
                                return true;
                            }
                            __.change = function(){
                                if(!!__.ngchange){
                                    return $scope.$eval(__.ngchange,{report:_,params: _.params});
                                }
                                return true;
                            }
                            _.params[__._id] = __.default;
                            if(!!__.list && __.list.length){
                                _.lists[__._id] = __.list;
                                __.list.forEach(function(l){
                                    l.show = function(){
                                        if(!!l.ngif){
                                            return $scope.$eval(l.ngif,{report:_,params: _.params});
                                        }
                                        return true;
                                    }
                                })
                            }

                            if(__.type=='bool'){
                                _.params[__._id] = __.default=='true' || __.default===true;
                            }
                        });
                    })
                }
                return function($scope) {
                    return {
                        reports: [],
                        ireports: {},
                        loaded: false,
                        current: null,
                        currentid: -1,
                        get : function(report){
                            var self = this;
                            report = report || self.current;
                            if(typeof report != 'object'){
                                report = self.ireports[report];
                            }
                            return report;
                        },
                        render : function(report,query,options){
                            var report = this.get(report);
                            if(!report)return;
                            options = $the.extend({
                                    target:null,
                                    compile:true,
                                    scope:$scope,
                                    onscope:function(s,q,r){
                                        s.report_query = q;
                                        s.report_def = r;
                                    }}, options || {});
                            var query = $the.extend({
                                id : report._id,
                                dataonly : null,
                                standalone : null,
                                format : null,
                                findquery : null,
                                flags : null,
                                success : null,
                                parameters : {}},query);
                            query.parameters = $the.extend(report.params,query.parameters);
                            if(!!report.onquery){
                                report.onquery(query,report,$scope);
                            }
                            var result = $http({
                               url:"/report",
                                method:"POST",
                                data :$the.jsonify(query,{nulls:false,defaults:false,functions:false})
                            }).then(function(data){
                                var report = data.data;
                                if(typeof(report)=="string"){
                                    if(!!options.target){
                                        var html  = $(report);
                                        $(options.target).html('');
                                        html.appendTo($(options.target));
                                        if(!!options.compile){
                                            var scope = !!options.scope? options.scope.$new() :  $rootScope.$new();
                                            if(!!options.onscope){
                                                options.onscope(scope,query,report);
                                            }
                                            $compile(html)(scope);
                                        }
                                    }
                                }
                                if(!!query.success){
                                    query.success(report);
                                }
                            });

                            return result;
                        },
                        select: function (report) {
                            var report = this.get(report);
                            if(!report)return;
                            if(this.current!=report){
                                this.current = report;
                            }
                            return report;
                        },
                        load: function () {
                            var self = this;
                            if (self.loaded) {
                                return $q(function (resolve) {
                                    resolve(self);
                                });
                            }
                            self.reports = [{_id: -1, name: "Выберите отчет"}];
                            self.ireports = {"-1": self.reports[0]};
                            var result =
                                $http.get('/reports/list', {noagents: true})
                                    .then(function (data) {
                                        loaded = true;
                                        prepare(self, data.data, $scope);
                                        return self;
                                    });
                            return result;
                        }
                    }
                }
            }
        ]);

    });
});