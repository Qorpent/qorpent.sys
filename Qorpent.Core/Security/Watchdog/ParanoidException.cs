using System;

namespace Qorpent.Security.Watchdog {
	/// <summary>
	/// Any exception in paranoid environment
	/// </summary>
	[Serializable]
	public class ParanoidException : QorpentSecurityException {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		public ParanoidException(ParanoidState state):base(state.ToString()) {
			this.State = state;
		}

		/// <summary>
		/// State of paranoid
		/// </summary>
		public ParanoidState State { get;private set; }

		/// <summary>
		/// ��� ��������������� � ����������� ������ ������ �������� �� ���������� ��� <see cref="T:System.Runtime.Serialization.SerializationInfo"/>.
		/// </summary>
		/// <param name="info">������ <see cref="T:System.Runtime.Serialization.SerializationInfo"/>, ���������� ��������������� ������ ������� � ������������� ����������. </param><param name="context">������ <see cref="T:System.Runtime.Serialization.StreamingContext"/>, ���������� ����������� �������� �� ��������� ��� ����������. </param><exception cref="T:System.ArgumentNullException">�������� <paramref name="info"/> �� ��������� NULL (Nothing � Visual Basic). </exception><filterpriority>2</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/></PermissionSet>
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("_State",State);
		}
	}
}