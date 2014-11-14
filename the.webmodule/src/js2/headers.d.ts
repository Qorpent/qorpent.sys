/**
 * Created by comdiv on 13.11.2014.
 */
declare module the {
    interface ICastOptions {
        defaultOnNull: boolean;
        ignoreCase: boolean;
        extensions: boolean;
        deep: boolean;
        clone: boolean;
        cloneInternals: boolean;
        functions: boolean;
        filter: (name: any, value: any, source: any) => boolean;
    }
    class CastOptions implements ICastOptions {
        defaultOnNull: boolean;
        ignoreCase: boolean;
        extensions: boolean;
        deep: boolean;
        clone: boolean;
        cloneInternals: boolean;
        functions: boolean;
        filter: (name: any, value: any, source: any) => boolean;
        static Default: CastOptions;
        static Cast: CastOptions;
        static Create: CastOptions;
        static Extend: CastOptions;
    }
    function extend<T>(target: T, source: any, options: ICastOptions): T;
    function create<T>(create: new () => T, source: any, options: ICastOptions): T;
    function cast<T>(ctor: new () => T, source: any, options: ICastOptions): T;
    function clone<T>(source: T, options: ICastOptions): T;
}
