﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Builder{
	/// <summary>
	/// </summary>
	public static class BSharpProjectSerializer{
		/// <summary>
		/// </summary>
		/// <param name="bSharpClass"></param>
		/// <returns></returns>
		public static BSharpProject TryParseBSharpProject(this IBSharpClass bSharpClass){
			
			
			var project = new BSharpProject{IsFullyQualifiedProject = true};
			project.Definition = bSharpClass.Compiled;
			ParseIncludes(bSharpClass, project);
			ParseExcludes(bSharpClass, project);

			project.GenerateSrcPkg = bSharpClass["GenerateSrcPkg"].ToBool();
			project.GenerateLibPkg = bSharpClass["GenerateLibPkg"].ToBool();
			project.GenerateJsonModule = bSharpClass["GenerateJsonModule"].ToBool();
			project.SrcPkgName = bSharpClass["SrcPkgName"];
			project.LibPkgName = bSharpClass["LibPkgName"];
			project.JsonModuleName = bSharpClass["JsonModuleName"];

			XElement outLayout = bSharpClass.Compiled.Element("Layout");
			if (outLayout != null){
				project.OutputAttributes = outLayout.Attribute("code").Value.To<BSharpBuilderOutputAttributes>();
			}

			XElement outDirectory = bSharpClass.Compiled.Element("OutputDirectory");
			if (outDirectory != null){
				project.MainOutputDirectory = outDirectory.Attribute("code").Value;
			}

			XElement outputExtension = bSharpClass.Compiled.Element("OutputExtension");
			if (outputExtension != null){
				project.OutputExtension = outputExtension.Attribute("code").Value;
			}

			XElement inputExtension = bSharpClass.Compiled.Element("InputExtensions");
			if (inputExtension != null){
				project.InputExtensions = inputExtension.Attribute("code").Value;
			}

			XElement ignoreElements = bSharpClass.Compiled.Element("IgnoreElements");
			if (ignoreElements != null){
				project.IgnoreElements = ignoreElements.Attribute("code").Value;
			}


			XElement generateGraph = bSharpClass.Compiled.Element("GenerateGraph");
			if (null != generateGraph){
				project.GenerateGraph = true;
			}

			IEnumerable<XElement> extensions = bSharpClass.Compiled.Elements("Extension");
			foreach (XElement e in extensions){
				project.Extensions.Add(e.GetCode());
			}
			IEnumerable<XElement> sourcedirs = bSharpClass.Compiled.Elements("Source");
			if (sourcedirs.Any()){

				project.SourceDirectories = new List<string>();
				foreach (XElement e in sourcedirs){
					project.SourceDirectories.Add(Environment.ExpandEnvironmentVariables( e.GetCode()));
				}
			}
			project.SrcClass = bSharpClass;

			foreach (XElement e in bSharpClass.Compiled.Elements()){
				if (!string.IsNullOrWhiteSpace(e.Value)){
					project.Set("_" + e.Name.LocalName, e.Value);
				}
				else{
					project.Set("_" + e.Name.LocalName, e.Attr("code"));
				}
			}

			return project;
		}

		/// <summary>
		/// </summary>
		/// <param name="bSharpClass"></param>
		/// <param name="project"></param>
		private static void ParseIncludes(IBSharpClass bSharpClass, IBSharpProject project){
			XElement include = bSharpClass.Compiled.Element("Include");
			if (include != null){
				foreach (XElement el in include.Elements("Path")){
					AppendInclude(project.Targets.Paths, el.Attribute("code").Value);
				}

				foreach (XElement el in include.Elements("Namespace")){
					AppendInclude(project.Targets.Namespaces, el.Attribute("code").Value);
				}

				foreach (XElement el in include.Elements("Class")){
					AppendInclude(project.Targets.Classes, el.Attribute("code").Value);
				}
			}
		}

		private static void ParseExcludes(IBSharpClass bSharpClass, IBSharpProject project){
			XElement exclude = bSharpClass.Compiled.Element("Exclude");
			if (exclude != null){
				Console.WriteLine("In exclude");
				foreach (XElement el in exclude.Elements("Path")){
					AppendExclude(project.Targets.Paths, el.Attribute("code").Value);
				}

				foreach (XElement el in exclude.Elements("Namespace")){
					AppendExclude(project.Targets.Namespaces, el.Attribute("code").Value);
				}

				foreach (XElement el in exclude.Elements("Class")){
					AppendExclude(project.Targets.Classes, el.Attribute("code").Value);
				}
			}
		}

		private static void AppendExclude(IDictionary<string, BSharpBuilderTargetType> collection, string value){
			collection.AppendTarget(value, BSharpBuilderTargetType.Exclude);
		}

		private static void AppendInclude(IDictionary<string, BSharpBuilderTargetType> collection, string value){
			collection.AppendTarget(value, BSharpBuilderTargetType.Include);
		}
	}
}