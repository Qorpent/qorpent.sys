using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Qorpent.Mvc;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.REST
{
	/// <summary>
	/// Возвращает список зарегистрированных соединений
	/// </summary>
	[Action("_db.connections",Role="DEVELOPER")]
	public class ConnectionsAction :ActionBase {
		[Bind] private bool testConnection = false;

		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess()
		{
			if(testConnection) {
				List<object> retobj = new List<object>();
				foreach(var c in Application.DatabaseConnections.Enlist()) {
					string result = "";
					IDbConnection con = null;
					try {
						con = Application.DatabaseConnections.GetConnection(c.Name);
					}catch(Exception ex) {
						result = ex.Message;
					}
					if(result.IsEmpty()) {
						try {
							using(con) {
								con.Open();
							}
						}catch(Exception ex) {
							result = ex.Message;
						}
					}
					if(result.IsEmpty()) {
						result = "OK";
					}

					retobj.Add(new {connection=c, result});

				}
				return retobj.ToArray();
			}
			else {
				return Application.DatabaseConnections.Enlist();
			}
		}
	}
}
