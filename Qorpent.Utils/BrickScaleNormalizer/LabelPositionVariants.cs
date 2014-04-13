using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	///		Описание порядка расположения элементов данных внутри колонки для <see cref="DataColonLabelHelper"/>
	/// </summary>
	public enum ColonDataItemOrder {
		/// <summary>
		///		В реальном виде (большее значение выше)
		/// </summary>
		Real,
		/// <summary>
		///		Инверсия <see cref="Real"/>: меньшее значение выше
		/// </summary>
		Inverted,
		/// <summary>
		///		Как передано в хэлпер
		/// </summary>
		AsSupplied,
		/// <summary>
		///		По умолчанию <see cref="Real"/>
		/// </summary>
		Default = Real
	}
	/// <summary>
	///		Хэлпер, позволяющий корректно выбирать расположение лычки внутри колонки данных
	/// </summary>
	public class DataColonLabelHelper : IEnumerable<DataItem> {
		/// <summary>
		///		Признак того, что все элементы данных нормализованы
		/// </summary>
		private bool _isNormalized;
		/// <summary>
		///		Список элементов данных
		/// </summary>
		private readonly List<DataItem> _dataItems = new List<DataItem>();
		/// <summary>
		///		Высота колонки
		/// </summary>
		public decimal Height { get; set; }
		/// <summary>
		///		Максимальное значение по шкале
		/// </summary>
		public decimal ScaleMax { get; set; }
		/// <summary>
		///		Минимальное значение по шкале
		/// </summary>
		public decimal ScaleMin { get; set; }
		/// <summary>
		///		Высота лычки
		/// </summary>
		public decimal LabelHeight { get; set; }
		/// <summary>
		///		Признак того, что конфигурация корректная
		/// </summary>
		public bool IsCorrectConfig {
			get { return this.Min(_ => _.Value) >= ScaleMin && this.Max(_ => _.Value) <= ScaleMax; }
		}
		/// <summary>
		///		Порядок расположения элементов данных
		/// </summary>
		public ColonDataItemOrder Order { get; set; }
		/// <summary>
		///		Признак того, что произошло перекрытие шкалы — лычка над или под видимой шкалой
		/// </summary>
		public bool IsScaleOverlap {
			get { return this.Any(_ => _.NormalizedLabelMax > ScaleMax || _.NormalizedLabelMin < ScaleMin); }
		}
		/// <summary>
		///		Количество коллизий — наезжаний лычек друг на друга
		/// </summary>
		public double CollisionCount {
			get {
				EnsureNormalized();
				double collisionCount = 0;
				if (IsAmbiguous) {
					return double.PositiveInfinity;
				}
				var ordered = GetOrderedItems(true).ToList();
				foreach (var dataItem in ordered) {
					var currentIndex = ordered.IndexOf(dataItem);
					var aboveItems = ordered.Skip(currentIndex + 1).ToArray();
					if (aboveItems.Length == 0) {
						return collisionCount;
					}
					collisionCount += aboveItems.Count(_ => _.NormalizedLabelBottom < dataItem.NormalizedLabelTop);
				}
				return collisionCount;
			}
		}
		/// <summary>
		///		Признак того, что есть двусмысленность положения лычки: меньшее значение имеет лычку выше большего
		/// </summary>
		public bool IsAmbiguous {
			get {
				EnsureNormalized();
				var ordered = GetOrderedItems(true).ToList();
				foreach (var dataItem in ordered) {
					var currentIndex = ordered.IndexOf(dataItem);
					var aboveItems = ordered.Skip(currentIndex + 1).ToArray();
					var normalizedLabelPosition = dataItem.NormalizedLabelPosition;
					if (aboveItems.Length == 0) {
						return false;
					}
					if (aboveItems.Any(_ => _.NormalizedLabelPosition < normalizedLabelPosition)) {
						return true;
					}
				}
				return false;
			}
		}
		/// <summary>
		///		Добавление элемента данных в хэлпер
		/// </summary>
		/// <param name="dataItem">Элемент данных</param>
		public void Add(DataItem dataItem) {
			_isNormalized = false;
			_dataItems.Add(dataItem);
		}
		/// <summary>
		///		Очистка датасета от данных
		/// </summary>
		public void Clear() {
			_dataItems.Clear();
		}
		/// <summary>
		///		Убеждается в идеальных лычках
		/// </summary>
		/// <returns>Замыкание на текущий <see cref="DataColonLabelHelper"/></returns>
		public DataColonLabelHelper EnsureBestLabels() {
			if (!IsCorrectConfig) {
				throw new Exception("Некорректная конфигурация");
			}
			EnsureNormalized();
			var variants = GetPossibleVariants();
			var tournament = variants.Select(_ => new KeyValuePair<double, LabelPosition[]>(Apply(_).GetTemperature(), _)).ToArray();
			if (!tournament.Any()) {
				throw new Exception("Ошибка при попытке выявить наилучший варант: нет данных");
			}
			var bestVariant = tournament.OrderBy(_ => _.Key).FirstOrDefault();
			return Apply(bestVariant.Value);
		}
		/// <summary>
		///		Возвращает перечисление <see cref="DataItem"/> в порядке, указанном в <see cref="Order"/> начиная с нижнего элемента
		/// </summary>
		/// <param name="onlyVisible">Только видимые</param>
		/// <returns>Перечисление <see cref="DataItem"/></returns>
		public IEnumerable<DataItem> GetOrderedItems(bool onlyVisible = false) {
			IEnumerable<DataItem> enumerable;
			if (Order == ColonDataItemOrder.AsSupplied) {
				enumerable = this;
			} else if (Order == ColonDataItemOrder.Inverted) {
				enumerable = this.OrderByDescending(_ => _.Value);
			} else if (Order == ColonDataItemOrder.Real) {
				enumerable = this.OrderBy(_ => _.Value);
			} else {
				throw new NotSupportedException("Неизвестная комбинация порядка расположения элементов данных");
			}
			if (onlyVisible) {
				return enumerable.Where(_ => !_.Hide);
			}
			return enumerable;
		}
		/// <summary>
		///		Возвращает текущую температуру колонки
		/// </summary>
		/// <returns>Температура колонки</returns>
		public double GetTemperature() {
			EnsureNormalized();

			if (IsAmbiguous) {
				return double.PositiveInfinity;
			}
			double temp = 0;
			var visible = GetOrderedItems(true).ToList();
			var alloweableOverlap = Convert.ToDouble(LabelHeight/10); // допустимый наезд на лычку в пикселях
			foreach (var dataItem in visible) {
				var currentIndex = visible.IndexOf(dataItem);
				var aboveItems = visible.Skip(currentIndex + 1);
				var overlaps = aboveItems.Where(_ => _.NormalizedLabelBottom < dataItem.NormalizedLabelTop).ToArray();
				var localTemp = overlaps.Sum(_ => {
					var overlap = dataItem.NormalizedLabelTop - _.NormalizedLabelBottom;
					if (overlap <= alloweableOverlap) {
						return 0;
					}
					return overlap*100/Convert.ToDouble(Height);
				});
				temp += localTemp;
			}
			return temp;
		}
		/// <summary>
		///		Применение массива позиций лычек к данному набору элементов данных
		/// </summary>
		/// <param name="labelPositions">Массив позиций, начинающихся с нижнего элемента</param>
		/// <returns>Замыкание на текущий экземпляр <see cref="DataColonLabelHelper"/></returns>
		public DataColonLabelHelper Apply(LabelPosition[] labelPositions) {
			var ordered = GetOrderedItems().ToArray();
			if (ordered.Length != labelPositions.Length) {
				throw new Exception("Длина массива позиций не соответствует длине массива элементов данных");
			}
			for (var i = 0; i < ordered.Length; i++) {
				ordered[i].LabelPosition = labelPositions[i];
			}
			return this;
		}
		/// <summary>
		///		Возвращает все возможные варианты разброса лычек
		/// </summary>
		/// <returns>Перечисление массивов лычек</returns>
		public IEnumerable<LabelPosition[]> GetPossibleVariants() {
			return LabelPositionRotator.GetVariants(this.ToArray());
		}
		/// <summary>
		///		Получение перечисления по <see cref="DataItem"/>
		/// </summary>
		/// <returns><see cref="IEnumerable"/> по <see cref="DataItem"/></returns>
		public IEnumerator<DataItem> GetEnumerator() {
			return _dataItems.GetEnumerator();
		}
		/// <summary>
		///		Получение перечисления по <see cref="DataItem"/>
		/// </summary>
		/// <returns><see cref="IEnumerable"/> по <see cref="DataItem"/></returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		/// <summary>
		///		Убеждается в корректном расположении элементов данных
		/// </summary>
		protected void EnsureNormalized() {
			if (_isNormalized) {
				return;
			}
			foreach (var dataItem in _dataItems) {
				dataItem.NormalizedValue = BrickDataSetHelper.GetNormalizedValue(ScaleMin, ScaleMax, Height, dataItem.Value);
				dataItem.LabelHeight = LabelHeight.ToInt();
			}
			_isNormalized = true;
		}
	}
	/// <summary>
	///		Ротатор всех возможых комбинаций лычек
	/// </summary>
	internal static class LabelPositionRotator {
		public static IEnumerable<LabelPosition[]> GetVariants(DataItem[] dataItems) {
			var values = new List<string>();
            var v = dataItems.Length;
            for (var ch = 'a'; ch <= 'c'; ch++) values.Add(ch.ToString(CultureInfo.InvariantCulture));

            for (var i = 1; i < v; i++) {
                var newValues = new List<string>();
                EnumerableExtensions.DoForEach(values, _ => { for (var ch = 'a'; ch <= 'c'; ch++) newValues.Add(_ + ch); });
                values = newValues;
            }

            var indexes = GetHiddenIndexes(dataItems).ToList();

            foreach (var el in values) {
                var c = new LabelPosition[v];
                var k = 0;
                foreach (var ch in el.ToCharArray()) {
                    switch (ch) {
                        case 'a': c[k] = LabelPosition.Auto; break;
                        case 'b': c[k] = LabelPosition.Above; break;
                        case 'c': c[k] = LabelPosition.Below; break;
                    }
                    k++;
                }
                indexes.ForEach(_ => c[_] = LabelPosition.Hidden);
                yield return c;
            }
		}
		private static IEnumerable<int> GetHiddenIndexes(DataItem[] dataItems) {
			var index = 0;
			foreach (var dataItem in dataItems) {
				if (dataItem.Hide) {
					yield return index;
				}
				index++;
			}
		}
	}
}