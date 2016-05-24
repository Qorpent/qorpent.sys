using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.Utils.Extensions;

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
                    return Path.Combine(_project.GetRootDirectory(),uri.Host+"/"+uri.AbsolutePath).NormalizePath();
                case "output":
                    return Path.Combine(_project.GetOutputDirectory(), uri.Host+"/"+uri.AbsolutePath).NormalizePath();
                case "compile":
                    return Path.Combine(_project.GetCompileDirectory(), uri.Host+"/"+uri.AbsolutePath).NormalizePath();
                default:
                    throw new Exception("not supported uri");
            }
        }

        public IEnumerable<IBSharpClass> GetClasses(Uri uri) {
            string query = "";
            switch (uri.Scheme)
            {

                case "class":
                    query = uri.Query;
                    IBSharpClass cls;
                    if (query.StartsWith("?")) {
                        query = query.Substring(1);
                        cls = _project.Context.ResolveAll(query).FirstOrDefault();
                    }
                    else {
                        cls = _project.Context.Get(uri.Host);
                    }
                    return null == cls ? new IBSharpClass[] {} : new []{cls};
                case "classes":
                    var path = uri.Host;
                    query = uri.Query;
                    if (query.StartsWith("?"))
                    {
                        query = query.Substring(1);
                    }
                    path += query;
                    return _project.Context.ResolveAll(path).ToArray();
              
                default:
                    throw new Exception("not supported uri");
            }
        }
    }
}