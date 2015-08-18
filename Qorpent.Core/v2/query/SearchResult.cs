using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;

namespace qorpent.v2.query {
    public  class SearchResult: IJsonSerializable {
        public bool IsLastPage;

        public virtual object[] Native { get; set; }
        public bool NoChange;
        public int OffSet;
        public int Page;
        public int PageCount;
        public int Size;
        public string TimeStamp;
        public int Total;
        public bool Ok;
        public Exception Error;

        public void WriteAsJson(TextWriter output, string mode, ISerializationAnnotator annotator) {
            var writer = new JsonWriter(output);
            writer.OpenObject();
            writer.WriteProperty("total", Total);
            writer.WriteProperty("offset", OffSet);
            writer.WriteProperty("size", Size);

            writer.WriteProperty("page", Page);
            writer.WriteProperty("pagecount", PageCount);
            writer.WriteProperty("islastpage",IsLastPage);

            writer.WriteProperty("nochange", NoChange);
            writer.WriteProperty("timestamp",TimeStamp);

            writer.WriteProperty("ok",Ok);
            if (null != Error) {
                writer.OpenProperty("error");
                writer.OpenObject();
                writer.WriteProperty("type",Error.GetType().Name);
                writer.WriteProperty("message",Error.Message);
                writer.WriteProperty("stack",Error.StackTrace);
                writer.CloseObject();
                writer.CloseProperty();
            }

            var mainitems = GetMainItems();
            writer.OpenProperty("items");
            writer.OpenArray();
            foreach (var item in mainitems) {
                writer.WriteObject(item,mode);
            }
            writer.CloseArray();
            writer.CloseProperty();
            var native = GetNative();
            if (null!=native) {
                writer.OpenProperty("native");
                writer.OpenArray();
                foreach (var item in native)
                {
                    writer.WriteObject(item, mode);
                }
                writer.CloseArray();
                writer.CloseProperty();
            }
            writer.CloseObject();
        }

        protected virtual IEnumerable GetNative() {
            return null;
            
        }

        protected virtual IEnumerable GetMainItems() {
            return Native;

        }


    }

    public class SearchResult<T> : SearchResult {
        public T[] Items;
        private object[] _native;

        public override object[] Native
        {
            get { return _native ?? (_native = Items.OfType<object>().ToArray()); }
            set { _native = value; }
        }

        protected override IEnumerable GetNative() {
            return null == Items ? Native : null;
        }

        protected override IEnumerable GetMainItems() {
            return Items ?? (IEnumerable)Native;
        }
    }
}