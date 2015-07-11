/**
 * Created by comdiv on 04.11.2014.
 */
define(["the-angular","the-angular-unsafe"], function ($the, template) {
    return $the(function (root, privates) {
        if (null == root.modules)return;
        var dialog = (function (dialog) {
            var self = dialog;

            dialog.setupElement = function (element, attr) {
                var e = $(element);
                e.addClass('toolwindow dialog');
                e.addClass(attr['theDialog'] || 'modal');
                var form = e.children('form');
                if(null==form || 0==form.length){

                    form = $('<form name="handler.forms.dialog"></form>').appendTo(e);

                }
                e = form;

                if (0 == e.children('.content').length) {
                        $("<div ng-show='handler.template||handler.getTemplate' class='content'></div>").appendTo(e);
                        $("<div ng-show='!(handler.template||handler.getTemplate)' class='content' ng-include='handler.getView()'></div>").appendTo(e);
                }
                if (0 == e.children('.title').length) {
                    $("<label class='title'><span ng-bind='handler.getLabel()'></span><div class='tbt'><i class='fa fa-remove closer' ng-click='handler.close()'></i></div></label>").prependTo(e);
                }

                if (0 == e.children('nav').length) {
                    $("<nav>" +
                    "<i ng-if='handler.getShowExpand() && !handler.expanded' ng-click='handler.expand()' style='cursor:pointer' class='icon iprimary fa fa-arrows-alt'></i>" +
                    "<i ng-if='handler.getShowExpand() && handler.expanded' ng-click='handler.collapse()' class='icon iprimary fa fa-compress'></i>" +
                    "<i ng-if='handler.getShowSuccess()' ng-click='!handler.isValid || handler.success()' class='button success' ng-disabled='!handler.isValid' ng-bind='handler.getSuccessText()'></i>" +
                    "<i ng-if='handler.getShowCancel()' ng-click='handler.close()' class='button default'>Отмена</i>" +
                    "</nav>").appendTo(e);
                }

                if (0 == $(document.body).children('.modalback').length) {
                    $('<div class="modalback"></div>').appendTo(document.body);
                }
            };
            dialog._initialized = false;
            dialog.init = function () {
                if (!dialog._initialized) {
                    if (0 == $(document.body).children('.modalback').length) {
                        $('<div class="modalback"></div>').appendTo(document.body);
                    }
                    dialog.modalBack = $(document.body).children('.modalback');
                    $(document).keydown(function(event){
                        if(null==dialog.current)return true;
                        if(27!=event.keyCode)return true;
                        if(!dialog.current.__handler)return true;
                        dialog.current.__handler.close();
                        event.stopPropagation();
                        return false;
                    });
                    dialog._initialized = true;
                }
            }
            dialog.modalCount = 0;
            dialog.create = function (options) {
                if (!angular)throw "cannot work without angular";
                var rootscope = angular.element(document.body).scope();
                var compile = angular.element(document.body).injector().get('$compile');

                if (!rootscope)throw "cannot find root scope";
                var scope = rootscope.$new();
                if(!!options){
                    //it's initDialog funciton
                    if(typeof options=="function"){
                        scope.initDialog = function(handler,options,scope){options(handler,options,scope);};
                    }else{
                        //it's scope defintion
                        if(!!options.initDialog){
                            for(var i in options){
                                scope[i] = options[i];
                            }
                            scope.initDialog = function(handler,options,scope){options.initDialog(handler,options,scope);};
                        }
                        //it's handler overrides
                        else{
                            scope.initDialog = function(handler){
                                for(var i in options){
                                    if(i=="scope"){
                                        for(var s in options[i]){
                                            scope[s] = options[i][s];
                                        }
                                    }else {
                                        handler[i] = options[i];
                                    }
                                }
                            }
                        }
                    }
                }
                var element = $('<div><div the-dialog class="fixed"></div></div>');
                element = $(compile(element.contents())(scope)[0]);



                element.appendTo(document.body);
                if(!scope.$$phase) {
                    scope.$apply();
                }

                scope = angular.element(element[0]).scope();

                element[0].__scope = scope;
                element[0].__handler = scope.handler;
                return scope.handler;
            }

            dialog.alert = function(message,label,callback,createOnly){
                if(typeof(message)=="object"){

                    label = message.label;
                    callback = message.callback;
                    createOnly= message.createOnly;
                    message = message.message;
                }
                message = String(message);
                var finish = function(){
                    if(!!callback) {
                        callback(d);
                    }
                    return true;
                }
                var d = dialog.create({
                    message : message,
                    label : label,
                    onSuccess : finish,
                    onCancel : finish,
                    showCancel : false,
                    autoremove:true
                });
                if(!createOnly) {
                    d.execute();
                }
                return d;
            }

            dialog.prompt = function(message,label,value,callback,createOnly){

                if(typeof(message)=="object"){
                    label = message.label;
                    value = message.value;
                    callback = message.callback;
                    createOnly= message.createOnly;
                    message = message.message;
                }
                var d = dialog.create({
                    message : message,
                    label : label,
                    value:value,
                    view : '--prompt-dialog.html',
                    autoremove:true,
                    onSuccess : function(){
                        if(!!callback){
                            callback(true,this.value);
                        }
                        return true;
                    },
                    onCancel : function(){
                        if(!!callback){
                            callback(false);
                        }
                        return true;
                    }
                });
                if(!createOnly) {
                    d.execute();
                }
                return d;
            }

            dialog.confirm = function(message,label,callback,createOnly){

                if(typeof(message)=="object"){
                    label = message.label;
                    callback = message.callback;
                    createOnly= message.createOnly;
                    message = message.message;
                }
                var d = dialog.create({
                    message : message,
                    label : label,
                    view : '--message-dialog.html',
                    autoremove:true,
                    onSuccess : function(){
                        if(!!callback){
                            callback(true,this.value);
                        }
                        return true;
                    },
                    onCancel : function(){
                        if(!!callback){
                            callback(false);
                        }
                        return true;
                    }
                });
                if(!createOnly) {
                    d.execute();
                }
                return d;
            }

            dialog.current = null;

            dialog.show = function (element) {
                dialog.init();
                var je = $(element);
                var e = je[0];
                if(je.is(':visible'))return;
                var ismodal = je.hasClass('modal');
                if(ismodal){
                    dialog.modalCount++;
                    dialog.matchBackPosition();
                    e.__predialog = dialog.current;
                    dialog.current = e;
                }
                var zindex = 10000;
                if(ismodal){
                    zindex = 10000 + 10 * (dialog.modalCount -1);
                }
                je.css('z-index', zindex);
                je.addClass("active");
                dialog.resetonce =false;

                var content = je.find('.content');

                dialog.reset = function(){
                    var rect = je[0].getBoundingClientRect();
                    var width = window.innerWidth;
                    var height = window.innerHeight;
                    var top = height / 2 - rect.height / 2;
                    if(top > 100){
                        top = 50;
                    }
                    var left = width / 2 - rect.width / 2;
                    if(ismodal){
                        top += 20 * (dialog.modalCount - 1);
                        left  += 20 * (dialog.modalCount - 1);
                    }
                    je.css('top', top + "px");
                    je.css('left', left + "px");
                    content = je.find('.content.ng-scope');
                    if(!dialog.resetonce) {
                        content.on("mousewheel", function (ev) {
                            e = ev.currentTarget;
                            ev = ev.originalEvent;

                            var preventScroll = false;
                            var isScrollingDown = ev.wheelDelta < 0;

                            if (isScrollingDown) {
                                var isAtBottom = e.scrollTop + e.clientHeight >= e.scrollHeight - 2;

                                if (isAtBottom) {
                                    preventScroll = true;
                                }
                            } else {
                                var isAtTop = e.scrollTop == 0;
                                if (isAtTop) {
                                    preventScroll = true;
                                }
                            }

                            if (preventScroll) {
                                ev.preventDefault();
                            }
                        });
                        dialog.resetonce = true;
                    }
                }
               setTimeout(dialog.reset,300);

            }
            dialog.matchBackPosition = function(){
                if(dialog.modalCount==0){
                    dialog.modalBack.hide();
                }else{
                    var mbzindex = 10000-5 + 10 * (dialog.modalCount -1);
                    dialog.modalBack.css("z-index",mbzindex);
                    dialog.modalBack.show();
                }
            }

            dialog.hide = function (element) {
                dialog.init();
                var je = $(element);
                var e = je[0];
                if(!je.is(':visible'))return;
                var scope = null;
                var ismodal = je.hasClass('modal');
                if(ismodal){
                    if(dialog.modalCount>0) {
                        dialog.modalCount--;
                    }
                    dialog.current = e.__predialog;
                    dialog.matchBackPosition();
                }
                if (!!angular) {
                    scope = angular.element(je[0]).scope();
                    if (!!scope && !!scope.handler) {
                        scope.$tryApply(function () {
                            for (var i in scope.handler.forms) {
                                scope.handler.forms[i].$setPristine();
                            }
                            scope.item = "";
                        });

                    }

                }
                je.removeClass("active");
                if (scope && scope.handler.autoremove) {
                    je.remove();
                }
            }

            dialog.initScope = function (scope, element, attr) {
                var handler = scope.handler || (scope.handler = {
                        forms: {},
                        element : element,
                        attr: attr,
                        isValid: true,
                        autoremove:false,
                        execute: function () {
                            var self = this;
                            dialog.show(element[0]);
                            self.validate();
                            setTimeout(function(){
                                scope.$apply(function(){self.validate();});
                            },100);
                        },
                        close: function () {
                            if (!!this.onCancel) {
                                if (!this.onCancel()) {
                                    return;
                                }
                            }
                            dialog.hide(element[0]);
                        },
                        reset : function(){
                            dialog.reset();
                        },
                        success: function () {
                            if (!this.validate())return;
                            if (!!handler.onSuccess) {
                                if (!handler.onSuccess()) {
                                    return;
                                }
                            }
                            dialog.hide(element[0]);
                        },

                        validate: function () {
                           // console.log("validating...")
                            this.isValid = true;
                            if (!!this.onValidate) {
                                this.isValid = this.onValidate();
                            }
                            if (this.isValid) {
                                for (var i in this.forms) {
                                    var form = this.forms[i];
                                    if (!form.$valid) {
                                        this.isValid = false;
                                        break;
                                    }
                                }
                            }
                         //   console.log(this.isValid);
                            return this.isValid;
                        },
                        getLabel: function () {
                            return this.label || attr["label"] || "Диалог";
                        },
                        getView: function () {
                            var result =  this.view || attr["view"] || '--message-dialog.html';
                            return result;
                        },
                        refresh: function () {
                            scope.$tryApply();
                        },
                        getShowSuccess: function () {
                            if ('showSuccess' in this)return !!handler.showSuccess;
                            if ('showSuccess' in attr)return !!attr.showSuccess;
                            return true;
                        },
                        getShowCancel: function () {
                            if ('showCancel' in this)return !!handler.showCancel;
                            if ('showCancel' in attr)return !!attr.showCancel;
                            return true;
                        },
                        getShowExpand: function () {
                            if ('showExpand' in this)return !!handler.showExpand;
                            if ('showExpand' in attr)return !!attr.showExpand;
                            return false;
                        },
                        getSuccessText: function () {
                            if ('successText' in handler)return handler.successText;
                            if ('successText' in attr)return attr.successText;
                            return 'ОК';
                        },
                        expand : function(){
                            this.expanded =true;
                            this.element.addClass("expanded");
                        },
                        collapse : function(){
                            this.expanded =false;
                            this.element.removeClass("expanded");
                            this.element.find('.content').css('overflow-y','hidden');
                            var self = this;
                            setTimeout(function(){
                                self.element.find('.content').css('overflow-y','auto');
                            },100);
                        },
                        toggleExpand : function(){
                            if(this.expanded){
                                this.collapse();
                            }else{
                                this.expand();
                            }
                        }
                    });
                $the.$angular.directiveCall(scope, attr, "onInit", "initDialog", {handler: handler});
                if(handler.template || handler.getTemplate) {
                    var template  = (handler.getTemplate? handler.getTemplate():null)||handler.template;
                    if (!!template) {
                        var compile = angular.element(document.body).injector().get('$compile');
                        var body = $(compile(template)(scope)[0]);
                        var content = $(element[0]).children('.content')[0];
                        $(content).empty();
                        body.appendTo(content);

;                    }
                }
            }

            dialog.linkElement = function (scope, element, attr) {
                dialog.initScope(scope, element, attr);
            };
            dialog.setupModule = function (module) {
                module.directive('theDialog', ['$rootScope', function ($rootScope) {
                    return {
                        restrict: 'A',
                        scope: true,
                        compile: function (element, attr) {
                            self.setupElement(element, attr);
                            return {
                                pre: function (scope, element, attr) {
                                    return self.linkElement(scope, element, attr);
                                }
                            }
                        }
                    }
                }]);
                module.run(['$templateCache','$sce', function ($templateCache,$sce) {
                    var message = "<div ng-bind-html='handler.message | unsafe'></div>";
                    var prompt = '<div ng-bind-html="handler.message | unsafe"></div><input size="20" on-enter="!handler.isValid || handler.success()" autoselect ng-model="handler.value"/>';

                    $templateCache.put('--message-dialog.html',message );
                    $templateCache.put('--prompt-dialog.html',prompt );
                }]);
            };
            return dialog;
        })(root.dialog || (root.dialog = {}));
        root.modules.d_dialog = root.$angular.module("the-dialog", []);
        dialog.setupModule(root.modules.d_dialog);
        dialog.setupModule(root.modules.all);
    });
});