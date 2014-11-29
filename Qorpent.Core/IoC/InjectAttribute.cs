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
// PROJECT ORIGIN: Qorpent.Core/InjectAttribute.cs
#endregion
using System;

namespace Qorpent.IoC {
	/// <summary>
	/// 	Marks service-bound properties to be processed with container based injection
	/// </summary>
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, AllowMultiple = false)]
	public class InjectAttribute : ContainerAttribute {
	    private Type _factoryType;

	    /// <summary>
		/// 	Name of component (for named injections)
		/// </summary>
		public string Name { get; set; }
        /// <summary>
        /// Тип фабрики для формирования объекта по умолчанию, должна наследовать IFactory
        /// </summary>
        public Type FactoryType {
            get { return _factoryType; }
            set {
                if (null != value) {
                    if(!typeof(IFactory).IsAssignableFrom(value))throw new Exception("must be IFactory");
                    if (value.IsAbstract) throw new Exception("must be non-abstract");
                }
                _factoryType = value;
            }
        }
        /// <summary>
        /// Sygnals that this Injection MUST be resolved anyway , exception must be thrown otherwise
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Маска для сверки имен при связывании коллекций
        /// </summary>
	    public string NameMask { get; set; }
        /// <summary>
        /// Тип для создания по умолчанию
        /// </summary>
	    public Type DefaultType { get; set; }
	}
}