/**
 * Created by comdiv on 02.03.2015.
 */
define(["the-root"],function($the){
    return $the(function(root){
        var state = root.uistate = root.uistate || {};
        state.groups = state.groups || {};
        var stateObject = root.uistate.StateObject = function(code,group,system){
            this.code = code;
            this.group = group;
            this.system = !!system;

        };
        Object.defineProperty(stateObject.prototype,'active',{
            get  :function(){
                return this.group.isActive(this.code);
            },
            set : function(value){
                if(!!value){
                    this.group.activate(this.code);
                }else{
                    this.group.activate('null');
                }
            }
        });
        var stateGroup = root.uistate.StateGroup = function(code,parent,active,def){

            this.parent = parent;
            this.code = code;
            this.activeObject = active || 'null';
            this.toggleable = true;
            this.defaultObject = def || 'default';
            this.__visible= true;
            this.get = function(code){
                if(!this.objects.hasOwnProperty(code)){
                    return this.objects[code] = new stateObject(code,this);

                }
                return this.objects[code];
            },
            this.getActive = function(){
                return this.get(this.activeObject);
            },
            this.activate = function(code){
                if(this.toggleable && this.activeObject==code){
                    this.activeObject = "none";
                }else {
                    this.activeObject = code;
                }
                console.debug(code,this.code,this.parent.sync);
                if(this.code=='toolbar' && this.parent.sync){
                    this.parent.left.activate(code);
                }
            }
            this.isActive = function(code){
                return this.activeObject == code;
            }

            this.objects = {
                "null" : new stateObject('null',this,true),
                "default" : new stateObject('default',this,true)
            };
            this.objects.default.active = true;


        };
        Object.defineProperty(stateGroup.prototype,'visible',{
            set: function(value){
                this.__visible = value;
            },
            get: function(){
                if(!this.activeObject || this.activeObject=='null'){
                    return false;
                }
                return !!this.__visible;
            }
        });
        state.sync = false;
        state.getGroup = function(group){
            if(!state.groups.hasOwnProperty(group)){
                state.groups[group] = new stateGroup(group,state);
            }
            return state.groups[group];
        }
        Object.defineProperty(state,'toolbar',{
            writeable:false,
            get : function(){
                return state.getGroup('toolbar');
            }
        });
        Object.defineProperty(state,'left',{
            writeable:false,
            get : function(){
                return state.getGroup('left');
            }
        });
        Object.defineProperty(state,'right',{
            writeable:false,
            get : function(){
                return state.getGroup('right');
            }
        });
        state.isActive = function(group, code){
            return state.getGroup(group).isActive(code);
        }
        state.activate = function(group, code){
            return state.getGroup(group).activate(code);
        }
        state.get = function(group,code){
            return state.getGroup(group).get(code);
        }
    });
})