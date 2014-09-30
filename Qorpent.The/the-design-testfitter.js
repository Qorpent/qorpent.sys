/**
 * Created by comdiv on 26.09.14.
 * @description расширение для подгона размера шрифта текста под заданный размер целевого элемента
 */
(function (define) {
    define(["./the"], function ($the) {
        return $the(function ($root, $privates) {
            $root.design = $root.design || {};
            var fitter = null;
            var createFitter = function () {
                var result = document.createElement("div");
                result.setAttribute("id", "fitter$element");
                result.setAttribute("style", "position:absolute;visibility:hidden;z-index: -100; background-color:lightcoral;top:0;left:0;padding: 0px;margin: 0px;border:none;");
                document.body.appendChild(result);
                return result;
            }
            var textFitStyles = ["fontFamily", "fontWeight", "textAlign", "textIndent", "padding", "margin", "border", "verticalAlign"];
            var fitAccurancy = 0.93;
            var fitSingle = function (e, text, width, height) {
                fitter = fitter || createFitter();
                for (var i = 0; i < textFitStyles.length; i++) {
                    fitter.style[textFitStyles[i]] = e.style[textFitStyles[i]];
                }
                fitter.style.width = (width || e.offsetWidth) + "px";
                var t = text || e.textContent;
                if (t) {
                    fitter.textContent = t;
                    var fitheight = Math.round((height || e.offsetHeight));
                    i = Number(e.style.fontSize.match(/\d+/)) || 12;
                    fitter.style.fontSize = i + "pt";
                    var result = i;
                    if (fitter.offsetHeight < fitheight) {
                        for (var i = i; i < 256; i++) {
                            fitter.style.fontSize = i + "pt";
                            if (fitter.offsetHeight > fitheight) {
                                result = Math.round(i * fitAccurancy);
                                break;
                            }
                        }
                    } else if (fitter.offsetHeight > fitheight) {
                        for (i = i; i >= 8; i--) {
                            fitter.style.fontSize = i + "pt";
                            if (fitter.offsetHeight < fitheight) {
                                result = Math.round(i * fitAccurancy);
                                break;
                            }
                        }
                    }
                    e.style.fontSize = result + "pt";
                    if (!!text) {
                        e.textContent = t;
                    }
                }
            }
            var fit = $root.design.fitText = function (el, text, width, height) {
                if (!!el.length) { //marks arrays an $(...) enums
                    for (var i = 0; i < el.length; i++) {
                        fitSingle(el[i], text, width, height);
                    }
                } else {
                    fitSingle(el, text, width, height);
                }
            };

            $the.checkEnvironment();
            if (!!$the.$jQuery) {
                $the.$jQuery.fn.fitText = function (text, width, height) {
                    fit(this, text, width, height);
                    return this;
                }
            }
        });
    });
})(typeof define === "function" ? define : require('amdefine')(module));