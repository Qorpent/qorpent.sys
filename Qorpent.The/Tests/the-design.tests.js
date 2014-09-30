/**
 * Created by comdiv on 24.09.14.
 */
(function (describe, it, before) {
    if (typeof window === "undefined") {
        describe("the.design - design test can be executed only in browser context", function () {
            it.skip("it's Node.js");
        });
        return;
    }
    describe("the.design", function () {
        describe("#fitText", function () {
            var smallTextElement = null;
            var largeTextElement = null;
            var ensureFitElement = function () {
                smallTextElement = smallTextElement || $("<div style='width: 600px;height:  70px;color:white;background-color:black;'>was small the design fitText plugin</div>").appendTo(document.body);
                largeTextElement = largeTextElement || $("<div style='width: 600px;height:  70px;color:white;background-color:black;'>was large the design fitText plugin</div>").appendTo(document.body);
                smallTextElement.css("font-size", "12pt");
                largeTextElement.css("font-size", "40pt");

            }
            it("can increase font-size", function () {
                ensureFitElement();
                fit(smallTextElement);
                $(smallTextElement).css("font-size").should.equal("40px");
            });
            it("can decrease font-size", function () {
                ensureFitElement();
                fit(largeTextElement);
                largeTextElement.css("font-size").should.equal("40px");
            });
            it("was bound to JQuery", function () {
                ensureFitElement();
                largeTextElement.fitText();
                smallTextElement.fitText();
                largeTextElement.css("font-size").should.equal("40px");
                smallTextElement.css("font-size").should.equal("40px");
            });
        })

        var $ = null;
        var fit = null;
        var $root = null;
        var should = null;
        before(function (done) {
            require(["./lib/chai", "../the-design-testfitter"], function ($should, $the) {
                fit = $the.design.fitText;
                $root = $the;
                should = $should;
                $ = window.$;
                done();
            });
        });

    });
})(describe, it, before);