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
// PROJECT ORIGIN: Qorpent.Core/QViewInvariantBase.cs
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.IO;
using Qorpent.Security;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Abstract qview with invariant behavior - u have to inherit it even on main abstract base of view
	/// </summary>
	public abstract class QViewInvariantBase : ServiceBase, IQViewExtended {
		/// <summary>
		/// 	Создает вид и предоставляет ссылку на исходный файл
		/// </summary>
		protected QViewInvariantBase() {
			var attr =
				GetType().GetCustomAttributes(typeof (QViewAttribute), true).OfType<QViewAttribute>().FirstOrDefault();
			if (null != attr) {
				SourceFileName = attr.Filename;
				if (!string.IsNullOrEmpty(SourceFileName)) {
					var directoryName = Path.GetDirectoryName(SourceFileName);
					if (directoryName != null) {
						var mydir = directoryName.Replace("\\", "/").ToLower();
						mydir = Regex.Match(mydir, @"\w+/views/[\s\S]+$").Value;
						if (mydir.StartsWith("sys") || mydir.StartsWith("usr") || mydir.StartsWith("mod")) {
							mydir = "~/" + mydir;
						}
						else {
							mydir = "~" + Regex.Match(mydir, @"/views/[\s\S]+$").Value;
						}
						BaseDir = (mydir + "/").Replace("//", "/");
					}
				}
			}
		}

		/// <summary>
		/// 	Ссылка на исходный файл
		/// </summary>
		protected string SourceFileName { get; set; }


		/// <summary>
		/// 	Базовая папка для резольвера
		/// </summary>
		protected string BaseDir { get; set; }

		/// <summary>
		/// </summary>
		protected IQViewContext ViewContext { get; set; }

		/// <summary>
		/// 	Executes view
		/// </summary>
		public void Process(IQViewContext vctx) {
			try {
				SetViewContext(vctx);
				if (UseDirectLayout) {
					ViewContext.Output = ViewContext.RealOutPut;
					ViewContext.ChildContext.Output = ViewContext.RealOutPut;
				}
				Log.Trace("start view " + ViewContext.Name, this);

				Prepare();
				OutBeforeMainRender();
				Render();
				OutAfterMainRender();

				if (ViewContext.IsLayout && !UseDirectLayout) {
					WriteOutLayout();
				}

				Log.Trace("end view " + ViewContext.Name, this);
			}
			catch (Exception ex) {
				Log.Error("error in view " + ViewContext.Name, ex);
				OnError(ex);
				if (ViewContext.OutputErrors) {
					WriteOutError(ex);
				}
				else {
					throw;
				}
			}
			finally {
				Finish();
			}
		}

		/// <summary>
		/// 	allows to catch content in temporal stream
		/// </summary>
		public abstract void EnterTemporaryOutput(TextWriter output = null);

		/// <summary>
		/// 	retrieves catched content
		/// </summary>
		/// <returns> </returns>
		public abstract string GetTemporaryOutput();

		/// <summary>
		/// 	восстанавливает стандартный оутпут
		/// </summary>
		/// <returns> </returns>
		public abstract void RestoreOutput();

		/// <summary>
		/// 	Renders local url to named resource with file resolution
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="prepared"> True - если ссылка полностью подготовлена </param>
		/// <exception cref="NullReferenceException"></exception>
		public abstract void RenderLink(string name, bool prepared = false);

		/// <summary>
		/// 	Retrieves resource string from special-formed resources
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="lang"> </param>
		/// <returns> </returns>
		public abstract string GetResource(string name, string lang = null);

		/// <summary>
		/// 	Общий код для регистратора ошибок
		/// </summary>
		/// <returns> </returns>
		protected override string GetLoggerNameSuffix() {
			return "MvcQView";
		}

		/// <summary>
		/// 	Запрашивает наличие привязки к ресурсу, ресурс должен быть локальным по отношению к виду, либо начинаться с "/","~" ".." не допускаются
		/// </summary>
		/// <param name="resource"> </param>
		protected virtual void Require(string resource) {
			if (ViewContext.IsLayout || ViewContext.ParentContext != null) {
				if(resource.StartsWith("res:")) {
					var resourceName = GetExternalResourceLocalUrl(resource);
					if (!string.IsNullOrEmpty(resourceName)) {
						ViewContext.Require(resourceName);
					}
				}else {
					ViewContext.Require(resource);
				}
			}
		}

		/// <summary>
		/// 	Метод определения ресурса по пути
		/// </summary>
		/// <param name="resource"> </param>
		protected virtual string GetExternalResourceLocalUrl(string resource) {
			if (resource.StartsWith("res:")) {
				return resource;
			}
			if (resource.Contains("..")) {
				throw new QorpentSecurityException("В видах запрещены пути вида ../");
			}
			if (resource.StartsWith("/") || resource.StartsWith("~")) {
				return Application.Files.Resolve(FileSearchQuery.Leveled(resource, true, FileSearchResultType.LocalUrl));
			}
			if (string.IsNullOrEmpty(BaseDir)) {
				return ""; //отсуствует локальный контекст
			}
			var filetofind = BaseDir + resource;
			return Application.Files.Resolve(FileSearchQuery.Leveled(filetofind, true, FileSearchResultType.LocalUrl));
		}

		/// <summary>
		/// 	Поддержка буферизованногов вывода Layout (для обеспечения Require и т.д.)
		/// </summary>
		protected virtual void WriteOutLayout() {
			var content = ViewContext.Output.ToString();
			content = PostProcessLayout(content);
			ViewContext.RealOutPut.Write(content);
		}

		/// <summary>
		/// 	Поддержка пост-процессора для Layout (обычные виды идут без буфера)
		/// </summary>
		/// <param name="content"> </param>
		/// <returns> </returns>
		protected string PostProcessLayout(string content) {
			if (content.Contains("<!-- __REQUIREMENTS__ -->") && ViewContext.Requirements != null) {
				content = PostProcessRequirements(content);
			}
			return content;
		}

		/// <summary>
		/// 	Выполняет вставку ссылок на ресурсы
		/// </summary>
		/// <param name="content"> </param>
		/// <returns> </returns>
		protected string PostProcessRequirements(string content) {
			var output = new StringWriter();
			EnterTemporaryOutput(output);
			try {
				foreach (var x in ViewContext.Requirements) {
					RenderLink(x, x.StartsWith("res:"));
				}
			}
			finally {
				RestoreOutput();
			}
			var result = content.Replace("<!-- __REQUIREMENTS__ -->", output.ToString());
			return result;
		}


		/// <summary>
		/// 	Prepare view to be processed
		/// </summary>
		/// <param name="vctx"> </param>
		public void SetViewContext(IQViewContext vctx) {
			ViewContext = vctx;
			Bind(vctx);
			CustomSetViewContext();
		}

		/// <summary>
		/// 	bind logic explained here
		/// </summary>
		/// <param name="vctx"> </param>
		protected virtual void Bind(IQViewContext vctx) {
			var binder = GetBinder();
			if (null != binder) {
				binder.Apply(vctx, this);
			}
		}

		/// <summary>
		/// </summary>
		protected virtual void CustomSetViewContext() {}

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		protected IQViewBinder GetBinder() {
			if (null == _binder) {
				_binder = ResolveService<IQViewBinder>();
				if (null != _binder) {
					_binder.SetView(this);
				}
			}
			return _binder;
		}

		private void WriteOutError(Exception ex) {
			var output = ViewContext.IsLayout ? ViewContext.RealOutPut : ViewContext.Output;
			output.WriteLine("<div class='qview_error'>Error in " + ViewContext.Name + " view : " +
			                 ex.ToString().Replace("<", "&lt;").Replace("\n", "").Replace("\r", "<br/>") + "</div>");
		}

		/// <summary>
		/// 	implement to do something on error occured
		/// </summary>
		/// <param name="exception"> </param>
		protected virtual void OnError(Exception exception) {}

		/// <summary>
		/// 	u must implement it - main user render is executed here
		/// </summary>
		protected abstract void Render();

		/// <summary>
		/// 	implement to perform system-defined rendering after user render phase
		/// </summary>
		protected virtual void OutAfterMainRender() {}

		/// <summary>
		/// 	implement to perform system-defined rendering before user render phase
		/// </summary>
		protected virtual void OutBeforeMainRender() {}

		/// <summary>
		/// 	implement something after processing
		/// </summary>
		protected virtual void Finish() {}

		/// <summary>
		/// 	implement to do something in initial phase of processing
		/// </summary>
		protected virtual void Prepare() {}

		/// <summary>
		/// 	Executes subview
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="advanceddata"> </param>
		public void Subview(string name, object advanceddata = null) {
			var ctx = ViewContext.CreateSubviewContext(name, advanceddata);
			ctx.ParentView = this;
			OutView(ctx);
		}

		/// <summary>
		/// 	write out a view from current view
		/// </summary>
		/// <param name="ctx"> </param>
		protected void OutView(IQViewContext ctx) {
			var view = ViewContext.Factory.GetView(ctx.Name);
			try {
				view.Process(ctx);
			}
			finally {
				ViewContext.Factory.ReleaseView(ctx.Name, view);
			}
		}

		/// <summary>
		/// 	Can be used in layout to render main content
		/// </summary>
		public void RenderChild() {
			OutView(ViewContext.ChildContext);
		}

		/// <summary>
		/// 	Подготовленный реестр ресурсов
		/// </summary>
		protected IDictionary<string, string> Resources;

		/// <summary>
		/// 	Mark to avoid buferrized Layout with post process
		/// </summary>
		protected bool UseDirectLayout;

		private IQViewBinder _binder;
	}
}