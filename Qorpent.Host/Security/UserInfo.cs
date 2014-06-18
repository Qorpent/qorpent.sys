using System;
using System.IO;
using System.Security.Principal;
using Qorpent.IO;

namespace Qorpent.Host.Security{
	/// <summary>
	/// </summary>
	public class UserInfo : IBinarySerializable{
		/// <summary>
		///     Срок действия
		/// </summary>
		public DateTime Expire;

		/// <summary>
		///     Результирующий логин
		/// </summary>
		public string Login;

		/// <summary>
		///     Статус
		/// </summary>
		public bool Ok;

		/// <summary>
		///     Принципал
		/// </summary>
		public IPrincipal Principal;

		/// <summary>
		///     Токен
		/// </summary>
		public string Token;

		/// <summary>
		///     Базовый тип токена
		/// </summary>
		public TokenType Type;

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
		}
	}
}