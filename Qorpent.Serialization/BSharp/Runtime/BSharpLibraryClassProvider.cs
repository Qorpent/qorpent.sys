using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Events;
using Qorpent.IO;
using Qorpent.Utils.Extensions;
namespace Qorpent.BSharp.Runtime
{
    /// <summary>
    /// Провайдер классов на основе bslib
    /// </summary>
    public class BSharpLibraryClassProvider : BSharpClassProviderBase
    {
        /// <summary>
        /// Имя целевого файла библиотеки
        /// </summary>
        public string Filename { get; set; }

        private IFileSource _fileSource;

        /// <summary>
        /// Перегружает файл и формирует индекс на его основе
        /// </summary>
        protected override void RebuildIndex() {
            ReloadLibSource();
            var indexXml = XElement.Load(_fileSource.Open("/index"));
            foreach (var xcls in indexXml.Elements("class")) {
                var key = xcls.Attr("fullcode");
                if (!Cache.ContainsKey(key)) {
                    var desc = new BSharpRuntimeClassDescriptor();
                    desc.Fullname = key;
                    desc.ResourceName = xcls.Attr("uri");
                    var cls = new BSharpRuntimeClass(Container);
                    cls.Fullname = key;
                    cls.Name = xcls.Attr("code");
                    cls.Namespace = xcls.Attr("namespace");
                    cls.PrototypeCode = xcls.Attr("prototype");
                    cls.RuntimeCode = xcls.Attr("runtime");
                    cls.Loaded = false;
                    desc.CachedClass = cls;

                    Cache[key] = desc;
                    
                }
            }
        }

        /// <summary>
        ///     Перегружает класс с диска
        /// </summary>
        /// <param name="descriptor"></param>
        protected override void ReloadClass(BSharpRuntimeClassDescriptor descriptor) {
            var uri = descriptor.ResourceName;
            if (null == descriptor.CachedClass) {
                descriptor.CachedClass= new BSharpRuntimeClass(Container);    
            }
            var cls = (BSharpRuntimeClass) descriptor.CachedClass;
            cls.Definition = XElement.Load(_fileSource.Open(uri));
            cls.Loaded = true;
        }

        private void ReloadLibSource() {
            if (null != _fileSource) {
                if (_fileSource is IResetable) {
                    ((IResetable) _fileSource).Reset(null);
                }
            }
            else {
                if (Filename.StartsWith("res:")) {
                    var resdesc = Filename.Split(':');
                    var assembly = Assembly.Load(resdesc[1]);
                    _fileSource = ResolveService<IFileSource>("zip.file.source", assembly.OpenManifestResource(resdesc[2]));
                }
                else {
                    _fileSource = ResolveService<IFileSource>("zip.file.source", Filename);
                }
            }
        }
    }
}
