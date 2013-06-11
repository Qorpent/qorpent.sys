using System;
using System.Linq;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Wiki;

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// ������������ �������� ������� � ���� Wiki
	/// </summary>
	[Render("wiki")]
	public class WikiRender : RenderBase {
		/// <summary>
		/// ������ ��� ������� 304
		/// </summary>
		public const int WikiRenderVersion = 1;

		/// <summary>
		/// ������ ���� ���-������� ��� ������������� ����.
		/// </summary>
		/// <returns>
		/// ���-��� ��� �������� ������� <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode() {

			return WikiRenderVersion*10000 + WikiSerializer.GetHashCode()*100 + WikiSource.GetHashCode() *10;
		}
		/// <summary>
		/// ������ ������������� WIKI � HTML
		/// </summary>
		[Inject]
		public IWikiSerializer WikiSerializer { get; set; }
		/// <summary>
		/// ������ ������ � ���������� WIKI
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
			var onserver = context.Get("onserver", true);
			var page = GetWikiPage(context);
			Render(page,usage,standalone,onserver,context);
		}

		private WikiPage GetWikiPage(IMvcContext context) {
			WikiPage page = null;
			if (context.ActionResult is WikiPage) {
				page = context.ActionResult as WikiPage;
			}
			else if (context.ActionResult is WikiPage[]) {
				page = (context.ActionResult as WikiPage[])[0];
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
		/// ����� CSS WIKI ��� ���������� �� ���������
		/// </summary>
		public  string DefaultWikiCssReference = "/styles/qorpent.wiki.css";
		/// <summary>
		/// ����� JS ��� ���������� WIKI �� ���������
		/// </summary>
		public  string DefaultWikiJsReference = "/scripts/qorpent.wiki.js";

		/// <summary>
		/// ������ ��������� Wiki �������� � ��������� ����������
		/// </summary>
		public  string StandaloneServerBasedTemplate = @"
<!DOCTYPE html>
<html>
	<head>
		<link rel='stylesheet' href='{0}' type='text/css' >
	</head>
	<body>
		<h1 class='wiki-title'>{1}</h1>
		<div class='wiki-info'>
			��������� ��������: {2} , {3}
		</div>
		<div class='wiki'>
		{4}
		</div>
	</body>
</html>
		";
		/// <summary>
		/// ������ ��������� Wiki �������� � ���������� ����������
		/// </summary>

		public  string StandaloneClientBasedTemplate = @"
<!DOCTYPE html>
<html>
	<head>
		<link rel='stylesheet' href='{0}' type='text/css'  >
		<script src='{1}' type='text/javascript' ></script>
		<script type='text/javascript'>
			function render(){{
				document.getElementById('trg').innerHTML = qwiki.toHTML(document.getElementById('src').value);
			}}
		</script>
	</head>
	<body onload='render()'>
		<h1 class='wiki-title'>{2}</h1>
		<div class='wiki-info'>
			��������� ��������: {3} , {4}
		</div>
		<textarea id='src' style='display:none'>
			{5}
		</textarea>
		<div id='trg' class='wiki'>

		</div>
	</body>
</html>
		";

		private void RenderStandaloneClientProcessedPage(WikiPage page, string usage, IMvcContext context) {
			context.Output.Write(
					string.Format(StandaloneClientBasedTemplate,
						("/"+Application.ApplicationName+"/"+DefaultWikiCssReference).Replace("//","/"), 
						("/"+Application.ApplicationName+"/"+DefaultWikiJsReference).Replace("//","/"), 
						page.Title,
						page.Editor,
						page.LastWriteTime.ToString("dd.MM.yyyy hh:mm:ss"),
						page.Text));
		}

		private void RenderStandaloneServerProcessedPage(WikiPage page, string usage, IMvcContext context) {
				context.Output.Write(
					string.Format(StandaloneServerBasedTemplate, 
						("/"+Application.ApplicationName+"/"+DefaultWikiCssReference).Replace("//","/"), 
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