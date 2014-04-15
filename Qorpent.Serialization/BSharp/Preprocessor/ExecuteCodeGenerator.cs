using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// 
	/// </summary>
	internal class ExecuteCodeGeneratorOperation : PreprocessOperation
	{
		public override void Execute(XElement el){
			var code = el.Attr("code");
			var type = Type.GetType(code,false);
			if (null == type){
				_project.Log.Error("Не могу найти расширение - генератор с именем "+code);
				return;
			}
			var gen = Activator.CreateInstance(type) as ISourceCodeGenerator;
			if (null == gen){
				_project.Log.Error("Указанный класс " + code + " не соответствует интерфейсу ISourceCodeGenerator");
				return;
			}
			IEnumerable<XNode> replaces = null;
			try{
				replaces = gen.Execute(_project, el, null).ToArray();
			}
			catch (Exception ex){
				_project.Log.Error("Ошибка при вызове "+gen.GetType().Name+" на "+el,ex);
				return;
			}
			if (!replaces.Any()){
				el.Remove();
			}
			else{
				el.ReplaceWith((object[])replaces.OfType<object>().ToArray());
			}
		}
	}
}