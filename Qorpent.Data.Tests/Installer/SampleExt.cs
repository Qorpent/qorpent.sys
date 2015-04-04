using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace Qorpent.Data.Tests.Installer {
    public static class SampleExt {
        [SqlFunction]
        public static SqlInt32 Test(SqlInt32 id) {
            if (id.IsNull) {
                return -1;
            }
            return id.Value*4;
        }
    }
}