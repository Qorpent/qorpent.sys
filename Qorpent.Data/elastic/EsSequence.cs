using System;
using System.Threading;
using Qorpent.Experiments;
using Qorpent.IO.Net;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.elastic
{
    /// <summary>
    /// Helper class to implement sequence number in ElasticSearch
    /// </summary>
    public class EsSequence
    {
        private string _key = "default";
        private int _step = 10;
        private int _initial = 10;
        private string _index = "sequence";
        private string _type = "sequence";
        private string _url = "http://127.0.0.1:9200";
        private bool _inialized;
        private Uri _updateurl;
        private string _mappingquery;
        private string _nextquery;
        private Uri _mappingurl;
        private Uri _indexexistsurl;
        private HttpClient _http;
        private Uri _geturl;


        public string Url {
            get { return _url; }
            set {
                _url = value;
                _inialized = false;
            }
        }

        public string Key {
            get { return _key; }
            set {
                _key = value;
                _inialized = false;
            }
        }

        public int Step {
            get { return _step; }
            set {
                _step = value;
                _inialized = false;
            }
        }

        public int Initial {
            get { return _initial; }
            set {
                _initial = value;
                _inialized = false;
            }
        }

        public string Index {
            get { return _index; }
            set {
                _index = value;
                _inialized = false;
            }

        }

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                _inialized = false;
            }

        }

        public int Next() {
            Initialize();
            var resp = _http.Call(HttpRequest.Post(_updateurl, _nextquery));
            if (resp.State == 409) { //concurrent error
                return Next();
            }
            if (resp.State != 200 && resp.State!=201) {
                throw new Exception($"Some errors in {nameof(Next)}: {resp.State}, {resp.StringData}",resp.Error);
            }
            var j = resp.StringData.jsonify();
            return j.arr0("get.fields.iid").ToInt();
        }

        public void Set(int value) {
            Initialize();
            var query = SetQueryTemplate.Replace("_VALUE_", value.ToString());
            var resp = _http.Call(HttpRequest.Post(_updateurl,query));
            if (resp.State == 409)
            { //concurrent error
                Set(value);
            }
            if (resp.State != 200 && resp.State != 201)
            {
                throw new Exception($"Some errors in {nameof(Set)}: {resp.State}, {resp.StringData}", resp.Error);
            }
        }

        public int GetCurrent() {
            Initialize();
            var req = HttpRequest.Get(_geturl);
            var resp = _http.Call(req);
            if (resp.State == 404) {
                return int.MinValue;
            }
            if (resp.State == 200) {
                var j = resp.StringData.jsonify();
                return j.num("_source.iid");
            }
            if (resp.State == 503 && resp.StringData.Contains("POST_RECOVERY")) {
                Thread.Sleep(50);
                return GetCurrent();
            }
            throw new Exception($"Invalid {nameof(GetCurrent)} call: {resp.State}, {resp.StringData}",resp.Error);
        }

        private void Initialize() {
            if (_inialized) return;
            var scope = new Scope(this);
            _updateurl = new Uri(UpdateUrlTemplate.Interpolate(scope));
            _mappingquery = MappingTemplate.Interpolate(scope);
            _nextquery = NextQueryTemplate.Interpolate(scope);
            _mappingurl = new Uri(MappingUrlTemplate.Interpolate(scope));
            _geturl = new Uri(GetUrlTemplate.Interpolate(scope));
            _indexexistsurl = new Uri(IndexExistsUrlTemplate.Interpolate(scope));
            _http = new HttpClient();
            EnsureIndex();
            _inialized = true;
        }

        private void EnsureIndex() {
            var resp = _http.Call(HttpRequest.Head(_indexexistsurl));
            if (resp.State == 200) {
                return; //sequence index exists
            }
            if (resp.State == 404) {
//require creation

                resp = _http.Call(HttpRequest.Put(_mappingurl,_mappingquery));
                if (resp.State != 200 && resp.State!=400) {
                    throw new Exception($"Invalid {nameof(EnsureIndex)} PUT response {resp.State}, {resp.StringData}",
                        resp.Error);
                }
            }
            else {
                throw new Exception($"Invalid {nameof(EnsureIndex)} HEAD response {resp.State}, {resp.StringData}",
                    resp.Error);
            }
        }


        public const string IndexExistsUrlTemplate = "${Url}/${Index}";
        public const string UpdateUrlTemplate = "${Url}/${Index}/${Type}/${Key}/_update?fields=iid&retry_on_conflict=5";
        public const string GetUrlTemplate = "${Url}/${Index}/${Type}/${Key}";
        public const string NextQueryTemplate = @"{
     ""script"": ""ctx._source.iid += bulk_size"",
     ""params"": {""bulk_size"": ${Step}},
     ""lang"": ""groovy"",
     ""upsert"": {
         ""iid"": ${Initial}
     }
 }";
        public const string SetQueryTemplate = @"{
     ""doc"": {""iid"":_VALUE_},
     ""upsert"": {
         ""iid"": _VALUE_
     }
 }";
        public const string MappingUrlTemplate = "${Url}/${Index}";
        public const string MappingTemplate = @"{
     ""settings"": {
         ""number_of_shards"": 1,
         ""auto_expand_replicas"": ""0-all""
     },
     ""mappings"": {
         ""${Index}"": {
             ""_all"": {""enabled"": 0},
             ""_type"": {""index"": ""no""},
             ""dynamic"": ""strict"",
             ""properties"": {
                 ""iid"": {
                     ""type"": ""integer"",
                     ""index"": ""no"",
                 },
             },
         }
     }
 }";
    }
}
