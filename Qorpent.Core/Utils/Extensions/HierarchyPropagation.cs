using Qorpent.Model;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// Behavior of temporal property apply
	/// </summary>
	public enum HierarchyPropagation {
		/// <summary>
		/// No propagation - apply only current node
		/// </summary>
		None,
		/// <summary>
		/// Propagates up to hierarchy uses <see cref="IWithHierarchy{TEntity}.IsPropagationRoot"/> as finish node
		/// </summary>
		Up,
		/// <summary>
		/// Propagates down
		/// </summary>
		Down,
		/// <summary>
		/// Propagates both up and down
		/// </summary>
		UpAndDown
	}
}