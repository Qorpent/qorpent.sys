using Qorpent.Mvc;
using Qorpent.Mvc.Binding;

namespace Qorpent.Data.REST {
	/// <summary>
	/// Добавляет соединение к списку соединений (или обновляет его)
	/// </summary>
	[Action("_db.addconnection", Role = "DEVELOPER")]
	public class AddConnection : ActionBase {
		[Bind] private bool temporal = false;
		[Bind(Required = true)] private string name = "";
		[Bind(Required = true)] private string connection = "";

		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			var c = new ConnectionDescriptor {Name = name, ConnectionString = connection};
			Application.DatabaseConnections.Register(
				c, !temporal
				);
			return c;
		}
	}
}