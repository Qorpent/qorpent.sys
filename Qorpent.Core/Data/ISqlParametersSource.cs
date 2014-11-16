using System.Data;

namespace Qorpent.Data {
    /// <summary>
    /// 
    /// </summary>
    public interface ISqlParametersSource {
        /// <summary>
        /// Метод применения SQL аврвсктров к запросу
        /// </summary>
        /// <param name="query"></param>
        void SetupSqlParameters(IDbCommand query);
    }
}