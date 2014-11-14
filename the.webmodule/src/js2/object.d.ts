/// <reference path="headers.d.ts" />
/**
 * Created by comdiv on 14.11.2014.
 */
export declare module Extender {
    function setValues<T>(target: T, source: any): T;
    function extend<T>(target: T, source: any, options: the.ICastOptions): T;
    function create<T>(create: new () => T, source: any, options: the.ICastOptions): T;
    function cast<T>(source: T, options: the.ICastOptions): T;
    function isDefaultValue(obj: any): boolean;
    function isUserDefined(obj: any): boolean;
    function clone<T>(source: T, options: the.ICastOptions): T;
    function clone<T>(source: T[], options: the.ICastOptions): T[];
}
