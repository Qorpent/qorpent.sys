using System.Threading.Tasks;

namespace Qorpent.Data {
    public interface ISqlQueryExecutor
    {
        /// <summary>
        /// ���������� ����������� ����� Sql
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<SqlCallInfo> Execute(SqlCallInfo info);
    }
}