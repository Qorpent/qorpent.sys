using System.Collections.Generic;

namespace Qorpent.Log{
	/// <summary>
	/// Записывает сообщения в коллекцию в случае, если они соответствуют уровню
	/// </summary>
	public class CollectionBasedUserLog:LoggerBasedUserLog
	{
		/// <summary>
		/// /
		/// </summary>
		/// <param name="loggers"></param>
		/// <param name="manager"></param>
		/// <param name="name"></param>
		public CollectionBasedUserLog(IEnumerable<ILogger> loggers, ILogManager manager, string name) : base(loggers, manager, name){
			Messages = new List<LogMessage>();
		}
		/// <summary>
		/// 
		/// </summary>
		public CollectionBasedUserLog():this(null,null,null){
		}
		/// <summary>
		/// 
		/// </summary>
		public override LogLevel Level { get; set; }
		/// <summary>
		/// Заловленные события
		/// </summary>
		public IList<LogMessage> Messages { get; private set; } 
		/// <summary>
		/// 
		/// </summary>
		/// <param name="logmessage"></param>
		public override void Write(LogMessage logmessage)
		{
			base.Write(logmessage);
			if (logmessage.Level >= this.Level){
				Messages.Add(logmessage);
			}
			
		}
	}
}