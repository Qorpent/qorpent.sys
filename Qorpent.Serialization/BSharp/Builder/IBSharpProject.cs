using Qorpent.Config;

namespace Qorpent.BSharp.Builder {
	/// <summary>
	/// 
	/// </summary>
	public interface IBSharpProject:IConfig {}

	/// <summary>
	/// 
	/// </summary>
	public class BSharpProject :ConfigBase, IBSharpProject {}
}