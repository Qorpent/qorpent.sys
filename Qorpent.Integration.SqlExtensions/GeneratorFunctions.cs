#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : GeneratorFunctions.cs
// Project: Qorpent.SqlExtensions
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;
using Qorpent.Utils.Extensions;
using IOException = Qorpent.IO.IOException;

namespace Qorpent.Integration.SqlExtensions {
	/// <summary>
	/// 	Functions for using generators
	/// 	Generator's subsystem is Qorpent substitution of IDENTITY inspired with postgress/interbase generator objects
	/// 	<remarks>
	/// 		0. Generators uses simple seed/offset/currentvalue model
	/// 		1. Qorpent uses file storage placed under /qorpentsys/sql/generators
	/// 		2. Generators treated not like DATABASE BOUND, but as SYSTEM BOUND to provide cross-databse and cross-service
	/// 		generators scenario - U still can use database-only generators with adding self prefix to generator's code
	/// 		3. U can use qorpent.GeneratorList() function to get table of existed generators
	/// 		4. U can use qorpent.GeneratorCreate() procedure to create new generator
	/// 		5. U can use qorpent.GeneratorNext() function to accuire next value of generator
	/// 		6. U can use qorpent.GeneratorSynchronize() procedure to sync file and db generator's definitions (higher values and offsets will win)
	/// 		usefull for baclkup/restore scenario (files will not be affected by backup), call it regualry and before backup
	/// 		7. U can use qorpent.GeneratorUnlock() alarm procedure to fix problem if generators gain locked (only if problem exists!!!)
	/// 		7. GeneratorNext() uses optimistic-last-win lock model with wait file logic - so it must process consistentry or catch timeout
	/// 	</remarks>
	/// </summary>
	public static class GeneratorFunctions {
		/// <summary>
		/// 	System lock file for using with admin procedures (locks all file system)
		/// </summary>
		public const string SysLockFileName = "syslock";

		/// <summary>
		/// 	Directory where generators placed
		/// </summary>
		public const string GeneratorsDirectory = "/qorpentsys/sql/generators";


		/// <summary>
		/// 	Creates new generator
		/// </summary>
		/// <param name="code"> </param>
		/// <param name="seed"> </param>
		/// <param name="offset"> </param>
		[SqlProcedure]
		public static void GeneratorCreate(SqlString code, SqlInt64 seed, SqlInt32 offset) {
			CheckSystemLock();
			if (code.IsNull) {
				throw new ArgumentException("code");
			}
			if (seed.IsNull) {
				throw new ArgumentException("seed");
			}
			if (offset.IsNull) {
				throw new ArgumentException("offset");
			}

			var filename = Path.Combine(GeneratorsDirectory, code.Value + ".xml");
			if (File.Exists(filename)) {
				throw new IOException("generator with given code already existed");
			}
			using (var f = new FileLock(filename)) {
				f.WriteFile(DefineGeneratorXml(seed.Value, offset.Value).ToString());
			}
			SqlContext.Pipe.Send("generator created sucessfully");
		}

		/// <summary>
		/// 	Generators the next.
		/// </summary>
		/// <param name="code"> The code. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		[SqlFunction]
		public static SqlInt64 GeneratorNext(SqlString code) {
			if (code.IsNull) {
				throw new ArgumentException("code");
			}
			CheckSystemLock();

			var filename = Path.Combine(GeneratorsDirectory, code.Value + ".xml");

			if (!File.Exists(filename)) {
				throw new IOException("generator not existed");
			}
			long result = 0;
			using (var f = new FileLock(filename)) {
				var x = XElement.Parse(f.ReadFile());
				var value = Convert.ToInt64(x.Attr("value"));
				var offset = Convert.ToInt32(x.Attr("offset"));
				result = value + offset;
				x.SetAttributeValue("value", result);
				f.WriteFile(x.ToString());
			}
			return result;
		}

		private static void CheckSystemLock() {
			Directory.CreateDirectory(GeneratorsDirectory);
			var syslocker = Path.Combine(GeneratorsDirectory, SysLockFileName + ".lock");
			if (File.Exists(syslocker)) {
				throw new IOException("generator's file system is locked");
			}
		}

		private static void CheckAnyFileLock() {
			Directory.CreateDirectory(GeneratorsDirectory);
			if (Directory.GetFiles(GeneratorsDirectory, "*.lock").Length != 0) {
				throw new IOException("generators are busy");
			}
		}

		private static XElement DefineGeneratorXml(long seed, int offset) {
			var initialvalue = seed - offset;
			return new XElement("generator", new XAttribute("seed", seed), new XAttribute("offset", offset),
			                    new XAttribute("value", initialvalue));
		}


		/// <summary>
		/// 	returns generator's table
		/// </summary>
		/// <returns> </returns>
		[SqlFunction(DataAccess = DataAccessKind.Read,
			TableDefinition = "code nvarchar(255), seed bigint, offset int, value bigint",
			FillRowMethodName = "GeneratorListFiller")]
		public static IEnumerable GeneratorList() {
			Directory.CreateDirectory(GeneratorsDirectory);
			return Directory.GetFiles(GeneratorsDirectory, "*.xml");
		}

		private static void GeneratorListFiller(object filename, out SqlString code, out SqlInt64 seed, out SqlInt32 offset,
		                                        out SqlInt64 value) {
			var x = XElement.Load((string) filename);
			code = Path.GetFileNameWithoutExtension((string) filename);
			seed = Convert.ToInt64(x.Attr("seed"));
			offset = Convert.ToInt32(x.Attr("offset"));
			value = Convert.ToInt64(x.Attr("value"));
		}

