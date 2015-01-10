define(["the-root"
], function ($the) {
    return $the(function (root) {
        var printer = root.printer = root.printer || {};
        var PrintOptions = root.printer.PrinterOptions = function () {
            this.TargetUrl = "";
            this.LocalizeUrl = true;
            this.UrlParam = "reporturl";
            this.PrintHash = "print";
            this.CacheParam = "cached";
            this.UseCache = false;
            this.PrintCommand = "print";
            this.BasePort = 14075;
            this.DataUrl = "";
            this.TitleParam= "title";
            this.Title = "";
            this.Target ="_blank";
        };
        printer.print = function (options) {
            options = $the.cast(PrintOptions, options);

            var title = options.Title || document.title;
            var url = options.TargetUrl;
            if(!options.TargetUrl) {
                var islocal = document.location.hostname == "127.0.0.1";
                var issecure = document.location.protocol == "https:";
                var port = options.BasePort;
                if (issecure) port += 1;
                if (islocal) port -= 5;
                var url =
                    document.location.protocol + "//" +
                    document.location.hostname + ":" + port + "/" +
                    options.PrintCommand;
            }

            if(!url.match(/\?/)){
                url+="?";
            }
            if(options.UseCache){
                url += ("&"+options.CacheParam+"=true");
            }
            url += ("&"+options.TitleParam+"="+encodeURIComponent(title));
            var dataurl = options.DataUrl;
            if(!options.DataUrl) {
                dataurl = document.location.protocol + "//" + (options.LocalizeUrl ? "127.0.0.1" : document.location.hostname) + ":" + document.location.port +
                    document.location.pathname +
                    document.location.search +
                    document.location.hash;
            }
            if (! dataurl.match(/#/)){
                dataurl+="#";
            }
            if(!!options.PrintHash) {
                dataurl += ("&" + options.PrintHash + "=true");
            }
            dataurl = encodeURIComponent(dataurl);
            url+=("&"+options.UrlParam+"="+dataurl);
            window.open(url,options.Target);
        }
    });
});