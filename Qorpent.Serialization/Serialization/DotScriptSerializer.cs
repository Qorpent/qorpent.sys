using System;
using System.IO;
using Qorpent.Graphs;
using Qorpent.IoC;

namespace Qorpent.Serialization {
    /// <summary>
    /// Сериализатор для формата DotScript при разрисовке графиков
    /// </summary>
    [ContainerComponent(Lifestyle.Transient, ServiceType = typeof(ISerializer), Name = "dot.serializer")]
    public class DotScriptSerializer:ISerializer {
        /// <summary>
        /// Сериализует переданный объект в текстовой поток
        /// </summary>
        /// <param name="name"> Имя сериализуемого объекта</param>
        /// <param name="value">Сериализуемый объект </param>
        /// <param name="output">Целевой текстововй поток</param>
        /// <param name="options">В качестве опций могут быть переданы <see cref="GraphOptions"/></param>
        /// <remarks>
        /// Такое определение интерфейса предполагает, что сериализация производится в поток,
        /// но при этом мы не предполагаем бинарной сериализации, так как бинарная сериализация
        /// не является типовым сценарием для коммутриуемх API
        /// </remarks>
        public void Serialize(string name, object value, TextWriter output, object options =null) {
            var _options = options as GraphOptions;
            if (null == value) {
                output.Write("digraph NULL{}");
                return;
            }
            if (value is string) {
                output.Write((string)value);
                return;
            }
            if (value is IGraphConvertible) {
                output.Write( ((IGraphConvertible)value).GenerateGraphScript(_options));
                return;
            }
            if (value is IGraphSource)
            {
                output.Write(((IGraphSource)value).BuildGraph(_options).GenerateGraphScript());
                return;
            }
            throw  new Exception("given object cannot be represented as DOT");
        }
    }
}