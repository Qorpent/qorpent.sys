using System.Collections.Generic;
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
        /// Аттрибуты структуры
        /// </summary>
        public IList<StructField> Fields { get; set; }
    }
}