using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Xml.Linq;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;
using Qorpent.IO;
using Qorpent.Model;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.model
{
    public class Item: IItem,
        IJsonSerializable,
        IJsonDeserializable,
        IXmlReadable
    {
        public static T Create<T>(object src) where T : IItem,new() {
            var result = new T();
            result.Read(src);
            return result;
        } 
        public string Id { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public IDictionary<string,object> Custom { get; set; } 

        protected void WriteJson(TextWriter output, string mode,bool pretty, int level) {
            var jw=  new JsonWriter(output,pretty:pretty,level:level);
            jw.OpenObject();          
            WriteJsonInternal(jw, mode);
            jw.CloseObject();
        }

        protected virtual void WriteJsonInternal(JsonWriter jw, string mode) {
            jw.WriteProperty("_id", Id);
            jw.WriteProperty("_version", Version);
            jw.WriteProperty("name",Name,true);
            var roled = this as IWithRole;
            if (null != roled) {
                jw.WriteProperty("role",roled.Role);
            }
            jw.WriteProperty("custom",Custom,true);
        }

        protected virtual void LoadFromJson(object jsonsrc) {
            if (!(jsonsrc is IDictionary<string, object>)) return;
            var src = jsonsrc.nestorself("_source");
            Id = src.resolvestr("_id", "id");
            Version = src.resolvenum("_version", "version");
            Name = src.str("name");
            Custom = src.map("custom");

        }

        protected virtual void ReadFromXml(XElement xml) {
            Id = xml.ChooseAttr("_id", "id", "code");
            Version = xml.ChooseAttr("_version", "version").ToInt();
            Name = xml.Attr("name");
            var cxml = xml.Element("custom");
            if (null != cxml) {
                Custom = cxml.jsonify() as IDictionary<string,object>;
            }


        }


        void IJsonSerializable.WriteAsJson(TextWriter output, string mode, ISerializationAnnotator annotator, bool pretty = false, int level = 0) {
            WriteJson(output,mode,pretty,level);
        }

        void IJsonDeserializable.LoadFromJson(object jsonsrc)
        {
            LoadFromJson(jsonsrc);
        }

        void IXmlReadable.ReadFromXml(XElement xml)
        {
            ReadFromXml(xml);
        }

        public void Read(object src) {
            if (null == src) return;
            var json = src as IDictionary<string, object>;
            if (null != json) {
                LoadFromJson(json);
                return;
            }
            var xml = src as XElement;
            if (null != xml) {
                ReadFromXml(xml);
                return;
            }
            var item = src as IItem;
            if (null != item) {
                Merge(item);
            }
        }



        public void Write(TextWriter writer, string mode = null, bool pretty = false, int level = 0) {
            WriteJson(writer,mode,pretty,level);
        }

        public const string REMOVER = "_remove";
        public virtual void Merge(IItem src) {
            if (string.IsNullOrWhiteSpace(this.Id)) {
                Id = src.Id;
            }
            if (Version < src.Version) {
                Version = src.Version;
            }
            CheckRoled(this, src);
            if (null != src.Custom && 0!=src.Custom.Count) {
                Custom = Custom ?? new Dictionary<string, object>();
                JsonExtend.Extend(Custom, src.Custom);
            }
        }

        private static void CheckRoled(Item self, IItem src)
        {
            var roled = self as IWithRole;
            if (null != roled) {
                var roled2 = src as IWithRole;
                if (null != roled2 && !string.IsNullOrWhiteSpace(roled2.Role)) {
                    roled.Role = roled2.Role == REMOVER ? "" : roled2.Role;
                }
            }
        }

        public IItem Clone() {
            var result = Activator.CreateInstance(this.GetType()) as IItem;
            result.Merge(this);
            return result;
        }
    }
}
