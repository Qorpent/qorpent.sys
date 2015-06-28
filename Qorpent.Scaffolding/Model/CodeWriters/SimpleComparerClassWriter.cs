using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.CodeWriters {
    /// <summary>
    /// 
    /// </summary>
    public class SimpleComparerClassWriter : CodeWriterBase {
        /// <summary>
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="output"></param>
        public SimpleComparerClassWriter(PersistentClass cls, TextWriter output = null)
            : base(cls, output) {
        }


        Field[] _fields;

        /// <summary>
        /// </summary>
        protected override void InternalRun() {
            _fields = Cls.Fields.Values.Where(_ => _.IsHash).ToArray();
            if (_fields.Length == 0) return;

            WriteStartClass();
            WriteBody();
            GenerateCompareMethod();
            GenerateEqualMethod();
            GenerateCompareInternal();
            WriteEndClass();
        }

        private void GenerateCompareMethod() {
            o.WriteLine(@"        public CompareResult Compare(DateTime currentDateTime, " + Cls.Name + " o, " + Cls.Name + " n) {");
            o.WriteLine(@"            _messages = new List<CompareMessage>();");
            o.WriteLine(@"");
            o.WriteLine(@"            CompareInternal(o, n, false);");
            o.WriteLine(@"            ");
            o.WriteLine(@"            var c = new CompareResult {CompareDateTime = currentDateTime, CompareMessages = _messages};");
            o.WriteLine(@"            return c;");
            o.WriteLine(@"        }");

        }


        private void GenerateEqualMethod() {
            o.WriteLine(@"");
            o.WriteLine(@"        public bool Equal("+Cls.Name+" o, "+Cls.Name+" n) {");
            o.WriteLine(@"            _messages = new List<CompareMessage>();");
            o.WriteLine(@"            return CompareInternal(o, n, true);");
            o.WriteLine(@"        }");
        }
        private void GenerateCompareInternal() {
            o.WriteLine(@"");
            o.WriteLine(@"        private bool CompareInternal(" + Cls.Name + " o, "+Cls.Name+" n, bool exit) {");

            GeneratePropertyCompare();

            
            o.WriteLine(@"            return true;");
            o.WriteLine(@"        }");

        }
        /// <summary>
        /// 
        /// </summary>
        private void GeneratePropertyCompare() {
            foreach (var field in _fields) {
                switch (field.DataType.CSharpDataType) {
                    case "String":
                        o.WriteLine(@"            if (!StringEqual(o." + field.Name + ", n." + field.Name + ", " + "\"" + field.Comment + "\"" + ")) {");
                        o.WriteLine(@"                if (exit) return false;");
                        o.WriteLine(@"            }");
                        break;
   
                    case "Boolean":
                        o.WriteLine(@"            if (!BoolEqual(o." + field.Name + ", n." + field.Name + ", " + "\"" + field.Comment + "\"" + ")) {");
                        o.WriteLine(@"                if (exit) return false;");
                        o.WriteLine(@"            }");
                        break;

                    default:
                        o.WriteLine(@"\t\tthrow new NotSupportedException( Type: " + "\"" + field.DataType.CSharpDataType + " for " + field.Name + "\"" + ");");
                        break;
                }

            }
            o.WriteLine(";");
        }

        private void WriteBody(){
            o.WriteLine("\t\t");
            o.WriteLine("\t\tprivate IList<CompareMessage> _messages;");
            o.WriteLine("\t\t");
            var sb = new System.Text.StringBuilder(1168);
            o.WriteLine(@"  private bool StringEqual(string o, string n, string message) {");
            o.WriteLine(@"            o = StringUtils.Norm(o);");
            o.WriteLine(@"            n = StringUtils.Norm(n);");
            o.WriteLine(@"            ");
            o.WriteLine(@"            var action = """";");
            o.WriteLine(@"            if (o == """" && n != """") {");
            o.WriteLine(@"                action = ""Добавил"";");
            o.WriteLine(@"            } else if (o != """" && n != """" && o != n) {");
            o.WriteLine(@"                action = ""Изменил"";");
            o.WriteLine(@"            } else if (o != """" && n == """") {");
            o.WriteLine(@"                action = ""Удалил"";");
            o.WriteLine(@"            } else if (o != """" && n != """" && o == n) {");
            o.WriteLine(@"                return true;");
            o.WriteLine(@"            } else if (o == """" && n == """") {");
            o.WriteLine(@"                return true;");
            o.WriteLine(@"            }");

            o.WriteLine(
                @"            var cm = new CompareMessage {Message = string.Concat(action, "" ["", field, ""]""), OldValue = o, NewValue = n , Field = field};
            ");

            o.WriteLine(@"            _messages.Add(cm);");
            o.WriteLine(@"");
            o.WriteLine(@"            return false;");
            o.WriteLine(@"        }     ");
            o.WriteLine(@"        ");
            o.WriteLine(@"        private bool BoolEqual(bool o, bool n, string message) {");
            o.WriteLine(@"            var action = """";");
            o.WriteLine(@"            if (o != n) {");
            o.WriteLine(@"                action = ""Изменил"";");
            o.WriteLine(@"            } else {");
            o.WriteLine(@"                return true;");
            o.WriteLine(@"            }");
            o.WriteLine(@"            var cm = new CompareMessage {Message = string.Concat(action, "" ["", message, ""]""), OldValue = o, NewValue = n };");
            o.WriteLine(@"            _messages.Add(cm);");
            o.WriteLine(@"            return false;");
            o.WriteLine(@"        }");
		}

        private void WriteEndClass() {
            o.WriteLine("\t}");
            o.WriteLine("}");
        }

        private void WriteStartClass() {
            WriteHeader();
            o.WriteLine("using System;");
            o.WriteLine("using System.Collections.Generic;");
            o.WriteLine("using Qorpent.Utils;");
            o.WriteLine("#if !NOQORPENT");
            o.WriteLine("using Qorpent.Serialization;");
            o.WriteLine("using Qorpent.Model;");
            o.WriteLine("using Qorpent.Utils.Extensions;");
            o.WriteLine("#endif");
            o.Write("namespace {0} {{\r\n", Cls.Namespace);
            o.WriteLine("\t///<summary>");
            o.WriteLine("\t///" + "Сравнитель " + Cls.Comment);
            o.WriteLine("\t///</summary>");
            o.Write("\tpublic partial class {0} ", Cls.Name + "Comparer {");
        }
    }
}