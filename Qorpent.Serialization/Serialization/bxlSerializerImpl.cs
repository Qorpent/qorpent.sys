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
// PROJECT ORIGIN: Qorpent.Serialization/bxlSerializerImpl.cs
#endregion
using System.Linq;
using Qorpent.Applications;
using Qorpent.Bxl;

namespace Qorpent.Serialization {
    /// <summary>
    /// Реализует конвертацию XML в BXL, является оберткой над <see cref="XmlSerializerImpl"/>
    /// </summary>
	internal class BxlSerializerImpl : XmlSerializerImpl {
        /// <summary>
        /// В самом конце генерации при закрытии объекта производится собственно запись в поток
        /// результата преобразования Xml в Bxl 
        /// </summary>
        /// <remarks>Используется стандартный <see cref="IBxlParser"/> в режими NoRootElement, так как рутом является сам сериализуемый объект</remarks>
		public override void End() {
			var e = Root;
			Output.Write(Application.Current.Bxl.GetParser().Generate(e,new BxlGeneratorOptions{NoRootElement = true}));
		}
	}
}