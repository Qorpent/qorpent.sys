using System;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Occures where QView was not found
	/// </summary>
	[Serializable]
	public class ViewNotFoundException : MvcExecption {
		/// <summary>
		/// </summary>
		/// <param name="viewname"> </param>
		/// <param name="context"> </param>
		public ViewNotFoundException(string viewname, IMvcContext context = null)
			: base("view not found " + viewname, context) {
			ViewName = viewname;
		}

		/// <summary>
		/// 	Name of view havenot found
		/// </summary>
		public string ViewName { get; protected set; }

		/// <summary>
		/// При переопределении в производном классе задает сведения об исключении для <see cref="T:System.Runtime.Serialization.SerializationInfo"/>.
		/// </summary>
		/// <param name="info">Объект <see cref="T:System.Runtime.Serialization.SerializationInfo"/>, содержащий сериализованные данные объекта о выбрасываемом исключении. </param><param name="context">Объект <see cref="T:System.Runtime.Serialization.StreamingContext"/>, содержащий контекстные сведения об источнике или назначении. </param><exception cref="T:System.ArgumentNullException">Параметр <paramref name="info"/>  — указатель NULL (Nothing в Visual Basic). </exception><filterpriority>2</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/></PermissionSet>
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("_ViewName",ViewName);
		}
	}
}