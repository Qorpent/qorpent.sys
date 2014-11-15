/**
 * Action/Action builder module to wrapp HTTP/AJAX calls
 */
    define(["the-http"], function ($the) {
        return $the(function ($root) {

            var h = $root.http;
            var timeout = $root.timeout;

            var TestTransport = h.TestTransport = function () {
                h.Transport.call(this);
            };
            TestTransport.prototype = Object.create(h.Transport.prototype);
            TestTransport.prototype.callData = function (request, success, error) {
                var resp = null;
                if (typeof request.response === "undefined") {
                    resp = TestTransport.responseFactory(request);
                }

                var t = resp.timeout || 20;
                timeout(function () {
                    if (!!resp.error) {
                        error(resp.error, resp);
                    } else {
                        success(resp.data, resp);
                    }
                }, t);
            };
            TestTransport.responseFactory = function (req) {
                if (req.url === "/good") {
                    var res = {data: {good: true}};
                    if (!!req.timeout) {
                        res.timeout = req.timeout + 10;
                    }
                    return res;
                } else if (req.url === "/echo") {
                    return {data: req.params};
                } else {
                    return {error: "some fail"};
                }
            };
        });
    });