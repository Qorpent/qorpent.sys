using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests {
    [TestFixture]
    [Explicit]
    public class SpecialPathsInScopeToolkit {
        private Dictionary<string, Exception> _bxlerrors;
        private string[] _files;
        private Dictionary<string, XElement> _xmls;
        public string[] BSharpExtensions = {".bxl", ".bxls", ".bsproj", ".hostconf"};
        public string[] ExcludeDirectories = {"tiles", "_img", ".git"};
        private XElement _dots;

        public IEnumerable<string> FindBSharpAllFiles(string root = null) {
            if (string.IsNullOrWhiteSpace(root)) {
                root = EnvironmentInfo.GetRepositoryRoot();
                ;
            }
            foreach (var file in Directory.EnumerateFiles(root, "*.*", SearchOption.TopDirectoryOnly)) {
                if (-1 != Array.IndexOf(BSharpExtensions, Path.GetExtension(file))) {
                    yield return file;
                }
            }
            foreach (var dir in Directory.EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly)) {
                var name = Path.GetFileName(dir);
                if (-1 != Array.IndexOf(ExcludeDirectories, name)) {
                    continue;
                }
                foreach (var file in FindBSharpAllFiles(dir)) {
                    yield return file;
                }
            }
        }

        [TestFixtureSetUp]
        public void CollectFiles() {
            _files = FindBSharpAllFiles().OrderBy(_=>_).ToArray();
            var bxl = new BxlParser();
            _xmls = new Dictionary<string, XElement>();
            _bxlerrors = new Dictionary<string, Exception>();
            foreach (var file in _files) {
                try {
                    _xmls[file] = bxl.Parse(File.ReadAllText(file), file);
                }
                catch (Exception e) {
                    _bxlerrors[file] = e;
                }
            }
             _dots = FindDotInterpolations();
        }

        [Test]
        public void FilesCollected() {
            Assert.Less(200, _files.Length);
        }

        [Test]
        public void NoBxlErrors() {
            foreach (var exception in _bxlerrors) {
                Console.WriteLine(exception.Key);
                Console.WriteLine(exception.Value);
            }
            Assert.AreEqual(0, _bxlerrors.Count);
        }

  
        public XElement FindDotInterpolations() {

            XElement result = XElement.Parse("<analyzer code='FindDotInterpolations'></analyzer>");
            foreach (var xml in _xmls) {
                var fileelement = new XElement("file", new XAttribute("path", xml.Key));
                var clone  = CleanupFindDotInterpolations(xml.Value);
                if(null==clone)continue;
                
                foreach (var element in clone.Elements()) {
                    if (!element.IsRealEmpty()) {
                        fileelement.Add(element);
                    }
                }
                if (fileelement.HasElements)
                {
                    result.Add(fileelement);
                }
            }
            var file = Path.Combine(EnvironmentInfo.GetRepositoryRoot(), ".analyze/FindDotInterpolations.xml");
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            result.Save(file);
            return result;
        }

        public struct XmlObserverResult {
            public bool Remove;
            public bool Preserve;
            public bool Include;
            public object Replace;
        }

        public class XmlObserver {
            public bool RemoveEmpty;
            public bool PreserveLexInfo;
            public Func<XElement, XmlObserverResult> OnElement;
            public Func<XAttribute, XmlObserverResult> OnAttribute;
            public Func<XText, XmlObserverResult> OnText;
            public XElement Visit(XElement element) {
                var result = new XElement(element);
                var ownresult = VisitElement(result);
                if (ownresult.Remove) return null;
                if (ownresult.Include) return result;
                if (null != ownresult.Replace) return (XElement) ownresult.Replace;
                if (RemoveEmpty && result.IsRealEmpty()) return null;
                return result;
            }

            private XmlObserverResult VisitElement(XElement element) {
                List<XAttribute> preserved = new List<XAttribute>();
                foreach (var attribute in element.Attributes().ToArray())
                {
                    if (PreserveLexInfo) {
                        if (attribute.Name.LocalName == "_file" || attribute.Name.LocalName=="_line") {
                            preserved.Add(new XAttribute(attribute));
                            attribute.Remove();
                            continue;  
                        }
                    }
                    if (null != OnAttribute) {
                        var subresult = OnAttribute(attribute);
                        if (subresult.Preserve) {
                            preserved.Add(new XAttribute(attribute));
                            attribute.Remove();
                        }
                        if (subresult.Include) {
                            continue;
                        }
                        if (subresult.Remove) {
                            attribute.Remove();
                        }
                        else if (subresult.Replace is XAttribute) {
                            attribute.Remove();
                            var attr = subresult.Replace as XAttribute;
                            element.SetAttributeValue(attr.Name.LocalName,attr.Value);
                        }
                        else if (RemoveEmpty && string.IsNullOrWhiteSpace(attribute.Value)) {
                            attribute.Remove();
                        }
                    }
                }

                foreach (var text in element.Nodes().OfType<XText>().ToArray())
                {
                    if (null != OnText)
                    {
                        var subresult = OnText(text);
                        if (subresult.Include)
                        {
                            continue;
                        }
                        if (subresult.Remove)
                        {
                            text.Remove();
                        }
                        else if (subresult.Replace is XText)
                        {
                            var newtext = subresult.Replace as XText;
                            text.ReplaceWith(newtext);
                        }
                        else if (RemoveEmpty && string.IsNullOrWhiteSpace(text.Value))
                        {
                            text.Remove();
                        }
                    }
                }
                foreach (var subelement in element.Elements().ToArray())
                {
                    VisitElement(subelement);
                }

                if (null != OnElement) {
                    var subresult = OnElement(element);
                    if (subresult.Include) return subresult;
                    if (subresult.Remove && element.Parent!=null)element.Remove();
                }

                if (RemoveEmpty && element.IsRealEmpty()) {
                    if(element.Parent!=null)element.Remove();
                    return new XmlObserverResult {Remove = true};
                }
                foreach (var attribute in preserved) {
                    element.SetAttributeValue(attribute.Name,attribute.Value);
                }
                return new XmlObserverResult {Include = true};

            }
        }

        static bool IsValueInterpolation(string value, string regex = null) {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (!(value.Contains('{') && value.Contains('}'))) {
                return false;
            }
            var matches = Regex.Matches(value, @"[$^%~!@\.*&`#+-]\{([^\{\}]+)\}");
            if (matches.Count == 0) return false;
            if (string.IsNullOrWhiteSpace(regex)) return true;
            foreach (Match match in matches) {
                var interpolation = match.Groups[1].Value;
                if (!string.IsNullOrWhiteSpace(interpolation)) {
                    if (Regex.IsMatch(interpolation, regex)) {
                        return true;
                    }
                }
            }
            return false;
        }
        static bool IsNameInterpolation(string name, string regex = null) {
            return IsValueInterpolation(name.Replace("__LBLOCK__", "{").Replace("__RBLOCK__", "}"), regex);
        }
   
        private XElement CleanupFindDotInterpolations(XElement element) {

            string dotregex = @"(^|,)\.";
            var cleaner = new XmlObserver();
            cleaner.RemoveEmpty = true;
            cleaner.PreserveLexInfo = true;
            
            cleaner.OnElement = e => new XmlObserverResult {Include = IsNameInterpolation(e.Name.LocalName, dotregex)};
            cleaner.OnAttribute = e => {
                bool include = false;
                var name = e.Name.LocalName;
                var value = e.Value;
                if (IsValueInterpolation(value, dotregex)) {
                    include = true;
                }
                else {
                    value = "";

                }
                if (IsNameInterpolation(name, dotregex)) {
                    include = true;
                }
                if (!include) return new XmlObserverResult {Remove = true};
                return new XmlObserverResult{Replace = new XAttribute(name,value)};

            };
            cleaner.OnText = e => new XmlObserverResult {Remove = !IsValueInterpolation(e.Value,dotregex)};
            var result = cleaner.Visit(element);
            return result;
        }


       
    }
}