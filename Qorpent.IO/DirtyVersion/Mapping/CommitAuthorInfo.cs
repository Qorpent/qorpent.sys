using System;
using Qorpent.Applications;

namespace Qorpent.IO.DirtyVersion.Mapping {
	/// <summary>
	/// Информация о коммитере и дате коммита
	/// </summary>
	public class CommitAuthorInfo {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected bool Equals(CommitAuthorInfo other) {
			return string.Equals(Commiter, other.Commiter) && Time.Equals(other.Time);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() {
			unchecked {
				return ((Commiter != null ? Commiter.GetHashCode() : 0)*397) ^ Time.GetHashCode();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CommitAuthorInfo) obj);
		}
		/// <summary>
		/// Автор
		/// </summary>
		public string Commiter;

		/// <summary>
		/// Время записи
		/// </summary>
		public DateTime Time;
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