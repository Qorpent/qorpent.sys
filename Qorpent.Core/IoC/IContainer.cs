#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : IContainer.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace Qorpent.IoC {
	///<summary>
	///	Описывает контейнер активации типов приложения (IoC+DI)
	///</summary>
	///<qorpentimplemented ref="Qorpent.IoC~Qorpent.IoC.Container">Container</qorpentimplemented>
	///<remarks>
	///	<note>Контейнер является ключевым и наиболее главным узлом  Qorpent. 
	///		Контейнер позволяет обеспечить спайку приложения без необходимости знания о конкретных классах, выполняющих функционал</note>
	///	<note>Контейнер Qorpent реализует паттерны
	///		<see href="http://citforum.ru/SE/project/pattern/#3.1.7">Low-Coupling</see>
	///		,
	///		<see href="http://citforum.ru/SE/project/pattern/p_2.shtml#3.3.1">Factory</see>
	///		,
	///		<see
	///			href="http://ru.wikipedia.org/wiki/%D0%98%D0%BD%D0%B2%D0%B5%D1%80%D1%81%D0%B8%D1%8F_%D1%83%D0%BF%D1%80%D0%B0%D0%B2%D0%BB%D0%B5%D0%BD%D0%B8%D1%8F">Inversion-of-Control</see>
	///		,
	///		<see
	///			href="http://ru.wikipedia.org/wiki/%D0%92%D0%BD%D0%B5%D0%B4%D1%80%D0%B5%D0%BD%D0%B8%D0%B5_%D0%B7%D0%B0%D0%B2%D0%B8%D1%81%D0%B8%D0%BC%D0%BE%D1%81%D1%82%D0%B8">Dependency-Injection</see>
	///		Идеологическим предшественником контейнера в Qorpent является
	///		<see href="http://www.castleproject.org/">Castle Windsor</see>
	///	</note>
	///	При использовании Qorpent необходимо стремиться к тому, чтобы все взаимное связывание классов осуществлялось <strong>через интерфейсы</strong> и посредством <strong>контейнера</strong>.
	///	Только в этом случае обеспечивается максимальная гибкость, простота развертывания и настройки приложения.
	///	<h3>Терминология, используемая в Qorpent.IoC</h3>
	///	<list type="table">
	///		<item>
	///			<term>Контейнер</term>
	///			<description>Центральный компонент IoC, обслуживающий запросы на
	///				<strong>активацию объектов</strong>
	///				и конфигурацию компонентов.
	///				(cм.:
	///				<see cref="IContainer" />
	///				->
	///				<see href="Qorpent.IoC~Qorpent.IoC.Container" />
	///				)</description>
	///		</item>
	///		<item>
	///			<term>Компонент</term>
	///			<description>Структура, описывающая связь между интерфейсом сервиса и конкретным типом (классом), реализующим данный интерфейс. Компонент управляет
	///				<strong>циклом жизни</strong>
	///				объекта, определяет его персональное
	///				<strong>имя</strong>
	///				,
	///				<strong>приоритетность</strong>
	///				(см.:
	///				<see cref="IComponentDefinition" />
	///				,
	///				<see cref="EmptyComponent" />
	///				)</description>
	///		</item>
	///		<item>
	///			<term>Цикл жизни компонента</term>
	///			<description>Контейнер Qorpent поддерживает различные циклы поведения объекта при загрузке из контейнера,
	///				вариант определяется перечислением
	///				<see cref="Lifestyle" />
	///				, определенным через
	///				<see cref="IComponentDefinition.Lifestyle" />
	///				.
	///				Наиболее значимымим из них являются синглетон:
	///				<see cref="Lifestyle.Singleton" />
	///				и временные объекты
	///				<see cref="Lifestyle.Transient" />
	///			</description>
	///		</item>
	///		<item>
	///			<term>Фабрика контейнеров</term>
	///			<description>Вспомогательный класс, позволяющий быстро инстанцировать и настроить контейнер приложения, используя поведение настройки по умолчанию
	///				(см.
	///				<see cref="ContainerFactory" />
	///				)</description>
	///		</item>
	///		<item>
	///			<term>Расширение контейнера</term>
	///			<description>Любой класс, реализующий
	///				<see cref="IContainerExtension" />
	///				и включенный в контейнер через
	///				<see cref="RegisterExtension" />
	///				. Может использоваться
	///				для модификации поведения контейнера</description>
	///		</item>
	///	</list>
	///	<h3>Основные задачи контейнера</h3>
	///	<list type="bullet">
	///		<item>Хранение конфигурации, описывающей соответствие интерфейсов и классов, осуществляющих их имплементацию (компонентов)</item>
	///	</list>
	///	<invariant></invariant>
	///</remarks>
	///<example>
	///	<h3>Элементарный вариант создания, конфигурации и использования контейнера</h3>
	///	<note>Пример лишь показывает использование контейнера для обеспечения IoC,
	///		никакие дополнительные опции, включаяя DI, параметры, циклы жизни в данном примере не используются
	///		и он не являет собой рекомендуемую практику для работы с контейнером в Qorpent</note>
	///	<code>//ref Qorpent.Core,Qorpent.IoC
	///		using Qorpent.IoC;
	///		using System;
	///		namespace Ex1{
	///		public interface IMyService { //некий общедоступный сервис
	///		string GetHelloWorld();
	///		}
	///		public class MyServiceUser {  // некий класс, использующий сервис через контейнер
	///		IMyService _myservice1;
	///		IMyService _myservice2;
	///		public MyServiceUser(IContainer container){
	///		_myservice1 = container.Get&lt;IMyService&gt;();		// экземпляр с настройками по умолчанию						
	///		_myservice1 = container.Get&lt;IMyService&gt;(name:null,"my hello world!"); // экземпляр со специальным конструктором
	///		}
	///		public void Run(){
	///		Console.WriteLine(_myservice1.GetHelloWorld());
	///		Console.WriteLine(_myservice2.GetHelloWorld());
	///		}
	///		}
	///		public class MyProgram {
	///		private class MyService:IMyService{ //частная реализация сервиса, невидимая никому кроме MyProgram
	///		public MyService(){}
	///		public MyService(string customHello){
	///		_hello = customHello;
	///		}
	///		_hello = "Hello!";
	///		public string GetHelloWorld(){
	///		return _hello;
	///		}
	///		}
	///		public static void Main(string[] args) {
	///		var container = ContainerFactory.CreateEmpty(); //создаем пустой контейнер, тип определяется автоматически
	///		container.Register(container.NewComponent&lt;IMyService,MyService&gt;()); //региструем наш класс в контейнере (по умолчанию без имени, временный)
	///		var client = new MyServiceUser(container); // мы по сути передали в клиента некую среду где теперь есть сервис IMyService
	///		client.Run();
	///		// Output will be:
	///		// Hello!
	///		// my hello world!
	///		// ГЛАВНОЕ: MyServiceUser смог самостоятельно загрузить и использовать частный класс MyService без всякого знания о нем самом,
	///		// его конструкторов, а лишь пользуясь его интерфейсом.
	///		}
	///		}
	///		
	///		}</code>
	///</example>
	public interface IContainer {
		///<summary>
		///	Если <c>true</c> - то при отсутствии запрашиваемого сервиса будет возвращаться не Null, а <see
		///	 cref="ContainerException" />
		///</summary>
		bool ThrowErrorOnNotExistedComponent { get; set; }

		/// <summary>
		/// 	Возвращает сервис указанного типа. (обобщенный)
		/// </summary>
		/// <typeparam name="T"> Тип сервиса </typeparam>
		/// <param name="name"> опциональное имя компонента, если указано - поиск будет производиться только среди с компонентов с указаным именем </param>
		/// <param name="ctorArguments"> Параметры для вызова конструктора, если не указано - будет использован конструктор по умолчанию. </param>
		/// <returns> Сервис указанного типа или <c>null</c> , если подходящий компонент не обнаружен </returns>
		/// <exception cref="ContainerException">При ошибках в конструировании объектов</exception>
		/// <remarks>
		/// 	<invariant>Данный метод является простой оболочкой над
		/// 		<see cref="Get" />
		/// 		, см. документацию данного метода для более подробной информации</invariant>
		/// </remarks>
		T Get<T>(string name = null, params object[] ctorArguments) where T : class;

		///<summary>
		///	Возвращает сервис указанного типа. (прямое указание типа)
		///</summary>
		///<param name="type"> Тип сервиса </param>
		///<param name="ctorArguments"> Параметры для вызова конструктора, если не указано - будет использован конструктор по умолчанию. </param>
		///<param name="name"> опциональное имя компонента, если указано - поиск будет производиться только среди с компонентов с указаным именем </param>
		///<returns> Сервис указанного типа или <c>null</c> , если подходящий компонент не обнаружен </returns>
		///<exception cref="ContainerException">При ошибках в конструировании объектов</exception>
		///<remarks>
		///	При вызове метода, контейнер осуществляет поиск в своей конфигурации и находит компонент, максимаольно соответствующий условию на класс сервиса 
		///	и имя. Если компонент не находится, то возвращается null. <note>При включении параметра
		///		                                                          <see cref="ThrowErrorOnNotExistedComponent" />
		///		                                                          =
		///		                                                          <c>true</c>
		///		                                                          ,
		///		                                                          вместо возврата null будет осуществлятся генерация исключения
		///		                                                          <see cref="ContainerException" />
		///		                                                          .</note>
		///	<invariant>
		///		<h3>Разрешение компонентов</h3>
		///		<list type="bullet">
		///			<item>В качестве класса сервиса можно указывать как напрямую интерфейс требуемого сервиса,  так и базовый класс/интерфейс</item>
		///			<item>Поиск сервиса ведется среди зарегистрированных (см.
		///				<see cref="Register" />
		///				,
		///				<see cref="ContainerFactory" />
		///				,
		///				<see cref="GetComponents" />
		///				) компонентов.</item>
		///			<item>Если имя не указано то поиск ведется
		///				<strong>игнорируя имя</strong>
		///				компонента (а не среди компонентов с именем "" или null)</item>
		///			<item>Так как за одним типом/именем может быть закреплено несколько компонентов, то производится отбор наиболее релевантного:
		///				<list type="number">
		///					<item>Если один из компонентов явно эквивалентен по типу сервиса запрашиваемому, то он имеет приоритет перед теми, которые сконфигурированы через унаследованный интерфейс
		///						<note>На первый взгляд это кажется нелогичным, однако на деле типы сервисов являются явным ключем компонента и запроса, тогда
		///							как поиск по дополнительным сервисам - это дополнительая "фича" контейнера. Соответственно под разрешением типов следует понимать
		///							не "разрешение самого последнего в иерархии", а как "разрешение сначала точного, а затем лишь частично совпадаюшего ключа".
		///							Это помимо всего прочего избавляет от проблем при которых интерфейс наследуется от интерфейса сервиса для решения внутренних задач совместимости,
		///							но при этом он не должен использоваться как комонент для обслуживания базовго сервиса как такового</note>
		///					</item>
		///					<item>Если у компонента явно указан приоритет (см.
		///						<see cref="IComponentDefinition.Priority" />
		///						,
		///						<see cref="ContainerAttribute.Priority" />
		///						,
		///						<see cref="ContainerAttribute.Priority" />
		///						),
		///						то используется компонент с
		///						<strong>большим</strong>
		///						приоритетом</item>
		///					<item>При равенстве или отсутствии приоритетов, используется компонент зарегистрированные
		///						<strong>последним</strong>
		///					</item>
		///				</list>
		///			</item>
		///		</list>
		///		<h3>Возвращаемое значение и циклы жизни</h3>
		///		<para> Циклы жизни настраиваются в составе компонентов ( <see cref="IComponentDefinition.Lifestyle" /> ). И документацию по ним смотрите в <see
		///	 cref="Lifestyle" /> </para>
		///		<para> Циклы жизни влияют на поведение контейнера и метода Get. </para>
		///		<list type="number">
		///			<listheader>Порядок работы контейнера</listheader>
		///			<item>Поиск компонента (описано выше), вызов расширений определения компонента</item>
		///			<item>Идентификация или создание экземпляра (зависит от цикла жизни компонента)</item>
		///			<item>Если это новый экземпляр, то дополнительно производится впрыск зависимостей</item>
		///			<item>Активация компонента (формируется статистика вызова, вызываются расширения активации)</item>
		///			<item>Возврат экземпляра вызываемому коду</item>
		///		</list>
		///		<h3>Создание экземпляров</h3>
		///		Если цикл жизни подразумевает создание нового экземпляра, то его создание производится по следующему алгоритму:
		///		<list type="number">
		///			<item>Вызывается
		///				<see
		///					cref="Activator.CreateInstance(System.Type,System.Reflection.BindingFlags,System.Reflection.Binder,object[],System.Globalization.CultureInfo)" />
		///				,
		///				с поддержкой приватных конструкторов и передачей
		///				<paramref name="ctorArguments" />
		///			</item>
		///			<item>Производися впрыск параметров компонента
		///				<see cref="IComponentDefinition.Parameters" />
		///				, впрыск производится в защищенном режиме 
		///				(если свойство не найдено или тип не соответствует, то ничего не производится)</item>
		///			<item>Производится впрыск сервисов на основе
		///				<see cref="ContainerComponentAttribute" />
		///			</item>
		///			<item>Если экземпляр - наследник
		///				<see cref="IContainerBound" />
		///				(см. также
		///				<see cref="ServiceBase" />
		///				), то производится
		///				вызова
		///				<see cref="IContainerBound.SetContainerContext" />
		///				, для выполнения пользовательской настройки на контейнер</item>
		///		</list>
		///	</invariant>
		///</remarks>
		object Get(Type type, string name = null, params object[] ctorArguments);

		///<summary>
		///	Возвращает все объекты указаннго типа (обобщенииый)
		///</summary>
		///<typeparam name="T"> тип сервиса </typeparam>
		///<param name="ctorArguments"> Параметры для вызова конструктора, если не указано - будет использован конструктор по умолчанию. </param>
		///<param name="name"> опциональное имя компонента, если указано - поиск будет производиться только среди с компонентов с указаным именем </param>
		///<returns> Все экземпляры указанного сервиса </returns>
		///<remarks>
		///	<invariant>Метод All применим только для компонентов с циклом жизни
		///		<see cref="Lifestyle.Transient" />
		///		и
		///		<see cref="Lifestyle.Extension" />
		///		.
		///		<note>Не пытайтесь таким образом получить все экземпляры сервисов с другим циклом жизни</note>
		///		<para> Остальные особенности поведения контейнера при поиске и создании компонентов описаны в документации к <see
		///	 cref="Get" /> </para>
		///	</invariant>
		///</remarks>
		IEnumerable<T> All<T>(string name = null, params object[] ctorArguments) where T : class;

		/// <summary>
		/// 	Возвращает все объекты указаннго типа (прямое указание типа)
		/// </summary>
		/// <param name="ctorArguments"> Параметры для вызова конструктора, если не указано - будет использован конструктор по умолчанию. </param>
		/// <param name="type"> тип сервиса </param>
		/// <param name="name"> опциональное имя компонента, если указано - поиск будет производиться только среди с компонентов с указаным именем </param>
		/// <returns> Все экземпляры указанного сервиса </returns>
		/// <remarks>
		/// 	<invariant>Метод All применим только для компонентов с циклом жизни
		/// 		<see cref="Lifestyle.Transient" />
		/// 		и
		/// 		<see cref="Lifestyle.Extension" />
		/// 		.
		/// 		<note>Не пытайтесь таким образом получить все экземпляры сервисов с другим циклом жизни</note>
		/// 	</invariant>
		/// </remarks>
		IEnumerable All(Type type, string name = null, params object[] ctorArguments);

		/// <summary>
		/// 	Возвращает объект контейнеру
		/// </summary>
		/// <param name="obj"> Объект, ранее созданнй контейнером </param>
		/// <remarks>
		/// 	<invariant>Метод позволяет контейнеру высвобождать собственные ресурсы и вызывает
		/// 		<see cref="IContainerBound.OnContainerRelease" />
		/// 		метод,
		/// 		работает это только для
		/// 		<see cref="Lifestyle.Pooled" />
		/// 		и
		/// 		<see cref="Lifestyle.PerThread" />
		/// 		, для остальных данный метод игнорируется</invariant>
		/// </remarks>
		void Release(object obj);

		/// <summary>
		/// 	Регистрирует новый компонент в контейнере
		/// </summary>
		/// <param name="component"> Компонент для регистрации </param>
		/// <remarks>
		/// 	Вы можете, но не обязаны использовать компоненты ранее созданные при помощи <see cref="EmptyComponent" />, <see
		/// 	 cref="NewExtension{TService}" />,
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
		/// 		<see cref="GetComponents" />
		/// 		возвращает все версии всех компонентов.
		/// 		<br />
		/// 		При указании в компоненте
		/// 		<see cref="Lifestyle.ContainerExtension" />
		/// 		, компонент перенаправляется на регистрацию в
		/// 		<see cref="RegisterExtension" />
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
		/// 	Вызыает очистку контейнера
		/// </summary>
		/// <remarks>
		/// 	<invariant>Очистка очищает внутрненнее состояние и кэши контейнера, регистрации компонент это не касается, компоненты остаются в прежней конфигурации</invariant>
		/// </remarks>
		void CleanUp();

		/// <summary>
		/// 	Получить все зарегистрированные компоненты
		/// </summary>
		/// <returns> Все компоненты контейнера </returns>
		IEnumerable<IComponentDefinition> GetComponents();

		/// <summary>
		/// 	Возвращает класс-загрузчик контейнера из манифеста
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// 	Этот фабричный метод включен в интерфейс <see cref="IContainer" />, а не
		/// 	производится поиск в самом контейнере, так как загрузка манифеста относится к инициализации контейнера
		/// 	и соответственно использование его метода <see cref="Get{T}" /> считается небезопасным
		/// </remarks>
		IContainerLoader GetLoader();


		///<summary>
		///	Регистрирует расширение контейнера
		///</summary>
		///<param name="extension"> Расширение, для включения в контейнер </param>
		void RegisterExtension(IContainerExtension extension);

		/// <summary>
		/// 	Удаляет расширение контейнера
		/// </summary>
		/// <param name="extension"> </param>
		void UnRegisterExtension(IContainerExtension extension);

		///<summary>
		///	Получить список всех расширений контейнера
		///</summary>
		///<returns> Перечисление всех расширений контейнера </returns>
		IEnumerable<IContainerExtension> GetExtensions();

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

		/// <summary>
		/// 	Создает преднастроенный компонент для регистрации расширений с <see cref="Lifestyle.Extension" /> из заранее подготовленных объектов
		/// </summary>
		/// <param name="name"> Имя компонента </param>
		/// <param name="priority"> Приоритетность </param>
		/// <param name="implementation"> Готовый объект </param>
		/// <typeparam name="TService"> Тип сервиса </typeparam>
		/// <remarks>
		/// 	Используйте этот метод и затем вызывайте <see cref="Register" />.<br />
		/// 	метод удобен в случаях пользовательской настройки контейна из дополнительных конфигурационных файлов
		/// </remarks>
		/// <returns> </returns>
		IComponentDefinition NewExtension<TService>(TService implementation, string name = "", int priority = 10000)
			where TService : class;
	}
}