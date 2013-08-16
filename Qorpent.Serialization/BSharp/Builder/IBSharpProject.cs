using Qorpent.Config;

namespace Qorpent.BSharp.Builder {
	/// <summary>
	/// 
	/// </summary>
	public interface IBSharpProject:IConfig {
		/// <summary>
		/// Целевые проекты при билде
		/// </summary>
		string[] TargetNames { get; set; }
		/// <summary>
		/// Признак полностью загруженного проекта
		/// </summary>
		bool IsFullyQualifiedProject { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class BSharpProject :ConfigBase, IBSharpProject {
		private const string TARGET_NAMES = "target_names";
		private const string FULLY_QUALIFIED = "fully_qualified";

		/// <summary>
		/// Целевые проекты при билде
		/// </summary>
		public string[] TargetNames {
			get { return Get(TARGET_NAMES, new string[] {}); }
			set { Set(TARGET_NAMES, value); }
		}

		/// <summary>
		/// Признак полностью загруженного проекта
		/// </summary>
		public bool IsFullyQualifiedProject {
			get { return Get(FULLY_QUALIFIED, false); }
			set { Set(FULLY_QUALIFIED, value); }
		}
	}
}