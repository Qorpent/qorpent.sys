namespace Qorpent.BSharp{
	/// <summary>
	/// </summary>
	public class BSharpElement : IBSharpElement{
		private string _name;
		private string _targetName;

		/// <summary>
		/// </summary>
		public string Name{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		///     Имя цели мержинга (рут)
		/// </summary>
		public string TargetName{
			get { return _targetName; }
			set { _targetName = value; }
		}

		/// <summary>
		///     Тип импорта
		/// </summary>
		public BSharpElementType Type { get; set; }

	    public bool LeveledCodes { get; set; }

	    /// <summary>
		/// </summary>
		public bool Implicit { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected bool Equals(IBSharpElement other){
			return string.Equals(_name, other.Name) && string.Equals(_targetName, other.TargetName) && Type == other.Type &&
			       Implicit == other.Implicit;
		}

		/// <summary>
		///     Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		///     A hash code for the current <see cref="T:System.Object" />.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode(){
			unchecked{
				int hashCode = (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (TargetName != null ? TargetName.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (int) Type;
				hashCode = (hashCode*397) ^ (Implicit ? 1 : 0);
				return hashCode;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj){
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((IBSharpElement) obj);
		}
	}
}