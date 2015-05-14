using System;
using System.IO;
using System.Security.Principal;
using Qorpent.IO;
using Qorpent.Serialization;

namespace Qorpent.Host.Security{
	/// <summary>
	/// </summary>
	[Serialize]
    public class UserInfo : IBinarySerializable{
		/// <summary>
		///     Срок действия
		/// </summary>
		[SerializeNotNullOnly]
        public DateTime Expire;

		/// <summary>
		///     Результирующий логин
		/// </summary>
		[SerializeNotNullOnly]
        public string Login;

		/// <summary>
		///     Статус
		/// </summary>
		[IgnoreSerialize]
        public bool Ok;

		/// <summary>
		///     Принципал
		/// </summary>
		[IgnoreSerialize]
        public IPrincipal Principal;

		/// <summary>
		///     Токен
		/// </summary>
		[IgnoreSerialize]
        public string Token;

		/// <summary>
		///     Базовый тип токена
		/// </summary>
		[SerializeNotNullOnly]
        public TokenType Type;
        [SerializeNotNullOnly]
	    public DateTime LoginTime { get; set; }
        [IgnoreSerialize]
	    public string UserAgent { get; set; }
        [IgnoreSerialize]
	    public string LocalAddress { get; set; }
        [IgnoreSerialize]
	    public string RemoteAddress { get; set; }

	    /// <summary>
		///     Считать объект из ридера
		/// </summary>
		/// <param name="reader"></param>
		public void Read(BinaryReader reader){
			Ok = reader.ReadBoolean();
			if (Ok){
				Login = reader.ReadString();
				Token = reader.ReadString();
				Expire = DateTime.FromOADate(reader.ReadDouble());
				Type = (TokenType) reader.ReadByte();
			    LoginTime = DateTime.FromOADate(reader.ReadDouble());
			    UserAgent = reader.ReadString();
			    LocalAddress = reader.ReadString();
			    RemoteAddress = reader.ReadString();

			}
		}

		/// <summary>
		///     Записать объект в райтер
		/// </summary>
		/// <param name="writer"></param>
		public void Write(BinaryWriter writer){
			writer.Write(Ok);
			writer.Write(Login);
			writer.Write(Token);
			writer.Write(Expire.ToOADate());
			writer.Write((byte) Type);
            writer.Write(LoginTime.ToOADate());
            writer.Write(UserAgent);
            writer.Write(LocalAddress);
            writer.Write(RemoteAddress);
		}
	}
}