/**
 * Created by comdiv on 24.09.14.
 */
(function (describe, it, before) {
    if (typeof window === "undefined") {
        describe("the.design - design test can be executed only in browser context", function () {
            it("it's Node.js");
        });
        return;
    }
    describe("the.design", function () {
        describe("#fitText", function () {
            var smallTextElement = null;
            var largeTextElement = null;
            var angularElement = null;
            var angularControllerElement = null;
            var angularSmallElement = null;
            var ensureFitElement = function () {
                smallTextElement = smallTextElement || $("<div style='width: 600px;height:  70px;color:white;background-color:black;'>was small the design fitText plugin</div>").appendTo(document.body);
                largeTextElement = largeTextElement || $("<div style='width: 600px;height:  70px;color:white;background-color:black;'>was large the design fitText plugin</div>").appendTo(document.body);
                smallTextElement.css("font-size", "12pt");
                largeTextElement.css("font-size", "40pt");

            }

            it("can increase font-size", function () {
                ensureFitElement();
                fit(smallTextElement);
                $(smallTextElement).css("font-size").replace(/px/,"").should.within(39,42);
            });
            it("can decrease font-size", function () {
                ensureFitElement();
                fit(largeTextElement);
                largeTextElement.css("font-size").replace(/px/,"").should.within(39,42);
            });
            it("was bound to JQuery", function () {
                ensureFitElement();
                largeTextElement.fitText();
                smallTextElement.fitText();
                largeTextElement.css("font-size").replace(/px/,"").should.within(39,42);
                smallTextElement.css("font-size").replace(/px/,"").should.within(39,42);
            });
            it("can be used as angular directive", function (done) {
                angularElement = angularElement || $("<div></div>").appendTo(document.body);
                angularControllerElement = angularControllerElement || $("<div ng-controller='fitText'></div>").appendTo(angularElement);
                angularSmallElement = angularSmallElement || $("<div style='width: 600px;height:  70px;color:white;background-color:black;' the-text-fitter='message' ></div>").appendTo(angularControllerElement);
                angularSmallElement.css("font-size", "12pt");
                angular.module("fitText",["the-textfitter"]).controller('fitText',["$scope",function($scope){
                   $scope.message = "was small the design fitText angular";
                }]);
                angular.bootstrap(angularElement[0],["fitText"]);
                setTimeout(function(){
                    angularSmallElement.css("font-size").replace(/px/,"").should.within(38,42);
                    done();
                },100)

            });
        })

        var $ = null;
        var fit = null;
        var $root = null;
        var should = null;
        var angular = null;
        before(function (done) {
            require(["./lib/chai", "../the-design-textfitter"], function ($should, $the) {
                fit = $the.design.fitText;
                $root = $the;
                should = $should;
                $ = window.$;
                angular = window.angular;
                done();
            });
        });

    });
})(describe, it, before);