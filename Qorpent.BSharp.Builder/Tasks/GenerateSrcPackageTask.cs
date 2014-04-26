using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;

namespace Qorpent.Integration.BSharp.Builder.Tasks {
    /// <summary>
	/// Задача генерации переносимого пакета исходного кода
	/// </summary>
	public class GenerateSrcPackageTask  : GeneratePackageTaskBase {
        /// <summary>
        /// Возвращает имя файла пакета
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        protected override string GetInitialPkgName(IBSharpProject project) {
            return project.SrcPkgName;
        }

        /// <summary>
        /// Возвращает тип класса для запроса из контекста
        /// </summary>
        /// <returns></returns>
        protected override BSharpContextDataType GetContextClassType() {
            return BSharpContextDataType.SrcPkg;
        }

        /// <summary>
        /// Создает запись для класса
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override XElement CreateClassRecord(IBSharpClass cls, IBSharpContext context) {
			
			var result = GetIndexElement(cls);
			if (cls.AllImports.Any()) {
				var imports = new XElement("Imports");
				result.Add(imports);
				foreach (var i in cls.AllImports) {
					imports.Add(new XElement("Import", new XAttribute("code", i.FullName)));
				}
			}
			if (cls.AllElements.Any())
			{
				var elements = new XElement("Elements");
				result.Add(elements);
				foreach (var i in cls.AllElements)
				{
					elements.Add(new XElement("Element", new XAttribute("code", i.Name), 
						new XAttribute("target",i.TargetName??""),
						new XAttribute("type",i.Type)));
				}
			}
			var src = new XElement(cls.Source) {Name = "Source"};
			result.Add(src);
			return result;
		}

        /// <summary>
        /// Получить исходный XML для упаковки
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        protected override XElement GetXmlForPackaging(IBSharpClass cls) {
            return cls.Source;
        }
    }
}