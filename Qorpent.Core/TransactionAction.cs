using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent {
	/// <summary>
	/// Облегченная структура для действий в рамках транзакций
	/// </summary>
	public abstract class TransactionAction
	{
		/// <summary>
		/// Производит выполнение операции обновления (может генерировать ошибки)
		/// </summary>
		public abstract void Execute(IScope context);

		/// <summary>
		/// Выполняет операцию применения (для транзакций)
		/// </summary>
		/// <param name="context"></param>
		public abstract void Commit(IScope context);
		/// <summary>
		/// Осуществляет откат
		/// </summary>
		/// <param name="context"></param>
		public abstract void Rollback(IScope context);
	}
	/// <summary>
	/// Расширение для работы с транзакционными действиями
	/// </summary>
	public static class TransactionActionExtension {
		/// <summary>
		/// Выполняет транзакцию
		/// </summary>
		/// <param name="actions"></param>
		/// <param name="context"></param>
		public static void ExecuteTransaction(this IEnumerable<TransactionAction> actions, IScope context = null) {
			if (actions == null) throw new ArgumentNullException("actions");
			actions = actions.ToArray();
			context = context ?? new Scope();
			try
			{
				foreach (var a in actions)
				{
					a.Execute(context);
				}
				//если все выполнилось успешно - можно коммитить
				foreach (var a in actions)
				{
					a.Commit(context);
				}
			}
			catch
			{
				foreach (var a in actions)
				{
					a.Rollback(context);
				}
			}
		}

		/// <summary>
		/// Выполняет транзакцию в режиме имитации
		/// </summary>
		/// <param name="actions"></param>
		/// <param name="context"></param>
		public static void Imitate(this IEnumerable<TransactionAction> actions,IScope context = null) {
			if (actions == null) throw new ArgumentNullException("actions");
			context = context ?? new Scope();
			try
			{
				foreach (var a in actions)
				{
					a.Execute(context);
				}
			}
			finally 
			{
				foreach (var a in actions)
				{
					a.Rollback(context);
				}
			}
		}
	}
}