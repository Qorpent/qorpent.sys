using System.Security.Permissions;
using System.Xml.Linq;
using Qorpent.IO.Resources;

namespace Qorpent.BSharp {
	/// <summary>
	/// Интерфейс определения элемента класса
	/// </summary>
	public interface IBSharpElement {
		/// <summary>
		/// </summary>
		string Name { get; set; }

		/// <summary>
		///     Имя цели мержинга (рут)
		/// </summary>
		string TargetName { get; set; }

		/// <summary>
		///     Тип импорта
		/// </summary>
		BSharpElementType Type { get; set; }
        
        bool LeveledCodes { get; set; }
		/// <summary>
		/// implicit element type mark
		/// </summary>
		/// <remarks>Added to v1.2 due to Q-188</remarks>
		bool Implicit { get; set; }

	    string Alias { get; set; }
	    string TargetAttr { get; set; }
	    string TargetValue { get; set; }

        bool Copy { get; set; }

        XElement Definition { get; set; }
	}
}