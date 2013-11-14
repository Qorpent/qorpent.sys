using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	/// Запрос на кирпичи (нормализованный на 0-1000)
	/// </summary>
	public class BrickRequest {
		/// <summary>
		/// Стандартное значение размера по умолчанию
		/// </summary>
		public const int DefaultSizeOfChart = 400;
		/// <summary>
		/// размер, до которого надо дожимать при нормализации
		/// </summary>
		public const int DefaultMaxSize = 10000;
		/// <summary>
		/// минимальный размер, на который выводим
		/// </summary>
		public const int DefaultMinSize = 100;
		/// <summary>
		/// Минимальная стандартная шапка в пикселах
		/// </summary>
		public const int DefaultPixelTopHat = 20;

		/// <summary>
		/// Поведение нижней границы шкалы при фитинге
		/// </summary>
		public MiniamlScaleBehavior MinimalScaleBehavior { get; set; }

		/// <summary>
		/// Пустой конструктор
		/// </summary>
		public BrickRequest() {
			
		}

		/// <summary>
		/// Конструктор запроса с шапкой по умолчанию или без шапки
		/// </summary>
		/// <param name="srcmaxvalue"></param>
		/// <param name="usehat"></param>
		public BrickRequest(decimal srcmaxvalue, bool usehat =false) {
			SourceMaxValue = srcmaxvalue;
			MinPixelTop = usehat ? DefaultPixelTopHat : 0;
		}

		/// <summary>
		/// Конструктор запроса со смещенным минимумом и возможной шапкой
		/// </summary>
		/// <param name="srcmaxvalue"></param>
		/// <param name="srcminvalue"></param>
		/// <param name="usehat"></param>
		public BrickRequest(decimal srcmaxvalue, decimal srcminvalue, bool usehat = false):this(srcmaxvalue,usehat) {
			SourceMinValue = srcminvalue;
		}

		private decimal _maxValue;

		/// <summary>
		/// Нормализованное значение максимума
		/// </summary>
		public decimal MaxValue {
			get {
				if (!_evaluated) {
					_maxValue = EvaluateMaxValue();
				}
				return _maxValue;
			}
			private set { _maxValue = value; }
		}

	

		private decimal EvaluateMaxValue() {
			Offset = SourceMinValue - 0;
			Scale = 1;
			var correctedMax = SourceMaxValue - Offset;
			if (correctedMax == 0) {
				return correctedMax;
			}
			if (correctedMax >= DefaultMaxSize) {
				while (correctedMax >= DefaultMaxSize) {
					correctedMax = correctedMax/10;
					Scale = Scale/10;
				}
			}
			if (correctedMax < DefaultMinSize) {
				while (correctedMax <= DefaultMaxSize) {
					correctedMax = correctedMax*10;
					Scale = Scale*10;
				}
			}
			return correctedMax;
		}

		private bool _evaluated;
		private decimal _sourceMaxValue;
		private decimal _sourceMinValue;
		private int _size;

		/// <summary>
		/// Размер канвы
		/// </summary>
		public int Size {
			get {
				if (_size <= 0) {
					_size = DefaultSizeOfChart;
				}
				return _size;
			}
			set { _size = value; }
		}

		/// <summary>
		/// Максимальное кол-во разделителей
		/// </summary>
		public int MaxDelimit { get; set; }
		/// <summary>
		/// Минимальное кол-во разделителей
		/// </summary>
		public int MinDelimit { get; set; }
		/// <summary>
		/// Минимальная "шапка" в пикселах
		/// </summary>
		public int MinPixelTop { get; set; }

		/// <summary>
		/// Исходное максимальное значение
		/// </summary>
		public decimal SourceMaxValue {
			get { return _sourceMaxValue; }
			set {
				_sourceMaxValue = value;
				_evaluated = false;
			}
		}

		/// <summary>
		/// Исходное минимальное значение
		/// </summary>
		public decimal SourceMinValue
		{
			get { return _sourceMinValue; }
			set {
				_sourceMinValue = value;
				_evaluated = false;
			}
		}

		/// <summary>
		/// Число на которое пришлось примениь чтобы получить нужную разрядность
		/// </summary>
		public decimal Scale { get; private set; }
		/// <summary>
		/// Число, которое пришлось применить, чтобы получить смещение на 0
		/// </summary>
		public decimal Offset { get; private set; }
		class SetupInfo {

			public string Min;
			public string Max;
			public string Top;
		}
		/// <summary>
		/// Дополнительная настройка запроса при помощи строки
		/// </summary>
		/// <param name="setupInfo"></param>
		public void Setup(string setupInfo) {
			var parameters = setupInfo.SmartSplit(false, true, ':', ';', ',', ' ','/');
			var min = "";
			var max = "";
			var top = "";
			if (parameters.Count > 0) {
				min = parameters[0];
			}
			if (parameters.Count > 1) {
				max = parameters[1];
			}
			if (parameters.Count > 2) {
				top = parameters[2];
			}
			if (string.IsNullOrWhiteSpace(min)) {
				min = "0";
			}
			if (string.IsNullOrWhiteSpace(max)) {
				max = "auto";
			}
			if (string.IsNullOrWhiteSpace(top)) {
				top = "20";
			}
			var info = new SetupInfo {Min = min, Max = max, Top = top};
			Setup(info);
		}

		private void Setup(SetupInfo setupInfo) {
			if (setupInfo.Max != "auto") {
				var assertedMax = setupInfo.Max.ToDecimal();
				while (assertedMax < (SourceMaxValue *2 / 1000)) {
					assertedMax *= 1000;
				}
				if (assertedMax > SourceMaxValue) {
					SourceMaxValue = assertedMax;
				}
			}
			MinPixelTop = setupInfo.Top.ToInt();
			if (setupInfo.Min == "0") {
				MinimalScaleBehavior = MiniamlScaleBehavior.KeepZero;
			}
			else if (setupInfo.Min == "auto") {
				MinimalScaleBehavior = MiniamlScaleBehavior.FitMin;
			}
			else {
				MinimalScaleBehavior = MiniamlScaleBehavior.FitMin;
				var assertedMin = setupInfo.Min.ToDecimal();
				while (assertedMin < SourceMinValue / 1000)
				{
					assertedMin *= 1000;
				}
				if (assertedMin < SourceMinValue) {
					SourceMinValue = assertedMin;
					MinimalScaleBehavior = MiniamlScaleBehavior.MatchMin;
				}
			}
		}
	}
}