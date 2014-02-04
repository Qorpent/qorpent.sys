using System.Collections.Generic;

namespace Qorpent.IoC{
	/// <summary>
	/// 	direct assembly into container registration interface, called not for components in container
	/// </summary>
	public interface IContainerInitializer
	{
		/// <summary>
		/// 	called by ContainerFactory after loading manifest and default services against container to be configured
		/// </summary>
		/// <param name="container"> </param>
		/// <param name="context"></param>
		IEnumerable<IComponentDefinition> Setup(IContainer container,object context);
	}
}