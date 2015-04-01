using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.CodeWriters;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm {
    [TestFixture]
    public class CacheWriterTest {
        [Test]
        public void SqlCallsWrapper()
        {
            var code = @"
require data
class a prototype=dbtable 
    void X 'Bla-bla' cs-wrap=scalar cs-return=long
        @id=long : (
            SELECT 1;
        )
    void Y 'Ta-da' cs-wrap=multiplereader 
        @id=long : (
            SELECT 1;
            SELECT 2;
        )";
            var model = PersistentModel.Compile(code);
            var table = model["a"];
            var function = table.GetObject<SqlFunction>("X");
            Assert.NotNull(function);
            var writer = new PokoObjectCacheWriter(table)
            {
                DoWriteClassWrapper = false,
                DoWriteModelLink = false,
                DoWriteSqlMethods = false,
                DoWriteTableQuery = false,
                DoWriteObjectWrappers = true
                
            };
            var result = writer.ToString();
            Console.WriteLine(result);
            var simpleResult = Regex.Replace(result, @"\s", "").Replace("\"","'");
            Assert.AreEqual(simpleResult, Regex.Replace(@"		private DbCommandWrapper _XWrapper = new DbCommandWrapper{ObjectName='dbo.aX',Notation=DbCallNotation.Scalar};
		///<summary>Bla-bla</summary>
		public Int64 XGeneric (object parameters) {
			var command = _XWrapper.Clone(parameters,GetConnection());
			var result = DbCommandExecutor.Default.GetResultSync(command);
			var convertedResult = result.To<Int64>();
			return convertedResult;
		}
		///<summary>Bla-bla</summary>
		public Int64 X (Int64 id = default(Int64)){
			return XGeneric (new{id});
		}
		private DbCommandWrapper _YWrapper = new DbCommandWrapper{ObjectName='dbo.aY',Notation=DbCallNotation.MultipleReader};
		///<summary>Ta-da</summary>
		public object YGeneric (object parameters) {
			var command = _YWrapper.Clone(parameters,GetConnection());
			var result = DbCommandExecutor.Default.GetResultSync(command);
			return result;
		}
		///<summary>Ta-da</summary>
		public object Y (Int64 id = default(Int64)){
			return YGeneric (new{id});
		}", @"\s", ""));
        }
    }
}