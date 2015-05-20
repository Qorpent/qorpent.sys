using System.Security.Principal;

namespace Qorpent.Host.Security{
	/// <summary>
	///     Внутренний идент Qorpent.Host
	/// </summary>
	public class QorpentHostIdentity : IIdentity{
		private readonly string _authenticationType;
		private readonly bool _isAuthenticated;
		private readonly string _name;

		/// <summary>
		/// </summary>
		/// <param name="info"></param>
		public QorpentHostIdentity(UserInfo info){
			_name = info.Login;
			if (string.IsNullOrWhiteSpace(_name)){
				_name = "local\\guest";
			}
			_authenticationType = "Qorpent";
			_isAuthenticated = info.Ok;
		}

		public string Name{
			get { return _name; }
		}

		public string AuthenticationType{
			get { return _authenticationType; }
		}

		public bool IsAuthenticated{
			get { return _isAuthenticated; }
		}
	}
}