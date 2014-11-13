/**
 * Created by comdiv on 13.11.2014.
 */
declare module the {
    class CastOptions {
        DefaultOnNull: boolean;
        IgnoreCase: boolean;
        Extensions: boolean;
        Deep: boolean;
        Clone: boolean;
        CloneInternals: boolean;
        Functions: boolean;
        BindFunctions: boolean;
        Overridden: boolean;
        static Default: CastOptions;
        static Cast: CastOptions;
        static Create: CastOptions;
        static Extend: CastOptions;
    }
    function Extend(target: any, source: any, options: CastOptions): any;
    function Create<T>(ctor: new () => T, source: any, options: CastOptions): T;
    function Cast<T>(source: any, options: CastOptions): T;
    function Clone<T>(source: T, options: CastOptions): T;
}
