using System.Collections.Generic;
using System.IO;
using Qorpent.Serialization;

namespace Qorpent.IoC {
	/// <summary>
	/// Специальная сериализуемая обертка над компонентом контейнера
	/// </summary>
	[Serialize]
	public class ComponentDefinitionWrapper {
		/// <summary>
		/// Создает обертку
		/// </summary>
		/// <param name="c"></param>
		public ComponentDefinitionWrapper(IComponentDefinition c) {
			Name = c.Name;
			var impltype = c.Implementation == null
				               ? c.ImplementationType
				               : c.Implementation.GetType();
			ImplAssembly = impltype.Assembly.GetName().Name;
			ImplNs = impltype.Namespace;
			ImplType = impltype.Name;

			ServiceAssembly = c.ServiceType.Assembly.GetName().Name;
			ServiceNs = c.ServiceType.Namespace;
			ServiceType = c.ServiceType.Name;
			Lifestyle = c.Lifestyle;
			Tag = c.Tag;
			Role = c.Role;
			Priority = c.Priority;
			ActivationCount = c.ActivationCount;
			CreationCount = c.CreationCount;
			Parameters = c.Parameters;
			Help = c.Help;
			Id = c.ContainerId;
			if (null != c.Source) {
				FileName =Path.GetFileName( c.Source.Attribute("_file").Value);
				Line = c.Source.Attribute("_line").Value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[SerializeNotNullOnly]
		public string Line { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[SerializeNotNullOnly]
		public string FileName { get; set; }

		/// <summary>
		/// Номер в контейнере
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Помощь по компоненту
		/// </summary>
		[SerializeNotNullOnly]
		public string Help { get; set; }

		/// <summary>
		/// Счетчик созданных объектов
		/// </summary>
		[SerializeNotNullOnly]
		public int CreationCount { get; set; }
		/// <summary>
		/// Счетчик активаций
		/// </summary>
		[SerializeNotNullOnly]
		public int ActivationCount { get; set; }


		/// <summary>
		/// 
		/// </summary>
		[SerializeNotNullOnly]
		public IDictionary<string, object> Parameters { get; set; }

		/// <summary>
		/// Имя компонента
		/// </summary>
		[SerializeNotNullOnly]
		public int Priority { get; set; }
		/// <summary>
		/// Имя компонента
		/// </summary>
		[SerializeNotNullOnly]
		public string Role { get; set; }
		/// <summary>
		/// Имя компонента
		/// </summary>
		[SerializeNotNullOnly]
		public string Tag { get; set; }

		/// <summary>
		/// Цикл жизни
		/// </summary>
		[Serialize]
		public Lifestyle Lifestyle { get; set; }

		/// <summary>
		/// Имя компонента
		/// </summary>
		[SerializeNotNullOnly]
		public string ServiceType { get; set; }
		/// <summary>
		/// Имя компонента
		/// </summary>
		[SerializeNotNullOnly]
		public string ServiceNs { get; set; }
		/// <summary>
		/// Имя компонента
		/// </summary>
		[SerializeNotNullOnly]
		public string ServiceAssembly { get; set; }
		/// <summary>
		/// Имя компонента
		/// </summary>
		[SerializeNotNullOnly]
		public string ImplType { get; set; }
		/// <summary>
		/// Имя компонента
		/// </summary>
		[SerializeNotNullOnly]
		public string ImplNs { get; set; }
		/// <summary>
		/// Имя компонента
		/// </summary>
		[SerializeNotNullOnly]
		public string ImplAssembly { get; set; }

		/// <summary>
		/// Имя компонента
		/// </summary>
		[SerializeNotNullOnly]
		public string Name { get; set; }
	}
}