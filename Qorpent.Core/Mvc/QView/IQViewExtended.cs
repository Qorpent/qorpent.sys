using System;
using System.IO;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// Расширенные возможности видов
	/// </summary>
	public interface IQViewExtended :IQView{
		/// <summary>
		/// 	Renders local url to named resource with file resolution
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="prepared">Признак подготовленности ссылки </param>
		/// <exception cref="NullReferenceException"></exception>
		void RenderLink(string name, bool prepared = false);

		/// <summary>
		/// 	Retrieves resource string from special-formed resources
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="lang"> </param>
		/// <returns> </returns>
		string GetResource(string name, string lang = null);

		/// <summary>
		/// 	allows to catch content in temporal stream
		/// </summary>
		void EnterTemporaryOutput(TextWriter output = null);

		/// <summary>
		/// 	retrieves catched content
		/// </summary>
		/// <returns> </returns>
		string GetTemporaryOutput();

		/// <summary>
		/// восстанавливает стандартный оутпут
		/// </summary>
		/// <returns> </returns>
		void RestoreOutput();
	}
}