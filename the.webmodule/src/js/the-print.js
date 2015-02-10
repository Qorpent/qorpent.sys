define(["the-root"
], function ($the) {
    return $the(function (root) {
        var printer = root.printer = root.printer || {};
        var ServerPrinterOptions = root.printer.ServerPrinterOptions = function () {
            this.TargetUrl = "";
            this.LocalizeUrl = false;
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
        var PrinterOptions = root.printer.PrinterOptions = function(){
            this.PrinterName= "Bullzip PDF Printer";
            this.MarginTop= 0;
            this.MarginBottom= 0;
            this.MarginLeft= 0;
            this.MarginRight= 0;
            this.HeaderStrLeft = "";
            this.HeaderStrCenter = "";
            this.HeaderStrRight = "";
            this.FooterStrLeft = "";
            this.FooterStrCenter = "";
            this.FooterStrRight = "";
            this.ShrinkToFit = 0;
            this.PrintBGColors = 1;
            this.PrintBGImages = 1;

        }
        printer.print = function(options){
            options = $the.cast(PrinterOptions,options);
            jsPrintSetup.setPrinter(options.PrinterName);
            jsPrintSetup.clearSilentPrint();
            jsPrintSetup.setSilentPrint(true);
            jsPrintSetup.setOption('marginTop', options.MarginTop);
            jsPrintSetup.setOption('marginBottom', options.MarginBottom);
            jsPrintSetup.setOption('marginLeft', options.MarginLeft);
            jsPrintSetup.setOption('marginRight', options.MarginRight);
            jsPrintSetup.setOption('headerStrLeft', options.HeaderStrLeft);
            jsPrintSetup.setOption('headerStrCenter', options.HeaderStrCenter);
            jsPrintSetup.setOption('headerStrRight', options.HeaderStrRight);
            jsPrintSetup.setOption('footerStrLeft', options.FooterStrLeft);
            jsPrintSetup.setOption('footerStrCenter',options.FooterStrCenter);
            jsPrintSetup.setOption('footerStrRight', options.FooterStrRight);
            jsPrintSetup.setOption('footerStrRight', options.FooterStrRight);
            jsPrintSetup.setOption('shrinkToFit', options.ShrinkToFit);
            jsPrintSetup.setOption('printBGColors', options.PrintBGColors);
            jsPrintSetup.setOption('printBGImages', options.PrintBGImages);
            jsPrintSetup.print();
            jsPrintSetup.setSilentPrint(false);
        }
        printer.serverPrint = function (options) {
            options = $the.cast(ServerPrinterOptions, options);

            var title = options.Title || document.title;
            var url = options.TargetUrl;
            if(!options.TargetUrl) {
                var islocal = document.location.hostname == "127.0.0.1";
                var issecure = document.location.protocol == "https:";
                var port = options.BasePort;
                if (issecure) port += 1;
                if (!islocal) port -= 5;
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