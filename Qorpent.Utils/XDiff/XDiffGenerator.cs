using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
		/// Проверить, что элементы разные
		/// </summary>
		/// <param name="basis"></param>
		/// <param name="newest"></param>
		/// <returns></returns>
		public bool IsDifferent2(XElement basis, XElement newest)
		{
			return GetDiff2(basis, newest).Any();
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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="basis"></param>
		/// <param name="newest"></param>
		/// <returns></returns>
		public IEnumerable<XDiffItem> GetDiff2(XElement basis, XElement newest){
			foreach (var i in GetDiff2Internal(basis,newest)){
				if (i.Action == (i.Action & _options.ErrorActions)){
					throw new Exception("Error Diff occured " + new[]{i}.LogToString());
				}
				yield return i;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="basis"></param>
		/// <param name="newest"></param>
		/// <returns></returns>
		public IEnumerable<XDiffItem> GetDiff2Internal(XElement basis, XElement newest){
			var findmatches = GetMatchedElementsAsync(basis.Elements().ToArray(), newest.Elements().ToArray());
			var _stack = new ConcurrentBag<XDiffItem>();
			try{
				findmatches.AsParallel().ForAll(_ =>
				{
					foreach (var di in CreateDiffItems(_.Value))
					{
						_stack.Add(di);
					}
				});
			}
			catch (Exception ex){
				throw new Exception("error in item emmiting",ex);
			}
			
			return _stack;
			
		}

		private IEnumerable<XDiffItem> CreateDiffItems(XDiffMatch2 xd){

			if (null == xd.b && _options.IncludeActions.HasFlag(XDiffAction.CreateElement) ){
				yield return CreateNew(xd);
			}
			else if (null != xd.b && 0 == xd.ns.Length && _options.IncludeActions.HasFlag(XDiffAction.DeleteElement)){
				yield return new XDiffItem{Action = XDiffAction.DeleteElement, BasisElement = xd.b};
			}
			else{
				if (_options.IncludeActions.HasFlag(XDiffAction.ChangeAttribute | XDiffAction.ChangeElement)){
					foreach (var xDiffItem in GetDifference(xd)) yield return xDiffItem;
				}
			}


		}

		private IEnumerable<XDiffItem> GetDifference(XDiffMatch2 xd){
			var newest = new XElement(xd.ns.First());
			if (1 != xd.ns.Length){
				foreach (var source in xd.ns.Skip(1)){
					foreach (var attribute in source.Attributes()){
						if (attribute.Name.LocalName.StartsWith("set-")){
							newest.SetAttributeValue(attribute.Name.LocalName.Substring(4), attribute.Value);
						}
						else{
							newest.SetAttributeValue(attribute.Name, attribute.Value);
						}
					}
					if (!string.IsNullOrWhiteSpace(source.Value)){
						newest.Value = source.Value;
					}
				}
			}
			if (_options.IncludeActions.HasFlag(XDiffAction.ChangeElement)){
				if (!string.IsNullOrWhiteSpace(newest.Value) && xd.b.Value != newest.Value){
					yield return new XDiffItem{Action = XDiffAction.ChangeElement, BasisElement = xd.b, NewValue = newest.Value};
				}
			}

			if (_options.IncludeActions.HasFlag(XDiffAction.DeleteAttribute)){
				foreach (var attribute in xd.b.Attributes()){
					if(attribute.Name.LocalName=="id")continue;
					if(attribute.Name.LocalName=="code")continue;
					if (null == newest.Attribute(attribute.Name)){
						yield return new XDiffItem { Action = XDiffAction.DeleteAttribute, BasisElement = xd.b, BasisAttribute = attribute };
					}
				}
			}

			if (_options.IncludeActions.HasFlag(XDiffAction.ChangeAttribute|XDiffAction.CreateAttribute)){
				foreach (var attribute in newest.Attributes()){
					if (attribute.Name.LocalName == "id" && !_options.ChangeIds){
						continue;
					}
					var a = attribute;
					if (a.Name.LocalName.StartsWith("set-")){
						a = new XAttribute(a.Name.LocalName.Substring(4), a.Value);
					}
					if (null == xd.b.Attribute(a.Name) && _options.IncludeActions.HasFlag(XDiffAction.CreateAttribute)){
						
						yield return new XDiffItem { Action = XDiffAction.CreateAttribute, BasisElement = xd.b, NewestAttribute = a };
					}
					else if (attribute.Value != xd.b.Attr(a.Name.LocalName) && _options.IncludeActions.HasFlag(XDiffAction.ChangeAttribute))
					{
						yield return new XDiffItem{Action = XDiffAction.ChangeAttribute, BasisElement = xd.b,NewestAttribute = a};
					}
				}
			}

			if (_options.IncludeActions.HasFlag(XDiffAction.RenameElement)){
				if (xd.b.Name.LocalName != newest.Name.LocalName){
					yield return new XDiffItem { Action = XDiffAction.RenameElement, BasisElement = xd.b, NewValue = newest.Name.LocalName};
				}
			}
		}

		private static XDiffItem CreateNew(XDiffMatch2 xd){
			var newest = new XElement(xd.ns.First());
			foreach (var source in xd.ns.Skip(1)){
				foreach (var attribute in source.Attributes()){
					if (attribute.Name.LocalName.StartsWith("set-")){
						newest.SetAttributeValue(attribute.Name.LocalName.Substring(4), attribute.Value);
					}
					else{
						newest.SetAttributeValue(attribute.Name, attribute.Value);
					}
				}
				if (!string.IsNullOrWhiteSpace(source.Value)){
					newest.Value = source.Value;
				}
			}
			var item = new XDiffItem{Action = XDiffAction.CreateElement, NewestElement = newest};
			return item;
		}

		private bool Check(XDiffItem x){
			if (x.Action == (x.Action & _options.ErrorActions)){
				throw new Exception("Error Diff occured "+ new[]{x}.LogToString());
			}
			return _options.IncludeActions.HasFlag(x.Action);
		}

		private IEnumerable<XDiffItem> ProcessChangedElements(XElement[] belements, XElement[] nelements){
			var matchhash =  GetMatchedElements(belements, nelements);
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

		/// <summary>
		/// Промежуточный класс JOIN для одинаковых групп элементов
		/// </summary>
		private class XDiffMatch2
		{
			public XElement b;
			public XElement[] ns;
			
		}

		struct XDiffKey{
			public XDiffKey(XDiffOptions opts,XElement e){
				this._options = opts;
				this.Name = e.Name.LocalName;
				this.Id = e.Attr("id");
				this.Code = e.Attr("code");
			}

			public override string ToString(){
				return string.Format("{0}-{1}-{2}", Name, Id, Code);
			}

			private bool Equals(XDiffKey other){
				if (!_options.IsNameIndepended && this.Name != other.Name) return false;
				bool? equalId = null;
				if ( !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(other.Id)){
					equalId = Id == other.Id;
					if (!_options.ChangeIds){
						return equalId.Value;
					}
					
				}
				if (!string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(other.Code))
				{
					return Code == other.Code;
				}

				return equalId.HasValue && equalId.Value;
			}

			public override int GetHashCode(){
				unchecked{
					int hashCode = (Name != null ? Name.GetHashCode() : 0);
					/*
					hashCode = (hashCode*397) ^ (Id != null ? Id.GetHashCode() : 0);
					hashCode = (hashCode*397) ^ (Code != null ? Code.GetHashCode() : 0);
					*/
					return hashCode;
				}
			}

			public string Name;
			public string Id;
			public string Code;
			private XDiffOptions _options;


			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((XDiffKey) obj);
			}
		}

		class EndpointPack{
			public int Id;
			public bool IsSource;
			public XDiffKey Key;
			public XElement Element;
			public ConcurrentBag<XElement> Bag;
			public bool IsBound;
		}

		class InterPack{
			public EndpointPack Target;
			public EndpointPack Source;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="belements"></param>
		/// <param name="nelements"></param>
		/// <returns></returns>
		private IEnumerable<KeyValuePair<XDiffKey,XDiffMatch2>> GetMatchedElementsAsync(XElement[] belements, XElement[] nelements)
		{
		
				int trgid = 0;
				int srcid = 0;
				var trg = new ConcurrentDictionary<int, EndpointPack>();
				var src = new ConcurrentDictionary<int, EndpointPack>();
				var idinter = new ConcurrentDictionary<string, InterPack>();
				var codeinter = new ConcurrentDictionary<string, InterPack>();
				var iter1 = Task.Run(() =>{
					foreach (var e in belements){
						var id = trgid++;
						var key = new XDiffKey(_options,e);
						var _id = _options.IsNameIndepended? key.Id : (key.Name+"_"+key.Id);

						var _code = _options.IsNameIndepended ? key.Code : (key.Name + "_" + key.Code);
						var ep = new EndpointPack{ Key = key, Element = e, Bag = new ConcurrentBag<XElement>()};
						trg[id] = ep;
						
						if (!string.IsNullOrWhiteSpace(key.Id)){
							idinter.AddOrUpdate(_id, _ => new InterPack{Target = ep}, (k, _) =>{
					
								if (null != _.Target){
									throw new Exception("double id match in trg "+_.Target.Key );
								}
								_.Target = ep;
								var _bag = _.Source.Bag;
								_.Source.Bag = ep.Bag;
								foreach (var c  in _bag){
									ep.Bag.Add(c);	
								}
								
								ep.IsBound = true;
								_.Source.IsBound = true;
								return _;
							});
						}
						if ( !string.IsNullOrWhiteSpace(key.Code)){
							codeinter.AddOrUpdate(_code, _ => new InterPack { Target = ep }, (k, _) =>
							{
								if (null != _.Target)
								{
									throw new Exception("double code match in trg " + _.Target.Key);
								}
								_.Target = ep;
								if (_.Source.Bag != ep.Bag){
									var _bag = _.Source.Bag;
									_.Source.Bag = ep.Bag;
									foreach (var c in _bag){
										ep.Bag.Add(c);
									}
								}
								ep.IsBound = true;
								_.Source.IsBound = true;
								return _;
							});
						}
					}
				});
				var iter2 = Task.Run(() =>{
					foreach (var e in nelements)
					{
						var id = srcid++;
						var key = new XDiffKey(_options, e);
						var _id = _options.IsNameIndepended ? key.Id : (key.Name + "_" + key.Id);

						var _code = _options.IsNameIndepended ? key.Code : (key.Name + "_" + key.Code);
						var ep = new EndpointPack { Id=id, IsSource = true,Key = key, Element = e, Bag = new ConcurrentBag<XElement>() };
						ep.Bag.Add(e);
						src[id] = ep;
						EndpointPack _idpack = null;
						if (!string.IsNullOrWhiteSpace(key.Id))
						{
							idinter.AddOrUpdate(_id, _ =>new InterPack { Source =_idpack= ep }, (k, _) =>
							{
								if (null != _.Source){
									_idpack = _.Source; //it's not problem if same as code binding
									_.Source.Bag.Add(e);
									ep.IsBound = true;
									
								}
								else{
									_.Source = ep;
									_.Source.Bag = _.Target.Bag;
									_.Target.IsBound = true;
									ep.IsBound = true;
									_.Target.Bag.Add(ep.Element);
							
								}
								return _;
							});
						}
						
						
						if (!string.IsNullOrWhiteSpace(key.Code))
						{
							codeinter.AddOrUpdate(_code, _ => {
								if (null != _idpack){
									throw new Exception("ambigous source " + ep.Key);
								}
								return new InterPack{Source = ep};
							}, (k, _) =>
							{
								if (_.Source != null && _idpack != null && _.Source != _idpack){
									throw new Exception("ambigous source "+ep.Key);
								}
								if (null != _.Source)
								{
									_idpack = _.Source; //it's not problem if same as code binding
									_.Source.Bag.Add(e);
									ep.IsBound = true;
								}
								else
								{
									_.Source = ep;
									_.Source.Bag = _.Target.Bag;
									_.Target.IsBound = true;
									ep.IsBound = true;
									_.Target.Bag.Add(ep.Element);
								}
								return _;
							});
						}
					}
				});
				Task.WaitAll(iter1, iter2);
				var error = iter1.Exception ?? iter2.Exception;
				if (null != error){
					throw new Exception("error in indexer",error);
				}
				foreach (var ep in trg.Values){
					yield return new KeyValuePair<XDiffKey, XDiffMatch2>(
						ep.Key,
						new XDiffMatch2 { b = ep.Element, ns = ep.Bag.Distinct().ToArray() }
						);
				}
				if (_options.IncludeActions.HasFlag(XDiffAction.CreateElement)){
					foreach (var ep in src.Values)
					{
						if(!ep.IsBound)
						yield return new KeyValuePair<XDiffKey, XDiffMatch2>(
							ep.Key,
							new XDiffMatch2 { ns = ep.Bag.Distinct().ToArray() }
							);
					}	
				}
		
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
