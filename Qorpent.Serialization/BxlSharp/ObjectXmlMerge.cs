namespace Qorpent.BxlSharp {
	/// <summary>
	/// </summary>
	public class ObjectXmlMerge {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected bool Equals(ObjectXmlMerge other) {
			return string.Equals(Name, other.Name) && string.Equals(TargetName, other.TargetName) && Type == other.Type;
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
				int hashCode = (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (TargetName != null ? TargetName.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (int) Type;
				return hashCode;
			}
		}

		/// <summary>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Имя цели мержинга (рут)
		/// </summary>
		public string TargetName { get; set; }

		/// <summary>
		///     Тип импорта
		/// </summary>
		public ObjectXmlMergeType Type { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ObjectXmlMerge) obj);
		}
	}
}