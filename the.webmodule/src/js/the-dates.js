define(["the-object", "moment"], function ($the, $m) {
    return $the(function (root, privates) {
        root.dates = root.dates || {};
        var rusMonthMap = {
            i1 : "Январь",
            i2 : "Февраль",
            i3 : "Март",
            i4 : "Апрель",
            i5 : "Май",
            i6 : "Июнь",
            i7 : "Июль",
            i8 : "Август",
            i9 : "Сентябрь",
            i10 : "Октябрь",
            i11 : "Ноябрь",
            i12 : "Декабрь",
            r1 : "Января",
            r2 : "Февраля",
            r3 : "Марта",
            r4 : "Апреля",
            r5 : "Мая",
            r6 : "Июня",
            r7 : "Июля",
            r8 : "Августа",
            r9 : "Сентября",
            r10 : "Октября",
            r11 : "Ноября",
            r12 : "Декабря"
        };
        var toRusMonth = root.dates.toRusMonth = function(month,padezh){
            var n = 0;
            padezh = padezh || 'i';
            if(!!month){
                if(typeof month == "number"){
                    n = month;
                }
                else if(typeof month == "string"){
                    n = Number(month);
                    if(Number.isNaN(n)){
                        return month;
                    }
                }
                else if(month instanceof Date){
                    n = month.getMonth()+1;
                }
            }
            if(n==0){
                n = (new Date()).getMonth()+1;
            }

            var key = padezh+n;
            if(rusMonthMap.hasOwnProperty(key)){
                return rusMonthMap[key];
            }
            return "НЕ ОПР";
        };
        var DEFAULT_DATE_FORMATS = root.dates.DEFAULT_DATE_FORMATS = [
            "DD.MM.YYYY HH:mm:ss",
            "YYYY-MM-DD HH:mm:ss",
            "YYYYMMDDHHmmss",
            "ddd, DD MMM YYYY HH:mm:ss ZZ"
        ];
        var toDateFormat = root.dates.toDateFormat =  function(src){
            src = src || 'ru';
            if(src=='ru')return "DD.MM.YYYY";
            if(src=='RU')return "DD.MM.YYYY HH:mm:ss";
            if(src=='SQL')return "YYYYMMDD HH:mm:ss";
            if(src=='N')return "YYYYMMDDHHmmss";
            if(src=='RFC')return "ddd, DD MMM YYYY HH:mm:ss ZZ";
            return src;
        }
        var toDate = root.dates.toDate = function (src, formats) {
            if (!src)return null;
            if (src instanceof Date)return src;
            if (typeof src == "string") {
                formats = formats || root.dates.DEFAULT_DATE_FORMATS;
                if (typeof(formats) == "string")formats = [formats];
                for (var i = 0; i < formats.length; i++) {
                    var format = formats[i];
                    var md = moment(src, format);
                    if (md.isValid()) {
                        return md.toDate();
                    }
                }
                return null;
            }
            return null;
        }
        var DateRange = root.dates.DateRange = function () {
            this.Basis = null;
            this.Start = null;
            this.Finish = null;
            this.Range = "";
            this.IsValid = false;
            this.SingleDate = false;
            this.IsInfinity = false;
        };
        DateRange.prototype.formatStart = function(format){
            return $m(this.Start).format(toDateFormat(format));
        }
        DateRange.prototype.formatFinish = function(format){
            return $m(this.Finish).format(toDateFormat(format));
        }

        DateRange.prototype.format = function( format,template){
            format = toDateFormat(format);
            var templates = {
                single : "за ${s}",
                ranged : "с ${s} по ${f}",
                infinity  : "без ограничений"
            }
            if(!!template){
                if(typeof template == "string"){
                    templates.ranged = template;
                }else if(Array.isArray(template)){
                    if(template.length>0)templates.ranged = template[0];
                    if(template.length>1)templates.single = template[1];
                    if(template.length>2)templates.infinity = template[2];
                }else{
                    $the.extend(templates,template);
                }
            }

            var subst = {
                b: $m(this.Basis).format(format),
                s: this.formatStart(format),
                f : this.formatFinish(format)
            };
            var wtempl = templates.ranged;
            if(this.SingleDate && !format.match(/HH/)){
                wtempl = templates.single;
            }
            else if(this.IsInfinity){
                wtempl = templates.infinity;
            }
            return $the.interpolate(wtempl,subst);

        }
        DateRange.create = function (range, basis) {
            basis = toDate(basis || new Date());
            var result = $the.cast(DateRange, {Basis: basis, Range: range});
            DateRange.evaluate(result);
            return result;
        };

        DateRange.parse  = function(hash,options ){
            var getHash = $the.hash.get;
            options = $the.extend({
                UseRange : "userange",
                Range : "range",
                Start : "start",
                Finish : "end",
                Basis : "today"
            },options);
            var userange = Boolean(getHash(options.UseRange,hash));
            if(userange) {
                var range = getHash(options.Range, hash);
                var basis = toDate(getHash(options.Basis, hash));
                return DateRange.create(range,basis);
            }else {
                var start = toDate(getHash(options.Start, hash));
                var finish = toDate(getHash(options.Finish, hash));
                var result = new DateRange();
                result.IsValid = true;
                result.Range = "direct";
                result.Start = start;
                result.Finish = finish;
                return result;
            }
        };
        DateRange.evaluate = function (range) {
            if("any"==range.Range){
                range.IsValid = true;
                range.IsInfinity = true;
            }
            if ("today" == range.Range) {
                range.IsValid = true;
                var b = $m(range.Basis);
                range.Finish = b.clone().endOf('day').toDate();
                range.Start = b.clone().startOf('day').toDate();
                return;
            }
            if ("yesterday" == range.Range) {
                range.IsValid = true;
                var b = $m(range.Basis).add(-1, 'days');
                range.Start = b.clone().startOf('day').toDate();
                range.Finish = b.clone().endOf('day').toDate();
                return;
            }
            var match = range.Range.match(/^\s*((-?)\d+)([hdmyw])\s*$/);
            if(match){
                range.IsValid = true;
                var minus = match[2];
                var count = Number(match[1]);
                var type = match[3];
                if(type == "h")type="hour";
                else if(type=="d")type="day";
                else if(type=="m")type="month";
                else if(type=="y")type="year";
                else if(type=="w")type="week";
                range.Finish = range.Basis;
                if(count!=0) {
                    range.Start = $m(range.Finish).add(count, type).toDate();
                }else{
                    if(minus){
                        range.Start = $m(range.Finish).startOf(type);
                    }else{
                        range.Start = $m(range.Finish).endOf(type);
                    }
                }
                if(range.Finish <  range.Start){
                    range.Finish = range.Start;
                    range.Start = range.Basis;
                }
                if(type!='hour'){
                    range.Start = $m(range.Start).startOf('day').toDate();
                    range.Finish = $m(range.Finish).endOf('day').toDate();
                }

                if($m(range.Start).format("YYYYMMDD")==$m(range.Finish).format("YYYYMMDD")){
                    range.SingleDate = true;
                }
            }
        };


    });
});