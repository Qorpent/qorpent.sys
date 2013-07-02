namespace Qorpent.Mvc {
    /// <summary>
    ///     
    /// </summary>
    public interface IClientStatStorage {
        /// <summary>
        ///     Write statistics to MongoDB as a JSON string
        /// </summary>
        /// <param name="jsonStat">a json-string</param>
        void Write(string jsonStat);
    }
}
