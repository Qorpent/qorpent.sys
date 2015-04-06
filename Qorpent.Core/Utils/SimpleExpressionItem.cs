using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Qorpent.Utils {
    /// <summary>
    /// Единица простого выражения
    /// </summary>
    public class SimpleExpressionItem {
        /// <summary>
        /// Тип единицы
        /// </summary>
        public SimpleExpressionItemType Type;
#pragma warning disable 1591
        public SimpleExpressionItem Next;

        public SimpleExpressionItem Previous;
        public SimpleExpressionItem Parent;
        public List<SimpleExpressionItem> Children = new List<SimpleExpressionItem>();
        public char Prefix = '\0';
        public char Suffix = '\0';
        public bool IsQuoted;
        public char Quote;
        public char Opener;
        public char Closer;
        public char Escaper = '\\';
        public string Value = "";
        public override string ToString() {
            var value = InternalToString();
            return PrefixSuffix(value, "");
        }

        public IEnumerable<SimpleExpressionItem> All() {
            yield return this;
            foreach (var child in Children.ToArray()) {
                foreach (var i in child.All()) {
                    yield return i;
                }
            }
        } 

        private string PrefixSuffix(string value, string delim) {
            if (Prefix != '\0') {
                value = Prefix + delim + value;
            }
            if (Suffix != '\0') {
                value += delim + Suffix;
            }
            return value;
        }

        private string InternalToString() {
            if (Type == SimpleExpressionItemType.Literal) {
                if (IsQuoted) {
                    return Quote +
                           Value.Replace(Quote.ToString(CultureInfo.InvariantCulture),
                               Escaper.ToString(CultureInfo.InvariantCulture)) + Quote;
                }
                return Value;
            }
            if (Type == SimpleExpressionItemType.Operator) {
                return Value;
            }
            if (Parent == null) return string.Join(" ", Children);
            return Opener + " " + string.Join(" ", Children) + " " + Closer;
        }

        public string GetDescriptiveString() {
            string result = "";
            if (Type == SimpleExpressionItemType.Group) {
                result = "(G: " + string.Join(" ", Children.Select(_ => _.GetDescriptiveString())) + ")";
            }
            else {
                var type = Type.ToString().Substring(0, 1);
                if (IsQuoted) type = "Q("+Quote+")";
                result = "(" + type + ": " + Value+ ")";
            }
            return PrefixSuffix(result,":");
        }

        public SimpleExpressionItem Append(SimpleExpressionItem item) {
            if (0 != Children.Count) {
                var last = Children[Children.Count - 1];
                last.Next = item;
                item.Previous = last;
            }
            item.Parent = this;
            Children.Add(item);
            return this;
        }

        public void InsertAfter(SimpleExpressionItem i) {
            i.Next = this.Next;
            if (null != i.Next) {
                i.Next.Previous = i;
            }
            i.Parent = this.Parent;
            if (null != i.Parent) {
                if (null == i.Next) {
                    i.Parent.Children.Add(i);
                }
                else {
                    i.Parent.Children.Insert(i.Parent.Children.IndexOf(i.Next),i);
                }
            }
        }
    }
#pragma warning restore 1591
}