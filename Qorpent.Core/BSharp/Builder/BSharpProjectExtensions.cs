using Qorpent.Utils.Extensions;



namespace Qorpent.BSharp.Builder{
	/// <summary>
	/// 
	/// </summary>
	public static class BSharpProjectExtensions{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="project"></param>
		/// <param name="conditionName"></param>
		/// <returns></returns>
		public static bool IsSet(this IBSharpProject project, string conditionName){
			if (null == project) return false;
			if (!project.Conditions.ContainsKey(conditionName)) return false;
			if (project.Conditions[conditionName] == "") return true; //simple set conditions
			return project.Conditions[conditionName].ToBool();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="project"></param>
		/// <param name="conditionName"></param>
		/// <returns></returns>
		public static string GetCondition(this IBSharpProject project, string conditionName)
		{
			if (null == project) return string.Empty;
			if (!project.Conditions.ContainsKey(conditionName)) return string.Empty;
			return project.Conditions[conditionName];
		}
	}
}