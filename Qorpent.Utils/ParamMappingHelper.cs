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
// PROJECT ORIGIN: Qorpent.Utils/ParamMappingHelper.cs
#endregion
using System.Collections.Generic;
using Qorpent.Data;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
    /// <summary>
    /// Вспомогательный класс для подготовки параметров
    /// </summary>
    public class ParamMappingHelper {
        /// <summary>
        /// Получить параметры SQL из провайдера
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public IDictionary<string ,object > GetParameters(IParametersProvider provider) {
            if(provider.UseCustomMapping) {
                return provider.GetParameters();
            }
            var props = provider.GetType().GetProperties();
            var result = new Dictionary<string, object>();
            foreach (var pi in props) {
                var a = pi.GetFirstAttribute<ParamAttribute>();
                if(null!=a) {
                    var name = pi.Name;
                    if (a.Name.IsNotEmpty()) name = a.Name;
                    name = "@" + name;
                    result[name] = pi.GetValue(provider,null);
                }
            }
            return result;
        }
    }
}