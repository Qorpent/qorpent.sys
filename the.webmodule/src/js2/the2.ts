/**
 * Created by comdiv on 14.11.2014.
 */
import $extender = require("the2-object");
var result = {Extender:$extender.Extender};
result.extend = $extender.Extender.extend;
result.clone = $extender.Extender.clone;
result.cast = $extender.Extender.cast;
export = result;