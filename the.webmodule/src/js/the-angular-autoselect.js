/**
 * Created by comdiv on 04.11.2014.
 */
define(["the-angular"], function ($the) {
    return $the(function (root) {
        if (null == root.modules)return;
        root.modules.all.directive("autoselect",function(){
            return {
                compile :function(e,attr){
                    $(e[0]).focus(function() {
                        var $this = $(this);

                        $this.select();

                        window.setTimeout(function() {
                            $this.select();
                        }, 1);

                        // Work around WebKit's little problem
                        function mouseUpHandler() {
                            // Prevent further mouseup intervention
                            $this.off("mouseup", mouseUpHandler);
                            return false;
                        }

                        $this.mouseup(mouseUpHandler);
                    });
                }
            }
        });

    });
});