using System.IO;

namespace Qorpent.Security.SslCa {
	/// <summary>
	///		Конфиг центра выдачи сертификатов
	/// </summary>
	public class CaConfig : Scope {
		/// <summary>
		///		Базовая директория CA
		/// </summary>
		public string BaseDir {
			get { return Get<string>("basedir"); }
			set { Set("basedir", value); }
		}
		/// <summary>
		///		Директория с хранилищем данных
		/// </summary>
		public string DataStorageDir {
			get { return Get<string>("datastoragedir"); }
			set { Set("datastoragedir", value); }
		}
		/// <summary>
		///		Расширение файлов с пользовательскими сертификатами
		/// </summary>
		public string UserCertExtension {
			get { return Get("usercertextension", "crt"); }
			set { Set("usercertextension", value); }
		}
		/// <summary>
		///		Расширение файла с приватным ключём пользователя
		/// </summary>
		public string UserKeyExtension {
			get { return Get("userkeyextension", "key"); }
			set { Set("userkeyextension", value); }
		}
		/// <summary>
		///		Определяет признак валидности конфига
		/// </summary>
		/// <returns>Признак валидности конфимга</returns>
		public bool GetIsValid() {
			if (string.IsNullOrWhiteSpace(BaseDir)) {
				return false;
			}
			if (string.IsNullOrWhiteSpace(DataStorageDir)) {
				return false;
			}
			if (!Directory.Exists(BaseDir)) {
				return false;
			}
			if (!Directory.Exists(DataStorageDir)) {
				return false;
			}
			return true;
		}
	}
}