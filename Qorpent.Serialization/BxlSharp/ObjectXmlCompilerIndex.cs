﻿using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.BxlSharp {
	/// <summary>
	///     Внутренний индекс компилятора
	/// </summary>
	public class ObjectXmlCompilerIndex : ConfigBase {
		private const string RAWCLASSES = "rawclasses";
		private const string ORPHANED = "orphaned";
		private const string ABSTRACTS = "abstracts";
		private const string WORKING = "working";
		private const string STATIC = "static";
		private const string OVERRIDES = "overrides";
		private const string EXTENSIONS = "extensions";
		/// <summary>
		/// Возвращает рабочий класс по коду
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ObjectXmlClass Get(string name) {
			var full = Working.FirstOrDefault(_ => _.FullName == name);
			if (null != full) return full;
			return Working.FirstOrDefault(_ => _.Name == name);
		}

		/// <summary>
		///     Исходные сырые определения классов
		/// </summary>
		public IDictionary<string, ObjectXmlClass> RawClasses {
			get { return Get<IDictionary<string, ObjectXmlClass>>(RAWCLASSES); }
			set { Set(RAWCLASSES, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<ObjectXmlClass> Orphans {
			get { return Get<List<ObjectXmlClass>>(ORPHANED); }
			set { Set(ORPHANED, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<ObjectXmlClass> Abstracts {
			get { return Get<List<ObjectXmlClass>>(ABSTRACTS); }
			set { Set(ABSTRACTS, value); }
		}

		/// <summary>
		///     Классы с непроинициализированным наследованием
		/// </summary>
		public List<ObjectXmlClass> Working {
			get { return Get<List<ObjectXmlClass>>(WORKING); }
			set { Set(WORKING, value); }
		}

		/// <summary>
		/// Классы со статической компиляцией
		/// </summary>
		public List<ObjectXmlClass> Static {
			get { return Get<List<ObjectXmlClass>>(STATIC); }
			set { Set(STATIC, value); }
		}
		/// <summary>
		/// Реестр перезагрузок классов
		/// </summary>
		public List<ObjectXmlClass> Overrides
		{
			get { return Get<List<ObjectXmlClass>>(OVERRIDES); }
			set { Set(OVERRIDES, value); }
		}
		/// <summary>
		/// Реестр перезагрузок классов
		/// </summary>
		public List<ObjectXmlClass> Extensions
		{
			get { return Get<List<ObjectXmlClass>>(EXTENSIONS); }
			set { Set(EXTENSIONS, value); }
		}

		/// <summary>
		///     Присоединяет и склеивается с другим результатом
		/// </summary>
		/// <param name="subresult"></param>
		public void Merge(ObjectXmlCompilerIndex subresult) {
			if (null != subresult.Abstracts) {
				if (null == Abstracts) {
					Abstracts = new List<ObjectXmlClass>();
				}
				foreach (ObjectXmlClass a in subresult.Abstracts) {
					Abstracts.Add(a);
				}
			}
			if (null != subresult.Orphans) {
				if (null == Orphans) {
					Orphans = new List<ObjectXmlClass>();
				}
				foreach (ObjectXmlClass a in subresult.Orphans) {
					Orphans.Add(a);
				}
			}
			if (null != subresult.Working) {
				if (null == Working) {
					Working = new List<ObjectXmlClass>();
				}
				foreach (ObjectXmlClass a in subresult.Working) {
					Working.Add(a);
				}
			}

			if (null != subresult.Overrides)
			{
				if (null == Overrides)
				{
					Overrides = new List<ObjectXmlClass>();
				}
				foreach (ObjectXmlClass a in subresult.Overrides)
				{
					Overrides.Add(a);
				}
			}

			if (null != subresult.Extensions)
			{
				if (null == Extensions)
				{
					Extensions = new List<ObjectXmlClass>();
				}
				foreach (ObjectXmlClass a in subresult.Extensions)
				{
					Extensions.Add(a);
				}
			}
		}

		private IDictionary<string, ObjectXmlClass> _resolveclassCache = new Dictionary<string, ObjectXmlClass>();
		/// <summary>
		/// Разрешает класс по коду и заявленному пространству имен
		/// </summary>
		/// <param name="code"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		public  ObjectXmlClass ResolveClass( string code, string ns) {
			var key = ns + "." + code;
			if (_resolveclassCache.ContainsKey(key)) {
				return _resolveclassCache[key];
			}
			ObjectXmlClass import = null;
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
		/// Строит рабочий индекс классов
		/// </summary>
		public void Build() {
			Overrides = RawClasses.Values.Where(_ => _.IsClassOverride).OrderBy(_=>_.Source.Attr("priority").ToInt()).ToList();
			Extensions = RawClasses.Values.Where(_ => _.IsClassExtension).OrderBy(_ => _.Source.Attr("priority").ToInt()).ToList();
			ApplyOverrides();
			ApplyExtensions();
			ResolveOrphans();
			Orphans = RawClasses.Values.Where(_ => _.DetectIfIsOrphaned()).ToList();
			Abstracts = RawClasses.Values.Where(_ => _.Abstract && !_.DetectIfIsOrphaned()).ToList();
			Working = RawClasses.Values.Where(_ => !_.IsClassExtension && !_.IsClassOverride && !_.Abstract && !_.DetectIfIsOrphaned()).ToList();
			Static = RawClasses.Values.Where(_ => _.Static && !_.DetectIfIsOrphaned()).ToList();
			ResolveImports();
		}

		private void ApplyExtensions() {
			foreach (var o in Extensions) {
				var cls = ResolveClass(o.TargetClassName, o.Namespace);
				if (null == cls) {
					o.IsClassExtension = false;
					o.Name = o.TargetClassName;
				}
				else
				{
					ApplyExtension(o, cls);
				}
			}
		}

		private void ApplyExtension(ObjectXmlClass src, ObjectXmlClass trg) {
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
				var cls = ResolveClass(o.TargetClassName, o.Namespace);
				if (null == cls) {
					o.IsClassOverride = false;
					o.Name = o.TargetClassName;
				}
				else {
					ApplyOverride(o, cls);
				}
			}
		}

		private void ApplyOverride(ObjectXmlClass src, ObjectXmlClass trg) {
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
				foreach (ObjectXmlImport i in w.Imports)
				{
					i.Orphaned = true;
					var import = ResolveClass(i.TargetCode, w.Namespace);
					if (null != import)
					{
						i.Orphaned = false;
						i.Target = import;
					}
				}
			}
		}


		private void ResolveOrphans() {
			IEnumerable<ObjectXmlClass> _initiallyorphaned = RawClasses.Values.Where(_ => _.Orphaned);
			foreach (var o in _initiallyorphaned) {
				string code = o.DefaultImportCode;
				string ns = o.Namespace;
				var import = ResolveClass(code, ns);
				if (import != null) {
					o.Orphaned = false;
					o.DefaultImport = import;
				}
			}
		}
	}
}