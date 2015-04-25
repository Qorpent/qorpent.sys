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
// PROJECT ORIGIN: Qorpent.Serialization/jsonSerializer.cs
#endregion

using System.Xml.Linq;
using Qorpent.IoC;
using Qorpent.Json;
#if !EMBEDQPT
using Qorpent.Uson;
#endif
namespace Qorpent.Serialization {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle.Transient, ServiceType = typeof (ISerializer), Name = "json.serializer")]
	public class JsonSerializer : Serializer {
		/// <summary>
		/// 	Creates the impl.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
        /// <param name="options">Дополнительные опции при создании</param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected override ISerializerImpl CreateImpl(string name, object value, object options) {
			return new JsonSerializerImpl();
		}

        private XmlToJsonConverter converter = new XmlToJsonConverter();

	    /// <summary>
        /// Сериализует переданный объект в текстовой поток, перекрыта отрисовка XML через <see cref="XmlToJsonConverter"/>
	    /// </summary>
	    /// <param name="name"> Имя сериализуемого объекта</param>
	    /// <param name="value">Сериализуемый объект </param>
	    /// <param name="output">Целевой текстововй поток</param>
	    /// <param name="options">Опции сериализации, используются при создании имепдлементации</param>
	    /// <remarks>
	    /// </remarks>
	    public override void Serialize(string name, object value, System.IO.TextWriter output, object options = null)
        {
            if (value is XElement && null!=((XElement)value).Attribute(JsonItem.JsonTypeAttributeName)) {
                output.Write(converter.ConvertToJson((XElement)value));
                return;

            }
#if !EMBEDQPT
			if (value is UObj)
			{
				output.Write(((UObj)value).ToJson());
				return;
			}
#endif
            base.Serialize(name, value, output, options);
        }
	}
}