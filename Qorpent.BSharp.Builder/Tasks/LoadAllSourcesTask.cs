using System.IO;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Bxl;
using System;
using Qorpent.Integration.BSharp.Builder.Helpers;

namespace Qorpent.Integration.BSharp.Builder.Tasks {
	/// <summary>
	/// Загружает исходный код в проект
	/// </summary>
	public class LoadAllSourcesTask : BSharpBuilderTaskBase {
	    /// <summary>
		/// Загружет исходный код классов
		/// </summary>
		public LoadAllSourcesTask() {
			Phase = BSharpBuilderPhase.PreProcess;
			Index = TaskConstants.LoadAllSourcesTaskIndex;
		}

		/// <summary>
		/// Выполнение цели
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(IBSharpContext context) {
			
            LoadBxlSources(context);
            LoadJsonSources(context);
			Project.Log.Info("added "+Project.Sources.Count+" source files");
		}

        private void LoadJsonSources(IBSharpContext context) {
            var jsonparser = new Json.JsonParser();
            foreach (var file in Directory.GetFiles(Project.GetRootDirectory(), "*.bxls.json", SearchOption.AllDirectories)) {
                try {
                    var xml = jsonparser.ParseXml(File.ReadAllText(file));
                    ConvertToBSharpSourceXml(file, xml);
                    Project.Sources.Add(xml);
                    Project.Log.Debug("add src from " + file);
                } catch (Exception ex) {
                    context.RegisterError(BSharpErrors.Generic(ex));
                    Project.Log.Fatal("cannot load src from " + file + " : " + ex.Message);
                }

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="xml"></param>
        private void ConvertToBSharpSourceXml(string file, System.Xml.Linq.XElement xml) {
            throw new NotImplementedException();
        }

        private void LoadBxlSources(IBSharpContext context) {
			var bxlparser = new BxlParser();
            foreach (var file in DirectoryFileFilter.Create(Project).Collect()) {
                try {
                    var xml = bxlparser.Parse(null, file);
                    Project.Sources.Add(xml);
                    Project.Log.Debug("add src from " + file);
                } catch (Exception ex) {
                    context.RegisterError(BSharpErrors.Generic(ex));
                    Project.Log.Fatal("cannot load src from " + file + " : " + ex.Message);
                }

            }
        }
	}
}