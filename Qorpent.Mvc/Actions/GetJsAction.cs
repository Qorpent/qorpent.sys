using System;
using System.IO;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// Возвращает javascript с контролем ETag
	/// </summary>
	[Action("_sys.getjs",Role = "DEFAULT")]
	public class GetJsAction : ActionBase {
		private string _filename;


		/// <summary>
		/// 	First phase of execution - override if need special input parameter's processing
		/// </summary>
		protected override void Initialize() {
			base.Initialize();
			EvalScriptPath();
		}

		private void EvalScriptPath() {
			_filename = FileNameResolver.Resolve("~/scripts/" + ScriptName + ".js");
		}

		/// <summary>
		/// 	Second phase - validate INPUT/REQUEST parameters here - it called before PREPARE so do not try validate
		/// 	second-level internal state and authorization - only INPUT PARAMETERS must be validated
		/// </summary>
		protected override void Validate()
		{
			if (string.IsNullOrWhiteSpace(_filename)) {
				throw new Exception("script not exists");
			}
		}

		/// <summary>
		/// 	override if Yr action provides 304 state and return TRUE
		/// </summary>
		/// <returns> </returns>
		protected override bool GetSupportNotModified() {
			return true;
		}

		/// <summary>
		/// 	override if Yr action provides 304 state  and return Last-Modified-State header
		/// </summary>
		/// <returns> </returns>
		protected override DateTime EvalLastModified() {
			EvalScriptPath();
			if (string.IsNullOrWhiteSpace(_filename)) return DateTime.Now;
			return File.GetLastWriteTime(_filename);
		}

		
		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			return _filename;
		}
		/// <summary>
		/// Имя скрипта
		/// </summary>
		[Bind(true)] protected string ScriptName { get; set; }

	}
}