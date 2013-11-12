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
			if (SourceMinValue != 0) {
				KeepSourceMinHard = true;
			}
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

		/// <summary>
		/// Признак явного сохранения минимального значения
		/// </summary>
		public bool KeepSourceMinHard { get; set; }

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


	}
}