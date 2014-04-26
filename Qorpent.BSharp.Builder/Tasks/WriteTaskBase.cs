using System.Linq;
using System.Xml.XPath;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using System.IO;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Integration.BSharp.Builder.Tasks {
    /// <summary>
    /// 
    /// </summary>
    public abstract class WriteTaskBase : BSharpBuilderTaskBase {
        private BSharpBuilderWriteManager _writeManager;
        /// <summary>
        ///     Флаг, указывающий на тип файлов, которые обрабатывает экземпляр класса
        /// </summary>
        public BSharpBuilderOutputAttributes IncludeFlag { get; set; }
        /// <summary>
        ///     Тип данных, которые обрабатывает экземпляр класса
        /// </summary>
        public BSharpContextDataType DataType { get; set; }
        /// <summary>
        ///     Рабочая директория для данного экземпляра класса
        /// </summary>
        public string WorkingDirectory { get; set; }
        /// <summary>
        ///     Менеджер записи на диск
        /// </summary>
        public BSharpBuilderWriteManager WriteManager {
            get { return _writeManager ?? (_writeManager = new BSharpBuilderWriteManager());  }
        }
        /// <summary>
        ///     Выполнение операции в контексте
        /// </summary>
        /// <param name="context">Контекст</param>
        public override void Execute(IBSharpContext context) {
            if (!Project.OutputAttributes.HasFlag(IncludeFlag)) {
                return;
            }

            PrepareWorkEnviroment();
            RollRealWriting(context);
        }
        /// <summary>
        ///     Резольвинг процедуры сохранения исходя из флагов в OutputAttributes
        ///     Метод полностью реализует логику разделения реузльтатов компиляции
        ///     по директориям исходя из установленных флагов в OutputAttributes
        /// </summary>
        /// <param name="bSharpClass"></param>
        /// <returns></returns>
        protected virtual BSharpBuilderWriteTarget GenerateTarget(IBSharpClass bSharpClass) {
            var outputAttributes = Project.OutputAttributes;
            var target = new BSharpBuilderWriteTarget {
                Directory = WorkingDirectory,
                Extension = Project.GetOutputExtension(),
                Filename = BSharpBuilderClassUtils.GetClassname(bSharpClass.FullName),
                MergeIfExists = false
            };

            target.Entity.Add(GenerateBSharpClasset(bSharpClass.Compiled??bSharpClass.Source));
	        
            if (outputAttributes.HasFlag(BSharpBuilderOutputAttributes.TreeNamespace)) {
                target.Directory = Path.Combine(
                    WorkingDirectory,
                    BSharpBuilderClassUtils.GetRelativeDirByNamespace(bSharpClass.FullName)
                );
            }

            if (outputAttributes.HasFlag(BSharpBuilderOutputAttributes.PlainNamespace)) {
                target.Directory = Path.Combine(
                    WorkingDirectory,
                    BSharpBuilderClassUtils.GetNamespace(bSharpClass.FullName)
                );
            }

			if (outputAttributes.HasFlag(BSharpBuilderOutputAttributes.PrototypeAlign)){
				var proto = bSharpClass.Prototype;
				if (!string.IsNullOrWhiteSpace(proto)){
					target.Directory = Path.Combine(
						WorkingDirectory,
						proto
						);
				}
				else{
					target.Directory = WorkingDirectory;
				}
			}

            if (outputAttributes.HasFlag(BSharpBuilderOutputAttributes.UseFullName)) {
                target.Filename = bSharpClass.FullName;
            }

            if (outputAttributes.HasFlag(BSharpBuilderOutputAttributes.SingleFile)) {
                target.Filename = BSharpBuilderDefaults.SingleModeFilename;
                target.MergeIfExists = true;
                target.Directory = Path.Combine(
                    WorkingDirectory,
                    BSharpBuilderClassUtils.GetRelativeDirByNamespace(bSharpClass.FullName)
                );
            } else {
                target.Entity = target.Entity.XPathSelectElement("//" + BSharpBuilderDefaults.BSharpClassContainerName);
            }
		
            return target;
        }
        /// <summary>
        ///     подготовка рабочей среды
        /// </summary>
        private void PrepareWorkEnviroment() {
            WriteManager.Log = Project.Log;
        }
        /// <summary>
        ///     Произведение реальной записки скомпилированного контекста
        ///     на диск с резольвингом пути и формы записи согласно OutputAttributes
        /// </summary>
        /// <param name="context"></param>
        protected virtual void RollRealWriting(IBSharpContext context) {
            PrepareTargets(context);
            WriteManager.Roll();
        }
        /// <summary>
        ///     Подготавливает список целевых файлов с содержимым
        ///     для записи на диск
        /// </summary>
        /// <param name="context">Контекст компилятора</param>
        protected virtual void PrepareTargets(IBSharpContext context) {
			context.Get(DataType).ToArray().AsParallel().ForAll(_ =>{
				var target = GenerateTarget(_);
				WriteManager.Add(target);
			});
        }
        /// <summary>
        ///     Обёртывает класс с классет
        /// </summary>
        /// <param name="entity"></param>
        private XElement GenerateBSharpClasset(XElement entity) {
            var xElement = new XElement(BSharpBuilderDefaults.BSharpClassContainerName);
            xElement.SetAttributeValue("code", entity.ChooseAttr("fullcode","code"));
            xElement.Add(entity);

            return xElement;
        }

	    /// <summary>
	    /// Установить целевой проект
	    /// </summary>
	    /// <param name="project"></param>
	    public override void SetProject(IBSharpProject project)
		{
			base.SetProject(project);
			WorkingDirectory = project.GetOutputDirectory();
		}
    }
}
