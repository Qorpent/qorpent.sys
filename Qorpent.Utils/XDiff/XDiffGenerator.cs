using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.XDiff
{
	/// <summary>
	/// Генерирует поток изменений, необходимых для применения к документу
	/// </summary>
	public class XDiffGenerator
	{
		private XDiffOptions _options;

		/// <summary>
		/// 
		/// </summary>
		public XDiffGenerator(XDiffOptions options =null){
			_options = options ?? new XDiffOptions();
		}
		/// <summary>
		/// Проверить, что элементы разные
		/// </summary>
		/// <param name="basis"></param>
		/// <param name="newest"></param>
		/// <returns></returns>
		public bool IsDifferent(XElement basis, XElement newest){
			return GetDiff(basis, newest).Any();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<XDiffItem> GetDiff(XElement basis, XElement newest){
			if (basis.ToString() == newest.ToString()) yield break;
			XElement[] belements;
			XElement[] nelements;
			PrepareIndexes(basis, newest, out belements, out nelements);
			if(basis==newest)yield break;
				foreach (var x in ProcessNewElements(nelements, belements)){
					if(!Check(x))continue;
					x.Options = _options;
					yield return x;
				}
			
			
			foreach (var x in ProcessChangedElements(belements, nelements)){
				if (!Check(x)) continue;
				x.Options = _options;
				yield return x;
			}

				foreach (var x in ProcessDeleteElements(belements, nelements)){
					if (!Check(x)) continue;
					x.Options = _options;
					yield return x;
				}
			
		}

		private bool Check(XDiffItem x){
			if (x.Action == (x.Action & _options.ErrorActions)){
				throw new Exception("Error Diff occured "+ new[]{x}.LogToString());
			}
			return _options.IncludeActions.HasFlag(x.Action);
		}

		private IEnumerable<XDiffItem> ProcessChangedElements(XElement[] belements, XElement[] nelements){
			var matchhash = GetMatchedElements(belements, nelements);
			foreach (var e in matchhash){
				if (e.b.Name.LocalName != e.ns[0].Name.LocalName){
					yield return new XDiffItem{Action = XDiffAction.RenameElement, BasisElement = e.b, NewValue = e.ns[0].Name.LocalName};
				}
				var valb = string.Join(Environment.NewLine,
				                       e.b.Nodes().Except(e.b.Elements().Where(_ => null != _.Attribute("__parent"))));
				var valn = string.Join(Environment.NewLine,
				                       e.ns[0].Nodes().Except(e.ns[0].Elements().Where(_ => null != _.Attribute("__parent"))));
				if (valb != valn){
					yield return new XDiffItem{Action = XDiffAction.ChangeElement, BasisElement = e.b, NewValue = valn};
				}
				var newattributes =
					e.ns[0].Attributes().Where(_ => e.b.Attributes().All(__ => __.Name.LocalName != _.Name.LocalName)).ToArray();
				var delattributes =
					e.b.Attributes().Where(_ => e.ns[0].Attributes().All(__ => __.Name.LocalName != _.Name.LocalName)).ToArray();
				var matchattributes =
					e.b.Attributes()
					 .Select(_ => new{ba = _, na = e.ns[0].Attributes().FirstOrDefault(__ => __.Name.LocalName == _.Name.LocalName)})
					 .Where(_ => _.na != null).ToArray();
				foreach (var newattribute in newattributes){
					if (newattribute.Name.LocalName == "__parent"){
						yield return
							new XDiffItem{Action = XDiffAction.ChangeHierarchyPosition, BasisElement = e.b, NewValue = newattribute.Value};
						continue;
					}
					if (newattribute.Name.LocalName.StartsWith("set-"))
					{
						var realname = newattribute.Name.LocalName.Substring(4);
						yield return new XDiffItem { Action = XDiffAction.ChangeAttribute, BasisElement = e.b, NewestAttribute = new XAttribute(realname, newattribute.Value) };
						continue;
						
					}
					yield return new XDiffItem{Action = XDiffAction.CreateAttribute, BasisElement = e.b, NewestAttribute = newattribute};
				}
				foreach (var delattribute in delattributes){
					//codes and ids are not droppable
					if (delattribute.Name.LocalName == "id") continue;
					if (delattribute.Name.LocalName == "code") continue;
					if (delattribute.Name.LocalName == "__parent"){
						yield return new XDiffItem{Action = XDiffAction.ChangeHierarchyPosition, BasisElement = e.b, NewValue = ""};
						continue;
					}
					yield return new XDiffItem{Action = XDiffAction.DeleteAttribute, BasisElement = e.b, BasisAttribute = delattribute};
				}
				foreach (var ma in matchattributes){
					if (ma.ba.Value != ma.na.Value){
						if (ma.na.Name.LocalName == "__parent"){
							yield return
								new XDiffItem{Action = XDiffAction.ChangeHierarchyPosition, BasisElement = e.b, NewValue = ma.na.Value};
							continue;

						}
						if (ma.na.Name.LocalName.StartsWith("set-")){
							var realname = ma.na.Name.LocalName.Substring(4);
							yield return new XDiffItem { Action = XDiffAction.ChangeAttribute, BasisElement = e.b, NewestAttribute = new XAttribute(realname,ma.na.Value) };
							continue;
						}
						yield return new XDiffItem{Action = XDiffAction.ChangeAttribute, BasisElement = e.b, NewestAttribute = ma.na};
					}
				}
			}
		}

		private IEnumerable<XDiffItem> ProcessDeleteElements(XElement[] belements, XElement[] nelements){
			var deletedelements = belements.Where(__ => !nelements.Any(_ => XDiffExtensions.IsJoined(_, __,_options))).ToArray();
			foreach (var deletedelement in deletedelements){
				yield return new XDiffItem{Action = XDiffAction.DeleteElement, BasisElement = deletedelement};
			}
		}

		private IEnumerable<XDiffItem> ProcessNewElements(XElement[] nelements, XElement[] belements){
			var newelements = nelements.Where(__ => !belements.Any(_ => XDiffExtensions.IsJoined(_, __,_options))).ToArray();
			foreach (var newelement in newelements){
				yield return new XDiffItem{Action = XDiffAction.CreateElement, NewestElement = newelement};
			}
		}

		/// <summary>
		/// Промежуточный класс JOIN для одинаковых групп элементов
		/// </summary>
		private class XDiffMatch
		{
			public XElement b;
			public XElement[] ns;
		}

		private XDiffMatch[] GetMatchedElements(XElement[] belements, XElement[] nelements){
			var matchhash =
				belements
					.Select(_ => new XDiffMatch{b = _, ns = nelements.Where(__ => XDiffExtensions.IsJoined(__, _,_options)).ToArray()})
					.Where(_ => _.ns.Length != 0).ToArray();
			var reversehash =
				nelements
					.Select(_ => new XDiffMatch{b = _, ns = belements.Where(__ => XDiffExtensions.IsJoined(__, _,_options)).ToArray()})
					.Where(_ => _.ns.Length != 0).ToArray();
			if (matchhash.Union(reversehash).Any(_ => _.ns.Length > 1)){
				var sb = new StringBuilder();
				foreach (var m in matchhash.Where(_ => _.ns.Length > 1)){
					sb.AppendLine(m.b.ToString());
					sb.AppendLine(m.ns.Length.ToString());
					foreach (var x in m.ns){
						sb.AppendLine(x.ToString());
					}
					sb.AppendLine("----");
				}
				throw new Exception(sb.ToString());
			}
			return matchhash;
		}

		private void PrepareIndexes(XElement basis, XElement newest, out XElement[] belements, out XElement[] nelements){
			if (_options.IsHierarchy){
				foreach (var e in basis.Descendants().Union(newest.Descendants())){
					if (e.Parent != basis && e.Parent != newest){
						var attr = e.Parent.Attribute("id") ?? e.Parent.Attribute("code");
						if (null == attr){
							e.SetAttributeValue("__ignore", 1);
							continue;
						}
						if (_options.IsNameIndepended){
							e.SetAttributeValue("__parent", attr.Name.LocalName + "-" + attr.Value);
						}
						else{
							e.SetAttributeValue("__parent", e.Parent.Name.LocalName + "-" + attr.Name.LocalName + "-" + attr.Value);
						}
					}
				}
				belements = basis.Descendants().Where(_ => null == _.Attribute("__ignore")).ToArray();
				nelements = newest.Descendants().Where(_ => null == _.Attribute("__ignore")).ToArray();
			}
			else{
				belements = basis.Elements().ToArray();
				nelements = newest.Elements().ToArray();
			}
		}
	}
}
