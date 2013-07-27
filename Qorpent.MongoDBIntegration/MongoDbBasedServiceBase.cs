#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Zeta.Extreme.Form/StateRule.cs
#endregion

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Qorpent.MongoDBIntegration {
	/// <summary>
	/// Базовый класс для служб, основанных на Mongo
	/// </summary>
	public class MongoDbBasedServiceBase : ServiceBase, IMongoDbConnector {
		        private MongoClient _client;
        private MongoServer _server;
        private MongoDatabase _database;
        private MongoGridFS _gridFs;

        private string _connectionString;
        private string _databaseName;
        private string _collectionName;

        /// <summary>
        ///     MongoDB settings: db, client and GridFS
        /// </summary>
        private MongoDatabaseSettings _databaseSettings;

        /// <summary>
        ///     MongoDB GridFS settings
        /// </summary>
        private MongoGridFSSettings _gridFsSettings;

        /// <summary>
        ///     MongoDB Collection
        /// </summary>
        private MongoCollection<BsonDocument> _collection;

        /// <summary>
        ///     The database name you want to use to store attachements
        /// </summary>
        public string DatabaseName {
            get { return _databaseName ?? (_databaseName = MongoDbDefaults.DatabaseName); }
            set { _databaseName = value; }
        }

        /// <summary>
        ///     connection string
        /// </summary>
        public string ConnectionString {
            get { return _connectionString ?? (_connectionString = MongoDbDefaults.ConnectionString); }
            set { _connectionString = value; }
        }

        /// <summary>
        ///     Collection name
        /// </summary>
        public string CollectionName {
            get { return _collectionName ?? (_collectionName = MongoDbDefaults.CollectionName); }
            set { _collectionName = value; }
        }

        /// <summary>
        ///     MongoDB database setting
        /// </summary>
        public MongoDatabaseSettings DatabaseSettings {
            get {
                return _databaseSettings ?? (_databaseSettings = new MongoDatabaseSettings());
            }

            set { _databaseSettings = value; }
        }

        /// <summary>
        ///     MongoDB GridFS settings
        /// </summary>
        public MongoGridFSSettings GridFsSettings {
            get { return _gridFsSettings ?? (_gridFsSettings = new MongoGridFSSettings()); }
            set { _gridFsSettings = value; }
        }

        /// <summary>
        ///     MongoDB Client
        /// </summary>
        public MongoClient Client {
            get { return _client ?? (_client = ClientSetup()); }
            protected set { _client = value; }
        }

        /// <summary>
        ///     MongoDB Server
        /// </summary>
        public MongoServer Server {
            get { return _server ?? (_server = ServerSetup()); }
            protected set { _server = value; }
        }

        /// <summary>
        ///     MongoDB Database connection link
        /// </summary>
        public MongoDatabase Database {
            get { return _database ?? (_database = DatabaseSetup()); }
            protected set { _database = value; }
        }

        /// <summary>
        ///     MongoDB Collection link
        /// </summary>
        public MongoCollection<BsonDocument> Collection {
            get { return _collection ?? (_collection = CollectionSetup()); }
            protected set { _collection = value; }
        }

        /// <summary>
        ///     MongoDB GridFS connection link
        /// </summary>
        public MongoGridFS GridFs {
            get { return _gridFs ?? (_gridFs = GridFsSetup()); }
            protected set { _gridFs = value; }
        }

        /// <summary>
        ///     Client setup
        /// </summary>
        /// <returns></returns>
        private MongoClient ClientSetup() {
            return new MongoClient(ConnectionString);
        }

        /// <summary>
        ///     Gets server
        /// </summary>
        /// <returns></returns>
        private MongoServer ServerSetup() {
            return Client.GetServer();
        }

        /// <summary>
        ///     Gets database    
        /// </summary>
        /// <returns></returns>
        private MongoDatabase DatabaseSetup() {
            return Server.GetDatabase(
                DatabaseName,
                DatabaseSettings
            );
        }

        private MongoCollection<BsonDocument> CollectionSetup() {
            return Database.GetCollection(CollectionName);
        }

        /// <summary>
        ///     GridFS connection setup
        /// </summary>
        public MongoGridFS GridFsSetup() {
            var gridFs = new MongoGridFS(
                Database,
                GridFsSettings
            );

            GridFsEnsureIndexesAreActual(
                gridFs,
                new IndexKeysDocument {
                    {"files_id", "n"}
                }
            );

            return gridFs;
        }

        /// <summary>
        ///     MongoDB.Driver «bonus» fixing
        /// </summary>
        public void GridFsEnsureIndexesAreActual(MongoGridFS gridFs, IndexKeysDocument keys) {
            if (!gridFs.Chunks.IndexExists(keys)) {
                gridFs.Chunks.ResetIndexCache();
            }
        }
	}
}