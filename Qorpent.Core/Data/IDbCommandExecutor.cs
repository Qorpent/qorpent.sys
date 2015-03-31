using System.Threading.Tasks;

namespace Qorpent.Data {
    public interface IDbCommandExecutor
    {
        /// <summary>
        /// Производит асинхронный вызов Sql
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<DbCommandWrapper> Execute(DbCommandWrapper info);
    }
}