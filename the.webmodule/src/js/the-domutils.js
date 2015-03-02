/**
 * Created by comdiv on 26.02.2015.
 */
define([
    "the-root"
], function (the) {
    var $ = the.$jQuery;

    if (!$) {
        console.error('JQuery required');
        return the;
    }

    $.fn.exists = function () {
        return this.length !== 0;
    }
    $.fn.fst = function () {
        return this.length !== 0 ? this[0] : null;
    }

    $.fn.csss = function(options){
        var result = {};
        this.each(function(i,e){
            options.forEach(function(v){
               result[v] = $(e).css(v);
            });
        });
        return result;
    }


    var result = {};

    var maxWidth = result.maxWidth = function (e, defaultValue) {
        e = $(e);
        defaultValue = defaultValue || $(window).width();
        var cssmax = e.css('max-width');

        if (cssmax && cssmax.match(/px$/) && cssmax!="0px") {
            return Number(cssmax.match(/\d+/));
        }
        return defaultValue;
    }

    $.fn.maxWidth = function (defaultValue) {
        var result = [];
        if (this.length !== 0) {
            this.each(function (i, _) {
                result.push([maxWidth(_, defaultValue), _]);
            });
        }
        return result;
    }
    var minWidth = result.minWidth = function (e, defaultValue) {
        e = $(e);
        defaultValue = defaultValue || 50;
        var cssmin = e.css('min-width');
        if (cssmin && cssmin.match(/px$/) && cssmin!="0px") {
           return Number(cssmin.match(/\d+/));
        }
        return defaultValue;
    }

    var absolutize = result.absolutize = function(e){
        e = $(e);
        e.css('position','absolute');
        e.css('top','auto');
        e.css('bottom','auto');
        e.css('left','auto');
        e.css('right','auto');
    }

    var isrelative = result.isrelative = function(e){
        e = $(e);
        var position = e.css('position');
        return position=="relative" || position=="absolute";
    }


    $.fn.minWidth = function (defaultValue) {
        var result = [];
        if (this.length !== 0) {
            this.each(function (i, _) {
                result.push([minWidth(_, defaultValue), _]);
            });
        }
        return result;
    }

    var ensureVisibility = result.ensureVisibility = function (e, defaultDisplay, absolutize) {
        e = $(e);
        defaultDisplay = defaultDisplay || 'flex';

        var display = e.css('display');
        if (display != 'none') {
            return false;
        }
        e.css('visibility', 'hidden');
        e.css('display', defaultDisplay);
        if (!!absolutize) {
            e.css('position', 'absolute');
        }
        return true;
    }

    var restoreVisibility = result.restoreVisibility = function (e, stateObject) {
        if (stateObject === false)return;
        e = $(e);
        e.css({display: "", visibility: "", position: ""})
    }

    /*
     FIT WIDTH
     automatically determines best size for element
     that doesn't increase height, but is minimal
     */


    var FitWidthOptions = result.FitWidthOptions = function () {
        this.min = 50;
        this.max = ($(window).width() / 2);;
        this.step = 10;
        this.cache = false;
        this.reset = false;
        this.apply = true;
        this.defaultDisplay = 'flex';
        this.absolutize = true;
    }

    fitWidthMarker = FitWidthOptions.CacheMarker = "__hasFitWidth";

    var defaultFitWidthOptions = FitWidthOptions.Default = new FitWidthOptions();


    var fitWidth = result.fitWidth = function (e, options) {
        e = $(e);
        if (!e.exists())return;
        options = the.cast(FitWidthOptions, options);
        el = e.fst();
        if (options.cache && !options.reset && options.apply) {
            if (!!el[fitWidthMarker])return el[fitWidthMarker];
        }
        var vs = ensureVisibility(e, options.defaultDisplay, options.absolutize);
        e.css('overflow-y','none');
        e.css('word-break','break-all');
        e.height('auto');
        var max = Math.min(maxWidth(e), options.max);
        var min = Math.min(minWidth(e), options.min);
        var initialWidth = e.width();
        var initialHeight = e.height();
        e.width(max);
        var height = e.height();
        var currentSize = max;
        while (currentSize > min) {
            e.width(currentSize - options.step);
            if (e.height() > height) {
                e.width(currentSize);
                break;
            }
            currentSize = currentSize - options.step;
        }
        var resultWidth = e.width();
        var resultHeight = e.height();
        if (options.cache && options.apply) {
            el[fitWidthMarker] = resultWidth;
        }


        if (!options.apply) {
            e.width(initialWidth);
        }
        restoreVisibility(e, vs);
        var result = {width: resultWidth, height: resultHeight, beforeWidth: initialWidth, beforeHeight: initialHeight};
        return result;
    }

    $.fn.fitWidth = function (options) {
        if (this.length !== 0) {
            this.each(function (i, _) {
                fitWidth(_, options);
            });
        }
        return this;
    }
    $.fn.suggestWidth = function (options) {
        var result = [];
        if (this.length !== 0) {
            options = the.cast(FitWidthOptions, options);
            options.apply = false;
            this.each(function (i, _) {
                result.push([fitWidth(_, options), _]);
            });
        }
        return result;
    }


    PlaceAsidePositions = result.PlaceAsidePositions = {};

    var up = PlaceAsidePositions.up = 'up';
    var down = PlaceAsidePositions.down = 'down';
    var right = PlaceAsidePositions.right = 'right';
    var left = PlaceAsidePositions.left = 'left';
    var top = PlaceAsidePositions.top = 'top';
    var bottom = PlaceAsidePositions.down = 'bottom';



    var BL = PlaceAsidePositions.BL = {side: bottom, v: down, h: left};
    var BR = PlaceAsidePositions.BR = {side: bottom, v: down, h: right};
    var TL = PlaceAsidePositions.TL = {side: top, v: up, h: left};
    var TR = PlaceAsidePositions.TR = {side: top, v: up, h: right};

    var RD = PlaceAsidePositions.RD = {side: right, v: down, h: right};
    var RU = PlaceAsidePositions.RU = {side: right, v: up, h: right};
    var LD = PlaceAsidePositions.LD = {side: left, v: down, h: left};
    var LU = PlaceAsidePositions.LU = {side: left, v: up, h: left};
    var defaultPositions = PlaceAsidePositions.Default = [ BR,BL,  TR, TL, RD, RU, LD, LU];


    var PlaceAsideOptions = result.PlaceAsideOptions = function () {
        this.fitwidth = true;
        this.target = null;
        this.apply = true;
        this.cache = false;
        this.reset = false;
        this.overlap = 1;
        this.positions = defaultPositions;
        this.defaultDisplay = 'flex';
        this.maxWidth = $(window).width() / 2;
        this.minWidth = 50;
        this.fitWidthStep = 10;
        this.fixedContent = false;
        this.padding = 5;
    };

    var __getFittedRectangle = function (e, options) {
        el = e.fst();
        var vs = ensureVisibility(e, options.defaultDisplay, true);
        var selfRect = the.extend({},el.getBoundingClientRect());
        if (options.fitwidth) {
            var fitted = fitWidth(e, {
                min: options.minWidth,
                max: options.maxWidth,
                step: options.fitWidthStep,
                cache: options.fixedContent,
                apply: options.apply
            });
             the.extend(selfRect, fitted);
        }
        restoreVisibility(vs);
        return selfRect;
    }

    var paEvalSpace = function(variant, rect, win, options){
        var h = 0;
        if(variant.v==up){
            h = variant.side==top ? (rect.top - win.scroll) : (rect.bottom - win.scroll);
        }else{
            h = variant.side==bottom ? (win.height + win.scroll - rect.bottom) : (win.height + win.scroll - rect.top);
        }
        var w= 0;
        if(variant.h==right){
            w= variant.side == right ? (win.width - rect.right) : (win.width - rect.left);
        }else{
            w = variant.side == left ? (rect.left ) : (rect.right);
        }
        variant.width  =w - options.padding;
        variant.height = h - options.padding;
    }

    var paCheckFit = function(variant, self){
        if(self.width <= variant.width){
            variant.fitwidth = true;
        }else{
            variant.fitwidth = variant.width;
        }

        if(self.height <= variant.height){
            variant.fitheight = true;
        }else{
            variant.fitheight = variant.height;
        }

        variant.fit = (variant.fitheight===true) && (variant.fitwidth===true);
    }

    var paGetBestVariant = function(selfRect,targetrect,options){
        var win = {width: $(window).width(),height:$(window).height(),scroll:$(window).scrollTop()};
        var variants = [];
        if(!options.positions || options.positions.length==0){
            options.positions = PlaceAsidePositions.Default;
        }
        options.positions.forEach(function(position){
            if(typeof(position)=="string"){
                position = PlaceAsidePositions[position];
            }
            var variant = the.extend({},position);
            paEvalSpace(variant,targetrect,win,options);
            paCheckFit(variant,selfRect);
            variants.push(variant);
        });
        var bestfit = null;
        for(var i = 0;i<variants.length;i++){
            if(variants[i].fit){
                bestfit = variants[i];
                break;
            }
        }
        if(null==bestfit){
            for(var i = 0;i<variants.length;i++){
                if(variants[i].fitwidth===true){
                    bestfit = variants[i];
                    break;
                }
            }
        }
        if(null==bestfit){
            for(var i = 0;i<variants.length;i++){
                if(variants[i].fitheight===true){
                    bestfit = variants[i];
                    break;
                }
            }
        }
        if(null==bestfit){
            bestfit = variants[0];
        }
        
        return bestfit;
    }

    var paAdaptSize = function(e,variant){
        
        if(variant.fitwidth!==true){
            e.outerWidth(variant.fitwidth);
            if(e.outerHeight()>variant.height){
                variant.fitheight = variant.height;
            }
        }
        if(variant.fitheight!==true){
            e.css('overflow-y','auto');
            e.outerHeight(variant.fitheight);
        }
    }

    var paGetTarget = function(e,options) {
        var target = $(options.target || e.parent());
        if(target.fst()!= e.parent().fst()){
            e.detach();
            e.appendTo($(target));
        }
        return target;
    }

    var paSetPosition = function(e,targetrect,relative,variant,options){
        var result = {
            top: "auto",
            bottom:"auto",
            left:"auto",
            right:"auto"
        }

        if(variant.side == top || variant.side ==bottom){
            var tb = targetrect.height - options.overlap;
            if(variant.h == right){
                result.left = 0;
            }else{
                result.right = 0;
            }
            if(variant.side==top){
                result.bottom = tb;
            }else{
                result.top = tb;
            }
        }else{
            var lr = targetrect.width - options.overlap;
            if(variant.v == up){
                result.bottom = 0;
            }else{
                result.top = 0;
            }
            if(variant.side == left){
                result.right = lr;
            }else{
                result.left = lr;
            }
        }
        if(result.top!="auto"){
            result.top = result.top+="px";
        }
        if(result.bottom!="auto"){
            result.bottom =  result.bottom+="px";
        }
        if(result.left!="auto"){
            result.left =  result.left+="px";
        }
        if(result.right!="auto"){
            result.right =  result.right+="px";
        }
        e.css(result);
    }

    var placeAside = result.placeAside = function (e, options) {
        e = $(e);
        if (!e.exists())return;
        options = the.cast(PlaceAsideOptions, options);
        var restoreCss = e.csss(['top','bottom','left','right']);
        var target = paGetTarget(e,options);
        absolutize(e);
        var targetrect = target.fst().getBoundingClientRect();
        var selfRect = __getFittedRectangle(e, options);
        
        var variant = paGetBestVariant(selfRect,targetrect,options);
        paAdaptSize(e,variant);


        var relative =isrelative(target);
        if(!relative){
            target.css("position","relative");
            relative =true;
        }

        paSetPosition(e,targetrect,relative,variant,options);

        var resultRect = e.fst().getBoundingClientRect();

        var result = {
            base : selfRect,
            result : resultRect
        }

        if (!options.apply) {
            e.css(restoreCss);
        }

        return result;
    }

    $.fn.placeAside = function(options) {
        this.each(function(i,_){
           placeAside(_,options);
        });
    }

    var angular = the.$angular;
    if (!!angular) {
        angular.module("the-domutils", []).factory("the-domutils", [
            function () {
                return result;
            }
        ]);
    }

    return the(function (root) {
        root.domutils = result;
    });
})