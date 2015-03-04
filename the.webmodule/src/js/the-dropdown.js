/**
 * Created by comdiv on 26.02.2015.
 */
define([
    "the-root"
], function (the,ddutils) {

    var $ = the.$jQuery;
    var root = the;

    if (!$) {
        console.error('JQuery required');
        return the;
    }


    if(null==root.modules)return;



    var service = {
        __closeDropDown : function(e){
            $(e).removeClass('shown');
            $(e).removeClass('isdropdown');
            var options = e.__ddoptions;

            e.__ddoptions = null;
            e.__ddopened = false;

            if(!!options.onclose){
                options.onclose(e,options);
            }
        },

        __closeDropDownContainer : function(e){
            $e = $(e);
            e = $e[0];
            var self = this;
            if(!e.__ddcopened)return;
            e.__ddcopened = false;
            $(e).children(".dropdown").each(function(i,dd){
                self.__closeDropDown(dd);
            });
            $(e).find(".dropdown").each(function(i,dd){
                if(dd.__ddopened){
                    self.__closeDropDownContainer($(dd).parent());
                }
            });
        },

        __compileContainerElement : function(e,options){
            var self = this;
            $e = $(e);
            e = $e[0];
            $e.on('mouseleave',function(event){
                if(!e.__ddcopened)return;
                e.__timeout = setTimeout(function(){
                    self.__closeDropDownContainer(e);
                },options.timeout);
            });
            $e.on('mouseenter',function(event){
                if(!e.__ddcopened)return;
                clearTimeout(e.__timeout);
            });
            e.__timeout = null;
            e.__ddcopened = false;
            e.__ddcprepared = true;
        },

        __getOptions : function(options){
            options = options || {};
            if(!options.timeout){
                options.timeout = 500;
            }
            return options;
        },

        __compileDropDown :  function(e,options){
            var self = this;
            $e = $(e);
            e = $e[0];
            $(e).on('click',function(event){
                if(!e.__ddopened)return;
                event.preventDefault();
                event.stopPropagation();
                if(!!options.menu || $e.hasClass('menu')){
                    $('.dropdown.shown').each(function(i,e){
                        self.__closeDropDownContainer($(e).parent());
                    });
                }else {
                    self.__closeDropDownContainer($(e).parent());
                }
            })
            e.__ddopened = false;
            e.__ddprepared = true;
        },

        openDropdownElement : function(containerelement, options){
            containerelement = $(containerelement).fst();
            if(!!containerelement.__ddcopened)return;
            options = this.__getOptions(options);
            if(!containerelement.__ddcprepared){
                this.__compileContainerElement(containerelement,options);
            }
            var dropdown = $(containerelement).children('.dropdown');
            if(dropdown.length!=0){
                dropdown = dropdown[0];
                if(!dropdown.__ddprepared){
                    this.__compileDropDown(dropdown,options);
                }
                dropdown.__ddoptions = options;
                $(dropdown).fitWidth();
                $(dropdown).placeAside(options);
                $(dropdown).addClass('shown');
                $(dropdown).addClass('isdropdown');
                dropdown.__ddopened = true;
                containerelement.__ddcopened = true;
                if(!!options.onopen){
                    options.onopen(dropdown,options);
                }
            }
        },

        openDropdown :  function(elementOrEvent, options){
            var containerelement = (!!elementOrEvent.currentTarget)? elementOrEvent.currentTarget : elementOrEvent;
            console.debug(containerelement);
            if( $(containerelement).children('.dropdown').length==0){
                return this.openDropdown($(containerelement).parent()[0]);
            }
            return this.openDropdownElement(containerelement,options);
        }

    };

    var result = function(elementOrEvent,options){
        return service.openDropdown(elementOrEvent,options);
    };
    result.close = function(){
        $('.dropdown').each(function(i,e){
            service.__closeDropDownContainer($(e).parent());
        });
    }

    $.fn.openDropdown = function(options){
         this.each(function(i,e){
             result(e,options);
         })
    };

    $.closeDropdown = function(){
       result.close();
    };

    root.dropdown = result;

    var ddservice =  function () {

        return result;
    };
    root.modules.ddservice = root.$angular.module("the-ddservice",[]).factory('dropdownService',ddservice);
    root.modules.all.factory('dropdownService', ddservice);
})