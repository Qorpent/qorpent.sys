using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Qorpent.IO.DirtyVersion
{
	/// <summary>
	/// Содержит константы DirtyVersion
	/// </summary>
	public static class Const
	{
		

		/// <summary>
		/// Размерность хэша Md5 в байтах
		/// </summary>
		internal const int Md5HashSizeInBytes = 128/8;

		/// <summary>
		/// Размерность хэша
		/// </summary>
		public const int MaxHashSize = Md5HashSizeInBytes * 2;
		/// <summary>
		/// Размерность хэша
		/// </summary>
		public const int MinHashSize = 7;
		/// <summary>
		/// Специальное имя для отщепленного имени
		/// </summary>
		public const string DETACHEDHEAD = "DETACHED";
		/// <summary>
		/// Атрибут имени коммитера
		/// </summary>
		public const string COMMITERATTRIBUTE = "a";
		/// <summary>
		/// Элемент имени мапинга
		/// </summary>
		public const string MAPPINGNAMEATTRIBUTE = "n";
	/// <summary>
	/// Атрибут хэша имени мапинга
	/// </summary>
		public const string MAPPINGHASHATTRIBUTE = "nh";
		/// <summary>
		/// Атрибут текщего хида
		/// </summary>
		public const string HEADATTRIBUTE = "h";

		/// <summary>
		/// Атрибут времени коммита
		/// </summary>
		public const string TIMEATTRIBUTE = "t";
		/// <summary>
		/// Атрибут типа источника
		/// </summary>
		public const string SRCTYPEATTRIBUTE = "st";
		/// <summary>
		/// Атрибут единичного источника
		/// </summary>
		public const string SRCATTRIBUTE = "s";
		/// <summary>
		/// Элемент соавторства
		/// </summary>
		public const string COELEMENT = "co";
		/// <summary>
		/// Элемент источников для мержа
		/// </summary>
		public const string SOURCESELEMENT = "ss";
		/// <summary>
		/// Элемент коммитов
		/// </summary>
		public const string COMMITSELEMENT = "cs";
		/// <summary>
		/// Элемент альясов
		/// </summary>
		public const string ALIASESELEMENT = "as";

		/// <summary>
		/// Атрибут состояния относительно HEAD
		/// </summary>
		public const string HEADSTATEATTRIBUTE = "hs";
		/// <summary>
		/// Элемент мапинга
		/// </summary>
		public const string MAPPINGELEMENT = "map";
		/// <summary>
		/// Расширение файла блокировки
		/// </summary>
		public const string LOCKEXTENSION = ".l";
	}
}
