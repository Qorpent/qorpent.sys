using Qorpent.Serialization;

namespace Qorpent.Utils.Git{
	/// <summary>
	/// Запись об измененном файле
	/// </summary>
	[Serialize]
	public class FileRecord{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
	    protected bool Equals(FileRecord other) {
			return FirstState == other.FirstState && SecondState == other.SecondState && string.Equals(FileName, other.FileName) && string.Equals(Level, other.Level) && string.Equals(NewFileName, other.NewFileName);
	    }

	    /// <summary>
	    /// Serves as a hash function for a particular type. 
	    /// </summary>
	    /// <returns>
	    /// A hash code for the current <see cref="T:System.Object"/>.
	    /// </returns>
	    /// <filterpriority>2</filterpriority>
	    public override int GetHashCode() {
	        unchecked {
	            int hashCode = (int) FirstState;
	            hashCode = (hashCode*397) ^ (int) SecondState;
	            hashCode = (hashCode*397) ^ (FileName != null ? FileName.GetHashCode() : 0);
	            hashCode = (hashCode*397) ^ (NewFileName != null ? NewFileName.GetHashCode() : 0);
	            return hashCode;
	        }
	    }

	    /// <summary>
	    /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
	    /// </summary>
	    /// <returns>
	    /// true if the specified object  is equal to the current object; otherwise, false.
	    /// </returns>
	    /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
	    public override bool Equals(object obj) {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != this.GetType()) return false;
	        return Equals((FileRecord) obj);
	    }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="rawrecord"></param>
		public FileRecord(string rawrecord){
			var rawState = rawrecord.Substring(0,2);
			string firstfile = rawrecord.Substring(2).Trim();
			string secondfile = "";

			var filesplit = firstfile.IndexOf("->");
			if (filesplit != -1){
				secondfile = firstfile.Substring(filesplit + 2 ).Trim();
				firstfile = firstfile.Substring(0, filesplit).Trim();
			}
			
			FileName = GitUtils.ConvertToValidFileName(firstfile);
			NewFileName = GitUtils.ConvertToValidFileName(secondfile);
			FirstState = GitUtils.ConvertToValidState(rawState[0]);
			SecondState = GitUtils.ConvertToValidState(rawState[1]);
		}
		/// <summary>
		/// Статус
		/// </summary>
		public FileState FirstState { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public FileState SecondState { get; set; }

		/// <summary>
		/// Определяет конфликтность состояния
		/// </summary>
		public bool IsConflict{
			get{
				if (0 != (FirstState & (FileState.Deleted | FileState.Added | FileState.Updated)) && FirstState == SecondState)
					return true;
				if (FirstState == FileState.Added && SecondState == FileState.Updated) return true;
				if (FirstState == FileState.Updated && SecondState == FileState.Deleted) return true;
				if (FirstState == FileState.Updated && SecondState == FileState.Added) return true;
				if (FirstState == FileState.Deleted && SecondState == FileState.Updated) return true;
				return false;
			}
		}
		/// <summary>
		/// Уровень файла (логическое свойство для клиентских систем)
		/// </summary>
		public string Level { get; set; }

		/// <summary>
		/// Локальное имя файла
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		/// Второе имя файла
		/// </summary>
		public string NewFileName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			var result = GitUtils.ConvertToChar(FirstState).ToString() + GitUtils.ConvertToChar(SecondState).ToString() + ' ' +
			             '"' + FileName + '"';
			if (!string.IsNullOrWhiteSpace(NewFileName)){
				result += " -> \"" + NewFileName + "\"";
			}
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public static implicit operator string(FileRecord record){
			return record.ToString();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public static implicit operator FileRecord(string record){
			return new FileRecord(record);
		}
	}
}