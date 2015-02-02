define(["the","chai","moment"],function($the,chai,$m) {
    var should = chai.Should();
    describe("DateRange", function () {
        this.timeout(5000);
        describe("#api", function () {
            var dateeq = function (d1, d2) {
                return $m($the.dates.toDate(d1)).format("DD.MM.YYYY")==$m($the.dates.toDate(d2)).format("DD.MM.YYYY");
            }
            it("has DateRange class", function () {
                var dr = $the.dates.DateRange;
                should.exist(dr);
                dr = new dr();
                dr.hasOwnProperty('Basis').should.eq(true);
                dr.hasOwnProperty('Start').should.eq(true);
                dr.hasOwnProperty('Finish').should.eq(true);
                dr.hasOwnProperty('Range').should.eq(true);
                dr.hasOwnProperty('IsValid').should.eq(true);
            });
            it("toDate always return date",function(){
                var d = $the.dates.toDate(new Date(2015,0,2),'DD.MM.YYYY');
                d.should.be.instanceof(Date);
                $m(d).format('DD.MM.YYYY').should.eq("02.01.2015");
                var d = $the.dates.toDate('02.01.2015','DD.MM.YYYY');
                d.should.be.instanceof(Date);
                $m(d).format('DD.MM.YYYY').should.eq("02.01.2015");
                var d = $the.dates.toDate('20150102',['DD.MM.YYYY','YYYYMMDD']);
                d.should.be.instanceof(Date);
                $m(d).format('DD.MM.YYYY').should.eq("02.01.2015");
                var d = $the.dates.toDate('20150102');
                d.should.be.instanceof(Date);
                $m(d).format('DD.MM.YYYY').should.eq("02.01.2015");
            });
            it("#r:today is a range of 00:00:00 to 23:59:59 of today (basis date)",function(){
                var range = $the.dates.DateRange.create("today");
                range.IsValid.should.eq(true);
                range.Range.should.eq("today");
                dateeq(range.Basis,new Date()).should.eq(true);
                var td = $m(new Date()).format("YYYYMMDD");
                var s = td+"000000";
                var f = td+"235959";
                $m(s,"YYYYMMDDHHmmss").toDate().toString().should.eq(range.Start.toString());
                $m(f,"YYYYMMDDHHmmss").toDate().toString().should.eq(range.Finish.toString());

                range = $the.dates.DateRange.create("today", new Date(2015,0,12,15,44,22));
                range.Range.should.eq("today");
                dateeq(range.Basis,new Date(2015,0,12,15,44,22)).should.eq(true);
                var td = $m(new Date(2015,0,12,15,44,22)).format("YYYYMMDD");
                var s = td+"000000";
                var f = td+"235959";
                $m(s,"YYYYMMDDHHmmss").toDate().toString().should.eq(range.Start.toString());
                $m(f,"YYYYMMDDHHmmss").toDate().toString().should.eq(range.Finish.toString());
            });

            it("#r:yesterday is a range of 00:00:00 to 23:59:59 of yesterday (from basis date)",function(){
                range = $the.dates.DateRange.create("yesterday", new Date(2015,0,12,15,44,22));
                range.IsValid.should.eq(true);
                range.Range.should.eq("yesterday");
                var td = $m(new Date(2015,0,11,15,44,22)).format("YYYYMMDD");
                var s = td+"000000";
                var f = td+"235959";
                $m(s,"YYYYMMDDHHmmss").toDate().toString().should.eq(range.Start.toString());
                $m(f,"YYYYMMDDHHmmss").toDate().toString().should.eq(range.Finish.toString());
            });

            it("#r:delted on day",function(){
                range = $the.dates.DateRange.create("-2d", new Date(2015,0,12,15,44,22));
                range.IsValid.should.eq(true);
                var s = new Date(2015,0,11,00,00,00);
                var f = new Date(2015,0,12,23,59,59);
                s.toString().should.eq(range.Start.toString());
                f.toString().should.eq(range.Finish.toString());

                range = $the.dates.DateRange.create("2d", new Date(2015,0,12,15,44,22));
                range.IsValid.should.eq(true);
                var s = new Date(2015,0,12,00,00,00);
                var f = new Date(2015,0,13,23,59,59);
                s.toString().should.eq(range.Start.toString());
                f.toString().should.eq(range.Finish.toString());
            });

            it("#r:delted on month",function(){
                range = $the.dates.DateRange.create("-2m", new Date(2015,0,12,15,44,22));
                range.IsValid.should.eq(true);
                var s = new Date(2014,10,13,00,00,00);
                var f = new Date(2015,0,12,23,59,59);
                s.toString().should.eq(range.Start.toString());
                f.toString().should.eq(range.Finish.toString());

                range = $the.dates.DateRange.create("2m", new Date(2015,0,12,15,44,22));
                range.IsValid.should.eq(true);
                var s = new Date(2015,0,12,00,00,00);
                var f = new Date(2015,2,11,23,59,59);
                s.toString().should.eq(range.Start.toString());
                f.toString().should.eq(range.Finish.toString());
            });

            it("#r:delted ZERO on month",function(){
                range = $the.dates.DateRange.create("-0m", new Date(2015,4,12,15,44,22));
                range.IsValid.should.eq(true);
                var s = new Date(2015,4,1,00,00,00);
                var f = new Date(2015,4,12,23,59,59);
                s.toString().should.eq(range.Start.toString());
                f.toString().should.eq(range.Finish.toString());

                range = $the.dates.DateRange.create("0m", new Date(2015,4,12,15,44,22));
                range.IsValid.should.eq(true);
                var s = new Date(2015,4,12,00,00,00);
                var f = new Date(2015,4,31,23,59,59);
                s.toString().should.eq(range.Start.toString());
                f.toString().should.eq(range.Finish.toString());
            });

            it("#r:bug -3h",function(){
                range = $the.dates.DateRange.create("-3h", new Date(2015,0,12,15,44,22));
                range.IsValid.should.eq(true);
                var s = new Date(2015,0,12,12,44,22);
                var f = new Date(2015,0,12,15,44,22);
                s.toString().should.eq(range.Start.toString());
                f.toString().should.eq(range.Finish.toString());
            });

            it("can format output",function(){
                var r = $the.dates.DateRange.create("-0m", new Date(2015,0,12,15,44,22));
                var formated = r.format();
                formated.should.eq("с 01.01.2015 по 12.01.2015");
                formated = r.format('RU','${s} - ${f}');
                formated.should.eq("01.01.2015 00:00:00 - 12.01.2015 23:59:59");

                r = $the.dates.DateRange.create("-0d", new Date(2015,0,12,15,44,22));
                formated = r.format();
                formated.should.eq("за 12.01.2015");
                r = $the.dates.DateRange.create("-2h", new Date(2015,0,12,15,44,22));
                formated = r.format('RU');
                formated.should.eq("с 12.01.2015 13:44:22 по 12.01.2015 15:44:22");
            });

            it("today is singledate",function(){
               var r = $the.dates.DateRange.create("today");
                r.SingleDate.should.eq(true);
            });
        });
    });
});
