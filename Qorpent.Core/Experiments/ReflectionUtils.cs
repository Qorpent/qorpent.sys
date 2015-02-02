using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq.Expressions;
using System.Reflection;

namespace Qorpent.Experiments {
    /// <summary>
    ///     Вспомогательный класс для получения и сохранения значений в класс при помощи бы стрых методов
    /// </summary>
    public static class ReflectionUtils {

        private static readonly IDictionary<MemberInfo, object> UNGenGetCache = new Dictionary<MemberInfo, object>();
        private static readonly IDictionary<MemberInfo, object> UNGenSetCache = new Dictionary<MemberInfo, object>();
        private static readonly IDictionary<string,bool> GenericCompatibleTestCache =new Dictionary<string, bool>(); 
        /// <summary>
        /// Проверяет, что объект поддерживает базовый приведенный интерфейс
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="obj">object or Type</param>
        /// <returns></returns>
        /// <remarks>Например IsGenericCompatible[IEnumerable[KeyValuePair[object,object]]] должен вернуть True для всех генерик- словарей</remarks>
        public static bool IsGenericCompatible<I>(object obj) where I:class {
            if(null==obj)throw new ArgumentNullException("obj");
            var type = obj is Type ? obj as Type : obj.GetType();
            var testtype = typeof (I);
            var key = type.FullName + "_" + testtype.FullName;
            if (!GenericCompatibleTestCache.ContainsKey(key)) {
                var result = testtype.IsAssignableFrom(type);
                if (!result && testtype.IsGenericType) {
                    var gendef = testtype.GetGenericTypeDefinition();
                    var args = testtype.GetGenericArguments();
                    
                    foreach (var i in type.GetInterfaces()) {
                        if (i.IsGenericType) {
                            var idef = i.GetGenericTypeDefinition();
                            if (idef == gendef) {
                                var iargs = i.GetGenericArguments();
                                var argsResult = true;
                                for (var ia = 0; ia < args.Length; ia++) {
                                    var basea = args[ia];
                                    var testa = iargs[ia];
                                    if (basea.IsGenericType && testa.IsGenericType) {
                                        basea = basea.GetGenericTypeDefinition();
                                        testa = testa.GetGenericTypeDefinition();
                                        if (basea != testa) {
                                            argsResult = false;
                                            break;

                                        }
                                    }
                                    else {
                                        if (!basea.IsAssignableFrom(testa)) {
                                            argsResult = false;
                                            break;
                                            
                                        }
                                    }

                                }
                                if (argsResult) {
                                    result = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                GenericCompatibleTestCache[key] = result;
            }
            return GenericCompatibleTestCache[key];
        }


        /// <summary>
        ///     Возвращает нетипизированный сетер
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static Action<object, object> BuildSetter(PropertyInfo propertyInfo) {
            if (!UNGenSetCache.ContainsKey(propertyInfo)) {
                var targetType = propertyInfo.DeclaringType;
                var methodInfo = propertyInfo.GetSetMethod(true);
                var exTarget = Expression.Parameter(typeof (object), "t");
                var exValue = Expression.Parameter(typeof (object), "p");
                var exBody = Expression.Call(Expression.Convert(exTarget, targetType), methodInfo,
                Expression.Convert(exValue, propertyInfo.PropertyType));
                var lambda = Expression.Lambda<Action<object, object>>(exBody, exTarget, exValue);
                var action = lambda.Compile();
                UNGenSetCache[propertyInfo] = action;
            }
            return (Action<object, object>) UNGenSetCache[propertyInfo];
        }


        /// <summary>
        ///     Возвращает нетипизированный гетер
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static Func<object, object> BuildGetter(PropertyInfo propertyInfo) {
            if (!UNGenGetCache.ContainsKey(propertyInfo)) {
                var targetType = propertyInfo.DeclaringType;
                var methodInfo = propertyInfo.GetGetMethod(true);

                var exTarget = Expression.Parameter(typeof (object), "t");
                var exBody = Expression.Call(Expression.Convert(exTarget, targetType), methodInfo);
                var exBody2 = Expression.Convert(exBody, typeof (object));

                var lambda = Expression.Lambda<Func<object, object>>(exBody2, exTarget);
                // t => Convert(t.get_Foo())

                var action = lambda.Compile();

                UNGenGetCache[propertyInfo] = action;
            }
            return (Func<object, object>) UNGenGetCache[propertyInfo];
        }
    }
}