using System.Data;

namespace Qorpent.Data {
    /// <summary>
    /// 
    /// </summary>
    public interface ISqlParametersSource {
        /// <summary>
        /// ����� ���������� SQL ���������� � �������
        /// </summary>
        /// <param name="query"></param>
        void SetupSqlParameters(IDbCommand query);
    }
}