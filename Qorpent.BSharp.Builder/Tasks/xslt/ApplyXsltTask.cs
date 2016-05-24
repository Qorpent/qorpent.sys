using System;
using System.Xml.XPath;
using Qorpent.Integration.BSharp.Builder.Tasks;

namespace Qorpent.BSharp.Builder.Tasks.xslt
{
    /// <summary>
    /// Задача по генерации продукции на основе XSLT
    /// </summary>
    public class ApplyXsltTask : BSharpBuilderTaskBase
    {
        public ApplyXsltTask()
        {
            this.Phase = BSharpBuilderPhase.PostProcess;
            this.Index = Index = TaskConstants.WriteWorkingOutputTaskIndex + 10;
            this.Async = false;
        }
        public override void Execute(IBSharpContext context)
        {
            Project.Log.Info("Start xslt tasks");
            var xslttasks = context.ResolveAll("xslttask");
            foreach (var xslttask in xslttasks)
            {
                var task = new XsltTask(Project,xslttask.Compiled);
                Project.Log.Info("Start XSLT task "+xslttask.FullName);
                try
                {
                    task.Execute();
                    Project.Log.Info("XSLT task " + xslttask.FullName+" finished");
                }
                catch (Exception e)
                {
                    Project.Context.RegisterError(new BSharpError
                    {
                        Class = xslttask,
                        Error = e,
                        Message = "Error in xslt task:\r\n"+e.ToString()
                    });
                    Project.Log.Error("Error in xslt "+xslttask.FullName,e);
                }
            }
        }
    }
}