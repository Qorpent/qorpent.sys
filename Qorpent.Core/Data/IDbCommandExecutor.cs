using System.Threading.Tasks;

namespace Qorpent.Data {
    public interface IDbCommandExecutor
    {
        /// <summary>
        /// ���������� ����������� ����� Sql
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<DbCommandWrapper> Execute(DbCommandWrapper info);
    }
}