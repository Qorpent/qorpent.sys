(function(){

    /*
        here module is defined itself
        good idea that module is always object - for JS it's better
        because function() is everything in JS - it can be object, class and so on
        object modules are far more strict

        This sample is not real AMD it's just AMD-compatible, while no dependency resolution logic exists here
     */
    var module = function(){};

    /* add some functionality on your own */
    module.execute = function(){
        return "hello world";
    };



    /*
        some tricky but simple way to distinct AMD,CommonJS or no-module mode used
        when we have detects anchor objects we publish module by priority AMD, CommonJS, Global Scope
     */
    var $define = null;
    var $exports = null;
    try{$define = define;}catch(e){try{$exports = exports;}catch(e){}}

    if($define){
        $define([],function(){return module;}); //AMD style - we define module as function;
    }else if(!!$exports){
        $exports.module = module; // CommonJS style
    }else if(!!window){ //no modules provided - we are under browser with old-scool-window-as-global style
        window.$module = module; // in real world u have to provide valid name for global scope
    }
})();