		private static FileLock GetSysLock() {
			CheckAnyFileLock();
			Directory.CreateDirectory(GeneratorsDirectory);
			var filename = Path.Combine(GeneratorsDirectory, SysLockFileName);
			return new FileLock(filename);
		}

		/// <summary>
		/// 	Synchronizes local database table of generators and file storage
		/// </summary>
		[SqlProcedure]
		public static void GeneratorSynchronize() {
			using (var f = GetSysLock()) {
				using (var connection = new SqlConnection("context connection=true")) {
					connection.Open();
					var create_database = connection.CreateCommand();
					create_database.CommandText =
						@"
if object_id('qorpent.GeneratorTable') is null begin
	create table qorpent.GeneratorTable (
		code nvarchar(255) not null primary key,
		seed bigint not null,
		offset int not null,
		value bigint not null
	)
end
";
					create_database.ExecuteNonQuery();
					var generators =
						Directory.GetFiles(GeneratorsDirectory, "*.xml").Select(
							x => new {code = Path.GetFileNameWithoutExtension(x), xml = XElement.Load(x)}).ToDictionary(
								x => x.code, x => x.xml);
					var select_all = connection.CreateCommand();
					select_all.CommandText = "select code,seed,offset,value from qorpent.GeneratorTable";
					using (var r = select_all.ExecuteReader()) {
						while (r.Read()) {
							var code = r.GetString(0);
							var seed = r.GetInt64(1);
							var offset = r.GetInt32(2);
							var value = r.GetInt64(3);
							if (generators.ContainsKey(code)) {
								var generator = generators[code];
								var exoffset = Convert.ToInt32(generator.Attr("offset"));
								var exvalue = Convert.ToInt64(generator.Attr("value"));
								generator.SetAttributeValue("offset", Math.Max(offset, exoffset));
								generator.SetAttributeValue("value", Math.Max(value, exvalue));
							}
							else {
								var generator = DefineGeneratorXml(seed, offset);
								generator.SetAttributeValue("value", value);
								generators[code] = generator;
							}
						}
					}
					var insert_or_update = connection.CreateCommand();
					insert_or_update.CommandText =
						@"
if (select code from qorpent.GeneratorTable where code = @code) is null begin
	insert qorpent.GeneratorTable (code, seed, offset, value) values (@code, @seed, @offset, @value)
end else begin
	update qorpent.GeneratorTable set seed = @seed, offset = @offset, value = @value where code = @code
end
";
					insert_or_update.Parameters.Add(new SqlParameter("@code", SqlDbType.NVarChar));
					insert_or_update.Parameters.Add(new SqlParameter("@seed", SqlDbType.BigInt));
					insert_or_update.Parameters.Add(new SqlParameter("@offset", SqlDbType.Int));
					insert_or_update.Parameters.Add(new SqlParameter("@value", SqlDbType.BigInt));

					foreach (var generator in generators) {
						var filename = Path.Combine(GeneratorsDirectory, generator.Key + ".xml");
						File.WriteAllText(filename, generator.Value.ToString());
						insert_or_update.Parameters["@code"].Value = generator.Key;
						insert_or_update.Parameters["@seed"].Value = Convert.ToInt64(generator.Value.Attr("seed"));
						insert_or_update.Parameters["@offset"].Value = Convert.ToInt32(generator.Value.Attr("offset"));
						insert_or_update.Parameters["@value"].Value = Convert.ToInt64(generator.Value.Attr("value"));
						insert_or_update.ExecuteNonQuery();
					}
				}
			}
		}

		/// <summary>
		/// 	direcly drops all locker files (or just for specified generator)
		/// </summary>
		/// <param name="code"> </param>
		[SqlProcedure]
		public static void GeneratorUnlock(SqlString code) {
			Directory.CreateDirectory(GeneratorsDirectory);
			if (code.IsNull || code.Value.IsEmpty()) {
				foreach (var file in Directory.GetFiles(GeneratorsDirectory, "*.lock")) {
					DropGeneratorLock(file);
				}
			}
			else {
				var filename = Path.Combine(GeneratorsDirectory, code.Value + ".xml.lock");
				DropGeneratorLock(filename);
			}
		}

		private static void DropGeneratorLock(string filename) {
			if (File.Exists(filename)) {
				File.Delete(filename);
			}
		}

		#region Nested type: FileLock

		private class FileLock : IDisposable {
			public FileLock(string filename) {
				this.filename = filename;
				var mylock = Guid.NewGuid().ToString();
				locker = filename + ".lock";
				var trycount = 10;
				while (trycount > 0 && mylock != GetLock(mylock)) {
					trycount--;
				}
				if (0 == trycount) {
					throw new IOException("cannot lock generator file, retry , but may be U need call to GeneratorUnlock to fix it");
				}

				dropblockondispose = true;
			}


			public void Dispose() {
				if (dropblockondispose) {
					File.Delete(locker);
				}
			}


			public string ReadFile() {
				using (var streamreader = new StreamReader(filename, Encoding.UTF8)) {
					return streamreader.ReadToEnd();
				}
			}

			public void WriteFile(string content) {
				using (var streamwriter = new StreamWriter(filename, false, Encoding.UTF8)) {
					streamwriter.Write(content);
				}
			}

			private string GetLock(string mylock) {
				var trycount = 100;
				while (File.Exists(locker) && trycount > 0) {
					trycount--;
					Thread.Sleep(10);
				}
				if (0 == trycount) {
					throw new IOException("cannot wait while lock file have to be free");
				}
				File.WriteAllText(locker, mylock);
				var checkstillmy = File.ReadAllText(locker);
				return checkstillmy;
			}

			private readonly bool dropblockondispose;
			private readonly string filename;
			private readonly string locker;
		}

		#endregion
	}
}