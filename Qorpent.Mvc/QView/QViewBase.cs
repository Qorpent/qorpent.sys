#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Mvc/QViewBase.cs
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		protected QViewBase() {
			InitializeResources();
			
		}

		private void InitializeResources() {
			Resources = _getResources();
			lock (_getResourceLock())
			{
				if (!_getResourceLoaded())
				{
					LoadFromResourceFiles();
					buildResources();
					buildResourcesAdvanced();
					_setResourceLoaded();
				}
			}
		}

#if PARANOID
		static QViewBase() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif

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
			string ext;
			if (name.StartsWith("res:")) {
				//resource Request
				prepared = true;
				var urlp = name.Substring(4);
				var a = urlp.Split('/')[0];
				var r = urlp.Split('/')[1];
				ext = Path.GetExtension(r);
				url = "/" + Application.ApplicationName + "/_sys/getresource.filedesc.qweb?a=" + a + "&amp;r=" + r;
				url = url.Replace("//", "/");
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
				writef("\r\n<!-- lost link to {0} no resource found on server -->\r\n", name);
			}
			else if (ext.ToUpper() == ".JS") {
				writef("\r\n<script type='text/javascript' src='{0}'></script>\r\n", url);
			}
			else if (ext.ToUpper() == ".CSS") {
				writef("\r\n<link rel='stylesheet' href='{0}' />\r\n", url);
			}
			else {
				writef("\r\n<link type='text/{0}' href='{1}' />\r\n", ext.Substring(1), url);
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
			Debug.Assert(!string.IsNullOrWhiteSpace(lang), "lang != null");
			lang = lang.Split('-')[0].ToLower();

			var key = lang + "_" + name;
			if (Resources.ContainsKey(key)) {
				return Resources[key];
			}
			key = "default_" + name;
			if (Resources.ContainsKey(key)) {
				return Resources[key];
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
// ReSharper disable InconsistentNaming
		protected virtual void _setResourceLoaded() {}
// ReSharper restore InconsistentNaming

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
// ReSharper disable InconsistentNaming
		protected virtual IDictionary<string, string> _getResources() {
// ReSharper restore InconsistentNaming
			return new Dictionary<string, string>();
		}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
// ReSharper disable InconsistentNaming
		protected virtual object _getResourceLock() {
// ReSharper restore InconsistentNaming
			return GetType();
		}

		/// <summary>
		/// 	INTERNAL USAGE: OVERRIDET INTERNALLY IN VBXL
		/// </summary>
		/// <returns> </returns>
// ReSharper disable InconsistentNaming
		protected virtual bool _getResourceLoaded() {
// ReSharper restore InconsistentNaming
			return false;
		}

		/// <summary>
		/// </summary>
		protected void LoadFromResourceFiles() {
			var ass = GetType().Assembly;
			var resources =
				ass.GetManifestResourceNames().Where(x => x.Contains(GetType().Name)).OrderBy(x => x).ToArray();
			foreach (var resource in resources.Where(x => x.EndsWith("resb"))) {
				var deflang = Regex.Match(resource, @"\.(\d+_)?(\w\w)_*resb").Groups[2].Value;
				if (deflang.IsEmpty()) {
					deflang = QorpentConst.DefaultLanguage;
				}
				using (var s = ass.GetManifestResourceStream(resource)) {
					Debug.Assert(s != null, "s != null");
					var content = new StreamReader(s).ReadToEnd();
					var x = ResolveService<IBxlParser>().Parse(content, resource, BxlParserOptions.NoLexData);
					foreach (var e in x.Elements()) {
						var name = e.Name.LocalName;
						foreach (var a in e.Attributes()) {
							AddResource(name, a.Name.LocalName == "code" ? deflang : a.Name.LocalName, a.Value);
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
		protected void AddResource(string name, string lang, string value) {
			string resourceName = lang.ToLower() + "_" + name;
			Resources[resourceName] = value;
			if (!_resourcenames.Contains(name)) {
				Resources["default_" + name] = value;
				_resourcenames.Add(name);
			}
		}

		/// <summary>
		/// </summary>
// ReSharper disable InconsistentNaming
		protected virtual void buildResources() {
// ReSharper restore InconsistentNaming
			//in VBXL
		}

		/// <summary>
		/// </summary>
// ReSharper disable InconsistentNaming
		protected virtual void buildResourcesAdvanced() // in codebehind
// ReSharper restore InconsistentNaming
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
			_mainout = ViewContext.Output;
		}


		/// <summary>
		/// </summary>
		/// <param name="s"> </param>
		/// <returns> </returns>
// ReSharper disable InconsistentNaming
		protected string esc(object s) {
// ReSharper restore InconsistentNaming
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
// ReSharper disable InconsistentNaming
		protected bool inroles(params string[] roles) {
// ReSharper restore InconsistentNaming
			return roles.Any(role => inrole(role));
		}

		/// <summary>
		/// 	allows to catch content in temporal stream
		/// </summary>
		public override void EnterTemporaryOutput(TextWriter output = null) {
			_tempout = output ?? new StringWriter();
			_mainout = ViewContext.Output;
			ViewContext.Output = _tempout;
		}


		/// <summary>
		/// 	retrieves catched content
		/// </summary>
		/// <returns> </returns>
		public override string GetTemporaryOutput() {
			RestoreOutput();
			return _tempout.ToString();
		}

		/// <summary>
		/// 	восстанавливает стандартный оутпут
		/// </summary>
		/// <returns> </returns>
		public override void RestoreOutput() {
			ViewContext.Output = _mainout;
		}

		/// <summary>
		/// 	shortcut to roles system
		/// </summary>
		/// <param name="role"> </param>
		/// <param name="usr"> </param>
		/// <param name="exact"> </param>
		/// <returns> </returns>
// ReSharper disable InconsistentNaming
		protected bool inrole(string role, string usr = null, bool exact = false) {
// ReSharper restore InconsistentNaming
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
// ReSharper disable InconsistentNaming
		protected bool asbool(object obj) {
// ReSharper restore InconsistentNaming
			return obj.ToBool();
		}

		/// <summary>
		/// </summary>
		/// <param name="str"> </param>
		/// <param name="parameters"> </param>
// ReSharper disable InconsistentNaming
		protected void writef(string str, params object[] parameters) {
// ReSharper restore InconsistentNaming
			write(string.Format(str, parameters));
		}

		/// <summary>
		/// </summary>
		/// <param name="s1"> </param>
// ReSharper disable InconsistentNaming
		protected void write(string s1) {
// ReSharper restore InconsistentNaming
			ViewContext.Output.Write(s1);
		}

		/// <summary>
		/// </summary>
		/// <param name="s1"> </param>
		/// <param name="s2"> </param>
// ReSharper disable InconsistentNaming
		protected void write(string s1, string s2) {
// ReSharper restore InconsistentNaming
			ViewContext.Output.Write(s1);
			ViewContext.Output.Write(s2);
		}

		/// <summary>
		/// </summary>
		/// <param name="s1"> </param>
		/// <param name="s2"> </param>
		/// <param name="s3"> </param>
// ReSharper disable InconsistentNaming
		protected void write(string s1, string s2, string s3) {
// ReSharper restore InconsistentNaming
			ViewContext.Output.Write(s1);
			ViewContext.Output.Write(s2);
			ViewContext.Output.Write(s3);
		}

		/// <summary>
		/// </summary>
		/// <param name="data"> </param>
// ReSharper disable InconsistentNaming
		protected void write(params object[] data) {
// ReSharper restore InconsistentNaming
			if (null != data) {
				foreach (var o in data) {
					ViewContext.Output.Write(o.ToStr());
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="data"> </param>
// ReSharper disable InconsistentNaming
		protected void write(params string[] data) {
// ReSharper restore InconsistentNaming
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
		private TextWriter _mainout;

		private TextWriter _tempout;
	}
}