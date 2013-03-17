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
// PROJECT ORIGIN: Qorpent.Core/DefaultSerializerFactory.cs
#endregion
using Qorpent.IoC;

namespace Qorpent.Serialization {
	/// <summary>
	/// 	Container-bound serialization factory that uses ISerializer+formatname+'.serializer' logic to found serializers in container
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton)]
	public class DefaultSerializerFactory : ServiceBase, ISerializerFactory {
		/// <summary>
		/// 	Returns serializer to use for given format
		/// </summary>
		/// <param name="format"> </param>
		/// <param name="customFormatName"> </param>
		/// <returns> </returns>
		public ISerializer GetSerializer(SerializationFormat format, string customFormatName = null) {
			lock (Sync) {
				var name = format.ToString().ToLower();
				if (format == SerializationFormat.Default) {
					name = "xml";
				}
				else if (format == SerializationFormat.Custom) {
					name = customFormatName;
				}
				if (string.IsNullOrEmpty(name)) {
					throw new QorpentException("no format given for serialization");
				}
				var componentname = name + ".serializer";
				var serializer = ResolveService<ISerializer>(componentname);
				if (null == serializer) {
					throw new QorpentException("no serializer found for component name " + componentname);
				}
				return serializer;
			}
		}
	}
}