using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.XDiff{
	/// <summary>
	///     Генерирует поток изменений, необходимых для применения к документу
	/// </summary>
	public class XDiffGenerator{
		private readonly XDiffOptions _options;

		/// <summary>
		/// </summary>
		public XDiffGenerator(XDiffOptions options = null){
			_options = options ?? new XDiffOptions();
		}


		/// <summary>
		///     Проверить, что элементы разные
		/// </summary>
		/// <param name="basis"></param>
		/// <param name="newest"></param>
		/// <returns></returns>
		public bool IsDiff(XElement basis, XElement newest){
			return GetDiff(basis, newest).Any();
		}

		/// <summary>
		/// </summary>
		/// <param name="basis"></param>
		/// <param name="newest"></param>
		/// <returns></returns>
		public IEnumerable<XDiffItem> GetDiff(XElement basis, XElement newest){
			foreach (XDiffItem i in GetDiffInternal(basis, newest)){
				if (i.Action == (i.Action & _options.ErrorActions)){
					throw new Exception("Error Diff occured " + new[]{i}.LogToString());
				}
				yield return i;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="basis"></param>
		/// <param name="newest"></param>
		/// <returns></returns>
		public IEnumerable<XDiffItem> GetDiffInternal(XElement basis, XElement newest){
			try{
				IEnumerable<KeyValuePair<XDiffKey, XDiffMatch>> findmatches = GetMatchedElementsAsync(basis, newest,
				                                                                                      _options.IsHierarchy
					                                                                                      ? basis.Descendants()
					                                                                                      : basis.Elements(),
				                                                                                      _options.IsHierarchy
					                                                                                      ? newest.Descendants()
					                                                                                      : newest.Elements());
				var result = new ConcurrentBag<XDiffItem>();
				findmatches.AsParallel().ForAll(_ =>{
					foreach (XDiffItem di in CreateDiffItems(_.Value)){
						result.Add(di);
					}
				});

				return result;
			}
			catch (Exception ex){
				throw new Exception("error in item emmiting", ex);
			}
		}

		private IEnumerable<XDiffItem> CreateDiffItems(XDiffMatch xd){
			if (null == xd.b && _options.IncludeActions.HasFlag(XDiffAction.CreateElement)){
				yield return CreateNew(xd);
			}
			else if (null != xd.b && 0 == xd.ns.Length && _options.IncludeActions.HasFlag(XDiffAction.DeleteElement)){
				yield return new XDiffItem{Action = XDiffAction.DeleteElement, BasisElement = xd.b};
			}
			else if(null!=xd.b && 0!=xd.ns.Length){
				if (_options.IncludeActions.HasFlag(XDiffAction.ChangeAttribute | XDiffAction.ChangeElement)){
					foreach (XDiffItem xDiffItem in GetDifference(xd)) yield return xDiffItem;
				}
			}
		}

		private IEnumerable<XDiffItem> GetDifference(XDiffMatch xd){
			var newest = new XElement(xd.ns.First());
			if (1 != xd.ns.Length){
				foreach (XElement source in xd.ns.Skip(1)){
					foreach (XAttribute attribute in source.Attributes()){
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
				string valb = string.Join(Environment.NewLine,
				                          xd.b.Nodes().Except(xd.b.Elements().Where(_ => null != _.Attribute("__parent"))));
				string valn = string.Join(Environment.NewLine,
				                          newest.Nodes().Except(newest.Elements().Where(_ => null != _.Attribute("__parent"))));
				if (valb != valn){
					yield return new XDiffItem{Action = XDiffAction.ChangeElement, BasisElement = xd.b, NewValue = valn};
				}
			}

			if (_options.IncludeActions.HasFlag(XDiffAction.DeleteAttribute)){
				foreach (XAttribute attribute in xd.b.Attributes()){
					if (attribute.Name.LocalName == "id") continue;
					if (attribute.Name.LocalName == "code") continue;
					if (attribute.Name.LocalName == "__parent") continue;
					if (null == newest.Attribute(attribute.Name)){
						yield return new XDiffItem{Action = XDiffAction.DeleteAttribute, BasisElement = xd.b, BasisAttribute = attribute};
					}
				}
			}

			if (_options.IncludeActions.HasFlag(XDiffAction.ChangeAttribute | XDiffAction.CreateAttribute)){
				foreach (XAttribute attribute in newest.Attributes()){
					if (attribute.Name.LocalName == "id" && !_options.ChangeIds){
						continue;
					}
					if (attribute.Name.LocalName == "code" && !_options.ChangeIds){
						continue;
					}
					if (attribute.Name.LocalName == "__parent"){
						continue;
					}
					XAttribute a = attribute;
					if (a.Name.LocalName.StartsWith("set-")){
						a = new XAttribute(a.Name.LocalName.Substring(4), a.Value);
					}

					if (null == xd.b.Attribute(a.Name) && _options.IncludeActions.HasFlag(XDiffAction.CreateAttribute)){
						yield return new XDiffItem{Action = XDiffAction.CreateAttribute, BasisElement = xd.b, NewestAttribute = a};
					}
					else if (attribute.Value != xd.b.Attr(a.Name.LocalName) && _options.IncludeActions.HasFlag(XDiffAction.ChangeAttribute)){
						yield return new XDiffItem{Action = XDiffAction.ChangeAttribute, BasisElement = xd.b, NewestAttribute = a};
					}
				}
			}

			if (_options.IsHierarchy && _options.IncludeActions.HasFlag(XDiffAction.ChangeHierarchyPosition)){
				XAttribute newparent = newest.Attribute("__parent");
				XAttribute oldparent = xd.b.Attribute("__parent");
				if (null == newparent && null == oldparent){
				}
				else if (null == newparent && null != oldparent){
					//move to root
					yield return new XDiffItem{Action = XDiffAction.ChangeHierarchyPosition, BasisElement = xd.b};
				}
				else if ((null == oldparent && null != newparent) || (oldparent.Value != newparent.Value)){
					yield return
						new XDiffItem{Action = XDiffAction.ChangeHierarchyPosition, BasisElement = xd.b, NewValue = newparent.Value};
				}
			}

			if (_options.IncludeActions.HasFlag(XDiffAction.RenameElement)){
				if (xd.b.Name.LocalName != newest.Name.LocalName){
					yield return
						new XDiffItem{Action = XDiffAction.RenameElement, BasisElement = xd.b, NewValue = newest.Name.LocalName};
				}
			}
		}

		private static XDiffItem CreateNew(XDiffMatch xd){
			var newest = new XElement(xd.ns.First());
			foreach (XElement source in xd.ns.Skip(1)){
				foreach (XAttribute attribute in source.Attributes()){
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


		/// <summary>
		/// </summary>
		/// <param name="newest"></param>
		/// <param name="belements"></param>
		/// <param name="nelements"></param>
		/// <param name="basis"></param>
		/// <returns></returns>
		private IEnumerable<KeyValuePair<XDiffKey, XDiffMatch>> GetMatchedElementsAsync(XElement basis, XElement newest,
		                                                                                IEnumerable<XElement> belements,
		                                                                                IEnumerable<XElement> nelements){
			int trgid = 0;
			int srcid = 0;
			var trg = new ConcurrentDictionary<int, EndpointPack>();
			var src = new ConcurrentDictionary<int, EndpointPack>();
			var idinter = new ConcurrentDictionary<string, InterPack>();
			var codeinter = new ConcurrentDictionary<string, InterPack>();
			belements.AsParallel().ForAll(e => RegisterBase(basis, ref trgid, e, trg, idinter, codeinter));
			nelements.AsParallel().ForAll(e => RegisterDelta(newest, ref srcid, e, src, idinter, codeinter));
			foreach (EndpointPack ep in trg.Values){
				yield return new KeyValuePair<XDiffKey, XDiffMatch>(
					ep.Key,
					new XDiffMatch{b = ep.Element, ns = ep.Bag.Distinct().ToArray()}
					);
			}
			if (_options.IncludeActions.HasFlag(XDiffAction.CreateElement)){
				foreach (EndpointPack ep in src.Values){
					if (!ep.IsBound)
						yield return new KeyValuePair<XDiffKey, XDiffMatch>(
							ep.Key,
							new XDiffMatch{ns = ep.Bag.Distinct().ToArray()}
							);
				}
			}
		}

		private void RegisterDelta(XElement newest, ref int srcid, XElement e, ConcurrentDictionary<int, EndpointPack> src,
		                           ConcurrentDictionary<string, InterPack> idinter,
		                           ConcurrentDictionary<string, InterPack> codeinter){
			int id = Interlocked.Increment(ref srcid);
			var key = new XDiffKey(_options, e);
			string _id = _options.IsNameIndepended ? key.Id : (key.Name + "_" + key.Id);
			string _code = _options.IsNameIndepended ? key.Code : (key.Name + "_" + key.Code);
			if (_options.IsHierarchy){
				SetupParentAttribute(newest, e);
			}
			var ep = new EndpointPack{Id = id, IsSource = true, Key = key, Element = e, Bag = new ConcurrentBag<XElement>()};
			ep.Bag.Add(e);
			src[id] = ep;
			EndpointPack _idpack = null;
			EndpointPack _idtrg = null;
			if (!string.IsNullOrWhiteSpace(key.Id)){
				idinter.AddOrUpdate(_id, _ => new InterPack{Source = _idpack = ep}, (k, _) =>{
					if (null != _.Source){
						_idpack = _.Source; //it's not problem if same as code binding
						_.Source.Bag.Add(e);
						ep.IsBound = true;
					}
					else{
						_.Source = ep;
						_idpack = _.Source;
						_.Source.Bag = _.Target.Bag;
						_.Target.IsBound = true;
						_idtrg = _.Target;
						ep.IsBound = true;
						_.Target.Bag.Add(ep.Element);
					}
					return _;
				});
			}


			if (!string.IsNullOrWhiteSpace(key.Code)){
				codeinter.AddOrUpdate(_code, _ =>{
					if (null != _idpack && _idpack != ep){
						throw new Exception("ambigous source (1)" + ep.Key);
					}
					if (null != _idtrg){
						throw new Exception("ambigous source (2)" + ep.Key);
					}
					return new InterPack{Source = ep};
				}, (k, _) =>{
					if (_.Source != null && _idpack != null && _.Source != _idpack){
						throw new Exception("ambigous source (3)" + ep.Key);
					}
					if (null != _idtrg && _idtrg != _.Target){
						throw new Exception("ambigous source (4)" + ep.Key);
					}
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
			return;
		}

		private void RegisterBase(XElement basis, ref int trgid, XElement e, ConcurrentDictionary<int, EndpointPack> trg,
		                          ConcurrentDictionary<string, InterPack> idinter,
		                          ConcurrentDictionary<string, InterPack> codeinter){
			int id = Interlocked.Increment(ref trgid);
			var key = new XDiffKey(_options, e);
			string _id = _options.IsNameIndepended ? key.Id : (key.Name + "_" + key.Id);
			string _code = _options.IsNameIndepended ? key.Code : (key.Name + "_" + key.Code);
			if (_options.IsHierarchy){
				SetupParentAttribute(basis, e);
			}
			var ep = new EndpointPack{Key = key, Element = e, Bag = new ConcurrentBag<XElement>()};
			trg[id] = ep;

			if (!string.IsNullOrWhiteSpace(key.Id)){
				idinter.AddOrUpdate(_id, _ => new InterPack{Target = ep}, (k, _) =>{
					if (null != _.Target){
						throw new Exception("double id match in trg " + _.Target.Key);
					}
					_.Target = ep;
					ConcurrentBag<XElement> _bag = _.Source.Bag;
					_.Source.Bag = ep.Bag;
					foreach (XElement c in _bag){
						ep.Bag.Add(c);
					}

					ep.IsBound = true;
					_.Source.IsBound = true;
					return _;
				});
			}
			if (!string.IsNullOrWhiteSpace(key.Code)){
				codeinter.AddOrUpdate(_code, _ => new InterPack{Target = ep}, (k, _) =>{
					if (null != _.Target){
						throw new Exception("double code match in trg " + _.Target.Key);
					}
					_.Target = ep;
					if (_.Source.Bag != ep.Bag){
						ConcurrentBag<XElement> _bag = _.Source.Bag;
						_.Source.Bag = ep.Bag;
						foreach (XElement c in _bag){
							ep.Bag.Add(c);
						}
					}
					ep.IsBound = true;
					_.Source.IsBound = true;
					return _;
				});
			}
			return;
		}

		private void SetupParentAttribute(XElement basis, XElement e){
			if (e.Parent != basis){
				var parentKey = new XDiffKey(_options, e.Parent);
				string parentRef = "";
				if (!_options.IsNameIndepended){
					parentRef += parentKey.Name + "-";
				}
				if (!string.IsNullOrWhiteSpace(parentKey.Id)){
					parentRef += "id-" + parentKey.Id;
				}
				else{
					parentRef += "code-" + parentKey.Code;
				}
				e.SetAttributeValue("__parent", parentRef);
			}
		}

		private class EndpointPack{
			public ConcurrentBag<XElement> Bag;
			public XElement Element;
			public int Id;
			public bool IsBound;
			public bool IsSource;
			public XDiffKey Key;
		}

		private class InterPack{
			public EndpointPack Source;
			public EndpointPack Target;
		}

		private struct XDiffKey{
			public readonly string Code;
			public readonly string Id;
			public readonly string Name;
			private readonly XDiffOptions _options;

			public XDiffKey(XDiffOptions opts, XElement e){
				_options = opts;
				Name = e.Name.LocalName;
				Id = e.Attr("id");
				Code = e.Attr("code");
			}

			public override string ToString(){
				return string.Format("{0}-{1}-{2}", Name, Id, Code);
			}

			private bool Equals(XDiffKey other){
				if (!_options.IsNameIndepended && Name != other.Name) return false;
				bool? equalId = null;
				if (!string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(other.Id)){
					equalId = Id == other.Id;
					if (!_options.ChangeIds){
						return equalId.Value;
					}
				}
				if (!string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(other.Code)){
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


			public override bool Equals(object obj){
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != GetType()) return false;
				return Equals((XDiffKey) obj);
			}
		}

		/// <summary>
		///     Промежуточный класс JOIN для одинаковых групп элементов
		/// </summary>
		private class XDiffMatch{
			public XElement b;
			public XElement[] ns;
		}
	}
}