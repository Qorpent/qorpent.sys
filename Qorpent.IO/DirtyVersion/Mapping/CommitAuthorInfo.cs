using System;
using Qorpent.Applications;

namespace Qorpent.IO.DirtyVersion.Mapping {
	/// <summary>
	/// Информация о коммитере и дате коммита
	/// </summary>
	public struct CommitAuthorInfo {
		/// <summary>
		/// Автор
		/// </summary>
		public string Commiter { get; set; }
		/// <summary>
		/// Время записи
		/// </summary>
		public DateTime Time { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public void Normalize() {
			if (string.IsNullOrWhiteSpace(Commiter)) {
				var usrname = Application.Current.Principal.CurrentUser.Identity.Name;
				if (usrname.Contains("\\")) {
					usrname = usrname.Split('\\')[1];
				}
				Commiter = usrname;
			}
			if (Time.Year <= 1900) {
				Time = DateTime.Now;
			}
		}
	}
}