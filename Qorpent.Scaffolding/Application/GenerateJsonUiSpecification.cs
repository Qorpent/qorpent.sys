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
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			foreach (var targetclass in targetclasses){
				yield return GenerateUI(targetclass);
			}
		}


		private Production GenerateUI(IBSharpClass targetclass){
			var result = new Production{FileName = targetclass.FullName + ".ui.json"};
			dynamic obj = new UObj();
			obj.level = "ui";
			var trg = obj;
			StoreAttributes(targetclass.Compiled, trg);
			var zones = targetclass.Compiled.Elements("zone");
			var blocks = targetclass.Compiled.Elements("block");
			var controls = targetclass.Compiled.Elements("control");
			foreach (var z in zones){
				dynamic zone = new UObj();
				StoreAttributes(z,zone);
				var zb = blocks.Where(_ => _.Attr("zone") == z.Attr("code"));

				foreach (var b in zb){
					dynamic block = new UObj();
					StoreAttributes(b,block);

					var bc = controls.Where(_ => _.Attr("block") == b.Attr("code"));
					foreach (var c in bc){
						dynamic control = new UObj();
						StoreAttributes(c, control);
						
						var values = c.Elements().Where(_=>_.Name.LocalName=="value"||_.Name.LocalName=="values").ToArray();
						
							BindValues(control, values);
						


						block.controls.push(control);
					}

					zone.blocks.push(block);
				}
				obj.zones.push(zone);
			}

			result.Content = obj.ToJson();
			return result;
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
					dict[v.Attr("code")] = v.ToUson();
				}
			}
			

			

			foreach (var code in order){
				var obj = dict[code];
				control.values.push(obj);
			}
		}

		private static void StoreAttributes(XElement x, dynamic trg){
			foreach (var attribute in x.Attributes()){
				if (attribute.Name.LocalName == "prototype") continue;
				trg[attribute.Name.LocalName] = attribute.Value;
			}
		}
	}
}