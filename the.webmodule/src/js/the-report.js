define(["the-object","the-hash"], function ($the, $m) {
    return $the(function (root, privates) {
        var $hash = root.hash;
        var report = root.report = (root.report || {});
        report.getDefaultContext = function(options,doParse,hash,decode){
            var getHash = $the.hash.get;
            var s = $the.extend({
                showComments: true,
                showExtensions: true,
                numex : 1,
                addr1 :"",
                addr2:"",
                addr3:"",
                addr4:"",
                num : "",
                advcomment:"",
                day : "",
                month : $the.dates.toRusMonth(null,'r').toLowerCase()
            },options);
            if(typeof  doParse == "undefined"){
                doParse = true;
            }
            if(!!doParse) {
                var numex = getHash("numex",hash,decode);
                if (numex) {
                    s.numex = numex;
                }
                s.addr1 = getHash("addr1",hash,decode);
                s.addr2 = getHash("addr2",hash,decode);
                s.addr3 = getHash("addr3",hash,decode);
                s.addr4 = getHash("addr4",hash,decode);
                s.num = getHash("num",hash,decode);
                s.advcomment = getHash("advcomment",hash,decode);
                s.day = getHash("day",hash,decode) || moment().date();
                s.month = getHash("month",hash,decode) || s.month;
                s.showComments = !Boolean(getHash("noComments"),hash,decode);
                s.showExtensions = !Boolean(getHash("noExt"),hash,decode);
            }

            return s;
        }
        report.setupDefaultScope = function($scope){
            $scope.state = $the.report.getDefaultContext();
            $scope.changed = function(name){$hash.set(name,s[name]);}
            $scope.setShowComments = function (val) {
                $scope.state.showComments = val;
                $hash.setFlag("noComments",!val);
            }
            $scope.setShowExtensions = function (val) {
                $scope.state.showExtensions = val;
                $hash.setFlag("noExt",!val);
            }

            $scope.serverPrint = function(){
                $the.printer.serverPrint({Target:"_blank"});
            }
            $scope.canPrint = typeof jsPrintSetup != "undefined";
            $scope.doPrint = function(){
                setTimeout(function(){
                    $the.printer.print();
                },1000);
            }
            return $scope.state;
        }
    });
});