using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Builder.Tasks.xslt
{
    public class XsltTask
    {
        private ProjectUriResolver _resolver;

        public XsltTask(IBSharpProject project, XElement definition)
        {
            this.Project = project;
            this.TemplateUri = new Uri(definition.Attr("template"));
            this.ClassSelector = new Uri(definition.Attr("selector"));
            this.Batch = definition.Attr("batch").ToBool();
            this.OutputPath = new Uri(definition.Attr("output"));
            this.NoXml = definition.Attr("noxml").ToBool();
            this._resolver = new ProjectUriResolver(project);
        }
        public IBSharpProject Project { get; set; }
        public Uri TemplateUri { get; set; }
        public Uri ClassSelector { get; set; }
        public bool Batch { get; set; }
        public Uri OutputPath { get; set; }
        public bool NoXml { get; set; }
        

        public void Execute()
        {
            var xmlresolver = new BSharpXmlResolver(Project);
            var xslt = new XslCompiledTransform();
            var xsltsettings = new XsltSettings(true,true);
            xslt.Load(_resolver.GetPath(TemplateUri),xsltsettings,xmlresolver);
            var classes = _resolver.GetClasses(ClassSelector);
           
            if (Batch)
            {
                
                var xml = new XElement("batch",classes.Select(_=>_.Compiled));
                var args = BuildArgs( classes, null);
                var path = _resolver.GetPath(OutputPath);
                Execute(xslt,xml,args,path);
            }
            else
            {
                foreach (var cls in classes)
                {
                    var xml = cls.Compiled;
                    var args = BuildArgs(classes, cls);
                    var path =
                        _resolver.GetPath(OutputPath)
                            .Replace("_NS_", cls.Namespace)
                            .Replace("_NAME_", cls.Name)
                            .Replace("_PROTO_", cls.Prototype);
                    Execute(xslt,xml,args,path);
                }
            }
        }

        private void Execute(XslCompiledTransform xslt, XElement xml, XsltArgumentList args, string path)
        {
            if (NoXml)
            {
                using (var tw = new StreamWriter(path))
                {
                    xslt.Transform(xml.CreateReader(),args,tw);
                    tw.Flush();
                }
            }
            else
            {
                using (var xw = XmlWriter.Create(path))
                {
                    xslt.Transform(xml.CreateReader(), args, xw, new BSharpXmlResolver(Project));
                    xw.Flush();
                }
            }
        }

        private XsltArgumentList BuildArgs(IEnumerable<IBSharpClass> classes, IBSharpClass current)
        {
            return new XsltArgumentList();
        }
    }
}