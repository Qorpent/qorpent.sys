using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Log
{
	/// <summary>
	/// Специальный класс, реализующий динамический журнал для взаимодействия приложений и веб-среды
	/// </summary>
	public class SessionLog
	{
		ConcurrentDictionary<string, SessionLogTypeConfiguration> _config = new ConcurrentDictionary<string, SessionLogTypeConfiguration>();
		ConcurrentBag<LogMessage> _log = new ConcurrentBag<LogMessage>();  
		/// <summary>
		/// Регистрирует настройку для типа сообщений
		/// </summary>
		/// <param name="configuration"></param>
		public void Register(SessionLogTypeConfiguration configuration){
			_config[configuration.Code] = configuration;
		}
		/// <summary>
		/// Регистрирует сообщения
		/// </summary>
		/// <param name="message"></param>
		public LogMessage Register(LogMessage message){
			if (_config.ContainsKey(message.Code)){
				return ProcessConfigured(message,_config[message.Code]);
			}
			else{
				_log.Add(message);
				return message;
			}
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        public void Accept(string code) {
			if(!_config.ContainsKey(code))throw new Exception("cannot accept not configured message");
	        var _existed = Get(code);
	        if (null == _existed) return;
	        _existed.Accepted = true;
			_existed.Level = LogLevel.Trace;
	        var cfg = _config[code];
			if (null != cfg.TriggerAccept){
				foreach (var c in cfg.TriggerAccept){
					if (_config.ContainsKey(c)){
						Accept(c);
					}
				}
			}
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public void Accept(int id){
			var m = _log.FirstOrDefault(_ => _.Id == id);
			if (null != m){
				m.Accepted = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public LogMessage Get(string code){
			return _log.FirstOrDefault(_ => _.Code == code && _.Active);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<LogMessage> Get(SessionLogQuery query= null){
			query = query?? new SessionLogQuery();
			foreach (var message in _log){
				if (!message.Active) continue;
				if (message.Level < query.StartLevel) continue;
				if (!string.IsNullOrWhiteSpace(query.Code) && query.Code != message.Code) continue;
				if (0 != query.StartTimestamp){
					if(message.Timestamp<=query.StartTimestamp)continue;
				}
				if (query.OnlyAccepted || query.OnlyNotAccepted || query.OnlyRequests){
					if(!message.RequireAccept)continue;
					if (query.OnlyAccepted && !message.Accepted) continue;
					if (query.OnlyNotAccepted && message.Accepted) continue;
				}
				yield return message;
			}
		} 
		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		public void Clear(SessionLogClearQuery query = null){
			query = query??new SessionLogClearQuery();
			var newlog = new ConcurrentBag<LogMessage>();
			foreach (var message in _log){
				if (message.RequireAccept){
					if (query.KeepAllRequests){
						newlog.Add(message);
						continue;
					}
					if (query.KeepAccpeted || !message.Accepted){
						newlog.Add(message);
						continue;
					}
				}
				if(message.Level>query.MaxLevel)continue;
				if(0!=query.EndTimestamp && message.Timestamp<=query.EndTimestamp)continue;
				
				newlog.Add(message);
			}
			_log = newlog;
		}
		
		private LogMessage ProcessConfigured(LogMessage message, SessionLogTypeConfiguration cfg){
			lock (this){
				var realmessage = _log.FirstOrDefault(
					_ =>
					_.Code == message.Code &&
					(cfg.IsSingleton || (cfg.RequireAccept && !_.Accepted)));

				if (null == realmessage){
					_log.Add(message);
					realmessage = message;
				}
				else{
					bool changed = !string.IsNullOrWhiteSpace(message.Message) && (message.Message != realmessage.Message);
					changed = changed || message.Level != realmessage.Level;
					changed = changed || message.ETag != realmessage.ETag;
					if (!changed){
						if (null != message.Data && null != realmessage.Data){
							if (null != cfg.CustomComparer){
								changed = !cfg.CustomComparer.Equals(message.Data, realmessage.Data);
							}
							else{
								changed = !message.Data.Equals(realmessage.Data);
							}
						}
						else{
							if (null != message.Data || null != realmessage.Data){
								changed = true;
							}
						}
					}
					if (!changed) return realmessage;
					if (message.Time > realmessage.Time && cfg.UpgradeTime){
						realmessage.Time = message.Time;
					}
					realmessage.Level = message.Level;
					if (!string.IsNullOrWhiteSpace(message.Message)){
						realmessage.Message = message.Message;
					}
					realmessage.ETag = message.ETag;
					realmessage.Accepted = message.Accepted;

					realmessage.Data = message.Data;
				}

				if (cfg.RequireAccept){
					realmessage.RequireAccept = true;
				}
				if (realmessage.Level < cfg.AutoAcceptBelow){
					realmessage.Accepted = true;
				}
				if (realmessage.Level < cfg.AutoRemoveBelow){
					realmessage.Active = false;
				}

				return realmessage;
			}
		}
	}
}
