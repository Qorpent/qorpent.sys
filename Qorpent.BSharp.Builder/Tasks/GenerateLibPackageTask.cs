using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;

namespace Qorpent.Integration.BSharp.Builder.Tasks {
    /// <summary>
    /// Формирует ZIP с компилированным контентом
    /// </summary>
    public class GenerateLibPackageTask : GeneratePackageTaskBase
    {
        /// <summary>
        /// Возвращает имя файла пакета
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        protected override string GetInitialPkgName(IBSharpProject project)
        {
            return project.LibPkgName;
        }

        /// <summary>
        /// Возвращает тип класса для запроса из контекста
        /// </summary>
        /// <returns></returns>
        protected override BSharpContextDataType GetContextClassType()
        {
            return BSharpContextDataType.LibPkg;
        }

        /// <summary>
        /// Создает запись для класса
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override XElement CreateClassRecord(IBSharpClass cls, IBSharpContext context)
        {
            var result = GetIndexElement(cls);
            var src = new XElement(cls.Compiled) { Name = "Compiled" };
            result.Add(src);
            return result;
        }

        /// <summary>
        /// Получить исходный XML для упаковки
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        protected override XElement GetXmlForPackaging(IBSharpClass cls) {
            return cls.Compiled;
        }
    }
}