using System;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	public abstract class PreprocessOperation{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static PreprocessOperation Create(XElement e){
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