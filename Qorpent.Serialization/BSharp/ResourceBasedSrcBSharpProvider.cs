using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp{
	/// <summary>
	/// </summary>
	public class ResourceBasedSrcBSharpProvider : ServiceBase, IBSharpSourceCodeProvider{
		private IBxlParser _parser;

		/// <summary>
		/// </summary>
		[Inject]
		protected IBxlParser Parser{
			get { return _parser ?? (_parser = new BxlParser()); }
			set { _parser = value; }
		}

		/// <summary>
		/// </summary>
		public string ResourceMarker { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public IEnumerable<XElement> GetSources(IBSharpCompiler compiler, IBSharpContext context){
			if (string.IsNullOrWhiteSpace(ResourceMarker)) throw new Exception("marker not set");
            foreach (string rname in GetType().Assembly.FindAllResourceNames(ResourceMarker)){
               string resource = GetType().Assembly.ReadManifestResource(rname);
				yield return Parser.Parse(resource, rname);
			}
		}
	}
}