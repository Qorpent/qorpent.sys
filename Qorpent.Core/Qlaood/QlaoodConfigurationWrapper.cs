#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Core/QlaoodConfigurationWrapper.cs
#endregion
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