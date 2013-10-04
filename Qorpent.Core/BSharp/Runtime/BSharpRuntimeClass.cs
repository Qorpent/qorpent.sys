using System;
using System.Linq;
using System.Xml.Linq;
using Qorpent.IoC;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	///     Базовая реализация BSharp класса времени выполнения
	/// </summary>
	public class BSharpRuntimeClass : IBSharpRuntimeClass {
		private string _name;
		private string _ns;
		private string _fullname;
		private RuntimeClassDescriptor _runtimeDescriptor;
		private IContainer _container;
		private string _runtimeCode;
	    private string _prototypeCode;

	    /// <summary>
		/// Создает класс в увязке с контейнером
		/// </summary>
		/// <param name="container"></param>
		public BSharpRuntimeClass(IContainer container = null) {
			_container = container;
		}
		/// <summary>
		///     Имя класса
		/// </summary>
		public string Name {
			get { return _name ??(_name=GetName()); }
			set { _name = value; }
		}


		/// <summary>
		/// Создает 
		/// </summary>
		public object Create() {
			if (null == RuntimeDescriptor) throw new BSharpRuntimeException("cannot create - no descriptor configured");
			if (null == RuntimeDescriptor.GetActualType()) throw new BSharpRuntimeException("cannot create - no type resolved");
			var result = RuntimeDescriptor.Create();
			var bound = result as IBSharpRuntimeBound;
			if (null != bound) {
				bound.Initialize(this);
			}
			return result;
		}

		private string GetName() {
			var e = GetClassElement();
			if (null == e) return null;
			return e.Attribute(BSharpRuntimeDefaults.BSHARP_CLASS_NAME_ATTRIBUTE).Value;
		}
		/// <summary>
		/// Возвращает нативное определение класса
		/// </summary>
		/// <returns></returns>
		public XElement GetClassElement() {
			if (null == Definition) return null;
			var e = Definition;
			//хидер пропускаем
			if (e.Name.LocalName == BSharpRuntimeDefaults.BSHARP_CLASS_HEADER) {
				e = e.Elements().First();
			}
			return e;
		}

		/// <summary>
		///     Пространство имен
		/// </summary>
		public string Namespace {
			get { return _ns ?? (_ns=GetNameSpace()); }
			set { _ns = value; }
		}

		private string GetNameSpace() {
			var e = GetClassElement();
			if (null == e) return null;
			var fullname = e.Attribute(BSharpRuntimeDefaults.BSHARP_CLASS_FULLNAME_ATTRIBUTE).Value;
			var lastdot = fullname.LastIndexOf('.');
			if (-1 == lastdot) return "";
			return fullname.Substring(0, lastdot);
		}

		/// <summary>
		///     Полное имя
		/// </summary>
		public string Fullname {
			get { return _fullname ??(_fullname = string.IsNullOrWhiteSpace(Namespace)?Name:( Namespace+"."+Name) ); }
			set { _fullname = value; }
		}

		

		/// <summary>
		///     Определение
		/// </summary>
		public XElement Definition { get; set; }

		/// <summary>
		/// Дескриптор рантайм-класса
		/// </summary>
		public RuntimeClassDescriptor RuntimeDescriptor {
			get { return _runtimeDescriptor ?? (_runtimeDescriptor = GetRuntimeClassDescriptor() ); }
			private set { _runtimeDescriptor = value; }
		}
        /// <summary>
        /// Признак загрузки
        /// </summary>
	    public bool Loaded   { get; set; }

	    /// <summary>
		/// Код рантайма
		/// </summary>
		public string RuntimeCode {
			get { return _runtimeCode ??(_runtimeCode = GetRuntimeCode()); }
			set { _runtimeCode = value; }
		}

        /// <summary>
        /// Код прототипа
        /// </summary>
        public string PrototypeCode
        {
            get { return _prototypeCode ?? (_prototypeCode = GetPrototypeCode()); }
            set { _prototypeCode = value; }
        }

	    private string GetPrototypeCode() {
            var e = GetClassElement();
            if (null == e) return null;
            var ra = e.Attribute(BSharpRuntimeDefaults.BSHARP_PROTOTYPE_ATTRIBUTE);
            if (null == ra) return null;
            return ra.Value;
	    }

	    private string GetRuntimeCode() {
			var e = GetClassElement();
			if (null == e) return null;
			var ra = e.Attribute(BSharpRuntimeDefaults.BSHARP_RUNTIME_ATTRIBUTE);
			if (null == ra) return null;
			return ra.Value;
		}

		private RuntimeClassDescriptor GetRuntimeClassDescriptor() {
			return new RuntimeClassDescriptor(RuntimeCode,_container);
		
		}
	}
}