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
// PROJECT ORIGIN: Qorpent.Serialization/xmlSerializer.cs
#endregion

using System.Collections.Specialized;
using System.IO;
using System.Xml.Linq;
using Qorpent.IoC;
#if !EMBEDQPT
using Qorpent.Uson;
#endif
namespace Qorpent.Serialization {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle.Transient, ServiceType = typeof (ISerializer), Name = "xml.serializer")]
	public class XmlSerializer : Serializer {
	    /// <summary>
	    /// 	Creates the impl.
	    /// </summary>
	    /// <param name="name"> The name. </param>
	    /// <param name="value"> The value. </param>
	    /// <param name="options">Игнорируется</param>
	    /// <returns> </returns>
	    /// <remarks>
	    /// </remarks>
	    protected override ISerializerImpl CreateImpl(string name, object value, object options) {
			return new XmlSerializerImpl();
		}
        /// <summary>
        /// 
        /// </summary>
        public bool JsonCompatibleMode { get; set; }

	    /// <summary>
	    /// 	Serializes the specified name.
	    /// </summary>
	    /// <param name="name"> The name. </param>
	    /// <param name="value"> The value. </param>
	    /// <param name="output"> The output. </param>
	    /// <param name="options">Игнорируются </param>
	    /// <remarks>
	    /// </remarks>
	    public override void Serialize(string name, object value, TextWriter output, object options = null) {
			if (value is XElement) {
				output.Write(value.ToString());
			}
#if !EMBEDQPT
            else if (value is UObj)
			{
				output.Write(((UObj)value).ToXmlString());
			}
#endif
			else {
				base.Serialize(name, value, output);
			}
		}

        static XmlSerializer Default = new XmlSerializer();

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="obj"></param>
	    /// <param name="name"></param>
	    /// <returns></returns>
	    public static string GetString(object obj,string name = "root") {
	        return Default.Serialize(name ?? "root", obj);
	    }
	}
}