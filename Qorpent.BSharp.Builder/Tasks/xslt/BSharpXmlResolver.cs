using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Qorpent.BSharp.Builder.Tasks.xslt
{
    /// <summary>
    /// 
    /// </summary>
    public class BSharpXmlResolver : XmlResolver
    {
        private readonly IBSharpProject _project;
        private readonly XmlUrlResolver _nativeresolver;
        private readonly ProjectUriResolver _projectresolver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        public BSharpXmlResolver(IBSharpProject project)
        {
            this._project = project;
            this._projectresolver = new ProjectUriResolver(_project);
            this._nativeresolver = new XmlUrlResolver();
        }

        /// <summary>
        /// When overridden in a derived class, maps a URI to an object that contains the actual resource.
        /// </summary>
        /// <returns>
        /// A stream object or null if a type other than stream is specified.
        /// </returns>
        /// <param name="absoluteUri">The URI returned from <see cref="M:System.Xml.XmlResolver.ResolveUri(System.Uri,System.String)"/>.</param><param name="role">Currently not used.</param><param name="ofObjectToReturn">The type of object to return. The current version only returns System.IO.Stream objects.</param><exception cref="T:System.Xml.XmlException"><paramref name="ofObjectToReturn"/> is not a Stream type.</exception><exception cref="T:System.UriFormatException">The specified URI is not an absolute URI.</exception><exception cref="T:System.ArgumentNullException"><paramref name="absoluteUri"/> is null.</exception><exception cref="T:System.Exception">There is a runtime error (for example, an interrupted server connection).</exception>
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
       
                if (_projectresolver.IsProjectFileReference(absoluteUri))
                {
                    return GetFile(_projectresolver.GetPath(absoluteUri));
                }
                if (_projectresolver.IsClassesReference(absoluteUri))
                {
                    return WrapClasses(_projectresolver.GetClasses(absoluteUri));
                }
                return _nativeresolver.GetEntity(absoluteUri, role, ofObjectToReturn);
               
        }

        private Stream WrapClasses(IEnumerable<IBSharpClass> classes)
        {
            var xml = new XElement("classes", classes.Select(_ => _.Compiled));
            var data = Encoding.UTF8.GetBytes(xml.ToString());
            return new MemoryStream(data);
        }


        private Stream GetFile(string path)
        {
            
            if (File.Exists(path))
            {
                return new FileStream(path,FileMode.Open,FileAccess.Read);
            }
            throw new FileNotFoundException(path);
        }


        
    }
}