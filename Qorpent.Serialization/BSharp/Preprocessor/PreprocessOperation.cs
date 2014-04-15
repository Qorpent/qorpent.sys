using System;
using System.Xml.Linq;
using Qorpent.BSharp.Builder;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// 
	/// </summary>
	public abstract class PreprocessOperation{
		/// <summary>
		/// 
		/// </summary>
		protected IBSharpProject _project;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="project"></param>
		public PreprocessOperation InitProject(IBSharpProject project){
			_project = project;
			return this;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="project"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static PreprocessOperation Create(IBSharpProject project, XElement e){
			
			switch (e.Name.LocalName){
				case "renameattribute" :
					return e.Apply(new RenameAttributeOperation());
				case "renameelement":
					return e.Apply(new RenameElementOperation());
				case "stringreplace":
					return e.Apply(new StringReplaceOperation());
				case "cleanupelement":
					return e.Apply(new CleanupElementOperation());
				case "setattribute":
					return e.Apply(new SetAttributeOperation());
				case "elementtoattribute":
					return e.Apply(new ElementToAttributeOperation());
				case "pushtoglobal":
					return e.Apply(new PushToGlobalOperation().InitProject(project));
				case "bindglobal":
					return e.Apply(new BindGlobalOperation().InitProject(project));
				case "executegenerator":
					return e.Apply(new ExecuteCodeGeneratorOperation().InitProject(project));
				default:
					throw new Exception("unkonown operation "+e.Name.LocalName);			
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="el"></param>
		public abstract void Execute(XElement el);
	}
}