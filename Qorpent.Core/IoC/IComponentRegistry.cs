namespace Qorpent.IoC {
	/// <summary>
	/// Интерфейс реестра компонентов
	/// </summary>
	public interface IComponentRegistry : IComponentSource {
		/// <summary>
		/// 	Регистрирует новый компонент в контейнере
		/// </summary>
		/// <param name="component"> Компонент для регистрации </param>
		/// <remarks>
		/// 	Вы можете, но не обязаны использовать компоненты ранее созданные при помощи <see cref="EmptyComponent" />, <see
		/// 	 cref="IContainer.NewExtension{TService}" />,
		/// 	<see cref="NewComponent{TService,TImplementation}" />, однако использование данных методов позволит ваше приложение от необходимости знать классы - наследники
		/// 	<see cref="IComponentDefinition" />.<br />
		/// 	<invariant>В Qorpent новые компоененты не вытесняют старые, а кладутся в стек, соответственно при
		/// 		<see cref="Unregister" />
		/// 		тип сервиса из контейнера не исчезает, 
		/// 		а возвращается к более ранней версии.
		/// 		<br />
		/// 		Разрешение компонентов при этом ведется по логике "самый подходящий" исходя из порядка появления в контейнере, имени и явно указанного
		/// 		<see cref="IComponentDefinition.Priority" />
		/// 		.
		/// 		<br />
		/// 		Соответственно
		/// 		<see cref="IComponentSource.GetComponents" />
		/// 		возвращает все версии всех компонентов.
		/// 		<br />
		/// 		При указании в компоненте
		/// 		<see cref="Lifestyle.ContainerExtension" />
		/// 		, компонент перенаправляется на регистрацию в
		/// 		<see cref="IContainer.RegisterExtension" />
		/// 	</invariant>
		/// </remarks>
		void Register(IComponentDefinition component);

		/// <summary>
		/// 	Отменяет регистрацию компонента
		/// </summary>
		/// <param name="component"> компонент, который должен быть убран из контейнера </param>
		/// <remarks>
		/// 	<note>Очевидно что, такой метод обязан присутствовать в интерфейсе контейнера, однако его использование в задачах помимо тестирования,
		/// 		обозначает недостатки архитектуры приложения, так как в нормальном варианте использования контейнер меняет свое поведение по принципу наращивания
		/// 		обслуживаемых классов и компонентов, тогда как удаление части компонент может привести к неожиданным эффектам в случае кэширования более
		/// 		ранеей выдвачи клиентской стороной</note>
		/// </remarks>
		void Unregister(IComponentDefinition component);

		/// <summary>
		/// 	Возвращает класс-загрузчик контейнера из манифеста
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// 	Этот фабричный метод включен в интерфейс <see cref="IContainer" />, а не
		/// 	производится поиск в самом контейнере, так как загрузка манифеста относится к инициализации контейнера
		/// 	и соответственно использование его метода <see cref="ITypeResolver.Get{T}" /> считается небезопасным
		/// </remarks>
		IContainerLoader GetLoader();

		/// <summary>
		/// 	Создает пустой компонент для настройки (регистрации не производится!)
		/// </summary>
		/// <returns> Пустой экземпляр <see cref="IComponentDefinition" /> </returns>
		/// <remarks>
		/// 	Используйте этот метод и затем вызывайте <see cref="Register" />
		/// </remarks>
		IComponentDefinition EmptyComponent();

		/// <summary>
		/// 	Создает новый компонент с указанными настройками
		/// </summary>
		/// <param name="lifestyle"> Цикл жизни компонента </param>
		/// <param name="name"> Имя </param>
		/// <param name="priority"> Приоритетность </param>
		/// <param name="implementation"> Готовый экземпляр </param>
		/// <typeparam name="TService"> Тип сервиса </typeparam>
		/// <typeparam name="TImplementation"> Тип реализации </typeparam>
		/// <returns> Пред-настроенный экземпляр <see cref="IComponentDefinition" /> </returns>
		/// <remarks>
		/// 	Используйте этот метод и затем вызывайте <see cref="Register" />
		/// </remarks>
		IComponentDefinition NewComponent<TService, TImplementation>(Lifestyle lifestyle = Lifestyle.Transient,
		                                                             string name = "", int priority = 10000,
		                                                             TImplementation implementation = null)
			where TService : class where TImplementation : class, TService, new();
	}
}