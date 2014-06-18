using System;
using System.IO;
using Qorpent.IO;

namespace Qorpent.Host.Security{
	/// <summary>
	///     Протокол обмена данными аутентификации по SimpleSocket
	/// </summary>
	public class AuthProtocol : IBinarySerializable{
		/// <summary>
		/// </summary>
		public const int DefaultPort = 10511;

		private readonly DateTime basis = new DateTime(2014, 1, 1).ToUniversalTime();

		/// <summary>
		///     Код ошибки
		/// </summary>
		public int ErrorCode;

		/// <summary>
		///     Текст ошибки
		/// </summary>
		public string ErrorStatus;

		/// <summary>
		///     Срок действия
		/// </summary>
		public DateTime Expire;

		/// <summary>
		///     Логин
		/// </summary>
		public string Login;

		/// <summary>
		///     Пароль или дайджест
		/// </summary>
		public string PassOrDigest;

		/// <summary>
		///     Запрос
		/// </summary>
		public AuthProtocolRequestType Request;

		/// <summary>
		///     Отклик
		/// </summary>
		public AuthProtocolResponseType Response;

		/// <summary>
		/// </summary>
		public AuthProtocolStatus State;

		/// <summary>
		///     Сигнатура
		/// </summary>
		public byte[] Sygnature;

		/// <summary>
		///     Токен
		/// </summary>
		public string Token;

		void IBinarySerializable.Read(BinaryReader reader){
			byte type = reader.ReadByte();
			if (0 == type){
				ReadRequest(reader);
			}
			else{
				ReadResponse(reader);
			}
		}

		void IBinarySerializable.Write(BinaryWriter writer){
			if (AuthProtocolRequestType.None != Request){
				WriteRequest(writer);
			}
			else{
				WriteResponse(writer);
			}
		}

		private void ReadResponse(BinaryReader reader){
			Response = (AuthProtocolResponseType) reader.ReadByte();
			if (Response.HasFlag(AuthProtocolResponseType.True)){
				if (Response.HasFlag(AuthProtocolResponseType.Sygnature)){
					int length = reader.ReadInt32();
					Sygnature = reader.ReadBytes(length);
				}
				else if (Response.HasFlag(AuthProtocolResponseType.State)){
					State = (AuthProtocolStatus) reader.ReadByte();
				}
				else if (Response.HasFlag(AuthProtocolResponseType.Token)){
					Login = reader.ReadString();
					Token = reader.ReadString();
					int minutes = reader.ReadInt32();
					Expire = basis.AddMinutes(minutes).ToLocalTime();
				}
			}
			else{
				if (Response.HasFlag(AuthProtocolResponseType.Error)){
					ErrorCode = reader.ReadInt32();
					ErrorStatus = reader.ReadString();
				}
			}
		}

		private void ReadRequest(BinaryReader reader){
			Request = (AuthProtocolRequestType) reader.ReadByte();
			if (0 != (Request & AuthProtocolRequestType.Auth)){
				Login = reader.ReadString();
				PassOrDigest = reader.ReadString();
			}
			else if (0 != (Request & AuthProtocolRequestType.Token)){
				Token = reader.ReadString();
			}
		}

		private void WriteResponse(BinaryWriter writer){
			writer.Write((byte) 1);
			writer.Write((byte) Response);
			if (Response.HasFlag(AuthProtocolResponseType.True)){
				if (Response.HasFlag(AuthProtocolResponseType.Sygnature)){
					writer.Write(Sygnature.Length);
					writer.Write(Sygnature);
				}
				else if (Response.HasFlag(AuthProtocolResponseType.State)){
					writer.Write((byte) State);
				}
				else if (Response.HasFlag(AuthProtocolResponseType.Token)){
					writer.Write(Login);
					writer.Write(Token);
					writer.Write((int) ((Expire.ToUniversalTime() - basis).TotalMinutes));
				}
			}
			else{
				if (Response.HasFlag(AuthProtocolResponseType.Error)){
					writer.Write(ErrorCode);
					writer.Write(ErrorStatus);
				}
			}
		}

		private void WriteRequest(BinaryWriter writer){
			writer.Write((byte) 0);
			writer.Write((byte) Request);
			if (0 != (Request & AuthProtocolRequestType.Auth)){
				writer.Write(Login);
				writer.Write(PassOrDigest);
			}
			else if (0 != (Request & AuthProtocolRequestType.Token)){
				writer.Write(Token);
			}
		}
	}
}