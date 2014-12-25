/**
 * Created by comdiv on 14.11.2014.
 */
///<reference path="headers.d.ts"/>
export module Extender {
    export class CastOptions implements the.ICastOptions{
        defaultOnNull : boolean = false;
        ignoreCase : boolean = false;
        extensions : boolean = true;
        deep : boolean = false;
        clone : boolean =false;
        cloneInternals : boolean =false;
        functions : boolean =true;
        filter : (name,value,source)=>boolean = (name,value,source)=>true;
        static Default: the.ICastOptions = new CastOptions();
        static Cast : the.ICastOptions = new CastOptions({extensions:false,defaultOnNull:true,ignoreCase:true,cloneInternals:true});
        static Create : the.ICastOptions = new CastOptions({extensions:false});
        static Extend  : the.ICastOptions = new CastOptions({ defaultOnNull: true, ignoreCase: true, cloneInternals: true, deep:true});
        static Clone  : the.ICastOptions = new CastOptions({cloneInternals:true});
        constructor (options?:any){setValues(this,options);}

    }


    export function setValues<T>(target: T, ...source: any[]):T{
        source.forEach(function(_){
            for(var i in _){
                if(target.hasOwnProperty(i)){
                    target[i]= _[i];
                }
            }
        });


        return target;
    }

    export function propertise<T>(obj:T):T{
        for(var i in obj){
            if(obj.hasOwnProperty(i) &&(typeof obj[i] == "object") && ("get" in  obj[i])){
                Object.defineProperty(obj,i,obj[i]);
            }
        }
        return obj;
    }


    export function extend<T>(target: T, source: any, options: the.ICastOptions = CastOptions.Default):T{
        if(!(options instanceof CastOptions)){
            options= setValues(new CastOptions(),CastOptions.Default,options);
        }
       if (options.clone) {
            var copy  = target.constructor ? new target.constructor() : {};
            target = extend(copy, target, CastOptions.Clone);
        }
        if (typeof source === "undefined" || null === source)return target;
        var trgKeys = {};
        var srcKeys = {};
        var i;
        for (i in target) {
            //works only on own properties
            if (!target.hasOwnProperty(i))continue;
            trgKeys[i] = i;
            if (options.ignoreCase) {
                trgKeys[i.toLowerCase()] = i;
            }
        }
        for (i in source) {
            //works only on own properties
            if (!source.hasOwnProperty(i))continue;
            if (!options.filter(i, source[i], source))continue;
            if (!options.functions && typeof  source[i] === "function")continue;
            srcKeys[i] = i.toLowerCase();

        }
        for (i in srcKeys) {
            if (!srcKeys.hasOwnProperty(i))continue;
            var exists = false;
            var trg = trgKeys[i];
            if (typeof trg == "undefined")trg = trgKeys[i.toLowerCase()];
            if (typeof trg !== "undefined")exists = true;
            if (!exists && options.extensions)trg = i;
            if (typeof trg !== "undefined") {
                var src = source[i];
                if (options.cloneInternals && isUserDefined(target[trg])) {
                    target[trg] = clone(target[trg]);
                }
                if (options.cloneInternals && isUserDefined(src)) {
                    src = clone(src);
                }
                if (exists && options.deep && isUserDefined(target[trg]) && isUserDefined(src)) {
                    extend(target[trg], src, options);
                } else {

                    target[trg] = src;
                }
            }
        }
        return target;
    }
    export function create<T>(ctor : new ()=>T,  source :any, options:the.ICastOptions = CastOptions.Create) : T{
        if(!(options instanceof CastOptions)){
            options= setValues(new CastOptions(),CastOptions.Create,options);
        }
        var result;
        if (Array.isArray(source)) {
            result = Object.create(ctor.prototype);
            var _parse = ctor.__parse__ || ctor.prototype.__parse__;
            if (!!_parse && source.length == 1 && typeof source[0] === "string") {
                ctor.apply(result, []);
                _parse.apply(result, [source[0]]);
            } else {
                ctor.apply(result, source);
            }
            return result;
        } else if (typeof source == "function") {
            //treat them as factories
            result = source(ctor);
            return cast(ctor, result);
        } else if (typeof source != "object" || source instanceof RegExp) {
            return cast(ctor, [source]);
        } else {
            result = Object.create(ctor.prototype);
            ctor.apply(result, []);
            extend(result, source, options);
            return result;

        }
    }
    export function cast<T>(ctor : new ()=>T,obj: any, options:the.ICastOptions = CastOptions.Cast):T{
        if(!(options instanceof CastOptions)){
            options= setValues(new CastOptions(),CastOptions.Cast,options);
        }
        if (obj instanceof ctor)return obj;
        if (null == obj || typeof obj == "undefined") {
            if (options.defaultOnNull)return create(ctor, []);
            return null;
        }
        return create(ctor, obj, options);
    }
    export function isDefaultValue(obj:any):boolean{
        if (null === obj)return true;
        if ("" === obj)return true;
        if (0 === obj)return true;
        if (false === obj)return true;
        if (Array.isArray(obj) && 0 == obj.length)return true;
        if (isUserDefined(obj)) {
            var hasown = false;
            for (var i in obj) {
                if (obj.hasOwnProperty(i)) {
                    hasown = true;
                    break;
                }
            }
            return !hasown;
        }
        return false;
    }
    export function isUserDefined(obj : any) : boolean {
        if (typeof obj === "undefined" || null === obj)return false;
        if (typeof obj !== "object") return false;
        if (obj instanceof RegExp)return false;
        return !(obj instanceof Date);
    }
    export function clone<T>(source :T,options?:the.ICastOptions):T;
    export function clone<T>(source :T[],options?:the.ICastOptions):T[];
    export function clone(source : any, options:the.ICastOptions =CastOptions.Clone):any{
        if(!(options instanceof CastOptions)){
            options= setValues(new CastOptions(),CastOptions.Clone,options);
        }
        if (Array.isArray(source)) {
            var result = [];
            for (var i = 0; i < source.length; i++) {
                result.push(clone(source[i],options));
            }
            return result;
        }
        if (typeof undefined == source || null == source || !isUserDefined(source))return source;
        return extend({}, source, options );
    }
}