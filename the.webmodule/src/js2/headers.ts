/**
 * Created by comdiv on 13.11.2014.
 */
module the {
    export interface ICastOptions {
        defaultOnNull : boolean;
        ignoreCase : boolean;
        extensions : boolean;
        deep : boolean;
        clone : boolean;
        cloneInternals : boolean;
        functions : boolean;
        filter : (name,value,source)=>boolean;
    }
    export declare class CastOptions implements ICastOptions{
        defaultOnNull : boolean;
        ignoreCase : boolean;
        extensions : boolean;
        deep : boolean;
        clone : boolean;
        cloneInternals : boolean;
        functions : boolean;
        filter : (name,value,source)=>boolean;
        static Default: CastOptions;
        static Cast : CastOptions;
        static Create : CastOptions;
        static Extend  : CastOptions;
    }

    export declare function extend<T>(target: T, source: any, options: ICastOptions):T;
    export declare function create<T>(create : new ()=>T,  source :any, options:ICastOptions) : T;
    export declare function cast<T>(ctor : new ()=>T, source: any, options:ICastOptions):T;
    export declare function clone<T>(source :T, options:ICastOptions ):T;
}