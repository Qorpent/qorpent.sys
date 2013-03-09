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