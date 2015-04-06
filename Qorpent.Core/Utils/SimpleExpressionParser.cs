using System;
using System.Collections.Generic;
using System.Globalization;

namespace Qorpent.Utils {
    /// <summary>
    /// ¬спомогательный класс дл€ разбора выражений
    /// </summary>
    public class SimpleExpressionParser {
        //массив открывающих символов
#pragma warning disable 1591
        public char[] Openers = {'(','[','{'};

        public char[] Quotes = {'\'','"'};
        public char Escape = '\\';
        public char[] Closers = {')',']','}'};
        public char[] Operators = {'-','+','*','/','%'};
        public char[] Prefixes = {'@','$','#','!'};
        public char[] Suffixes = {};
        public char[] NotAllowed = {};
#pragma warning restore 1591
        /// <summary>
        /// ѕреобразует строку с выражением в набор <see cref="SimpleExpressionItem"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public SimpleExpressionItem Parse(string expression) {
            SimpleExpressionItem result = new SimpleExpressionItem{Type = SimpleExpressionItemType.Group,Opener=Openers[0],Closer=Closers[0]};
            InternalParse(expression, result);
            return AccomodateResult(result);
        }
        private enum Stage {
            None,

        }
        private void InternalParse(string expression, SimpleExpressionItem result) {
            var target = result;
            var skip = 0;
            char c = '\0';
            char next = '\0';
            char prefix = '\0';
            bool quoted = false;
            bool literal = false;
            Action newItem = () => {
                var newitem = new SimpleExpressionItem {Prefix = prefix};
                prefix = '\0';
                target.Append(newitem);
                target = newitem;
                
            };
            Func<bool,bool> endItem = (n) => {
                bool r = false;
                if (n) {
                    if (-1 != Array.IndexOf(Suffixes, next)) {
                        target.Suffix = next;
                        skip = 1;
                    }
                }
                else {
                    if (-1 != Array.IndexOf(Suffixes, c))
                    {
                        target.Suffix = c;
                        r = true;
                    }
                }
                quoted = false;
                literal = false;
                target = target.Parent;
                return r;
            };
            for (var i = 0; i < expression.Length; i++) {
                if (skip > 0) {
                    skip--;
                    continue;
                }
                next = i == expression.Length - 1 ? '\0' : expression[i + 1];
                c = expression[i];
                if (quoted) { //we are in quoted string
                    if (c == Escape) {
                        if (next == target.Quote) {
                            target.Value += next;
                            skip = 1;
                            continue;
                        }
                    }
                    if (c == target.Quote) {
                        endItem(true);
                        continue;
                    }
                    target.Value += c;
                    continue;
                }
                if (-1 != Array.IndexOf(Quotes, c)) {
                    newItem();
                    target.IsQuoted = true;
                    target.Quote = c;
                    quoted = true;
                    continue;
                }
                if (!char.IsLetterOrDigit(c) && literal) {
                    if(endItem(false))continue;
                }
                if (char.IsLetterOrDigit(c)) {
                    if (!literal) {
                        newItem();
                        literal = true;
                    }
                    target.Value += c;
                }
                if (-1 != Array.IndexOf(Prefixes, c)) {
                    
                    prefix = c;
                    continue;
                }
                if (char.IsWhiteSpace(c)) {
                    
                    continue;
                }
                if (-1 != Array.IndexOf(Operators, c)) {
                   
                    target.Append(new SimpleExpressionItem {
                        Type = SimpleExpressionItemType.Operator,
                        Value = c.ToString(CultureInfo.InvariantCulture)
                    });
                    continue;
                }
                if (-1 != Array.IndexOf(Closers,c)) {
                    
                    if (target.Opener != '\0') {
                        if (c==target.Closer) {
                            endItem(false);
                            continue;
                        }
                    }
                }
                if (-1 != Array.IndexOf(Openers, c)) {
                    newItem();
                    target.Type = SimpleExpressionItemType.Group;
                    target.Opener = c;
                    var idx = Array.IndexOf(Openers, c);
                    target.Closer = Closers[idx];
                    continue;
                }

               
                
               
                
            }
        }

        private static SimpleExpressionItem AccomodateResult(SimpleExpressionItem result) {
            if (result.Children.Count == 0) {
                result = new SimpleExpressionItem {Value = null};
            }

            while (result.Children.Count == 1) {
                result = result.Children[0];
            }
            return result;
        }
    }
}