using System;
using Qorpent.IO.DirtyVersion.Mapping;

namespace Qorpent.IO.DirtyVersion {
	/// <summary>
	/// Интерфейс оператора мапинга
	/// </summary>
	public interface IMappingOperator:IDisposable{
		/// <summary>
		/// Целевая информация по мапингу
		/// </summary>
		MappingInfo MappingInfo { get; }

		/// <summary>
		/// Мержит новый коммит в структуру
		/// </summary>
		/// <param name="commit"></param>
		/// <param name="commitHeadBehavior"></param>
		/// <returns></returns>
		Commit Commit(Commit commit, CommitHeadBehavior commitHeadBehavior = CommitHeadBehavior.Auto);

		/// <summary>
		/// Перемещает хид
		/// </summary>
		/// <param name="headHash"></param>
		void MoveHead(string headHash);

		/// <summary>
		/// Устанавливает альяс
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="hash"></param>
		void SetAlias(string alias, string hash);

		/// <summary>
		/// Удаляет альяс
		/// </summary>
		/// <param name="alias"></param>
		void RemoveAlias(string alias);

		/// <summary>
		/// Удаляет коммит и все его зависимости
		/// </summary>
		/// <param name="commitHash"></param>
		/// <param name="deleteHeadBehavior"></param>
		void Delete(string commitHash, DeleteHeadBehavior deleteHeadBehavior = DeleteHeadBehavior.Deny);
		/// <summary>
		/// Отменяет изменения в сессии
		/// </summary>
		void Cancel();

		/// <summary>
		/// 
		/// </summary>
		void Commit();

		/// <summary>
		/// Возвращает хэш хида
		/// </summary>
		/// <returns></returns>
		string GetHeadHash();

		/// <summary>
		/// Осуществляет разрешение коммитов
		/// </summary>
		/// <param name="hash"></param>
		/// <returns></returns>
		Commit Resolve(string hash);
	}
}