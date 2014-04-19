namespace Qorpent.BSharp{
	/// <summary>
	/// 
	/// </summary>
	public class BSharpErrorDigest{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected bool Equals(BSharpErrorDigest other){
			return ErrorLevel == other.ErrorLevel && Phase == other.Phase && string.Equals(Message, other.Message) && Type == other.Type && string.Equals(ClassName, other.ClassName) && string.Equals(FileName, other.FileName) && Line == other.Line && Column == other.Column;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode(){
			unchecked{
				int hashCode = (int) ErrorLevel;
				hashCode = (hashCode*397) ^ (int) Phase;
				hashCode = (hashCode*397) ^ (Message != null ? Message.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (int) Type;
				hashCode = (hashCode*397) ^ (ClassName != null ? ClassName.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (FileName != null ? FileName.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ Line;
				hashCode = (hashCode*397) ^ Column;
				return hashCode;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((BSharpErrorDigest) obj);
		}
		/// <summary>
		/// 
		/// </summary>
		public ErrorLevel ErrorLevel { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public BSharpCompilePhase Phase { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public BSharpErrorType Type { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ClassName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int Line { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int Column { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			return string.Format("{0}:{1}:{2} - {3}", FileName, Line, Column, Message);
		}
	}
}