require.config({
    paths: {
        angular: '../lib/angular'
    },
    shim: {
        angular: {
            exports: 'angular'
        }
    },
    deps: ["angular", 'demo-controllers'],
    callback: function () {
        angular.bootstrap(document, ['demo-controllers']);
    }
});