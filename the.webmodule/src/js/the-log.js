define(["the-object","the-hash"], function ($the, $m) {
    return $the(function (root, privates) {
        var log = root.log || (root.log = {});
        var generic = log.CodeGeneric = '000';
        var Message = log.Message || (
              log.Message = function(text,level,type,code){
                  this.level  =level;
                  this.text = text;
                  this.type = type;
                  this.code = code;
                  this.time = new Date();
                  this.url = "";
                  if(typeof document != undefined){
                      this.url = document.location.href;
                  }
                  this.visible = true;
              }
            );
        Message.prototype.hide = function(){
            this.visible = false;
            console.debug(log.activemessages);
            var idx = log.activemessages.indexOf(this);
            log.activemessages.splice(idx,1);
        }
        Message.prototype.remove = function(){
            var idx = log.messages.indexOf(this);
            if(idx>=0){
                log.messages.splice(idx,1);
            }
        }
        log.messages = log.messages || (log.messages = []);

        log.activemessages = log.activemessages || (log.activemessages = []);
        log.add = function(text,level,type,code){
            var message = null;
            if(text instanceof Message){
                log.messages.unshift(text);
                message = text;
            }else {
                level = level || "info";
                type = type || "generic";
                code = code || (level[0].toUpperCase() + type[0].toUpperCase() + generic);
                var message = new Message(text, level, type, code);
                log.messages.unshift(message);
            }

            if(message.visible){
                log.activemessages.unshift(message);
            }

            if(!!root.$angular && !!root.$rootScope){
                root.$rootScope.$broadcast('THELOG',message);

                if(!root.$rootScope.$$phase){
                    root.$rootScope.$apply();
                }
            }

            return message;
        }

        log.clear = function(){
            this.messages = [];
        }

        log.error = function(text,type,code){
            log.add(text,'error',type,code);
        }

        log.warn = function(text,type,code){
            log.add(text,'warn',type,code);
        }

        log.hideAll = function(){
            log.messages.forEach(function(e){
                e.hide();
            });
        }

        log.copyToClipboard = function(){
            clipboardData
        }

    });
});