using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.BrickScaleNormalizer
{
	/// <summary>
	/// Описывает набор данных для обсчета
	/// </summary>
	public class BrickDataSet {
		/// <summary>
		/// 
		/// </summary>
		public BrickDataSet() {
			Rows = new List<DataRow>();
			Preferences = new UserPreferences();
		}
		
		/// <summary>
		/// Рассчитанный размер первой шкалы
		/// </summary>
		public Scale FirstScale { get; set; }
		/// <summary>
		/// Рассчитанный вариант второй шкалы
		/// </summary>
		public Scale SecondScale { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public IList<DataRow> Rows { get; private set; }
		/// <summary>
		/// Требования пользователя
		/// </summary>
		public UserPreferences Preferences { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public void Calculate() {
			CalculateFirstScale();

			if (Rows.Any(_ => _.ScaleType == ScaleType.Second)) {
				CalculateSecondScale();
			}

			if(IsRequireLabelPositionCalculate()) {
				CalculateNormalizedValue();
				CalculateLabelPosition();
			}
		}

		private void CalculateNormalizedValue() {
			foreach (var scaleType in new[]{ScaleType.First,ScaleType.Second, }) {
				var data = Rows.Where(_ => _.ScaleType == scaleType).ToArray();
			    if (0 == data.Length) {
			        continue;
			    }

			    var scale = scaleType == ScaleType.First ? FirstScale : SecondScale;
			    foreach (var row in data) {
			        foreach (var item in row.Items) {
			            item.NormalizedValue = (item.Value - scale.Min)/scale.ValueInPixel;
			        }
			    }
			}
		}

		private bool IsRequireLabelPositionCalculate() {
			return 0!=(Preferences.SeriaCalcMode & (SeriaCalcMode.SeriaLinear|SeriaCalcMode.CrossSeriaLinear))  && Rows.Select(_=>_.SeriaNumber).Distinct().Count()>1;
		}
        /// <summary>
        ///     Собирает абстрактные олонки значений
        /// </summary>
        /// <returns>Перечисление абстрактных колонок значений</returns>
        public IEnumerable<DataItemColon> BuildColons() {
            return this.GetColons();
        }
        /// <summary>
        ///     Производит обсчёт расположения лэйблов значений и пытается максимально раздвинуть их между собой
        /// </summary>
		private void CalculateLabelPosition() {
            BuildColons().ForEach(CalculateLabelPosition);
        }
        /// <summary>
        ///     Производит обсчёт расположения лэйблов внутри одной колонки
        /// </summary>
        /// <param name="colon">Представление колонки данных</param>
        private void CalculateLabelPosition(DataItemColon colon) {
            colon.MinimizeTemperature();
        }
		private void CalculateSecondScale() {
			if (0 == Preferences.SYFixMin && 0 == Preferences.SYFixMin && 0 == Preferences.SYFixDiv)
			{
				var realMin = GetMin(ScaleType.Second);
				var realMax = GetMax(ScaleType.Second);
				var req = new BrickRequest { SourceMinValue = realMin, SourceMaxValue = realMax };
			    req.Setup(Preferences.SY, Preferences.SYMin, Preferences.SYMax, Preferences.SYTop.ToString(), Preferences.SYSignDelta.ToString());
				var cat = new BrickCatalog();
				var result = cat.GetBestVariant(req);
				SecondScale = new Scale {
					Prepared = true, 
					Min = result.ResultMinValue, 
					Max = result.ResultMaxValue, 
					DivLines = result.ResultDivCount,
					
				};

				SecondScale.ValueInPixel = (SecondScale.Max - SecondScale.Min)/Preferences.Height;
			} else {
				SecondScale = new Scale();
			}
		}

		private void CalculateFirstScale() {
			if (0 == Preferences.YFixMin && 0 == Preferences.YFixMin && 0 == Preferences.YFixDiv) {
				var realMin = GetMin();
				var realMax = GetMax();
				var req = new BrickRequest();
				req.SourceMinValue = realMin;
				req.SourceMaxValue = realMax;
				req.Setup(Preferences.Y, Preferences.YMin, Preferences.YMax, Preferences.YTop.ToString(),
				          Preferences.YSignDelta.ToString());
				var cat = new BrickCatalog();
				var result = cat.GetBestVariant(req);
				FirstScale = new Scale{Prepared = true, Min = result.ResultMinValue,Max = result.ResultMaxValue,DivLines = result.ResultDivCount};
				FirstScale.ValueInPixel = (FirstScale.Max - FirstScale.Min) / Preferences.Height;
			}
			else {
				FirstScale = new Scale();
			}
			
			
		}


		

		private bool _isNormalizedRecord;
		private BrickDataSet _normalizedSet;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="scaleType"></param>
		/// <returns></returns>
		public decimal GetMax(ScaleType scaleType = ScaleType.First) {

				if (_isNormalizedRecord) {
					var _seria = Rows.FirstOrDefault(_ => _.ScaleType == scaleType);
					if (null == _seria) return 0;
					return _seria.Items.Select(_ => _.Max).Max();
				}

				return EnsureNormalized().GetMax(scaleType);
		}
        /// <summary>
        ///     Собирает нормализованный экземпляр <see cref="BrickDataSet"/>
        /// </summary>
        /// <returns>Нормализованный экземпляр <see cref="BrickDataSet"/></returns>
        public BrickDataSet EnsureNormalized() {
            return _normalizedSet ?? (_normalizedSet = GetNormalizedRecord());
        }

	    private BrickDataSet GetNormalizedRecord() {
			var result = new BrickDataSet {_isNormalizedRecord = true};
			foreach (var scaleType in new[]{ScaleType.First,ScaleType.Second}) {
				var rows = Rows.Where(_ => _.ScaleType == scaleType).ToArray();
				if (rows.Length == 0) continue;
				var maxCount = rows.Select(_ => _.Items.Count).Max();
				var scaleRow = InitNormalizedScaleRow(scaleType, maxCount);
				result.Rows.Add(scaleRow);
				var serias = rows.GroupBy(_ => _.SeriaNumber).ToArray();
				for (var i = 0; i < maxCount; i++)
				{
				
					var currentPosMax = 0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum) ? 0 : decimal.MinValue;
					var currentPosMin =  decimal.MaxValue;
					var currentNegMax =  decimal.MinValue;
					var currentNegMin = 0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum) ? 0 : decimal.MaxValue;

					foreach (var seria in serias)
					{
						var seriaPosMax = 0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum) ? 0 : decimal.MinValue;
						var seriaNegMin = 0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum) ? 0 : decimal.MaxValue;
						foreach (var row in seria) {
							decimal val = 0;
							if (i < row.Items.Count) {
								val = row.Items[i].Value;
							}
							if (0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum))
							{
								if (val >= 0) {
									if (0 != (Preferences.SeriaCalcMode & SeriaCalcMode.CrossSeriaSum)) {
										currentPosMax += val;
									}
									seriaPosMax += val;
								}
								else {
									if (0 != (Preferences.SeriaCalcMode & SeriaCalcMode.CrossSeriaSum)) {
										currentNegMin += val;
									}
									seriaNegMin += val;
								}
								
							}
							else
							{
								if (val >= 0)
								{
									currentPosMax = Math.Max(val,currentPosMax);
									currentPosMin = Math.Min(val, currentPosMin);
								}
								else
								{
									currentNegMin = Math.Min(val,currentNegMin);
									currentNegMax = Math.Max(val, currentNegMax);
								}
							}
						}
						if (0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum) ) {

							currentNegMax = Math.Max(currentNegMax, seriaNegMin);
							currentPosMin = Math.Min(currentPosMin, seriaPosMax);
							if (!(0 != (Preferences.SeriaCalcMode & SeriaCalcMode.CrossSeriaSum))) {
								currentPosMax = Math.Max(currentPosMax, seriaPosMax);
								currentNegMin = Math.Min(currentNegMin, seriaNegMin);
							}
						}
						
					}
					if (0 != (Preferences.SeriaCalcMode & SeriaCalcMode.CrossSeriaSum))
					{
						scaleRow.Items[i].PosMax += currentPosMax;
						scaleRow.Items[i].NegMin += currentNegMin;
					}
					else
					{
						scaleRow.Items[i].PosMax = Math.Max(currentPosMax, scaleRow.Items[i].PosMax);
						scaleRow.Items[i].NegMin = Math.Min(currentNegMin, scaleRow.Items[i].NegMin);
						scaleRow.Items[i].NegMax = Math.Max(currentNegMax, scaleRow.Items[i].NegMax);
						scaleRow.Items[i].PosMin = Math.Min(currentPosMin, scaleRow.Items[i].PosMin);
					}
				}
				
			}
			return result;
		}

		private DataRow InitNormalizedScaleRow(ScaleType scaleType, int maxCount) {
			var scaleRow = new DataRow();
			scaleRow.ScaleType = scaleType;


			for (var i = 0; i < maxCount; i++) {
				scaleRow.Items.Add(new DataItem {
					NegMin = 0 != (Preferences.SeriaCalcMode & SeriaCalcMode.CrossSeriaSum) ? 0 : decimal.MaxValue,
					PosMax = 0 != (Preferences.SeriaCalcMode & SeriaCalcMode.CrossSeriaSum) ? 0 : decimal.MinValue,
					PosMin = decimal.MaxValue,
					NegMax = decimal.MinValue
				});
			}
			return scaleRow;
		}


		/// <summary>
		/// Получить реальное полное минимальное значение по всему набору данных с учетом стеков и рядов
		/// </summary>
		/// <param name="scaleType"></param>
		/// <returns></returns>
		public decimal GetMin(ScaleType scaleType = ScaleType.First) {
            if (_isNormalizedRecord) {
                var seria = Rows.FirstOrDefault(_ => _.ScaleType == scaleType);
                return null == seria ? 0 : seria.Items.Select(_ => _.Min).Min();
            }

			return EnsureNormalized().GetMin(scaleType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serianum"></param>
		/// <param name="rownum"></param>
		/// <param name="value"></param>
		public void Add(int serianum, int rownum, decimal value) {
			Add(serianum, rownum, value, false);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serianum"></param>
		/// <param name="rownum"></param>
		/// <param name="value"></param>
		/// <param name="secondscale"></param>
		public void Add(int serianum, int rownum, decimal value, bool secondscale) {
			var row = ResolveRow(serianum, rownum, secondscale?ScaleType.Second:ScaleType.First);
			row.Items.Add(new DataItem { Value = value, Index = row.Items.Count });
		}
		/// <summary>
		/// Возвращает элемент данных по позиции
		/// </summary>
		public DataItem GetItem(int serianum, int rownum, int pos) {
			return GetItem(serianum, rownum, pos, false);
		}
		/// <summary>
		/// Возвращает элемент данных по позиции
		/// </summary>
		/// <param name="serianum"></param>
		/// <param name="rownum"></param>
		/// <param name="pos"></param>
		/// <param name="secondscale"></param>
		/// <returns></returns>
		public DataItem GetItem(int serianum, int rownum, int pos, bool secondscale) {
			var row = ResolveRow(serianum, rownum, secondscale ? ScaleType.Second : ScaleType.First);
			if (pos < row.Items.Count) return row.Items[pos];
			return new DataItem();
		}

		private DataRow ResolveRow(int serianum, int rownum, ScaleType scaleType) {
			var result = Rows.FirstOrDefault(_ => _.RowNumber == rownum && _.SeriaNumber == serianum && _.ScaleType == scaleType);
			if (null == result) {
				result = new DataRow {ScaleType = scaleType, RowNumber = rownum, SeriaNumber = serianum};
				Rows.Add(result);
			}
			return result;
		}
	}
    internal static class BrickDataSetExtensions {
        /// <summary>
        ///     Возвращает перечисление <see cref="DataRow"/> внутри <see cref="BrickDataSet"/> по номеру серии
        /// </summary>
        /// <param name="dataSet">Датасет</param>
        /// <param name="serianum">Номер серии</param>
        /// <returns>Перечисление <see cref="DataRow"/></returns>
        public static IEnumerable<DataRow> GetSeriaRows(this BrickDataSet dataSet, int serianum) {
            return dataSet.Rows.Where(_ => _.SeriaNumber == serianum);
        }
        /// <summary>
        ///     Возвращает упорядоченное перечисление серий из <see cref="BrickDataSet"/>
        /// </summary>
        /// <param name="dataSet">Датасет</param>
        /// <returns>Упорядоченное перечисление серий из <see cref="BrickDataSet"/></returns>
        public static IEnumerable<BrickDataSetSeria> GetSeries(this BrickDataSet dataSet) {
            return dataSet.Rows.GroupBy(_ => _.SeriaNumber).Select(_ => new BrickDataSetSeria(_.Key, _.Select(__ => __)));
        }
        /// <summary>
        ///     Собирает упорядоченное перечисление колонок данных графика в виде <see cref="DataItemColon"/>
        /// </summary>
        /// <param name="dataSet">Датасет</param>
        /// <returns>Упорядоченное перечисление колонок данных графика в виде <see cref="DataItemColon"/></returns>
        public static IEnumerable<DataItemColon> GetColons(this BrickDataSet dataSet) {
            return dataSet.Rows.SelectMany(_ => _.Items).GroupBy(_ => _.Index).Select(_ => new DataItemColon(_.Select(__ => __)));
        }
    }
    /// <summary>
    ///     Представление серии из <see cref="DataRow"/>
    /// </summary>
    internal class BrickDataSetSeria : IEnumerable<DataRow> {
        /// <summary>
        ///     Внутренний список <see cref="DataRow"/>, присущих данной серии
        /// </summary>
        private readonly List<DataRow> _rows = new List<DataRow>();
        /// <summary>
        ///     Номер серии
        /// </summary>
        public int SeriaNumber { get; private set; }
        /// <summary>
        ///     Представление серии из <see cref="DataRow"/>
        /// </summary>
        /// <param name="seriaNumber">Номер серии</param>
        public BrickDataSetSeria(int seriaNumber) {
            SeriaNumber = seriaNumber;
        }
        /// <summary>
        ///     Представление серии из <see cref="DataRow"/>
        /// </summary>
        /// <param name="seriaNumber">Номер серии</param>
        /// <param name="rows">Перечисление <see cref="DataRow"/>, присущих данной серии</param>
        public BrickDataSetSeria(int seriaNumber, IEnumerable<DataRow> rows) {
            SeriaNumber = seriaNumber;
            rows.ForEach(Add);
        }
        /// <summary>
        ///     Добавление <see cref="DataRow"/> в серию
        /// </summary>
        /// <param name="dataRow">Экземпляр <see cref="DataRow"/>, связанный с данной серие</param>
        public void Add(DataRow dataRow) {
            if (dataRow.SeriaNumber != SeriaNumber) {
                throw new Exception("Cannot assign a datarow from another seria");
            }

            _rows.Add(dataRow);
        }
        /// <summary>
        ///     Получение <see cref="IEnumerator{T}"/> по <see cref="DataRow"/>
        /// </summary>
        /// <returns><see cref="IEnumerator{T}"/> по <see cref="DataRow"/></returns>
        public IEnumerator<DataRow> GetEnumerator() {
            return _rows.GetEnumerator();
        }
        /// <summary>
        ///     Получение <see cref="IEnumerator{T}"/> по <see cref="DataRow"/>
        /// </summary>
        /// <returns><see cref="IEnumerator{T}"/> по <see cref="DataRow"/></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
