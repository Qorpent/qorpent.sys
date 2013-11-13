namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	/// 
	/// </summary>
	public enum MiniamlScaleBehavior {
		/// <summary>
		/// Привязать к 0
		/// </summary>
		KeepZero ,
		/// <summary>
		/// Привязать к указанному минимальному значению
		/// </summary>
		MatchMin,
		/// <summary>
		/// Динамически подобрать под значение 
		/// </summary>
		FitMin ,
	}
}