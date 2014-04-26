using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Utils.Extensions;

namespace Qorpent.Integration.BSharp.Builder.Tasks {
    /// <summary>
    /// Базовый класс для генерации пакетов
    /// </summary>
    public abstract class GeneratePackageTaskBase : BSharpBuilderTaskBase {
        /// <summary>
        /// 
        /// </summary>
        public GeneratePackageTaskBase()
        {
            Phase = BSharpBuilderPhase.PostProcess;
            Index = TaskConstants.GenerateSrcPackageTaskIndex;
	        Async = true;
        }

        /// <summary>
        /// Корневая папка исходников
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// Имя пакета
        /// </summary>
        public string PackageName { get; set; }
        /// <summary>
        /// Возвращает имя файла пакета
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        protected abstract string GetInitialPkgName(IBSharpProject project);

        /// <summary>
        /// Установить целевой проект
        /// </summary>
        /// <param name="project"></param>
        public override void SetProject(IBSharpProject project)
        {
            base.SetProject(project);
            Root = Project.GetRootDirectory().Replace("\\","/");
            PackageName = GetInitialPkgName(project);
            if (!Path.IsPathRooted(PackageName)) {
                PackageName = Path.Combine(project.GetOutputDirectory(), PackageName);
            }
        }
        /// <summary>
        /// Возвращает тип класса для запроса из контекста
        /// </summary>
        /// <returns></returns>
        protected abstract BSharpContextDataType GetContextClassType();

        private Uri GetUri(IBSharpClass cls) {
            var url = "/" + cls.Name;
            if (!string.IsNullOrWhiteSpace(cls.Namespace)) {
                url = "/" + cls.Namespace.Replace(".", "/") + "/" + cls.Name;
            }
            var uri = new Uri(url, UriKind.Relative);
            return uri;
        }
        /// <summary>
        /// Записывает индекс
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pkg"></param>
        private void WriteIndex(IBSharpContext context, Package pkg) {
            var manifest = CreateIndex(context);
            var part = pkg.CreatePart(new Uri("/index", UriKind.Relative), "text/xml", CompressionOption.Normal);
            using (var s = XmlWriter.Create(part.GetStream(FileMode.Create)))
            {
                manifest.WriteTo(s);
                s.Flush();
            }
            pkg.CreateRelationship(part.Uri, TargetMode.Internal, "bsharp://index");
        }
        /// <summary>
        /// Создает индекс
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private XElement CreateIndex(IBSharpContext context) {
            var result = new XElement("bsharp-index");
            var classes = context.Get(GetContextClassType()).ToArray();
            result.SetAttributeValue("count",classes.Length);
            foreach (var c in context.Get(GetContextClassType()))
            {
                var e = GetIndexElement(c);
                e.SetAttributeValue("uri",GetUri(c));
                result.Add((object) e );

            }
            return result;
        }
        /// <summary>
        /// Создает элемент индекса
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        protected XElement GetIndexElement(IBSharpClass cls) {
            var result = new XElement("class");
            var xml = GetXmlForPackaging(cls);
            result.SetAttributeValue("code",cls.Name);
            result.SetAttributeValue("fullcode", cls.FullName);
            result.SetAttributeValue("namespace", cls.Namespace);
            if (cls.Source.Name.LocalName != "class") {
                result.SetAttributeValue("localname",xml.Name.LocalName);
            }
            var usefullAttributes = cls.GetAttributes() & BSharpClassAttributes.SrcPkgSet;
            if (0 != usefullAttributes) {
                result.SetAttributeValue("attributes", usefullAttributes);
            }
            
            var file = xml.Attr("_file");
            if (!string.IsNullOrWhiteSpace(file)) {
                var line = xml.Attr("_line");
                file = file.Replace("\\", "/").Replace(Root, ".");
                result.SetAttributeValue("file", file);
                result.SetAttributeValue("line", line);
            }
            if (null !=xml.Attribute(BSharpSyntax.ClassPrototypeAttribute)) {
                result.SetAttributeValue(BSharpSyntax.ClassPrototypeAttribute,xml.Attr(BSharpSyntax.ClassPrototypeAttribute));
            }
            if (null != xml.Attribute(BSharpSyntax.ClassRuntimeAttribute))
            {
                result.SetAttributeValue(BSharpSyntax.ClassRuntimeAttribute, xml.Attr(BSharpSyntax.ClassRuntimeAttribute));
            }
            return result;
        }
        /// <summary>
        /// Получить исходный XML для упаковки
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        protected abstract XElement GetXmlForPackaging(IBSharpClass cls);

        private void WriteManifest(IBSharpContext context, Package pkg) {
            var manifest = CreateManifest(context);
            var part = pkg.CreatePart(new Uri("/manifest", UriKind.Relative), "text/xml", CompressionOption.Normal);
            using (var s = XmlWriter.Create( part.GetStream(FileMode.Create))) {
                manifest.WriteTo(s);
                s.Flush();
            }
            pkg.CreateRelationship(part.Uri, TargetMode.Internal, "bsharp://manifest");

        }

        private XElement CreateManifest(IBSharpContext context) {
            var result = new XElement("bsharp-manifest");
            result.SetAttributeValue("create-time",DateTime.Now);
            result.SetAttributeValue("user-name",Application.Current.Principal.CurrentUser.Identity.Name);
            result.SetAttributeValue("project-name",Project.ProjectName);
            return result;
        }
        /// <summary>
        /// Создает запись для класса
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract XElement CreateClassRecord(IBSharpClass cls, IBSharpContext context);

        private void WriteClass(IBSharpClass cls, IBSharpContext context, Package pkg) {
            var classRecord = CreateClassRecord(cls, context);
            var uri = GetUri(cls);
            var part = pkg.CreatePart(uri, "text/xml",CompressionOption.Normal);
            using (var s = XmlWriter.Create(part.GetStream(FileMode.Create)))
            {
                classRecord.WriteTo(s);
                s.Flush();
            }
            pkg.CreateRelationship(part.Uri, TargetMode.Internal, "bsharp://class");
        }

        /// <summary>
        /// Формирует ZIP пакет с индексированным исходным кодом пакета
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(IBSharpContext context) {
            using (var pkg = Package.Open(PackageName, FileMode.Create)) {
                WriteManifest(context, pkg);
                WriteIndex(context, pkg);
                foreach (var cls in context.Get(GetContextClassType())) {
                    WriteClass(cls, context, pkg);
                }
                pkg.Flush();
                pkg.Close();
            }
        }
    }
}