#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : QViewBase.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.IO;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Default base for IQView - extends invariant behavior with usefull features (resources, some shortcuts)
	/// </summary>
	public abstract class QViewBase : QViewInvariantBase {
		/// <summary>
		/// 	При старте вызывает конструктор статических ресурсов
		/// </summary>
		public QViewBase() {
			_resources = _getResources();
			lock (_getResourceLock()) {
				if (!_getResourceLoaded()) {
					loadFromResourceFiles();
					buildResources();
					buildResourcesAdvanced();
					_setResourceLoaded();
				}
			}
		}

		/// <summary>
		/// </summary>
		protected ActionDescriptor MyAction {
			get { return ViewContext.Context.ActionDescriptor; }
		}

		/// <summary>
		/// 	Быстрый доступ к основному объекту данных
		/// </summary>
		protected object ViewData {
			get { return ViewContext.ViewData; }
		}

		/// <summary>
		/// 	Быстрый доступ к фабрике
		/// </summary>
		protected IMvcFactory Factory {
			get { return ViewContext.Factory; }
		}


		/// <summary>
		/// 	Renders local url to named resource with file resolution
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="prepared"> True - если ссылка полностью подготовлена </param>
		/// <exception cref="NullReferenceException"></exception>
		public override void RenderLink(string name, bool prepared = false) {
			var url = "";
			var ext = "";
			if (name.StartsWith("res:")) {
				//resource Request
				prepared = true;
				var urlp = name.Substring(4);
				var a = urlp.Split('/')[0];
				var r = urlp.Split('/')[1];
				ext = Path.GetExtension(r);
				url = "/" + Application.ApplicationName + "/_sys/getresource.filedesc.qweb?a=" + a + "&amp;r=" + r;
			}
			else {
				ext = Path.GetExtension(name);
			}


			if (null == ext) {
				throw new NullReferenceException("ext");
			}
			if (null == ViewContext.Context || prepared && url.IsEmpty()) {
				//embeded or autnome mode
				url = name;
			}
			else {
				if (null == ViewContext.Context) {
					throw new QorpentException("Попытка отрисовки ссылок из вида без контекста и без признака подготовки");
				}
				var res = ViewContext.Context.Application.Files;
				if (!prepared) {
					url = res.Resolve(name, pathtype: FileSearchResultType.LocalUrl);
				}
			}

			if (url.IsEmpty()) {
				writef("<!-- lost link to {0} no resource found on server -->", name);
			}
			else if (ext.ToUpper() == ".JS") {
				writef("<script type='text/javascript' src='{0}'></script>", url);
			}
			else if (ext.ToUpper() == ".CSS") {
				writef("<link rel='stylesheet' href='{0}' />", url);
			}
			else {
				writef("<link type='text/{0}' href='{1}' />", ext.Substring(1), url);
			}
		}

		/// <summary>
		/// 	Retrieves resource string from special-formed resources
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="lang"> </param>
		/// <returns> </returns>
		public override string GetResource(string name, string lang = null) {
			//if (lang == null) throw new ArgumentNullException("lang");
			lang = lang.IsNotEmpty() ? lang : GetCurrentLang();
			lang = lang.Split('-')[0].ToLower();

			var key = lang + "_" + name;
			if (_resources.ContainsKey(key)) {
				return _resources[key];
			}
			key = "default_" + name;
			if (_resources.ContainsKey(key)) {
				return _resources[key];
			}

			if (null != ViewContext.ParentView) {
				var baseview = ViewContext.ParentView as IQViewExtended;
				if (null != baseview) {
					return baseview.GetResource(name, lang);
				}
			}

			return "";
		}


		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
		protected virtual void _setResourceLoaded() {}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
		protected virtual IDictionary<string, string> _getResources() {
			return new Dictionary<string, string>();
		}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
		protected virtual object _getResourceLock() {
			return GetType();
		}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
		protected virtual bool _getResourceLoaded() {
			return false;
		}

		/// <summary>
		/// </summary>
		protected void loadFromResourceFiles() {
			var ass = GetType().Assembly;
			var resources =
				ass.GetManifestResourceNames().Where(x => x.Contains(GetType().Name)).OrderBy(x => x).ToArray();
			foreach (var resource in resources.Where(x => x.EndsWith("resb"))) {
				var deflang = Regex.Match(resource, @"\.(\d+_)?(\w\w)_*resb").Groups[2].Value;
				if (deflang.IsEmpty()) {
					deflang = QorpentConst.DefaultLanguage;
				}
				using (var s = ass.GetManifestResourceStream(resource)) {
					var content = new StreamReader(s).ReadToEnd();
					var x = ResolveService<IBxlParser>().Parse(content, resource, BxlParserOptions.NoLexData);
					foreach (var e in x.Elements()) {
						var name = e.Name.LocalName;
						foreach (var a in e.Attributes()) {
							addResource(name, a.Name.LocalName == "code" ? deflang : a.Name.LocalName, a.Value);
						}
					}
				}
			}
		}

		/// <summary>
		/// 	Динамическое добавление ресурса в коллекцию ресурсов
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="lang"> </param>
		/// <param name="value"> </param>
		protected void addResource(string name, string lang, string value) {
			var name_ = lang.ToLower() + "_" + name;
			_resources[name_] = value;
			if (!_resourcenames.Contains(name)) {
				_resources["default_" + name] = value;
				_resourcenames.Add(name);
			}
		}

		/// <summary>
		/// </summary>
		protected virtual void buildResources() {
			//in VBXL
		}

		/// <summary>
		/// </summary>
		protected virtual void buildResourcesAdvanced() // in codebehind
		{}


		/// <summary>
		/// 	Returns current user language
		/// </summary>
		/// <returns> </returns>
		public string GetCurrentLang() {
			try {
				return ViewContext.Context.Language.Substring(0, 2);
			}
			catch {
				return CultureInfo.CurrentCulture.Name;
			}
		}

		/// <summary>
		/// </summary>
		protected override void CustomSetViewContext() {
			base.CustomSetViewContext();
			mainout = ViewContext.Output;
		}


		/// <summary>
		/// </summary>
		/// <param name="s"> </param>
		/// <returns> </returns>
		protected string esc(object s) {
			return s.ToStr()
				.Replace("\"", "&quot;")
				.Replace("'", "&apos;")
				.Replace("&", "&amp;")
				.Replace("<", "&lt;")
				.Replace("<", "&gt;");
		}

		/// <summary>
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		protected object GetShared(string name) {
			return GetShared<object>(name);
		}

		/// <summary>
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <param name="name"> </param>
		/// <returns> </returns>
		protected T GetShared<T>(string name) {
			return ViewContext.GetShared<T>(name);
		}

		/// <summary>
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="value"> </param>
		protected void SetShared(string name, object value) {
			ViewContext.SetShared(name, value);
		}

		/// <summary>
		/// </summary>
		/// <param name="roles"> </param>
		/// <returns> </returns>
		protected bool inroles(params string[] roles) {
			return roles.Any(role => inrole(role));
		}

		/// <summary>
		/// 	allows to catch content in temporal stream
		/// </summary>
		public override void EnterTemporaryOutput(TextWriter output = null) {
			tempout = output ?? new StringWriter();
			mainout = ViewContext.Output;
			ViewContext.Output = tempout;
		}


		/// <summary>
		/// 	retrieves catched content
		/// </summary>
		/// <returns> </returns>
		public override string GetTemporaryOutput() {
			RestoreOutput();
			return tempout.ToString();
		}

		/// <summary>
		/// 	восстанавливает стандартный оутпут
		/// </summary>
		/// <returns> </returns>
		public override void RestoreOutput() {
			ViewContext.Output = mainout;
		}

		/// <summary>
		/// 	shortcut to roles system
		/// </summary>
		/// <param name="role"> </param>
		/// <param name="usr"> </param>
		/// <param name="exact"> </param>
		/// <returns> </returns>
		protected bool inrole(string role, string usr = null, bool exact = false) {
			var u = ViewContext.Context.Application.Principal.CurrentUser;
			if (!string.IsNullOrWhiteSpace(usr)) {
				u = new GenericPrincipal(new GenericIdentity(usr), new string[] {});
			}
			return ViewContext.Context.Application.Roles.IsInRole(u, role, exact, ViewContext.Context);
		}


		/// <summary>
		/// 	write content from resources to output
		/// </summary>
		protected override void OutBeforeMainRender() {
			var a = GetType().Assembly;
			var resources = a.GetManifestResourceNames().Where(x => x.StartsWith("_view_resource_"));
			foreach (var resource in resources) {
				using (var s = a.GetManifestResourceStream(resource)) {
					if (null == s) {
						throw new NullReferenceException("resource stream");
					}
					write(new StreamReader(s).ReadToEnd());
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="obj"> </param>
		/// <returns> </returns>
		protected bool asbool(object obj) {
			return obj.ToBool();
		}

		/// <summary>
		/// </summary>
		/// <param name="str"> </param>
		/// <param name="parameters"> </param>
		protected void writef(string str, params object[] parameters) {
			write(string.Format(str, parameters));
		}

		/// <summary>
		/// </summary>
		/// <param name="s1"> </param>
		protected void write(string s1) {
			ViewContext.Output.Write(s1);
		}

		/// <summary>
		/// </summary>
		/// <param name="s1"> </param>
		/// <param name="s2"> </param>
		protected void write(string s1, string s2) {
			ViewContext.Output.Write(s1);
			ViewContext.Output.Write(s2);
		}

		/// <summary>
		/// </summary>
		/// <param name="s1"> </param>
		/// <param name="s2"> </param>
		/// <param name="s3"> </param>
		protected void write(string s1, string s2, string s3) {
			ViewContext.Output.Write(s1);
			ViewContext.Output.Write(s2);
			ViewContext.Output.Write(s3);
		}

		/// <summary>
		/// </summary>
		/// <param name="data"> </param>
		protected void write(params object[] data) {
			if (null != data) {
				foreach (var o in data) {
					ViewContext.Output.Write(o.ToStr());
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="data"> </param>
		protected void write(params string[] data) {
			if (null != data) {
				foreach (var o in data) {
					ViewContext.Output.Write(o);
				}
			}
		}

		/// <summary>
		/// 	Выполняет дочерний вид, возвращая результат как Xhtml
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="advancedData"> </param>
		/// <returns> </returns>
		public IEnumerable<XElement> XhtmlSubview(string name, object advancedData = null) {
			EnterTemporaryOutput();
			Subview(name, advancedData);
			var result = "<root>" + GetTemporaryOutput() + "</root>";
			return XElement.Parse(result).Elements();
		}

		private readonly IList<string> _resourcenames = new List<string>();
		private TextWriter mainout;

		private TextWriter tempout;
	}
}