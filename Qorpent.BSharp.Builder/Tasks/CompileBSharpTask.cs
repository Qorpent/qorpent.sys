using System.Linq;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Utils.Extensions;

namespace Qorpent.Integration.BSharp.Builder.Tasks {
	/// <summary>
	/// Задача собственно вызова компилятора B#
	/// </summary>
	public class CompileBSharpTask :BSharpBuilderTaskBase {
	    /// <summary>
		/// Загружет исходный код классов
		/// </summary>
		public CompileBSharpTask() {
			Phase = BSharpBuilderPhase.Build;
			Index = TaskConstants.CompileBSharpTaskIndex;
		}

		/// <summary>
		/// Выполнение цели
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(IBSharpContext context) {
			var compiler = new BSharpCompiler();
		    foreach (var e in Project.CompilerExtensions ) {
		        compiler.Extensions.Add(e);
		    }
		    var cfg = GetConfig();
			compiler.Initialize(cfg);
            compiler.Compile(Project.Sources, context);
            HandleErrorStream(context);
		}

        private IBSharpConfig GetConfig() {
            var config = new BSharpConfig {
                SingleSource = true,
                UseInterpolation = true,
				IgnoreElements = Project.IgnoreElements.SmartSplit().ToArray(),
				Global = Project.Global,
            };

            config.SetParent(Project);

            return config;
        }

        private void HandleErrorStream(IBSharpContext context) {
            var errors = context.GetErrors();
            foreach (var c in context.Get(BSharpContextDataType.Working)) {
                var witherrors = errors.Any(_ => _.Class == c || _.AltClass == c || _.ClassName == c.FullName);
                if (witherrors) {
                    Project.Log.Warn("class " + c.FullName + " compiled with some errors");
                } else {
                    Project.Log.Debug("class " + c.FullName + " compiled");
                }

            }
        }
	}
}
