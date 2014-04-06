using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Uson;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application{
	/// <summary>
	/// 
	/// </summary>
	public class GenerateJsonUiSpecification : CodeGeneratorTaskBase{
		/// <summary>
		/// 
		/// </summary>
		public GenerateJsonUiSpecification(){
			this.ClassSearchCriteria = "ui";
			this.DefaultOutputName = "Js";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			foreach (var targetclass in targetclasses){
			//	yield return GenerateUI(targetclass,"json");
				yield return GenerateUI(targetclass,"js");
			}
		}


		private Production GenerateUI(IBSharpClass targetclass,string ext){
			var result = new Production { FileName = Project.ProjectName + "_ui." + ext };
			var obj = GetJson(targetclass,ext);
			if (ext == "json"){
				result.Content = obj.ToJson();
			}
			else{
				result.Content = "//ui specification\r\ndefine([\"" + Project.ProjectName + "_api\",\"" + Project.ProjectName + "_types\"],function(apictor,types){return function($http,siteroot) { var api = apictor($http,siteroot);  return " + obj.ToJson(UObjSerializeMode.Javascript) + "}});";
			}
			return result;
		}

		private UObj _obj = null;
		private dynamic GetJson(IBSharpClass targetclass,string ext){
			if (null != _obj) return _obj;
			dynamic obj = _obj = new UObj();
			obj.level = "ui";
			var trg = obj;
			StoreAttributes(targetclass.Compiled, trg,ext);
			var zones = targetclass.Compiled.Elements("zone");
			var blocks = targetclass.Compiled.Elements("block");
			var controls = targetclass.Compiled.Elements("control");
			foreach (var z in zones){
				dynamic zone = new UObj();
				StoreAttributes(z, zone,ext);
				var zb = blocks.Where(_ => _.Attr("zone") == z.Attr("code"));

				foreach (var b in zb){
					dynamic block = new UObj();
					StoreAttributes(b, block,ext);

					var bc = controls.Where(_ => _.Attr("block") == b.Attr("code"));
					foreach (var c in bc){
						dynamic control = new UObj();
						StoreAttributes(c, control, ext, "control");

						var values = c.Elements().Where(_ => _.Name.LocalName == "value" || _.Name.LocalName == "values").ToArray();

						BindValues(control, values);


						block.controls.push(control);
					}

					zone.blocks.push(block);
				}
				obj.zones.push(zone);
			}
			return obj;
		}

		private void BindValues(dynamic control, XElement[] dvalues){
			
			var order = new List<string>();
			var dict = new Dictionary<string, dynamic>();
			foreach (var v in dvalues ){
				if (v.Name.LocalName == "values"){
					var importfrom = v.Attr("import-from");
					if (!string.IsNullOrWhiteSpace(importfrom)){
						var enumeration = _context.Get(importfrom);
						foreach (var element in enumeration.Compiled.Elements("item")){
							if (!order.Contains(element.Attr("code"))) order.Add(element.Attr("code"));
							dict[element.Attr("code")] = new { code = element.Attr("code"), name = element.Attr("name") }.ToUson();
						}
					}
					foreach (var attribute in v.Attributes()){
						if (!order.Contains(attribute.Name.LocalName)) order.Add(attribute.Name.LocalName);
						dict[attribute.Name.LocalName] = new { code = attribute.Name.LocalName, name = attribute.Value }.ToUson();
					}
				}
				else{
					if (!order.Contains(v.Attr("code"))) order.Add(v.Attr("code"));
					dict[v.Attr("code")] =v.Attributes().ToDictionary(_=>_.Name.LocalName,_=>_.Value).ToUson();
				}
			}
			

			

			foreach (var code in order){
				var obj = dict[code];
				control.values.push(obj);
			}
		}

		private  void StoreAttributes(XElement x, dynamic trg, string ext, string type =""){
			foreach (var attribute in x.Attributes()){
				if (attribute.Name.LocalName == "prototype") continue;
				if (attribute.Name.LocalName == "fullcode") continue;
				if (attribute.Name.LocalName == "block" && type=="control") continue;
				if (attribute.Name.LocalName == "zone" && type=="block") continue;
				if (ext == "js"){
					if (attribute.Name.LocalName.ToLower().Contains("action") && type=="control"){
						var clsname = attribute.Value;
						if (clsname.Contains(".")){
							var action = this._context.Get(clsname);
							var controller = action.Compiled.Attr("controller");
							trg[attribute.Name.LocalName] = "javascript://api." + controller + "_" + action.Name;
							continue;
						}
					}

					if (attribute.Name.LocalName == "bindtype"){
						var clsname = attribute.Value;
						var dtype = this._context.Get(clsname);
						trg[attribute.Name.LocalName] = "javascript://types." + dtype.Name;
						continue;
						
					}

				}
				trg[attribute.Name.LocalName] = attribute.Value;
			}
		}
	}
}