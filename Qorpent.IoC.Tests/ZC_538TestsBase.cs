using System.Collections.Generic;
using NUnit.Framework;
using Qorpent.Applications;

namespace Qorpent.IoC {
    /// <summary>
    /// 
    /// </summary>
    class HostClass : IHostClass {
        /// <summary>
        /// 
        /// </summary>
        [Inject]
        public IList<IExtensionClass> List { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public HostClass() {
            List = new List<IExtensionClass>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExtensionClass : IExtensionClass {}

    /// <summary>
    /// 
    /// </summary>
    public interface IHostClass {
        /// <summary>
        /// 
        /// </summary>
        IList<IExtensionClass> List { get; }

    }

    /// <summary>
    /// 
    /// </summary>
    public interface IExtensionClass {}

    /// <summary>
    /// 
    /// </summary>
    class ZC_538TestsBase {
        protected Container Container;

        [TestFixtureSetUp]
        public void TestFixtureSetup() {
            Container = new Container();

            Container.Register(
                new ComponentDefinition<IExtensionClass, ExtensionClass>(Lifestyle.Extension, "Ext1")
            );

            Container.Register(
                new ComponentDefinition<IExtensionClass, ExtensionClass>(Lifestyle.Extension, "Ext2")
            );

            Container.Register(
                new ComponentDefinition<IExtensionClass, ExtensionClass>(Lifestyle.Extension, "Ext3")
            );

            Container.Register(
                new ComponentDefinition<IHostClass, HostClass>()
            );
        }
    }
}