namespace Qorpent.MongoDBIntegration {
    /// <summary>
    ///     Default for MongoDB connector
    /// </summary>
    public abstract class MongoDbDefaults {
        /// <summary>
        ///     Default database name
        /// </summary>
        public const string DatabaseName = "TestDb";

        /// <summary>
        ///     Default collection name
        /// </summary>
        public const string CollectionName = "TestCollection";

        /// <summary>
        ///     Default connection string
        /// </summary>
        public const string ConnectionString = "mongodb://localhost";
    }
}
