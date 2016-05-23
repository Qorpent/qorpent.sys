using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qorpent.BSharp.Builder.Tasks.xslt
{
    public class ProjectUriResolver
    {
        private IBSharpProject _project;

        public ProjectUriResolver(IBSharpProject project)
        {
            this._project = project;
        }
        public bool IsClassesReference(Uri uri)
        {
            switch (uri.Scheme)
            {

                case "class":
                    return true;
                case "classes":
                    return true;
             
                default:
                    return false;
            }
        }
        public bool IsProjectFileReference(Uri uri)
        {
            switch (uri.Scheme)
            {
               
                case "project":
                    return true;
                case "output":
                    return true;
                case "compile":
                    return true;
                default:
                    return false;
            }
        }

        public string GetPath(Uri uri)
        {
            
            switch (uri.Scheme)
            {

                case "project":
                    return Path.Combine(_project.GetRootDirectory(),uri.Host,uri.AbsolutePath);
                case "output":
                    return Path.Combine(_project.GetOutputDirectory(), uri.Host, uri.AbsolutePath);
                case "compile":
                    return Path.Combine(_project.GetCompileDirectory(), uri.Host, uri.AbsolutePath);
                default:
                    throw new Exception("not supported uri");
            }
        }

        public IEnumerable<IBSharpClass> GetClasses(Uri uri)
        {

            switch (uri.Scheme)
            {

                case "class":
                    var cls = _project.Context.Get(uri.Host);
                    return null == cls ? new IBSharpClass[] {} : new []{cls};
                case "classes":
                    var path = uri.Host;
                    var query = uri.Query;
                    if (query.StartsWith("?"))
                    {
                        query = query.Substring(1);
                    }
                    path += query;
                    return _project.Context.ResolveAll(query).ToArray();
              
                default:
                    throw new Exception("not supported uri");
            }
        }
    }
}