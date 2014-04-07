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
		///		Порядок расположения элементов данных
		/// </summary>
		public ColonDataItemOrder Order { get; set; }
		/// <summary>
		///		Добавление элемента данных в хэлпер
		/// </summary>
		/// <param name="dataItem">Элемент данных</param>
		public void Add(DataItem dataItem) {
			_dataItems.Add(dataItem);
		}
		/// <summary>
		///		Убеждается в идеальных лычках
		/// </summary>
		/// <returns>Замыкание на текущий <see cref="DataColonLabelHelper"/></returns>
		public DataColonLabelHelper EnsureBestLabels() {
			EnsureCorrectNormalizedValue();
			throw new NotImplementedException();
		}
		/// <summary>
		///		Возвращает перечисление <see cref="DataItem"/> в порядке, указанном в <see cref="Order"/> начиная с нижнего элемента
		/// </summary>
		/// <returns>Перечисление <see cref="DataItem"/></returns>
		public IEnumerable<DataItem> GetOrderedItems() {
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
			return enumerable;
		}
		/// <summary>
		///		Возвращает текущую температуру колонки
		/// </summary>
		/// <returns>Температура колонки</returns>
		public decimal GetCurrentTemperature() {
			throw new NotImplementedException();
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
		protected void EnsureCorrectNormalizedValue() {
			foreach (var dataItem in _dataItems) {
				dataItem.NormalizedValue = BrickDataSetHelper.GetNormalizedValue(ScaleMin, ScaleMax, Height, dataItem.Value);
			}
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
                values.DoForEach(_ => { for (var ch = 'a'; ch <= 'c'; ch++) newValues.Add(_ + ch); });
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






    /// <summary>
    ///     Представление набора вариантов для <see cref="DataItemColon"/>
    /// </summary>
    internal class LabelPositionVariants : IEnumerable<Tuple<decimal,LabelPosition[]>> {
        /// <summary>
        ///     Количество элементов в <see cref="DataItemColon"/>
        /// </summary>
        private readonly DataItemColon _colon;
        /// <summary>
        ///     Представление набора вариантов для <see cref="DataItemColon"/>
        /// </summary>
        /// <param name="colon">Экземпляр <see cref="DataItemColon"/></param>
        public LabelPositionVariants(DataItemColon colon) {
            _colon = colon;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<decimal,LabelPosition[]>> Get() {
            var values = new List<string>();
            var v = _colon.Count();
            for (var ch = 'a'; ch <= 'c'; ch++) values.Add(ch.ToString(CultureInfo.InvariantCulture));

            for (var i = 1; i < v; i++) {
                var newValues = new List<string>();
                values.DoForEach(_ => { for (var ch = 'a'; ch <= 'c'; ch++) newValues.Add(_ + ch); });
                values = newValues;
            }

            var indexes = GetHiddenIndexes().ToList();

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
	            _colon.Apply(c);
	            var result = new Tuple<decimal,LabelPosition[]>(_colon.Temperature, c);
                yield return result;
            }
        }
        private IEnumerable<int> GetHiddenIndexes() {
            var itemNum = -1;
            var hidden = _colon.Select(_ => new KeyValuePair<int, LabelPosition>(++itemNum, _.LabelPosition));
            return hidden.Where(_ => _.Value == LabelPosition.Hidden).Select(_ => _.Key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Tuple<decimal, LabelPosition[]>> GetEnumerator() {
            return Get().GetEnumerator();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}