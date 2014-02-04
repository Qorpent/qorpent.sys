#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Core/IContainer.cs
#endregion

using System;
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
	///				<see cref="IComponentRegistry.EmptyComponent" />
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
	public interface 
		IContainer : ITypeResolver, IComponentRegistry {
		///<summary>
		///	Если <c>true</c> - то при отсутствии запрашиваемого сервиса будет возвращаться не Null, а <see
		///	 cref="ContainerException" />
		///</summary>
		bool ThrowErrorOnNotExistedComponent { get; set; }

		/// <summary>
		/// 	Вызыает очистку контейнера
		/// </summary>
		/// <remarks>
		/// 	<invariant>Очистка очищает внутрненнее состояние и кэши контейнера, регистрации компонент это не касается, компоненты остаются в прежней конфигурации</invariant>
		/// </remarks>
		void CleanUp();
		/// <summary>
		/// Время последнего изменения 
		/// </summary>
		DateTime TimeStamp { get; }
		/// <summary>
		/// Устанавливает признак обновления версии локального контейнера
		/// </summary>
	    void Upgrade();


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

		/// <summary>
		/// Регистрирует дочерний резольвер типов, может использоваться для объединения нескольких IOC
		/// </summary>
		/// <param name="resolver"></param>
		void RegisterSubResolver(ITypeResolver resolver);

		///<summary>
		///	Получить список всех расширений контейнера
		///</summary>
		///<returns> Перечисление всех расширений контейнера </returns>
		IEnumerable<IContainerExtension> GetExtensions();

		/// <summary>
		/// 	Создает преднастроенный компонент для регистрации расширений с <see cref="Lifestyle.Extension" /> из заранее подготовленных объектов
		/// </summary>
		/// <param name="name"> Имя компонента </param>
		/// <param name="priority"> Приоритетность </param>
		/// <param name="implementation"> Готовый объект </param>
		/// <typeparam name="TService"> Тип сервиса </typeparam>
		/// <remarks>
		/// 	Используйте этот метод и затем вызывайте <see cref="IComponentRegistry.Register" />.<br />
		/// 	метод удобен в случаях пользовательской настройки контейна из дополнительных конфигурационных файлов
		/// </remarks>
		/// <returns> </returns>
		IComponentDefinition NewExtension<TService>(TService implementation, string name = "", int priority = 10000)
			where TService : class;
	}
}