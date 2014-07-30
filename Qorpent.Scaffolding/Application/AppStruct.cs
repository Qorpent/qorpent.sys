using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application {
    /// <summary>
    /// 
    /// </summary>
    public class AppStruct:AppObject<AppStruct> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cls"></param>
        /// <returns></returns>
        public override AppStruct Setup(ApplicationModel model, IBSharpClass cls) {
            base.Setup(model, cls);
            Fields = new List<StructField>();
            foreach (var field in cls.Compiled.Elements()) {
                Fields.Add(ResolveField(field));
            }
            return this;
        }

        private StructField ResolveField(XElement field) {
            return new StructField {
                Type = field.Name.ToString(),
                Code = field.GetCode(),
                Name = field.GetName(),
                DefaultValue = field.Value
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var fieldsString = new StringBuilder();
            fieldsString.AppendLine(string.Format("{0} {1} '{2}'", GetType().Name, Code, Name));
            foreach (var field in Fields) {
                var line = string.Format("\t{0}({1})", field.Code, field.Type);
                if (null != field.DefaultValue) {
                    line += " : " + field.DefaultValue;
                }
                line += " //" + field.Name;
                fieldsString.AppendLine(line);
            }
            return fieldsString.ToString();
        }

        /// <summary>
        /// Аттрибуты структуры
        /// </summary>
        public IList<StructField> Fields { get; set; }
    }
}