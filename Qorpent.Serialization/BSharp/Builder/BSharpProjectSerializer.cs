using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Builder {
    /// <summary>
    /// 
    /// </summary>
    public static class BSharpProjectSerializer {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bSharpClass"></param>
        /// <returns></returns>
        public static BSharpProject TryParseBSharpProject(this IBSharpClass bSharpClass) {
            var project = new BSharpProject { IsFullyQualifiedProject = true };

            ParseIncludes(bSharpClass, project);
            ParseExcludes(bSharpClass, project);

            var outLayout = bSharpClass.Compiled.Element("Layout");
            if (outLayout != null) {
                project.OutputAttributes = outLayout.Attribute("code").Value.To<BSharpBuilderOutputAttributes>();
            }

            var outDirectory = bSharpClass.Compiled.Element("OutputDirectory");
            if (outDirectory != null) {
                project.MainOutputDirectory = outDirectory.Attribute("code").Value;
            }

            var outputExtension = bSharpClass.Compiled.Element("OutputExtension");
            if (outputExtension != null) {
                project.OutputExtension = outputExtension.Attribute("code").Value;
            }

            var inputExtension = bSharpClass.Compiled.Element("InputExtension");
            if (inputExtension != null) {
                project.InputExtension = inputExtension.Attribute("code").Value;
            }

            return project;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bSharpClass"></param>
        /// <param name="project"></param>
        private static void ParseIncludes(IBSharpClass bSharpClass, IBSharpProject project) {
            var include = bSharpClass.Compiled.Element("Include");
            if (include != null) {
                FlushIncludeDefaultsIfNeeded(include, project);

                foreach (var el in include.Elements("Path")) {
                    AppendInclude(project.Targets.Paths, el.Attribute("code").Value);
                }

                foreach (var el in include.Elements("Namespace")) {
                    AppendInclude(project.Targets.Namespaces, el.Attribute("code").Value);
                }

                foreach (var el in include.Elements("Class")) {
                    AppendInclude(project.Targets.Classes, el.Attribute("code").Value);
                }
            }
        }

        private static void ParseExcludes(IBSharpClass bSharpClass, IBSharpProject project) {
            var exclude = bSharpClass.Compiled.Element("Exclude");
            if (exclude != null) {
                Console.WriteLine("In exclude");
                foreach (var el in exclude.Elements("Path")) {
                    AppendExclude(project.Targets.Paths, el.Attribute("code").Value);
                }

                foreach (var el in exclude.Elements("Namespace")) {
                    AppendExclude(project.Targets.Namespaces, el.Attribute("code").Value);
                }

                foreach (var el in exclude.Elements("Class")) {
                    AppendExclude(project.Targets.Classes, el.Attribute("code").Value);
                }
            }
        }

        private static void AppendExclude(IDictionary<string, BSharpBuilderTargetType> collection, string value) {
            collection.AppendTarget(value, BSharpBuilderTargetType.Exclude);
        }

        private static void AppendInclude(IDictionary<string, BSharpBuilderTargetType> collection, string value) {
            collection.AppendTarget(value, BSharpBuilderTargetType.Include);
        }
        /// <summary>
        ///     сбрасывает настройки по умолчанию для инклудов, если это имеются
        ///     инклуды на Path, Namespace
        /// </summary>
        /// <param name="include"></param>
        /// <param name="project"></param>
        private static void FlushIncludeDefaultsIfNeeded(XElement include, IBSharpProject project) {
            if (!include.Elements("Path").IsEmptyCollection()) {
                project.Targets.Paths.RemoveTarget("*");
            }

            if (!include.Elements("Namespace").IsEmptyCollection()) {
                project.Targets.Namespaces.RemoveTarget("*");
            }

            if (!include.Elements("Class").IsEmptyCollection()) {
                project.Targets.Classes.RemoveTarget("*");
            }
        }
    }
}
