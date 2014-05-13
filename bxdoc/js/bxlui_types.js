/*QPT:::AUTOGENERATED*/// Type definitions for bxlui

define ([], function() {
	var result = {};

	//Тип сообщения лога
	var LogLevel = result.LogLevel = {
		// Отсутвующее значение = 0
		Undefined : 'Undefined',

		// Все = 0
		All : 'All',

		// Отладка = 1
		Debug : 'Debug',

		// Трассировка = 2
		Trace : 'Trace',

		// Информационные сообщения = 4
		Info : 'Info',

		// Предупреждение = 8
		Warning : 'Warning',

		// Ошибка = 16
		Error : 'Error',

		// Фатальная ошибка = 32
		Fatal : 'Fatal',

		// Никакой = 64
		None : 'None',

		// Пользоватеьский тип = 256
		Custom : 'Custom',

		// Зарезервированное значение = 512
		Reserved : 'Reserved',
		__Values : {
			Undefined : 0,
			All : 0,
			Debug : 1,
			Trace : 2,
			Info : 4,
			Warning : 8,
			Error : 16,
			Fatal : 32,
			None : 64,
			Custom : 256,
			Reserved : 512,

			__TERMINAL : null
		}
	};

	// Структура
	var BxOptions= result.BxOptions = function(args){
		args=args||{}
		this.__getClassInfo=function(){return {name:"Ugmk.Assoi.Team.BxOptions"}};
		// 
		this.AutoCompile = args.hasOwnProperty("AutoCompile") ? args.AutoCompile : ( args.hasOwnProperty("autocompile") ? args.autocompile : false) ;
	};

	// Структура
	var ToXmlQuery= result.ToXmlQuery = function(args){
		args=args||{}
		this.__getClassInfo=function(){return {name:"Ugmk.Assoi.Team.ToXmlQuery"}};
		// 
		this.Lang = args.hasOwnProperty("Lang") ? args.Lang : ( args.hasOwnProperty("lang") ? args.lang : ('bxl')) ;
		// 
		this.Format = args.hasOwnProperty("Format") ? args.Format : ( args.hasOwnProperty("format") ? args.format : ('wiki')) ;
		// 
		this.Script = args.hasOwnProperty("Script") ? args.Script : ( args.hasOwnProperty("script") ? args.script : "") ;
	};

	// Сообщение журнала
	var LogMessage= result.LogMessage = function(args){
		args=args||{}
		this.__getClassInfo=function(){return {name:"Qorpent.Scaffolding.LogMessage"}};
		// Начальный таймстэмп
		this.Timestamp = args.hasOwnProperty("Timestamp") ? args.Timestamp : ( args.hasOwnProperty("timestamp") ? args.timestamp : (0)) ;
		// Идентификатор сообщения
		this.Id = args.hasOwnProperty("Id") ? args.Id : ( args.hasOwnProperty("id") ? args.id : (0)) ;
		// Код сообщения
		this.Code = args.hasOwnProperty("Code") ? args.Code : ( args.hasOwnProperty("code") ? args.code : "") ;
		// Уровень сообщения
		this.Level = args.hasOwnProperty("Level") ? args.Level : ( args.hasOwnProperty("level") ? args.level : LogLevel.Info) ;
		// Время  записи в читаемом формате
		this.Time = args.hasOwnProperty("Time") ? args.Time : ( args.hasOwnProperty("time") ? args.time : (new Date())) ;
		// Сообщение
		this.Message = args.hasOwnProperty("Message") ? args.Message : ( args.hasOwnProperty("message") ? args.message : "без комментариев") ;
		// Дополнительная информация
		this.Data = args.hasOwnProperty("Data") ? args.Data : ( args.hasOwnProperty("data") ? args.data : ({})) ;
		// Информация об акцептации и снятии сообщения
		this.Accepted = args.hasOwnProperty("Accepted") ? args.Accepted : ( args.hasOwnProperty("accepted") ? args.accepted : false) ;
		// Признак необходимости акцептации
		this.RequireAccept = args.hasOwnProperty("RequireAccept") ? args.RequireAccept : ( args.hasOwnProperty("requireaccept") ? args.requireaccept : false) ;
	};

	// Запрос журнала сообщений
	var LogQuery= result.LogQuery = function(args){
		args=args||{}
		this.__getClassInfo=function(){return {name:"Qorpent.Scaffolding.LogQuery"}};
		// Начальный таймстэмп
		this.Timestamp = args.hasOwnProperty("Timestamp") ? args.Timestamp : ( args.hasOwnProperty("timestamp") ? args.timestamp : (0)) ;
		// Уровень сообщения
		this.Level = args.hasOwnProperty("Level") ? args.Level : ( args.hasOwnProperty("level") ? args.level : LogLevel.All) ;
		// Только не обработанные
		this.NotAcceptedOnly = args.hasOwnProperty("NotAcceptedOnly") ? args.NotAcceptedOnly : ( args.hasOwnProperty("notacceptedonly") ? args.notacceptedonly : false) ;
		// Только обработанные
		this.AcceptedOnly = args.hasOwnProperty("AcceptedOnly") ? args.AcceptedOnly : ( args.hasOwnProperty("acceptedonly") ? args.acceptedonly : false) ;
	};

	// Запрос на фиксацию задачи
	var AcceptCall= result.AcceptCall = function(args){
		args=args||{}
		this.__getClassInfo=function(){return {name:"Qorpent.Scaffolding.AcceptCall"}};
		// Идентификатор задачи
		this.Id = args.hasOwnProperty("Id") ? args.Id : ( args.hasOwnProperty("id") ? args.id : (0)) ;
	};

	// Строка поиска
	var SearchString= result.SearchString = function(args){
		args=args||{}
		this.__getClassInfo=function(){return {name:"Qorpent.Scaffolding.SearchString"}};
		// Паттерн поиска
		this.Pattern = args.hasOwnProperty("Pattern") ? args.Pattern : ( args.hasOwnProperty("pattern") ? args.pattern : "") ;
	};

	// Единица чего-то
	var Item= result.Item = function(args){
		args=args||{}
		this.__getClassInfo=function(){return {name:"Qorpent.Scaffolding.Item"}};
		// Идентификатор элемента
		this.Id = args.hasOwnProperty("Id") ? args.Id : ( args.hasOwnProperty("id") ? args.id : (0)) ;
		// Код элемента
		this.Code = args.hasOwnProperty("Code") ? args.Code : ( args.hasOwnProperty("code") ? args.code : "") ;
		// Полное имя
		this.Name = args.hasOwnProperty("Name") ? args.Name : ( args.hasOwnProperty("name") ? args.name : "") ;
		// Краткое имя
		this.ShortName = args.hasOwnProperty("ShortName") ? args.ShortName : ( args.hasOwnProperty("shortname") ? args.shortname : "") ;
		// Комментарий
		this.Comment = args.hasOwnProperty("Comment") ? args.Comment : ( args.hasOwnProperty("comment") ? args.comment : "") ;
	};
	return result;
});
