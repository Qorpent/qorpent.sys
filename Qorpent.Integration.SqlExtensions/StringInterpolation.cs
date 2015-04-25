using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils
{
	/// <summary>
	/// Выполняет трнсляцию ${code1,code2,...;default} в подстановочные коды из параметров с резолюцией приоритета
	/// </summary>
	/// <remarks>
	/// Синтаксис ANCOR STARTBLOCK CODE [CODEDELIMITER CODE]* [DEFAULTDELIMOTER DEFAULT] ENDBLOCK
	/// По умолчанию ${ CODE [, CODE] * [: DEFAULT] }
	/// </remarks>
	public class StringInterpolation {
		/// <summary>
		/// Символ начала обработки подстановки в целевом элементе
		/// </summary>
		public char AncorSymbol = '$';

		/// <summary>
		/// Символ начала блока данных
		/// </summary>
		public char StartSymbol = '{';
		/// <summary>
		/// Символ начала блока данных
		/// </summary>
		public char EndBlockSymbol = '}';

		/// <summary>
		/// Разделитель кодов в основнойчасти
		/// </summary>
		public char CodeDelimiterSymbol = ',';

		/// <summary>
		/// Разделитель кодов и Default части
		/// </summary>
		public char DefaultDelimiterSymbol = ':';
		/// <summary>
		/// Строка - заместитель пустой при посдстановках
		/// </summary>
		public char EmptyStringReplacer = '¶';

		private StringBuilder _targetBuffer;
		private StringBuilder _currentBuffer;

		private IDictionary<string, object> _source;
		
		static readonly IDictionary<string,object> DefaultSubst = new Dictionary<string,object>();

		/// <summary>
		/// Производит прошивку словаря
		/// </summary>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <param name="source2"></param>
		/// <param name="controlkey"></param>
		/// <returns></returns>
		public string Interpolate(string target, object source = null, IDictionary<string,object> source2 = null, string controlkey = null){
			if (string.IsNullOrEmpty(target)) return target;
			if (-1 == target.IndexOf(AncorSymbol)) return target;
			if (-1 == target.IndexOf('{')) return target;
			if (-1 == target.IndexOf('}')) return target;
			//оптимизация возврата исходной строки где нет вообще контента
			_source2 = source2;
			if (string.IsNullOrWhiteSpace(target)) {
				return target;
			}
			//оптимизация возврата исходной строки при отсутствии анкоров
			if (target.All(_ => _ != AncorSymbol)) {
				return target;
			}
			if (null == source) {
				source = DefaultSubst;
			}
			if (source is IDictionary<string, object>)
			{
				_source = (IDictionary<string, object>)source;
			}else if (source is IDictionary<string, string>) {
				_source = ((IDictionary<string, string>)source).ToDictionary(_=>_.Key,GetValue);
			}else if (source is IEnumerable<KeyValuePair<string, object>>){
				_source = new Dictionary<string, object>();
				foreach (var values in (IEnumerable<KeyValuePair<string, object>>)source){
					_source[values.Key] = values.Value.ToStr();
				}
			}
			else {
				_source = new Dictionary<string, object>();
				foreach (var getval in ReflectionExtensions.FindAllValueMembers(source.GetType(), null, true, true)) {
					_source[getval.Member.Name] = getval.Get(source);
				}
			}
			_sourceString = target;
			_targetBuffer = new StringBuilder();
			_currentBuffer = new StringBuilder();
			_currentSubst = new StringBuilder();
			_currentCode = new StringBuilder();
			_wasAncor = false;
			_wasOpen = false;
			_controlKey = controlkey;
			Interpolate();
			// если есть остаточное открытие - значит у нас не до конца была произведена подстановка
			// мы должны допотставить данные из currentBuffer
			if (_wasAncor && !_wasOpen){
				_targetBuffer.Append(AncorSymbol);
			}
			if (_wasOpen ) {
				StornateTail();
			}

			

			return _targetBuffer.ToString();
		}
		/// <summary>
		/// Префикс для преобразования строковых значений в даты
		/// </summary>
		public string DateTimeParsePrefix = "datetime~";

		/// <summary>
		/// префикс для преобразования чисел
		/// </summary>
		public string DecimalParsePrefix = "decimal~";
		/// <summary>
		/// Префикс строчных интов
		/// </summary>
		public string IntParsePrefix = "int~";
		private  object GetValue(KeyValuePair<string, string> _) {
			var s = _.Value;
			if (s.StartsWith(DateTimeParsePrefix))
			{
				return DateTime.Parse(s.Substring(DateTimeParsePrefix.Length), CultureInfo.InvariantCulture);
			}
			else if (s.StartsWith(DecimalParsePrefix))
			{
				return decimal.Parse(s.Substring(DecimalParsePrefix.Length),CultureInfo.InvariantCulture);
			}
			else if (s.StartsWith(IntParsePrefix))
			{
				return int.Parse(s.Substring(IntParsePrefix.Length), CultureInfo.InvariantCulture);
			}
			return s;
		}


		private void StornateTail(int skiplastincurrent = 0) {
			_targetBuffer.Append(AncorSymbol);
			_targetBuffer.Append(StartSymbol);
			if (skiplastincurrent == 0) {
				_targetBuffer.Append(_currentBuffer);
			}
			else {
				_currentBuffer.Remove(_currentBuffer.Length - skiplastincurrent, skiplastincurrent);
				_targetBuffer.Append(_currentBuffer);
			}
			_wasOpen = false;
			_wasAncor = false;
		}

		private string _sourceString;
		private int _idx;
		private char _currentChar;

		private bool _wasAncor;
		private bool _wasOpen;
		private bool _indefault;
		private StringBuilder _currentSubst;
		private StringBuilder _currentCode;
		private bool _resolved;
		private IDictionary<string, object> _source2;
		private string _controlKey;
	    private int _infun = 0;


	    private void Interpolate() {
			
			for (_idx = 0; _idx < _sourceString.Length; _idx++) {
				_currentChar = _sourceString[_idx];
				// сначала обеспечиваем поведении при наличии открытого текущего анкора
				if (_wasAncor) {
					if (_currentChar == AncorSymbol) {
						_targetBuffer.Append(AncorSymbol);
						continue;
					}
					//теперь следующий вариант - проверяем, находимся ли мы уже в блоке данных
					if (!_wasOpen) {

						if (_currentChar == StartSymbol) {
							_wasOpen = true;
						}
						else {
							//снимаем признак анкора
							_wasAncor = false;
							_targetBuffer.Append(AncorSymbol);
							_targetBuffer.Append(_currentChar);
						}
					}
					// теперь проверяем не пришло ли закрытие группы
					else if (_currentChar == EndBlockSymbol) {
						_wasOpen = false;
						_wasAncor = false;
						//пытаемся дорезольвить
						if (!(_resolved||_indefault)) {
							TryResolve();
						}
						if (_currentSubst.Length != 0 && _currentSubst[0]!=EmptyStringReplacer){
							_targetBuffer.Append(_currentSubst);
						}
					}
					//ситуаця ошибки синтаксиса, сжимаем текущий поток
					else if (_currentChar == StartSymbol ) {
						StornateTail();
					}else if (_currentChar == AncorSymbol) {
						StornateTail(1);
						StartInterpolation();
					}

					else {
						//нам в любом случае приходится резервировать весь поток
						_currentBuffer.Append(_currentChar);
						// если уже резолюция  завершена, то нам надо просто пропустить дальнейшую резолюцию
						if (!_resolved) {
							if (_indefault) {
								//если мы внутри блока с дефолтным значением, то просто добавляем символ к подстановке
								_currentSubst.Append(_currentChar);
							}
							else {
								//иначе мы в блоке разбора кодов и тут есть 3 варианта - разделители-кодов и дефолта и символ кода
								//при этом пробелы игнорируются!
								if (!char.IsWhiteSpace(_currentChar)) {
								    if (_currentChar == '(') {
								       
								        _infun++;
								        _currentCode.Append(_currentChar);
                                        continue;
								    }
                                    if (_currentChar == ')') {
                                        _infun--;
                                        if (_infun<0)
                                        {
                                            throw new Exception("invalid close brace");
                                        }
                                        _currentCode.Append(_currentChar);
                                        continue;
                                    }
									if (_currentChar == CodeDelimiterSymbol && _infun==0) {
										_resolved = TryResolve();
									}
									else if (_currentChar == DefaultDelimiterSymbol && _infun==0) {
										_resolved = TryResolve();
										_indefault = true;
									}
									else {
										_currentCode.Append(_currentChar);
									}

								}
							}
						}
					}

				}
				//иначе мы должны проверить не является ли символ анкором
				else if (_currentChar == AncorSymbol) {
					StartInterpolation();
				}
				else {
					//в остальных случаях мы должны просто добавить символ в буфер
					_targetBuffer.Append(_currentChar);
				}
			}
		}

		private void StartInterpolation() {
//сбрасываем текущую подстановка
			_currentSubst.Clear();
			_currentCode.Clear();
			_currentBuffer.Clear();
			_wasAncor = true;
			_wasOpen = false;
			_resolved = false;
			_indefault = false;
		    _infun = 0;

		}

		private bool TryResolve() {
			_currentSubst.Clear();
			if (0 == _currentCode.Length) return false;
			var fullcode = _currentCode.ToString();
			var code = fullcode;
			string format = null;
			if (fullcode.IndexOf('%') != -1) {
				code = fullcode.Split('%')[0];
				format = fullcode.Split('%')[1];
			}
			_currentCode.Clear();
            if (!string.IsNullOrWhiteSpace(_controlKey) && _controlKey == code)
            {
                throw new Exception("Cyclic interpolation");
            }
		    
		    var val = GetRawValue(code);
		    if (null == val) return false;

			string strval = null;
			if (null == format)
			{
                if (val is DateTime)
                {
                    strval = ((DateTime)val).ToString( CultureInfo.InvariantCulture);
                }
                else if (val is Decimal)
                {
                    strval = ((Decimal)val).ToString( CultureInfo.InvariantCulture);
                }
                else if (val is int)
                {
                    strval = ((int)val).ToString( CultureInfo.InvariantCulture);
                }
                else {
                    strval = val.ToString();
                }
			}
			else
			{
				if (val is DateTime) {
					strval = ((DateTime) val).ToString(format,CultureInfo.InvariantCulture);
				}else if (val is Decimal) {
					strval = ((Decimal)val).ToString(format,CultureInfo.InvariantCulture);
				}
				else if (val is int) {
					strval = ((int)val).ToString(format, CultureInfo.InvariantCulture);
				}
				else{
					strval = string.Format("{0:" + format + "}", val);
				}
			}
			
			if (string.IsNullOrEmpty(strval)) {
				
				return false;
			}
			if (null != val) {
				_currentSubst.Append(strval);
			}
			_currentCode.Clear();
			return true;
		}

	    private object GetRawValue(string code) {
	        var resolvedCode = code;
	        string[] args = null;
	        if (code.Contains("(") && code.EndsWith(")")) {
	            resolvedCode = code.Substring(0, code.IndexOf('('));
	            var _args = code.Substring(code.IndexOf('(') + 1);
	            _args = _args.Substring(0, _args.Length - 1);
	            args = _args.SmartSplit().ToArray();
	        }
	        object val = null;
	        if (!_source.ContainsKey(resolvedCode)) {
	            if (null != _source2 && _source2.ContainsKey(resolvedCode)) {
	                val = _source2[resolvedCode];
	            }
	            else if(CoreFunctions.ContainsKey(resolvedCode )) {
	                val = CoreFunctions[resolvedCode];
                }
                else if (code.Contains(".") && !code.StartsWith(".")) {
                    val = TryGetStructureValue(resolvedCode);
                }
	        }
	        else {
                val = _source[resolvedCode];
	        }
	        if (null != args && val is Delegate) {
                List<object> arguments = new List<object>();
	            foreach (var a in args) {
	                if ((a.StartsWith("\'") && a.EndsWith("\'")) || (a.StartsWith("\"") && a.EndsWith("\""))) {
	                    arguments.Add(a.Substring(1,a.Length-2));
                    }
                    else if (a == "0" || a.ToDecimal() != 0) {
                        if (a.Contains(".")) {
                            arguments.Add(a.ToDecimal());
                        }
                        else {
                            arguments.Add(a.ToInt());
                        }
                    }
                    else if (a == "true" || a == "false") {
                        arguments.Add(a.ToBool());
                    }
                    else {
                        arguments.Add(GetRawValue(a));
                    }
	            }
	            var d = (Delegate) val;
	            var dargs = d.Method.GetParameters();
	            for (var i = 0; i < dargs.Length; i++) {
	                var darg = dargs[i];
                    var t = darg.ParameterType;
	                if (arguments.Count > i) {
	                    var myarg = arguments[i];
                        if (null != myarg)
                        {
                            if (!t.IsInstanceOfType(myarg))
                            {
                                arguments[i] = myarg.ToTargetType(t);
                            }
                        }
                        else
                        {
                            if (t.IsValueType)
                            {
                                arguments[i] =Activator.CreateInstance(t);
                            }
                        }
	                }
	                else {
                        if (t.IsValueType)
                        {
                            arguments.Add(Activator.CreateInstance(t));
                        }
                        else
                        {
                            arguments.Add(null);
                        }
	                }
	                
	                
	            }
	            
	            val = d.DynamicInvoke((object[]) arguments.ToArray());
	        }
	        return val;
	    }

	    private object TryGetStructureValue(string code) {
	        var paths = code.Split('.');
	        var result = GetRawValue(paths[0]);
	        if (null == result) return null;
	        for (var i = 1; i < paths.Length; i++) {
	            var propInfo = result.GetType().GetProperty(paths[i], 
                    BindingFlags.GetProperty | BindingFlags.IgnoreCase
                    | BindingFlags.Instance | BindingFlags.Public
                    );
	            if (null == propInfo) return null;
	            result = propInfo.GetValue(result);
	        }
	        return result;
	    }
	    static readonly Dictionary<string,object> CoreFunctions = new Dictionary<string, object> {
	        {"len",(Func<string,int>)(s=>s==null?0:s.Length)},
	        {"upper",(Func<string,string>)(s=>s==null?string.Empty:s.ToUpperInvariant())},
	        {"lower",(Func<string,string>)(s=>s==null?string.Empty:s.ToLowerInvariant())},
	        {"trim",(Func<string,string>)(s=>s==null?string.Empty:s.Trim())},
	        {"match",(Func<string,string,string,string>)((i,p,g) => {
	            if (null == i) return string.Empty;
	            if (null == p) return string.Empty;
	            var match = Regex.Match(i, p);
	            if (string.IsNullOrWhiteSpace(g)) return match.Value;
	            if (g.ToInt() != 0) return match.Groups[g.ToInt()].Value;
	            return match.Groups[g].Value;
	        })},
            {"replace",(Func<string,string,string,string>)((i, p, r) => {
                if (null == i) return string.Empty;
	            if (null == p) return string.Empty;
                if (null == r) return string.Empty;
                return Regex.Replace(i, p, r);
            })},

	    };
	}
}
