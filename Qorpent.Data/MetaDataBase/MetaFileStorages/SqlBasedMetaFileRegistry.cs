using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// 
	/// </summary>
	public class SqlBasedMetaFileRegistry: MetaFileRegistryBase{
		/// <summary>
		/// 
		/// </summary>
		public SqlBasedMetaFileRegistry(){
			
		}
		/// <summary>
		/// 
		/// </summary>
		public string ConnectionString { get; set; }

		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		private void InDb(Action<IDbConnection> action){
			using (var c = Container.Get<IDatabaseConnectionProvider>().GetConnection(ConnectionString)){
				c.Open();
				action(c);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public override MetaFileDescriptor GetByRevision(MetaFileDescriptor descriptor){
			MetaFileDescriptor[] result = null;
			InDb(c => result = c.ExecuteOrm<MetaFileDescriptor>(@"
select MDFileCode as Code,MDFileName as Name,Content as Content,Comment,Revision as Revision,RevisionTime as RevisionTime,
Hash as Hash , UserName from [qptmds].[MDFileContentFull] where MDFileCode=@Code and Revision = @Revision", new {descriptor.Code,descriptor.Revision}));
			return result.FirstOrDefault();
		}

		/// <summary>
		/// Прочитать набор кодов
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<string> GetCodes(string prefix = null){
			string[] result = null;
			prefix = prefix ?? "";
			InDb(c => result = c.ExecuteList<string>("select Code from qptmds.MDFile where ''=@prefix or  Code like  @prefix+'%' ",new{prefix}).ToArray());
			return result;
		}

		/// <summary>
		/// Проверяет наличие файла с кодом
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public override bool Exists(string code){
			int result = 0;
			InDb(c => result = c.ExecuteScalar<int>("select Id from qptmds.MDFile where Code = @code",new{code}));
			return 0 != result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="savedescriptor"></param>
		protected override void InternalRegister(MetaFileDescriptor savedescriptor){
			const string commandText =
				"exec [qptmds].[MDFileRegister] @code=@Code, @name=@Name, @content=@Content, @hash=@Hash, @revision=@Revision, @filetime=@RevisionTime, @comment=@Comment, @username = @UserName";
			InDb(c => c.ExecuteNonQuery(commandText, savedescriptor));
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public override MetaFileDescriptor Checkout(MetaFileDescriptor descriptor){
			const string commandText =
				"exec [qptmds].[MDFileContentCheckout] @code=@Code, @revision=@Revision";
			InDb(c => c.ExecuteNonQuery(commandText, descriptor));
			return descriptor;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public override IEnumerable<RevisionDescriptor> GetRevisions(string code){
			RevisionDescriptor[] result = null;
			InDb(c => result = c.ExecuteOrm<RevisionDescriptor>("select Revision,RevisionTime,Hash from [qptmds].[MDFileContentFull] where MDFileCode=@code order by RevisionTime desc",new{code}));
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public override MetaFileDescriptor GetCurrent(string code){
			MetaFileDescriptor[] result = null;
			InDb(c => result = c.ExecuteOrm<MetaFileDescriptor>(@"
select Code,Name,ActiveRevisionContent as Content,ActiveRevisionUserName as UserName, ActiveRevisionRevision as Revision,ActiveRevisionComment as Comment,ActiveRevisionRevisionTime as RevisionTime,
ActiveRevisionHash as Hash from [qptmds].[MDFileFull] where Code=@code", new { code }));
			return result.FirstOrDefault();
		}
	}
}