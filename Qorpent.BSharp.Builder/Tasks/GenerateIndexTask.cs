using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;

namespace Qorpent.Integration.BSharp.Builder.Tasks {
	/// <summary>
    ///     Класс для генерации индекса классов
    /// </summary>
    public class GenerateIndexTask : BSharpBuilderTaskBase {
	    /// <summary>
        ///     Перечисление целей, записанных на диск, по которым нужно строить индекс
        /// </summary>
        public IList<BSharpBuilderWriteTarget> WroteTargets { get; set; }
        /// <summary>
        ///     Менеджер записи на диск
        /// </summary>
        public BSharpBuilderWriteManager WriteManager { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public GenerateIndexTask() {
            Phase = BSharpBuilderPhase.PostProcess;
            Index = TaskConstants.GenerateIndexTaskIndex;
        }
        /// <summary>
        ///     Запуск задачи на выполнение
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(IBSharpContext context) {
			BSharpBuilderWriteManager.Join();
			
	        WroteTargets = Project.Get<IList<BSharpBuilderWriteTarget>>("WroteTargets");
            Project.Log.Info("GenerateIndexTask");
            PrepareWorkEnviroment();
			
            PrepareIndexesForWriting();
            WriteManager.Roll();
        }
        /// <summary>
        /// 
        /// </summary>
        private void PrepareIndexesForWriting() {
			if (null == WroteTargets){
				Project.Log.Error("Cannot find wrote targets");
				return;
			}
            foreach (var el in WroteTargets) {
                HandleTarget(el);
            }
        }
        private void HandleTarget(BSharpBuilderWriteTarget target) {
            foreach (var bsClass in target.Entity.Elements()) {
                var bSharpClass = new XElement(BSharpBuilderDefaults.BSharpClassContainerName);
                var bSharpIndexSet = new XElement(
                    BSharpBuilderDefaults.DefaultClassIndexContainerName,
                    bSharpClass
                );

                var writeTarget = new BSharpBuilderWriteTarget {
                    Directory = Project.GetOutputDirectory(),
                    Entity = bSharpIndexSet,
                    EntityContainerName = BSharpBuilderDefaults.DefaultClassIndexContainerName,
                    Extension = BSharpBuilderDefaults.IndexFileExtension,
                    Filename = BSharpBuilderDefaults.IndexFileName,
                    MergeIfExists = true
                };

                WriteIndexAttributeIfExists(bSharpClass, bsClass,BSharpSyntax.ClassNameAttribute);
                WriteIndexAttributeIfExists(bSharpClass, bsClass, BSharpSyntax.ClassPrototypeAttribute);
                WriteIndexAttributeIfExists(bSharpClass, bsClass, BSharpSyntax.ClassRuntimeAttribute);

                bSharpClass.SetAttributeValue("file", target.Path.Remove(0, Project.GetOutputDirectory().Length).Replace("\\","/"));
				if (bsClass.Attribute(BSharpSyntax.ClassFullNameAttribute) != null)
				{
					bSharpClass.SetAttributeValue(BSharpSyntax.Namespace, BSharpBuilderClassUtils.GetNamespace(bsClass.Attribute(BSharpSyntax.ClassFullNameAttribute).Value));
                }

                WriteManager.Add(writeTarget);
            }
        }
        /// <summary>
        ///     Вытаскивает из IBSharpClass атрибут (если существует)
        ///     с именем name и записывает его в
        ///     качестве атрибута в класс-индекс
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="name"></param>
        private void WriteIndexAttributeIfExists(XElement target, XElement source, string name) {
            if (source.Attribute(name) != null) {
                target.SetAttributeValue(name, source.Attribute(name).Value);
            }
        }
        /// <summary>
        ///     Подготавливает рабочую среду
        /// </summary>
        private void PrepareWorkEnviroment() {
            WriteManager = new BSharpBuilderWriteManager {
                Log = Project.Log
            };
        }
    }
}
