using System.Data;

namespace Qorpent.Utils.Sql
{
    /// <summary>
    /// Позволяет формировать полноценные полнотекстовые запросы для SQL в упрощенной нотации
    /// </summary>
    public class SqlFullTextSearchAdapter {
        private SimpleExpressionParser parser = new SimpleExpressionParser {
            Operators = new[] {'+','-','?','|','&','~'},
            Prefixes = new char[] { },
            Suffixes = new[] { '*','!','%'}
        };
        /// <summary>
        /// Конвертирует входную макростроку в FullText Query
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string Convert(string source) {
            var exp = parser.Parse(source);
            FixTypesAndValues(exp);
            FillGapsOfOperators(exp);
            return exp.ToString();
        }

        private void FillGapsOfOperators(SimpleExpressionItem exp) {
            foreach (var e in exp.All()) {
                if (e.Type == SimpleExpressionItemType.Literal && null != e.Next &&
                    e.Next.Type == SimpleExpressionItemType.Literal) {
                    e.InsertAfter(new SimpleExpressionItem {Type = SimpleExpressionItemType.Operator, Value = "|"});
                }
            }
            
        }

        private void FixTypesAndValues(SimpleExpressionItem exp) {
            foreach (var e in exp.All()) {
                if (e.Type == SimpleExpressionItemType.Operator) {
                    if (e.Value == "+") e.Value = "&";
                    else if (e.Value == "-") e.Value = "&!";
                    else if (e.Value == "?") e.Value = "|";
                }
                 
                else if (e.Suffix == '*' && !e.IsQuoted) {
                    e.Quote = '"';
                    e.Suffix = '\0';
                    e.Value += '*';
                    e.IsQuoted = true;
                }

                else if (e.Value == "и") {
                    e.Type = SimpleExpressionItemType.Operator;
                    e.Value = "&";
                }
                else if (e.Value == "или")
                {
                    e.Type = SimpleExpressionItemType.Operator;
                    e.Value = "|";
                }
                else if (e.Value == "не")
                {
                    e.Type = SimpleExpressionItemType.Operator;
                    e.Value = "&!";
                }

              
                
                
                if (e.Type==SimpleExpressionItemType.Literal && !e.IsQuoted) {
                    bool formsof = true;
                    var type = "INFLECTIONAL";
                    if (e.Suffix == '!' ||e.Value.Length<3) formsof = false;
                    if (e.Suffix == '%') {
                        type = "THESAURUS";
                        formsof = true;
                    }
                    
                    e.Suffix = '\0';
                    if (formsof) {

                        e.Value = "FORMSOF( " + type + ", " + e.Value + " )";
                    }
                }
                
            }

            
        }
    }
}
