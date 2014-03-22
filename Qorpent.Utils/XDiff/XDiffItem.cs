using System;
using System.Xml.Linq;

namespace Qorpent.Utils.XDiff{
	/// <summary>
	/// Элементарная единица изменения
	/// </summary>
	public class XDiffItem{
		/// <summary>
		/// Действие
		/// </summary>
		public XDiffAction Action { get; set; }
		/// <summary>
		/// Исходный элемент
		/// </summary>
		public XElement BasisElement { get; set; }
		/// <summary>
		/// Измененный элемент
		/// </summary>
		public XElement NewestElement { get; set; }
		/// <summary>
		/// Исходный атрибут
		/// </summary>
		public XAttribute BasisAttribute { get; set; }
		/// <summary>
		/// Целевой атрибут
		/// </summary>
		public XAttribute NewestAttribute { get; set; }
		/// <summary>
		/// Новое значение для элемента
		/// </summary>
		public string NewValue { get; set; }
		/// <summary>
		/// Применяет разницу к исходному элементу
		/// </summary>
		public XElement Apply(XElement target, bool throwOnError = true){
			switch (Action){
					case XDiffAction.CreateElement:
						return DoCreateElement(target,throwOnError);
					case XDiffAction.DeleteElement:
						return DoDeleteElement(target,throwOnError);
					case XDiffAction.ChangeElement:
						return DoChangeElement(target, throwOnError);
					case XDiffAction.RenameElement:
						return DoRenameElement(target, throwOnError);
					case XDiffAction.CreateAttribute:
						return DoCreateAttribute(target, throwOnError);
					case XDiffAction.DeleteAttribute:
						return DoDeleteAttribute(target, throwOnError);
					case XDiffAction.ChangeAttribute:
						return DoChangeAttribute(target, throwOnError);
			}
			return target;
		}

		

		private XElement DoChangeAttribute(XElement target, bool throwOnError){
			//throw new NotImplementedException();
			return target;
		}

		private XElement DoDeleteAttribute(XElement target, bool throwOnError){
			//throw new NotImplementedException();
			return target;
		}

		private XElement DoCreateAttribute(XElement target, bool throwOnError){
			//throw new NotImplementedException();
			return target;
		}

		private XElement DoRenameElement(XElement target, bool throwOnError){
			//throw new NotImplementedException();
			return target;
		}

		private XElement DoChangeElement(XElement target, bool throwOnError){
			//throw new NotImplementedException();
			return target;
		}

		private XElement DoDeleteElement(XElement target, bool throwOnError){
			//throw new NotImplementedException();
			return target;
		}

		private XElement DoCreateElement(XElement target, bool throwOnError){
			//throw new NotImplementedException();
			return target;
		}
	}
}