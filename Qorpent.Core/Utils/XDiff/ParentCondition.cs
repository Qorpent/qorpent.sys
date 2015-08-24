using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.XDiff {
    internal class ParentCondition{
        public ParentCondition(string value){
            var parts = value.Split('-');
            if (parts.Length == 3){
                Name = parts[0];
                if (parts[1] == "id"){
                    Id = parts[2];
                }
                else{
                    Code = parts[2];
                }
            }
            else{
                if (parts[0] == "id"){
                    Id = parts[1];
                }
                else{
                    Code = parts[1];
                }
            }

        }

        public string Name;
        public string Id;
        public string Code;

        public bool Match(XElement e){
            if (Name != null && e.Name.LocalName != Name) return false;
            if (Id != null && e.Attr("id") != Id) return false;
            return Code == e.Attr("code");
        }

    }
}