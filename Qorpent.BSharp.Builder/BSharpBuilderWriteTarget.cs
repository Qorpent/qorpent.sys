using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using System.Xml.Linq;

namespace Qorpent.Integration.BSharp.Builder {
    /// <summary>
    ///     Описание файла-цели BSharp builder'а
    /// </summary>
    public class BSharpBuilderWriteTarget {
        private string _entityContainerName;
        private XElement _entity;
        /// <summary>
        ///     Директория, в которой хранится файл
        /// </summary>
        public string Directory { get; set; }
        /// <summary>
        ///     Имя файла без расширения
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        ///     Расширение файла
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        ///     Имя контейнера для сущности
        /// </summary>
        public string EntityContainerName {
            get { return _entityContainerName ?? (_entityContainerName = BSharpBuilderDefaults.BSharpClassetName); }
            set { _entityContainerName = value; }
        }
        /// <summary>
        ///     XML содержимое файла
        /// </summary>
        public XElement Entity {
            get { return _entity; }
            set { _entity = value; }
        }
        /// <summary>
        ///     Указывет на то, стоит ли сливать цель в одну, если
        ///     уже такая существует
        /// </summary>
        public bool MergeIfExists { get; set; }
        /// <summary>
        ///     Акцессор генерации полного пути файла
        /// </summary>
        public string Path {
            get {
                return System.IO.Path.Combine(Directory, Filename + "." + Extension);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public BSharpBuilderWriteTarget() {
            _entity = new XElement(EntityContainerName);
        }
        /// <summary>
        ///     Примитивное слияние двух целей.
        ///     Параметры (имя, расширение, директория) цели приобретает
        ///     значения источника, сущность сливается через Add
        /// </summary>
        /// <param name="source">Источник слияния</param>
        public void Merge(BSharpBuilderWriteTarget source) {
            Directory = source.Directory;
            Extension = source.Extension;
            Filename = source.Filename;

            _entity.Add(source.Entity.Elements());
        }
    }
}
