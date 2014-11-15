/**
 * Created by comdiv on 12.11.2014.
 */
var requirejs = require("requirejs");
var amdefine = require("amdefine");
var package = require("../package.json");

requirejs.define("package",[], function () {
    return package;
});
requirejs(["testmap"],function($tests) {
    requirejs.config({
        baseUrl : "../tests/",
        nodeRequire: require,
        paths: {
            "mocha" :"../lib/mocha",
            "chai" :"../lib/chai",
            "angular" : "../lib/angular",
            "teamcity" : "../lib/teamcity",
            "angular-mocks" : "../lib/angular-mocks"
        },
        deps: ["mocha", "chai", "teamcity"],
        callback: function () {
            global.location = {};
            var tc =requirejs("teamcity");
            mocha.setup({ui:"bdd",ignoreLeaks: true,reporter: function(runner) {
                new tc(Mocha.reporters.Base,runner);
            }});
            var profile = {node:true};
            profile.context = process.argv.join(" ");
            requirejs ($tests(profile),function() {
                var chai = requirejs("chai");
                var should = chai.Should();
                mocha.run();
            });
        }
    });
});
