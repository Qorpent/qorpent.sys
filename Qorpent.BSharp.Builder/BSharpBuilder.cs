using System;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Integration.BSharp.Builder.Tasks;
using Qorpent.Integration.BSharp.Builder.Tasks.WriteTasks;
using Qorpent.Log;

namespace Qorpent.Integration.BSharp.Builder {
	/// <summary>
	/// Реальный билдер
	/// </summary>
	public class BSharpBuilder : BSharpBuilderBase {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="project"></param>
		protected override void PrepareTasksFromProject(IBSharpProject project) {
            
			Tasks.Add(new LoadAllSourcesTask());
			Tasks.Add(new ResolveClassesAndNamespacesTask());
            Tasks.Add(new CompileBSharpTask());
			Tasks.Add(new CleanUpTask());

		    if (project.WriteCompiled) {
				Tasks.Add(new WriteWorkingOutputTask());    
           //     Tasks.Add(new GenerateIndexTask());
		    }

			if (project.OutputAttributes.HasFlag(BSharpBuilderOutputAttributes.IncludeOrphans))
			{
				Tasks.Add(new WriteOrphansOutputTask());
			}
			
			if (project.GenerateSrcPkg) {
				Tasks.Add(new GenerateSrcPackageTask());
			}


            if (project.GenerateLibPkg)
            {
                Tasks.Add(new GenerateLibPackageTask());
            }

            if (project.GenerateGraph) {
                Tasks.Add(new GenerateClassGraphTask());
            }

			if (project.GenerateJsonModule)
			{
				Tasks.Add(new GenerateJsonModuleTask());
			}

			Tasks.Add(new WriteErrorInfoTask());
			Tasks.Add(new WritePrettyErrorDigest());


		    
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="compiledProject"></param>
		/// <returns></returns>
		protected override IBSharpProject ConvertToBSharpBuilderProject(IBSharpContext compiledProject) {
			var projectClass = compiledProject.Get(Project.ProjectName);
			if (null != projectClass) {
				return GenerateProject(projectClass);
			}
			throw new Exception("cannot find project " + Project.ProjectName);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override IBSharpContext CompileInternalProject() {
			var builder = new BSharpBuilder
			{
				Log = new StubUserLog()
			};

			builder.Initialize(
				new BSharpProject
				{
					IsFullyQualifiedProject = true,
					InputExtensions = BSharpBuilderDefaults.DefaultBSharpProjectExtension,
					WriteCompiled = false,
                    Conditions = Project.Conditions,
				}
			);

			var projectsContext = builder.Build();

			if (projectsContext == null)
			{
				throw new Exception("Can not compile project!");
			}
			return projectsContext;
		}
        /// <summary>
        ///     Сгенерировать проект из класса
        /// </summary>
        /// <returns></returns>
        public BSharpProject GenerateProject(IBSharpClass bSharpClass) {
            return bSharpClass.TryParseBSharpProject();
        }
	}
}