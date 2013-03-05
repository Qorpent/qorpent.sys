using System;
using System.Xml.Linq;

namespace Qorpent.Qlaood {
	/// <summary>
	/// Strong-typed XML Qlaood configuration wrapper
	/// </summary>
	[Serializable]
	public class QlaoodConfigurationWrapper {
		/// <summary>
		/// Reference to working instance for configuration (preserved)
		/// </summary>
		[NonSerialized]
		protected readonly XElement Config;
		/// <summary>
		/// Reference to not-changed copy of config (not totally preserved)
		/// </summary>
		[NonSerialized]
		private readonly XElement _srcconfig;
		/// <summary>
		/// Access to native source configuration
		/// </summary>
		public XElement SourceConfig { get { return _srcconfig; } }

		/// <summary>
		/// Creates new read-only configuration wrapper
		/// </summary>
		/// <param name="config"></param>
		public QlaoodConfigurationWrapper(XElement config) {
			Config = config; //self copy with immutable guaranty
			_srcconfig = new XElement(config); //export copy without warranty
		}
	}
}