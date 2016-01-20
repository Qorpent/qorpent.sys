/**
 * Created by comdiv on 04.11.2014.
 */
    define(["the-angular","the-wiki"], function ($the, wiki) {
        return $the(function(root, privates)
        {
            if(null==root.modules)return;

            var tohtml = [function() {
                return function (val) {
                    return wiki.toHTML(val);
                };
            }];

            root.modules.all.filter('wiki',tohtml);
        });
    });