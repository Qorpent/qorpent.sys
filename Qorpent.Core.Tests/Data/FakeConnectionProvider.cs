using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Qorpent.Data;

namespace Qorpent.Core.Tests.Data {
    public class FakeConnectionProvider:IDatabaseConnectionProvider {
        public readonly IDictionary<string,string> Registry = new Dictionary<string, string>();

        public IDbConnection GetConnection(string name, string defaultConnectionString = null) {
            var connection = GetConnectionString(name);
            if (string.IsNullOrWhiteSpace(connection)) return null;
            return new SqlConnection(connection);
        }

        public string GetConnectionString(string name, string defaultConnectionString = null) {
            var connection = "Server=(local);Database=" + name + ";Trusted_Connection=true";
            if (Registry.ContainsKey(name)) connection = Registry[name];
            return connection;
        }

        public void Register(ConnectionDescriptor connectionDescriptor, bool persistent) {
            throw new NotImplementedException();
        }

        public void UnRegister(string name, bool persistent) {
            throw new NotImplementedException();
        }

        public bool Exists(string name) {
            throw new NotImplementedException();
        }

        public IEnumerable<ConnectionDescriptor> Enlist() {
            throw new NotImplementedException();
        }
    }
}