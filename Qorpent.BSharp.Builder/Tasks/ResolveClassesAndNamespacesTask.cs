using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Integration.BSharp.Builder.Helpers;
using Qorpent.Utils.Extensions;

namespace Qorpent.Integration.BSharp.Builder.Tasks {
    /// <summary>
    /// 
    /// </summary>
    public class ResolveClassesAndNamespacesTask : BSharpBuilderTaskBase {
        /// <summary>
        /// 
        /// </summary>
        public ResolveClassesAndNamespacesTask() {
            Phase = BSharpBuilderPhase.PreProcess;
            Index = TaskConstants.ResolveClassesAndNamespacesTaskIndex;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(IBSharpContext context) {
	        var filter = ClassFilter.Create(Project);
	        if (!filter.HasConditions) return;
			Project.Sources.AsParallel().ForAll(_ => Execute(filter, _, ""));
            
        }

	    private void Execute(ClassFilter filter, XElement e, string rootns) {
		    if (e.Name.LocalName == BSharpSyntax.Namespace) {
			    var ns = e.Attr("code");
				if (!string.IsNullOrWhiteSpace(rootns)) {
					ns = rootns + "." + ns;
				}
				if (!filter.IsAvailableNamespace(ns)) {
					e.Remove();
				}
				foreach (var c in e.Elements().ToArray())
				{
					Execute(filter, c,ns);
				}
		    }
		    else {
			    var clsname = e.Attr("code");
				if (!filter.IsAvailableClassname(clsname)) {
					e.Remove();
				}
		    }
	    }
    }
}
