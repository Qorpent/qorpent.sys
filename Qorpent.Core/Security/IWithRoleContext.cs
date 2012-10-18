namespace Qorpent.Security {
	/// <summary>
	/// Позволяет указать дополнительный строковый контекст роли
	/// </summary>
	public interface  IWithRoleContext {
		/// <summary>
		/// Дополнительный контекст проверки роли
		/// </summary>
		string RoleContext { get; set; }
	}
}