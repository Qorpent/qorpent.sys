using Qorpent.Mvc;
using Qorpent.Mvc.Binding;

namespace Qorpent.Data.REST {
	/// <summary>
	/// Удаляет соединение из списка соединений
	/// </summary>
	[Action("_db.addconnection", Role = "DEVELOPER")]
	public class RemoveConnection : ActionBase
	{
		[Bind]private bool temporal = false;
		[Bind(Required = true)]
		private string name = "";

		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess()
		{
			Application.DatabaseConnections.UnRegister(name,!temporal);
			return "OK";
		}
	}
}