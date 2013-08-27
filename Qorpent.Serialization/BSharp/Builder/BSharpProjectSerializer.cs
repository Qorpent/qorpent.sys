using System;
using System.Collections.Generic;
using System.Linq;
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

            var exclude = bSharpClass.Compiled.Element("Exclude");
            if (exclude != null) {
                var parsed = ParseKeyValueEnumeration(exclude.Value, ',', ':');
                foreach (var el in parsed) {
                    WriteTarget(project, el.Key, el.Value, BSharpBuilderTargetType.Exclude);
                }
            }

            var include = bSharpClass.Compiled.Element("Include");
            if (include != null) {
                var parsed = ParseKeyValueEnumeration(include.Value, ',', ':');
                foreach (var el in parsed) {
                    WriteTarget(project, el.Key, el.Value, BSharpBuilderTargetType.Include);
                }
            }

            var outLayout = bSharpClass.Compiled.Element("Out-Layout");
            if (outLayout != null) {
                project.OutputAttributes = outLayout.Value.To<BSharpBuilderOutputAttributes>();
            }

            return project;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <param name="modifier"></param>
        /// <param name="target"></param>
        /// <param name="type"></param>
        private static void WriteTarget(BSharpProject project, string modifier, string target, BSharpBuilderTargetType type) {
            switch (modifier) {
                case "n": project.Targets.Namespaces.AppendTarget(target, type); break;
                case "c": project.Targets.Classes.AppendTarget(target, type); break;
                case "p": project.Targets.Paths.AppendTarget(target, type); break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <param name="delimeter"></param>
        /// <returns></returns>
        private static IEnumerable<KeyValuePair<string, string>> ParseKeyValueEnumeration(string source, char separator, char delimeter) {
            return source.Split(
                new[] { separator }
            ).Select(
                item => item.Split(new[] { delimeter })
            ).Select(
                el => new KeyValuePair<string, string>(el[0], el[1])
            ).ToList();
        }
    }
}
