if (typeof define !== 'function') {
    var define = require('amdefine')(module);
}
define(["./AnyHostModuleTemplate"],function($ah){
    var dep = (typeof $ah == "function"?$ah:$ah.module)
   var module = function(){};
    module.execute = function(){
        return dep.execute()+" me too!";
    }
    return module;
});