using System;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

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
		/// Признак возможной смены кода в иерархии
		/// </summary>
		public bool CanChangeHierarchyTarget{
			get {
				if (Action == XDiffAction.RenameElement) return true;
				if (Action == XDiffAction.ChangeAttribute){
					if (null == NewestAttribute) return false;
					if (NewestAttribute.Name.LocalName == "id") return true;
					if (NewestAttribute.Name.LocalName == "code") return true;
				}
				return false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public XDiffOptions Options { get; set; }

		/// <summary>
		/// Применяет разницу к исходному элементу
		/// </summary>
		public XElement Apply(XElement target, bool throwOnError = true, XDiffOptions options = null){
			options = options ?? new XDiffOptions();
			switch (Action){
					case XDiffAction.CreateElement:
						return DoCreateElement(target,throwOnError,options);
					case XDiffAction.DeleteElement:
						return DoDeleteElement(target, throwOnError, options);
					case XDiffAction.ChangeElement:
						return DoChangeElement(target, throwOnError, options);
					case XDiffAction.RenameElement:
						return DoRenameElement(target, throwOnError, options);
					case XDiffAction.CreateAttribute:
						return DoCreateAttribute(target, throwOnError, options);
					case XDiffAction.DeleteAttribute:
						return DoDeleteAttribute(target, throwOnError, options);
					case XDiffAction.ChangeAttribute:
						return DoChangeAttribute(target, throwOnError, options);
					case XDiffAction.ChangeHierarchyPosition:
						return DoChangeHierarchyPosition(target, throwOnError, options);
			}
			return target;
		}

		

		XElement Find(XElement target, XElement search, XDiffOptions options){
			XElement[] result;
			if (options.IsHierarchy){
				result = target.Descendants().Where(_ => XDiffExtensions.IsJoined(_, search, options)).ToArray();
			}
			else{
				result = target.Elements().Where(_ => XDiffExtensions.IsJoined(_, search, options)).ToArray();
			}
			if (0 == result.Length) return null;
			if (1 == result.Length) return result[0];
			throw new Exception("double match");
		}


		XElement Find(XElement target, string finder, XDiffOptions options){
			var finds = finder.Split('-');
			var name = "";
			var attrname = "";
			var val = "";
			if (finds.Length == 3){
				name = finds[0];
				attrname = finds[1];
				val = finds[2];
			}
			else{
				attrname = finds[0];
				val = finds[1];
			}
			Func<XElement, bool> ismatch = e => {
				if (name != "" && name != e.Name.LocalName) return false;
				var a = e.Attribute(attrname);
				if (null == a) return false;
				return a.Value == val;
			};
			XElement[] result;
			if (options.IsHierarchy)
			{
				result = target.Descendants().Where(ismatch).ToArray();
			}
			else
			{
				result = target.Elements().Where(ismatch).ToArray();
			}
			if (0 == result.Length) return null;
			if (1 == result.Length) return result[0];
			throw new Exception("double match");
		}


		private XElement DoChangeHierarchyPosition(XElement target, bool throwOnError, XDiffOptions options)
		{
			var e = Find(target, BasisElement, options);
			if (null == e)
			{
				if (throwOnError)
				{
					throw new Exception("cannot find target for " + BasisElement);
				}
				return target;
			}
			var __parent = e.Attribute("__parent");
			var pval = NewValue;
			__parent.Remove();
			e.Remove();
			if (string.IsNullOrWhiteSpace(pval)){
				target.Add(e);
			}
			else{
				var newtarg = Find(target, pval, options);
				if (null == newtarg){
					if (throwOnError)
					{
						throw new Exception("cannot find target for " + pval);
					}
					return target;
				}
				newtarg.Add(e);
			}
			return target;
		}

		private XElement DoChangeAttribute(XElement target, bool throwOnError, XDiffOptions options){
			var e = Find(target, BasisElement, options);
			if (null == e){
				if (throwOnError){
					throw new Exception("cannot find target for "+BasisElement);
				}
				return target;
			}
			if (null == NewestAttribute){
				foreach (var a in NewestElement.Attributes()){
					e.SetAttributeValue(a.Name, a.Value);
				}
			}
			else{
				e.SetAttributeValue(NewestAttribute.Name.LocalName, NewestAttribute.Value);
			}
			return target;
		}

		private XElement DoDeleteAttribute(XElement target, bool throwOnError, XDiffOptions options)
		{
			var e = Find(target, BasisElement, options);
			if (null == e)
			{
				if (throwOnError)
				{
					throw new Exception("cannot find target for " + BasisElement);
				}
				return target;
			}
			var ex = e.Attribute(BasisAttribute.Name.LocalName);
			if (null != ex){
				ex.Remove();
			}
			return target;
		}

		private XElement DoCreateAttribute(XElement target, bool throwOnError, XDiffOptions options)
		{
			var e = Find(target, BasisElement, options);
			if (null == e)
			{
				if (throwOnError)
				{
					throw new Exception("cannot find target for " + BasisElement);
				}
				return target;
			}
			if (null == NewestAttribute){
				foreach (var a in NewestElement.Attributes()){
					e.SetAttributeValue(a.Name,a.Value);
				}
			}
			else{
				e.SetAttributeValue(NewestAttribute.Name.LocalName, NewestAttribute.Value);
			}
			return target;
		}

		private XElement DoRenameElement(XElement target, bool throwOnError, XDiffOptions options)
		{
			var e = Find(target, BasisElement, options);
			if (null == e)
			{
				if (throwOnError)
				{
					throw new Exception("cannot find target for " + BasisElement);
				}
				return target;
			}
			e.Name = NewValue;
			return target;
		}

		private XElement DoChangeElement(XElement target, bool throwOnError, XDiffOptions options)
		{
			var e = Find(target, BasisElement, options);
			if (null == e)
			{
				if (throwOnError)
				{
					throw new Exception("cannot find target for " + BasisElement);
				}
				return target;
			}
			if (NewValue.Contains("<") && NewValue.Contains(">")){
				e.Nodes().ToArray().Remove();
				var x = XElement.Parse("<a>" + NewValue + "</a>");
				foreach (var n in x.Nodes()){
					e.Add(n);
				}
			}
			else{
				e.Value = NewValue;
			}
			return target;
		}

		private XElement DoDeleteElement(XElement target, bool throwOnError, XDiffOptions options)
		{
			var e = Find(target, BasisElement, options);
			if (null == e)
			{
				if (throwOnError)
				{
					throw new Exception("cannot find target for " + BasisElement);
				}
				return target;
			}
			e.Remove();
			return target;
		}

		private XElement DoCreateElement(XElement target, bool throwOnError, XDiffOptions options){
			var t = target;
			var parent = NewestElement.Attribute("__parent");
			if (null != parent){
				t = FindByParent(target,new ParentCondition(parent.Value));
			}
			if (null == t){
				throw new Exception("cannot find target parent elment "+parent);
			}
			if (options.IsHierarchy){ //мы должны порождать элементы без дочек во избежание дублирования, дочки накатятся сами
				var header = new XElement(NewestElement);
				header.Elements().Remove();
				var p = header.Attribute("__parent");
				if(null!=p)p.Remove();
				t.Add(header);
			}
			else{
				t.Add(new XElement(NewestElement));	
			}
			
			return target;
		}

		private XElement FindByParent(XElement target, ParentCondition finder){
			return target.DescendantsAndSelf().FirstOrDefault(finder.Match);

		}
	}

	internal class ParentCondition{
		public ParentCondition(string value){
			var parts = value.Split('-');
			if (parts.Length == 3){
				Name = parts[0];
				if (parts[1] == "id"){
					Id = parts[2];
				}
				else{
					Code = parts[2];
				}
			}
			else{
				if (parts[0] == "id"){
					Id = parts[1];
				}
				else{
					Code = parts[1];
				}
			}

		}

		public string Name;
		public string Id;
		public string Code;

		public bool Match(XElement e){
			if (Name != null && e.Name.LocalName != Name) return false;
			if (Id != null && e.Attr("id") != Id) return false;
			return Code == e.Attr("code");
		}

	}
}