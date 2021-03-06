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
// PROJECT ORIGIN: Qorpent.Utils/ContainerExtensions.cs
#endregion
using System.Linq;
using Qorpent.IoC;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// </summary>
	public static class ContainerExtensions {
	    public static IContainer Set<T>(this IContainer container, T implementation, Lifestyle lifestyle = Lifestyle.Singleton, string name = null) {
	        var component = container.EmptyComponent();
	        component.ServiceType = typeof (T);
	        component.Implementation = implementation;
            component.Lifestyle = lifestyle;
	        component.Name = name;
            container.Register(component);
	        return container;
	    }
        public static IContainer Set<T,I>(this IContainer container, Lifestyle lifestyle = Lifestyle.Singleton, string name = null)
            where I:T
        {
            var component = container.EmptyComponent();
            component.ServiceType = typeof(T);
            component.ImplementationType = typeof (I);
            component.Lifestyle = lifestyle;
            component.Name = name;
            container.Register(component);
            return container;
        }

        /// <summary>
        /// </summary>
        /// <param name="container"> </param>
        /// <param name="name"> </param>
        /// <param name="value"> </param>
        /// <typeparam name="T"> </typeparam>
        /// <returns> </returns>
        public static IContainer SetParameter<T>(this IContainer container, string name, object value) {
			if (null != container) {
				var component = container.GetComponents().FirstOrDefault(x => x.ServiceType == typeof (T) || x.ImplementationType==typeof(T));
				if (null != component) {
					component.Parameters[name] = value;
				}
			}
			return container;
		}
	}

	
}