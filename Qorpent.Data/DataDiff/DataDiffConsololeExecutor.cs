using System.IO;

namespace Qorpent.Data.DataDiff{
	/// <summary>
	/// Имплементация инструмента по обновлению баз данных из метаданных B# посредством XDiff
	/// </summary>
	public class DataDiffConsololeExecutor{
		/// <summary>
		/// Выполняет обновление данных с указанными настройками
		/// </summary>
		/// <param name="parameters"></param>
		public void Execute(DataDiffConsoleParameters parameters){
				var ctx = new TableDiffGeneratorContext{
					RootDirectory =parameters.CheckoutDirectory,
					GitUrl = parameters.RepositoryDirectory,
					ProjectDirectory = parameters.ProjectDirectory,
					OutputDirectory = parameters.ProjectDirectory+".output",
					GitBranch = parameters.Branch,
					ProjectName = parameters.ProjectName,
					SqlConnectionString = parameters.Connection,
					NoApply = !parameters.RegisterInMetaTable &&  !parameters.ApplyToDatabase,
					FullUpdate = parameters.FullUpdate,
					OnlyRegister = !parameters.ApplyToDatabase && parameters.RegisterInMetaTable,
					Log = parameters.Log
				};
				ctx.Log.Info("Start updater");
				new DiffExecutor(ctx).Execute();
				File.WriteAllText(Path.Combine(parameters.CheckoutDirectory,"diff-script.sql"), string.Join("\r\nGO\r\n",ctx.SqlScripts));
				ctx.Log.Info("End updater");
			
		}
	}
}