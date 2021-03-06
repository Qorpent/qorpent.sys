using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;

namespace qorpent.v2.query {
    public  class SearchResult: IJsonSerializable {
        public bool IsLastPage;
        public string Id { get; set; }
       
        public virtual object[] Items { get; set; }
        public bool NoChange;
        public int OffSet;
        public int Page;
        public int PageCount;
        public int Size;
        public string Hash;
        public DateTime Timestamp;
        public int Total;
        public bool Ok;
        public SearchState Status { get; set; }
        public IScope Scope { get; set; }
        public string Session { get; set; }
        public int Count { get; set; }
        public IDictionary<string, object> DebugInfo { get; set; } = new Dictionary<string, object>();
        public IDictionary<string, object> Custom { get; set; } = new Dictionary<string, object>();
        public string Message { get; set; }

        public Exception Error;

        public void WriteAsJson(TextWriter output, string mode, ISerializationAnnotator annotator, bool pretty = false, int level = 0) {
            var writer = new JsonWriter(output,pretty:pretty,level:level);
            writer.OpenObject();
            writer.WriteProperty("total", Total,true);
            writer.WriteProperty("offset", OffSet, true);
            writer.WriteProperty("size", Size, true);
            writer.WriteProperty("count", Count, true);

            writer.WriteProperty("page", Page, true);
            writer.WriteProperty("pagecount", PageCount, true);
            writer.WriteProperty("islastpage",IsLastPage, true);

            writer.WriteProperty("nochange", NoChange, true);
            writer.WriteProperty("hash",Hash,true);
            writer.WriteProperty("timestamp",Timestamp,true);
            writer.WriteProperty("status",Status, true);

            writer.WriteProperty("ok",Ok);
            writer.WriteProperty("message",Message,true);
            if (null != Error) {
                writer.OpenProperty("error");
                writer.OpenObject();
                writer.WriteProperty("type",Error.GetType().Name);
                writer.WriteProperty("message",Error.Message);
                writer.WriteProperty("stack",Error.StackTrace);
                writer.CloseObject();
                writer.CloseProperty();
            }

            if (null != DebugInfo && 0 != DebugInfo.Count) {
                writer.WriteProperty("debug",DebugInfo);
            }
            if (null != Custom && 0 != Custom.Count)
            {
                writer.WriteProperty("custom", Custom);
            }

            var mainitems = GetMainItems();
            if (null != mainitems) {
                writer.OpenProperty("items");
                writer.OpenArray();
                foreach (var item in mainitems) {
                    writer.WriteObject(item, mode);
                }
                writer.CloseArray();
                writer.CloseProperty();
                var native = GetNative();
                if (null != native) {
                    writer.OpenProperty("native");
                    writer.OpenArray();
                    foreach (var item in native) {
                        writer.WriteObject(item, mode);
                    }
                    writer.CloseArray();
                    writer.CloseProperty();
                }
            }
            writer.CloseObject();
        }

        protected virtual IEnumerable GetNative() {
            return null;
            
        }

        protected virtual IEnumerable GetMainItems() {
            return Items;

        }


    }

    public class SearchResult<T> : SearchResult {
        public T[] TypedItems;
        private object[] _native;


        public override object[] Items
        {
            get { return _native ?? (_native = TypedItems.OfType<object>().ToArray()); }
            set { _native = value; }
        }

        protected override IEnumerable GetNative() {
            return null == TypedItems ? Items : null;
        }

        protected override IEnumerable GetMainItems() {
            return TypedItems ?? (IEnumerable)Items;
        }
    }
}