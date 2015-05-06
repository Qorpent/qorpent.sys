/**
 * Created by comdiv on 03.05.2015.
 */
define(["the", "chai", "jquery", "angular"], function (the, chai) {
    var expect = chai.expect;
    describe("API ensures", function () {
        it("has correspond THE object", function () {
            expect(the.dialog).exist;
            expect(the.dialog).has.property('setupModule');
            expect(the.dialog).has.property('setupElement');
            expect(the.dialog).has.property('linkElement');
        });


        it("has correspond ANGULAR module", function () {
            var module = angular.module("the-dialog");
            expect(module).exist;
        });
        it("has unsafe filter", function(){
           var injector = angular.injector(["ng","the-all"]);
            expect(injector.get('unsafeFilter')).exist;
        });
        it("has correspond the-dialog directive in the-dialog and the-all modules", function () {
            var injector = angular.injector(['ng', 'the-dialog']);
            var directive = injector.get('theDialogDirective');
            expect(directive).exist;

            injector = angular.injector(['ng', 'the-all']);
            directive = injector.get('theDialogDirective');
            expect(directive).exist;
        });
    });
    describe("Static dialog binding", function () {
        var e = $('#dialog1');
        var mb = $('.modalback');
        it("setups dialog classes", function () {
            expect(e.hasClass('dialog'), 'dialog').true;
            expect(e.hasClass('toolwindow'), 'toolwindow').true;
            expect(e.hasClass('modal'), 'modal').true;
        });

        it("document has modal back",function(){
           expect($(document).find('.modalback').length).eq(1);
        });

        it("is invisible by default",function(){
           expect(e.is(":visible")).false;
        });

        describe("Automatic controller-driven setup", function () {
            before(function (done) {
                setTimeout(done, 100);
            });
            it("has title bar", function () {
                var tb = e.children('form').children('.title')[0];
                expect(tb, 'title bar existed').exist;
            });
            it("has content zone", function () {
                var tb = e.children('form').children('.content')[0];
                expect(tb, 'content zone existed').exist;
            });
            it("has nav bar", function () {
                var tb = e.children('form').children('nav')[0];
                expect(tb, 'bottom zone existed').exist;
            });

            it("was initialized", function () {
                var scope = angular.element(e[0]).scope();
                expect(scope, 'controll of scope dialogController ixistence').exist;
                expect(scope.handler).exist;
                expect(scope.handler.execute,'execute callback').exist;
            });
        });

        describe("Automatic controller-driven behavior", function () {
            before(function (done) {
                setTimeout(done, 100);
            });
            var timer = 0;
            beforeEach(function(done){
                setTimeout(function(){
                    handler.close();
                    expect(e.is(":visible"),"dialog is invisible").false;
                    expect(mb.is(":visible"),"modalback is invisible").false;
                    done();
                }, timer+=20);
            });
            var scope = angular.element(e[0]).scope();
            var handler = scope.handler;
            var injector = angular.element(e[0]).injector();
            var vcontext = injector.get('viewcontext');

            it("environment valid",function(){
                expect(handler,'ensure handler').exist;
                expect(vcontext,'ensure vcontext').exist;
            });
            it("will show and hide dialog",function(done){
                scope.$apply(vcontext.setObject({code:1,name:2}));
                setTimeout(function(){
                    expect(e.is(":visible"),"dialog is visible").true;
                    handler.close();
                    expect(e.is(":visible"),"dialog is invisible").false;
                    done();
                },10);
            });
            it("empty valid cycle", function (done) {
                scope.$apply(vcontext.setObject({code:1,name:2}));
                setTimeout(function(){
                    expect(handler.validate(),"data is valid").true;
                    handler.close();
                    done();
                },10);
            });
            it("change valid cycle", function (done) {
                scope.$apply(vcontext.setObject({code:1,name:2}));
                setTimeout(function(){
                    expect(handler.validate(),"data is valid").true;
                    scope.item.name = 3;
                    handler.success();
                    expect(vcontext.editObject.name).eq(3);
                    expect(e.is(":visible"),"dialog is invisible").false;
                    done();
                },10);
            });
            it("change custom invalid cycle",function(done){
                console.log('change custom invalid cycle');
                scope.$apply(vcontext.setObject({name:2}));
                setTimeout(function(){
                    console.log('change custom invalid cycle - internal');
                    expect(handler.validate(),"data is invalid").false;
                    scope.item.name = 3;
                    handler.success();
                    //cannot appply
                    expect(vcontext.editObject.name).eq(2);
                    expect(e.is(":visible"),"dialog is invisible").true;
                    //fix problem
                    scope.item.code = 2;
                    expect(handler.validate(),"data is valid").true;
                    handler.success();
                    expect(vcontext.editObject.name).eq(3);
                    expect(e.is(":visible"),"dialog is invisible").false;
                    done();
                },10);
            });
            it("change form invalid state",function(done){
                scope.$apply(vcontext.setObject({code:1,name:2}));
                setTimeout(function(){
                    expect(handler.validate(),"data is valid").true;
                    scope.$apply(scope.item.name = "");
                    handler.success();
                    //cannot appply
                    expect(vcontext.editObject.name).eq(2);
                    expect(e.is(":visible"),"dialog is invisible").true;
                    expect(handler.validate(),"data is valid").false;
                    //fix problem
                    scope.$apply(scope.item.name = 3);
                    expect(handler.validate(),"data is valid").true;
                    handler.success();
                    expect(vcontext.editObject.name).eq(3);
                    expect(e.is(":visible"),"dialog is invisible").false;
                    done();
                },10);
            });
        });

    });
    describe("Inline dialog opening",function(){
        var e = $('#dialog1');
        var mb = $('.modalback');
        before(function (done) {
            setTimeout(done, 500);
        });
        var timer = 0;
        beforeEach(function(done){
            setTimeout(function(){
                done();
            }, timer+=20);
        });
        it("can be opened",function(){
            var d = the.dialog.create({message:"hello"});
            d.execute();
            expect($(d.element[0]).is(":visible")).true;
            expect(d.element.html()).contain("hello");
            expect(the.dialog.modalCount).eq(1);
            d.close();
        });
        it("nest  open",function(){
            var d = the.dialog.create({message:"hello"});
            d.execute();
            var zi = function(e){
                return Number($(e[0]).css("z-index")||0);
            }
            expect(zi(mb)).below(zi(d.element));
            var d2 = the.dialog.create({message:"hello2"});
            d2.execute();
            expect(zi(mb)).below(zi(d2.element));
            expect(zi(mb)).above(zi(d.element));
            expect(zi(d2.element)).above(zi(d.element));
            var d3 = the.dialog.create({message:"hello2"});
            d3.execute();
            expect(zi(mb)).below(zi(d3.element));
            expect(zi(mb)).above(zi(d2.element));
            expect(zi(d3.element)).above(zi(d2.element));
            d3.close();
            d2.close();
            d.close();
        });
    });
});