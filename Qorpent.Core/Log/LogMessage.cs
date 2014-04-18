#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Core/LogMessage.cs
#endregion
using System;
using System.Security.Cryptography;
using System.Text;
using Qorpent.Dsl;
using Qorpent.Mvc;
using Qorpent.Serialization;

namespace Qorpent.Log {
	/// <summary>
	/// 	encapsulate UserLog message with context information
	/// </summary>
	[Serialize]
	public class LogMessage {
		/// <summary>
		/// </summary>
		public LogMessage(){
			Active = true;
			Server = Environment.MachineName;
			Time = DateTime.Now;
			//TODO chacnge user to new current user
			User = Environment.UserDomainName + "/" + Environment.UserName;
		    Id = BitConverter.ToUInt32( MD5.Create().ComputeHash(Guid.NewGuid().ToByteArray()), 0);
		}
		/// <summary>
		/// 
		/// </summary>
		[SerializeNotNullOnly]
		public object Data { get; set; }
		/// <summary>
		/// 	Имя логгера
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 	Минимальный уровень логгирования
		/// </summary>
		public LogLevel Level { get; set; }
		/// <summary>
		/// 	Код стандартного сообщения
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 	StartWrite message itself
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// 	Объект - "держатель" хост журнала, может пониматься как SourceObject, но
		/// 	это зависит от характера использования журнала
		/// </summary>
		public object HostObject { get; set; }
		/// <summary>
		/// 	Имя пользователя
		/// </summary>
		public string User { get; set; }
		/// <summary>
		///     Ошибка, прикреплённая к данному сообщению
		/// </summary>
		public Exception Error { get; set; }
		/// <summary>
		/// 	Имя машины, где было собрано сообщение
		/// </summary>
		public string Server { get; set; }
		/// <summary>
		/// 	Имя приложения, где было собрано сообщение
		/// </summary>
		public string ApplicationName { get; set; }
		/// <summary>
		/// 	Time of message
		/// </summary>
		public DateTime Time { get; set; }
		/// <summary>
		/// 	Lexical info for language - awared messages
		/// </summary>
		[SerializeNotNullOnly] public LexInfo LexInfo { get; set; }

		/// <summary>
		/// 	Information about calling MVC context
		/// </summary>
		[SerializeNotNullOnly] public MvcCallInfo MvcCallInfo {
			get { return _callinfo ?? (_callinfo = (MvcContext == null ? null : MvcContext.GetCallInfo())); }
			set { _callinfo = value; }
		}
		/// <summary>
		/// 	Context of mvc calling
		/// </summary>
		[IgnoreSerialize] public IMvcContext MvcContext { get; set; }

		private  DateTime basis = new DateTime(2014,1,1);
		/// <summary>
		/// 
		/// </summary>
		public int Timestamp{
			get { return (int)((Time - basis).TotalSeconds); }
		}
		/// <summary>
		/// Признак требования на реагирование
		/// </summary>
		public bool RequireAccept { get; set; }

		/// <summary>
		/// Признак обработанного сообщения
		/// </summary>
		public bool Accepted { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long Id { get; set; }
		/// <summary>
		/// Признак активности сообщения
		/// </summary>
		public bool Active { get; set; }

		/// <summary>
		/// 	Generates BXL-like representation of LogMessage
		/// </summary>
		/// <returns> </returns>
		public override string ToString() {
			var sb = new StringBuilder();
			sb.AppendFormat("logitem level={0}, time='{1}', user='{2}', server={3} ", Level, Time.ToString("yyyy-MM-dd HH:mm:ss"), User, Server);
			if (!string.IsNullOrEmpty(Name)) {
				sb.AppendFormat(",logger='{0}' ", Name);
			}
			if (!string.IsNullOrEmpty(Code)) {
				sb.AppendFormat(",code='{0}' ", Code);
			}
			sb.AppendLine();
			if (null != HostObject) {
				sb.AppendFormat("\thost='''" + HostObject + "'''\r\n");
			}

			if (null!=LexInfo && !string.IsNullOrEmpty(LexInfo.File)) {
				sb.AppendFormat("\tlexinfo='{0}'\r\n", LexInfo);
			}
			if (null != MvcCallInfo) {
				sb.AppendFormat("\tmvccallinfo='{0}'\r\n", MvcCallInfo);
			}
			if (!string.IsNullOrEmpty(Message)) {
				sb.AppendLine("\tmessage='''");
				sb.AppendLine(Message);
				sb.AppendLine("\t'''");
			}

			if (null != Error) {
				sb.AppendLine("\terror='''");
				sb.AppendLine(Error.ToString());
				sb.AppendLine("\t'''");
			}


			return sb.ToString();
		}

		private MvcCallInfo _callinfo;
	}
}