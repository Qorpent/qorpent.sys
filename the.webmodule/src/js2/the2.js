define(["require", "exports", "the2-object"], function (require, exports, $extender) {
    var result = { Extender: $extender.Extender };
    result.extend = $extender.Extender.extend;
    result.clone = $extender.Extender.clone;
    result.cast = $extender.Extender.cast;
    return result;
});
//# sourceMappingURL=the2.js.map