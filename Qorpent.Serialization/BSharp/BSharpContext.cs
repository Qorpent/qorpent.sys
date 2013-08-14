using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp {
	/// <summary>
	///     Внутренний индекс компилятора
	/// </summary>
	public class BSharpContext : ConfigBase, IBSharpContext {
		private const string RAWCLASSES = "rawclasses";
		private const string ORPHANED = "orphaned";
		private const string ABSTRACTS = "abstracts";
		private const string WORKING = "working";
		private const string STATIC = "static";
		private const string OVERRIDES = "overrides";
		private const string EXTENSIONS = "extensions";
		private const string ERRORS = "errors";

		/// <summary>
		///     Исходные сырые определения классов
		/// </summary>
		public IDictionary<string, IBSharpClass> RawClasses {
			get { return Get<IDictionary<string, IBSharpClass>>(RAWCLASSES); }
			set { Set(RAWCLASSES, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<IBSharpClass> Orphans {
			get { return Get<List<IBSharpClass>>(ORPHANED); }
			set { Set(ORPHANED, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<IBSharpClass> Abstracts {
			get { return Get<List<IBSharpClass>>(ABSTRACTS); }
			set { Set(ABSTRACTS, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<IBSharpClass> Working {
			get { return Get<List<IBSharpClass>>(WORKING); }
			set { Set(WORKING, value); }
		}

		/// <summary>
		/// Классы со статической компиляцией
		/// </summary>
		public List<IBSharpClass> Static {
			get { return Get<List<IBSharpClass>>(STATIC); }
			set { Set(STATIC, value); }
		}
		/// <summary>
		/// Реестр перезагрузок классов
		/// </summary>
		public List<IBSharpClass> Overrides
		{
			get { return Get<List<IBSharpClass>>(OVERRIDES); }
			set { Set(OVERRIDES, value); }
		}
		/// <summary>
		/// Реестр перезагрузок классов
		/// </summary>
		public List<IBSharpClass> Extensions
		{
			get { return Get<List<IBSharpClass>>(EXTENSIONS); }
			set { Set(EXTENSIONS, value); }
		}

		/// <summary>
		/// Реестр перезагрузок классов
		/// </summary>
		public List<BSharpError> Errors
		{
			get { return Get<List<BSharpError>>(ERRORS); }
			set { Set(ERRORS, value); }
		}

		/// <summary>
		/// Загружает исходные определения классов
		/// </summary>
		/// <param name="rawclasses"></param>
		public void Setup(IEnumerable<IBSharpClass> rawclasses) {
			if (null == RawClasses) {
				RawClasses = new Dictionary<string, IBSharpClass>();
			}
			if (null == Errors) {
				Errors = new List<BSharpError>();
			}
			foreach (var cls in rawclasses) {
				if (RawClasses.ContainsKey(cls.FullName)) {
					if (RawClasses[cls.FullName] != cls) {
						Errors.Add(BSharpErrors.DuplicateClassNames(cls));
					}
				}
				else {
					RawClasses[cls.FullName] = cls;
				}
			}
		}

		

		/// <summary>
		///     Присоединяет и склеивается с другим результатом
		/// </summary>
		/// <param name="othercontext"></param>
		public void Merge(IBSharpContext othercontext) {
			var subresult = othercontext as BSharpContext;

			if (null != subresult.Abstracts) {
				if (null == Abstracts) {
					Abstracts = new List<IBSharpClass>();
				}
				foreach (IBSharpClass a in subresult.Abstracts) {
					Abstracts.Add(a);
				}
			}
			if (null != subresult.Orphans) {
				if (null == Orphans) {
					Orphans = new List<IBSharpClass>();
				}
				foreach (IBSharpClass a in subresult.Orphans) {
					Orphans.Add(a);
				}
			}
			if (null != subresult.Working) {
				if (null == Working) {
					Working = new List<IBSharpClass>();
				}
				foreach (IBSharpClass a in subresult.Working) {
					Working.Add(a);
				}
			}

			if (null != subresult.Overrides)
			{
				if (null == Overrides)
				{
					Overrides = new List<IBSharpClass>();
				}
				foreach (IBSharpClass a in subresult.Overrides)
				{
					Overrides.Add(a);
				}
			}

			if (null != subresult.Extensions)
			{
				if (null == Extensions)
				{
					Extensions = new List<IBSharpClass>();
				}
				foreach (IBSharpClass a in subresult.Extensions)
				{
					Extensions.Add(a);
				}
			}

			if (null != subresult.Errors)
			{
				if (null == Errors)
				{
					Errors = new List<BSharpError>();
				}
				foreach (var a in subresult.Errors)
				{
					Errors.Add(a);
				}
			}
		}

		private IDictionary<string, IBSharpClass> _resolveclassCache = new Dictionary<string, IBSharpClass>();
		/// <summary>
		/// Разрешает класс по коду и заявленному пространству имен
		/// </summary>
		/// <param name="code"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		public  IBSharpClass Get( string code, string ns = null) {

			if (null == ns) {
				var full = Working.FirstOrDefault(_ => _.FullName == code);
				if (null != full) return full;
				return Working.FirstOrDefault(_ => _.Name == code);
			}
			var key = ns + "." + code;
			if (_resolveclassCache.ContainsKey(key)) {
				return _resolveclassCache[key];
			}
			IBSharpClass import = null;
			if (!String.IsNullOrWhiteSpace(code)) {
				if (code.Contains('.')) {
					if (RawClasses.ContainsKey(code)) {
						import = RawClasses[code];
					}
				}
				else if (ns.Contains(".")) {
					var nsparts = ns.SmartSplit(false, true, '.');
					for (var i = nsparts.Count - 1; i >= -1; i--) {
						if (i == -1) {
							var probe = code;
							if (RawClasses.ContainsKey(probe)) {
								import = RawClasses[probe];
								break;
							}
						}
						else {
							var probe = "";
							for (var j = 0; j <= i; j++) {
								probe += nsparts[j] + ".";
							}
							probe += code;
							if (RawClasses.ContainsKey(probe)) {
								import = RawClasses[probe];
								break;
							}
						}
					}


				}
				else if(!String.IsNullOrWhiteSpace(ns)) {
					var probe = ns+"." + code;
					if (RawClasses.ContainsKey(probe))
					{
						import = RawClasses[probe];
					}
					
				}
			}
			if (null == import) {
				if (RawClasses.ContainsKey(code))
				{
					import = RawClasses[code];
				}
			}
			if (null != import) {
				_resolveclassCache[key] = import;
			}
			return import;
		}
		/// <summary>
		/// Возвращает коллекцию классов по типу классов
		/// </summary>
		/// <param name="datatype"></param>
		/// <returns></returns>
		public IEnumerable<IBSharpClass> Get(BSharpContextDataType datatype) {
			if (!_built) throw new Exception("not still built");
			switch (datatype) {
				case BSharpContextDataType.Working:
					return Working;
				case BSharpContextDataType.Orphans:
					return Orphans;
				case BSharpContextDataType.Abstracts:
					return Abstracts;
				case BSharpContextDataType.Statics:
					return Static;
				case BSharpContextDataType.Overrides:
					return Overrides;
				case BSharpContextDataType.Extensions:
					return Extensions;
				case BSharpContextDataType.Errors:
					
				default :
					return new IBSharpClass[] {};
			}
		}

		/// <summary>
		/// Возвращает ошибки указанного уровня
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public IEnumerable<BSharpError> GetErrors(ErrorLevel level = ErrorLevel.None) {
			if (null == Errors) yield break;
			foreach (var e in Errors.ToArray()) {
				if (e.Level >= level) {
					yield return e;
				}
			}
		}

		private bool _built = false;
		/// <summary>
		/// Строит рабочий индекс классов
		/// </summary>
		public void Build() {
			if (_built) return;
			Overrides = RawClasses.Values.Where(_ => _.Is(BSharpClassAttributes.Override)).OrderBy(_=>_.Source.Attr("priority").ToInt()).ToList();
			Extensions = RawClasses.Values.Where(_ => _.Is(BSharpClassAttributes.Extension)).OrderBy(_ => _.Source.Attr("priority").ToInt()).ToList();
			ApplyOverrides();
			ApplyExtensions();
			ResolveOrphans();
			Orphans = RawClasses.Values.Where(_ => _.IsOrphaned).ToList();
			Abstracts = RawClasses.Values.Where(_ => _.Is(BSharpClassAttributes.Abstract) && !_.IsOrphaned).ToList();
			Working = RawClasses.Values.Where(_ => !_.Is(BSharpClassAttributes.Extension) && !_.Is(BSharpClassAttributes.Override) && !_.Is(BSharpClassAttributes.Abstract) && !_.IsOrphaned).ToList();
			Static = RawClasses.Values.Where(_ => _.Is(BSharpClassAttributes.Static) && !_.IsOrphaned).ToList();
			ResolveImports();
			_built = true;
		}

		private void ApplyExtensions() {
			foreach (var o in Extensions) {
				var cls = Get(o.TargetClassName, o.Namespace);
				if (null == cls) {
					o.Remove(BSharpClassAttributes.Extension);
					o.Name = o.TargetClassName;
				}
				else
				{
					ApplyExtension(o, cls);
				}
			}
		}

		private void ApplyExtension(IBSharpClass src, IBSharpClass trg) {
			foreach (var e in src.Source.Attributes()) {
				if (null == trg.Source.Attribute(e.Name)) {
					trg.Source.Add(e);
				}
			}
			trg.Source.Add(src.Source.Elements());
		}

		private void ApplyOverrides()
		{
			foreach (var o in Overrides) {
				var cls = Get(o.TargetClassName, o.Namespace);
				if (null == cls) {
					o.Remove(BSharpClassAttributes.Override);
					o.Name = o.TargetClassName;
				}
				else {
					ApplyOverride(o, cls);
				}
			}
		}

		private void ApplyOverride(IBSharpClass src, IBSharpClass trg) {
			foreach (var e in src.Source.Attributes())
			{
				trg.Source.SetAttributeValue(e.Name,e.Value);
			}
			trg.Source.Add(src.Source.Elements());
		}


		private void ResolveImports()
		{
			foreach (var w in Working.Union(Abstracts))
			{
				foreach (IBSharpImport i in w.SelfImports)
				{
					i.Orphaned = true;
					var import = Get(i.TargetCode, w.Namespace);
					if (null != import)
					{
						i.Orphaned = false;
						i.Target = import;
					}
				}
			}
		}


		private void ResolveOrphans() {
			IEnumerable<IBSharpClass> _initiallyorphaned = RawClasses.Values.Where(_ => !_.Is(BSharpClassAttributes.Explicit));
			foreach (var o in _initiallyorphaned) {
				string code = o.DefaultImportCode;
				string ns = o.Namespace;
				var import = Get(code, ns);
				if (import != null) {
					o.Remove(BSharpClassAttributes.Orphan);
					o.DefaultImport = import;
				}
			}
		}
	}
}