using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Integration.BSharp.Builder.Tasks.WriteTasks {
	/// <summary>
	/// 
	/// </summary>
    public class WriteErrorInfoTask : WriteTaskBase {
        
	    /// <summary>
		/// 
		/// </summary>
		public WriteErrorInfoTask() {
			Phase = BSharpBuilderPhase.PostProcess;
			Index = TaskConstants.WriteErrorInfoTaskIndex;
		}
		/// <summary>
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(IBSharpContext context) {
			var errors = context.GetErrors().ToArray();

            if (errors.Length == 0) {
                return;
            }

            PrepareWorkEnv();
		    var xmlstring = GetSerializedErrors(errors);
		    var xml = XElement.Parse(xmlstring);
            RollWriting(GetWritePath(), xml.ToString());
            RollWriting(GetWritePath()+".html",ConvertToErrorHtml(xml));
		}

	    private string ConvertToErrorHtml(XElement xml) {
	        var xslt = GetType().Assembly.ReadManifestResource("errors.xslt");
	        var cxslt = new XslCompiledTransform();
            cxslt.Load(XmlReader.Create(new StringReader(xslt)),XsltSettings.TrustedXslt, new XmlUrlResolver());
	        var sw = new StringWriter();
            cxslt.Transform(xml.CreateReader(),new XsltArgumentList(),sw);
	        return sw.ToString();
	    }

	    /// <summary>
        ///     Возвращает серилизованную строку из массива ошибок BSharpError
        /// </summary>
        /// <param name="errors">Массив ошибок</param>
        /// <returns>Серилизованная строка</returns>
        private string GetSerializedErrors(BSharpError[] errors) {
            return new XmlSerializer().Serialize("errors", errors);
        }
        /// <summary>
        ///     Подготовка рабочей среды
        /// </summary>
        private void PrepareWorkEnv() {
            Directory.CreateDirectory(Project.GetLogDirectory());
        }
        /// <summary>
        ///     Возвращает путь для записи файла с ошибками
        /// </summary>
        /// <returns>Путь для записи файла с ошибками</returns>
        private string GetWritePath() {
            return Path.Combine(
                Project.GetLogDirectory(),
                BSharpBuilderDefaults.ErrorsFilename
            );
        }
        /// <summary>
        ///     Производит реальную запись файла с ошибками на диск
        /// </summary>
        /// <param name="path">Путь для записи</param>
        /// <param name="entity">Контент</param>
        private void RollWriting(string path, string entity) {
            File.WriteAllText(path, entity);
            Project.Log.Trace("erorr log " + path + " wrote");
        }
	}
}