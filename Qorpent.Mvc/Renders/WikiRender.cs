using System;
using System.Linq;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Wiki;

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// Отрисовывает строчный контент в виде Wiki
	/// </summary>
	[Render("wiki")]
	public class WikiRender : RenderBase {
		/// <summary>
		/// Объект трансформации WIKI в HTML
		/// </summary>
		[Inject]
		public IWikiSerializer WikiSerializer { get; set; }
		/// <summary>
		/// Объект работы с хранилищем WIKI
		/// </summary>
		[Inject]
		public IWikiSource WikiSource { get; set; }


		/// <summary>
		/// 	Renders given context
		/// </summary>
		/// <param name="context"> </param>
		public override void Render(IMvcContext context) {
			var standalone = context.Get("standalone", true);
			var usage = context.Get("usage", "default");
			var onserver = context.Get("onserver", false);
			var page = GetWikiPage(context);
			Render(page,usage,standalone,onserver,context);
		}

		private WikiPage GetWikiPage(IMvcContext context) {
			WikiPage page = null;
			if (context.ActionResult is WikiPage) {
				page = context.ActionResult as WikiPage;
			}
			else if (context.ActionResult is string) {
				page = new WikiPage {Text = context.ActionResult as string};
			}
			else {
				var code = context.Get("code", "");
				if (!string.IsNullOrWhiteSpace(code)) {
					page = WikiSource.Get(code).First();
				}
				else {
					throw new Exception("cannot determine wiki to be rendered");
				}
			}
			return page;
		}

		private void Render(WikiPage page, string usage, bool standalone, bool onserver,IMvcContext context) {
			context.ContentType = MimeHelper.HTML;
			if (standalone) {
				RenderStandalone(page, usage, onserver,context);
			}
			else {
				RenderEmbeded(page, usage, context);
			}
		}

		private void RenderStandalone(WikiPage page, string usage, bool onserver, IMvcContext context) {
			if (onserver) {
				RenderStandaloneServerProcessedPage(page,usage,context);
			}
			else {
				RenderStandaloneClientProcessedPage(page,usage,context);
			}
		}
		/// <summary>
		/// Адрес CSS WIKI для разрисовки по умолчанию
		/// </summary>
		public  string DefaultWikiCssReference = "./styles/qorpent.wiki.css";
		/// <summary>
		/// Адрес JS для разрисовки WIKI по умолчанию
		/// </summary>
		public  string DefaultWikiJsReference = "./styles/qorpent.wiki.css";

		/// <summary>
		/// Шаблон отдельной Wiki страницы с серверной отрисовкой
		/// </summary>
		public  string StandaloneServerBasedTemplate = @"
<DOCTYPE html>
<html>
	<head>
		<link rel='stylesheet' src='{0}' />
	</head>
	<body>
		<h1 class='wiki-title'>{1}</h1>
		<div class='wiki-info'>
			Последняя редакция: {2} , {3}
		</div>
		<div class='wiki-content'>
		{4}
		</div>
	</body>
</html>
		";
		/// <summary>
		/// Шаблон отдельной Wiki страницы с клиентской отрисовкой
		/// </summary>

		public  string StandaloneClientBasedTemplate = @"
<DOCTYPE html>
<html>
	<head>
		<link rel='stylesheet' type='text/css' src='{0}' />
		<script href='{1}' type='text/javascript' ></script>
		<script type='text/javascript'>
			function render(){
				document.getElementById('trg').innerHTML = qwiki.toHTML(document.getElementById('src').value);
			}
		</script>
	</head>
	<body onload='render()'>
		<h1 class='wiki-title'>{2}</h1>
		<div class='wiki-info'>
			Последняя редакция: {3} , {4}
		</div>
		<textarea id='src' style='display:none'>
			{5}
		</textarea>
		<div id='trg' class='wiki-content'>

		</div>
	</body>
</html>
		";

		private void RenderStandaloneClientProcessedPage(WikiPage page, string usage, IMvcContext context) {
			context.Output.Write(
					string.Format(StandaloneClientBasedTemplate,
						DefaultWikiCssReference,
						DefaultWikiJsReference,
						page.Title,
						page.Editor,
						page.LastWriteTime.ToString("dd.MM.yyyy hh:mm:ss"),
						page.Text));
		}

		private void RenderStandaloneServerProcessedPage(WikiPage page, string usage, IMvcContext context) {
				context.Output.Write(
					string.Format(StandaloneServerBasedTemplate, 
						DefaultWikiCssReference, 
						page.Title, 
						page.Editor,
				        page.LastWriteTime.ToString("dd.MM.yyyy hh:mm:ss"),
						WikiSerializer.ToHTML(usage,page,context)));
			
		}


		private void RenderEmbeded(WikiPage page, string usage, IMvcContext context) {
			context.Output.Write(WikiSerializer.ToHTML(usage,page,context));
		}

		/// <summary>
		/// 	Renders error, occured in given context
		/// </summary>
		/// <param name="error"> </param>
		/// <param name="context"> </param>
		public override void RenderError(Exception error, IMvcContext context) {
			context.ContentType = MimeHelper.HTML;
			context.Output.Write( "<html><head></head><body><h1>Error while try render WIKI</h1>"
			+"<h2>"+error.Message+"</h2><div class='error'>"+error.ToString().Replace(Environment.NewLine,"<br/>").Replace("\t","&nbsp;&nbsp;&nbsp;&nbsp;")+"</div>"
			+"</body></html>"
			);
		}
	}
}