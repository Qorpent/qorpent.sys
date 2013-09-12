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
// PROJECT ORIGIN: Qorpent.Serialization/bxlSerializer.cs
#endregion
using System.IO;
using Qorpent.IoC;

namespace Qorpent.Serialization {
	/// <summary>
	/// Конвертирует входной объект в BXL-представление.
	/// </summary>
	/// <remarks>
    /// В своей работе использует <see cref="BxlSerializerImpl"/>.
    /// Соответственно сначала объект приводится к XML представлению, которое приводится к BXL-эквиваленту.
    /// Напрямую использовать не рекомендуется, стандартное использование через IOC
	/// </remarks>
	/// <example>
	/// var mydata = new {x=1,y=2,z=3};
	/// var output = new StringWriter();
	/// var serializer = Application.Current.Container.Get&lt;ISerializer&gt;("bxl.serializer");
	/// serializer.Serialize("mytest",mydata,output);
	/// Console.WriteLine(output.ToString());
	/// </example>
	[ContainerComponent(Lifestyle.Transient, ServiceType = typeof (ISerializer), Name = "bxl.serializer")]
	public class BxlSerializer : Serializer {
		/// <summary>
        /// Создает экземпляр <see cref="ISerializerImpl"/> ( <see cref="BxlSerializerImpl"/> )
		/// </summary>
		/// <param name="name">Имя объекта сериализации</param>
		/// <param name="value">Объект сериализации</param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected override ISerializerImpl CreateImpl(string name, object value) {
			return new BxlSerializerImpl();
		}

		/// <summary>
		/// 	Serializes the specified name.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <param name="output"> The output. </param>
		/// <remarks>
		/// </remarks>
		public override void Serialize(string name, object value, TextWriter output) {
			if (null == value) {
				output.Write(name + " : null");
			}
			else {
				base.Serialize(name, value, output);
			}
		}
	}
}