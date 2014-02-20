using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qorpent.Config;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	///     Описывает набор данных для обсчета
	/// </summary>
	public class BrickDataSet : ConfigBase {
        /// <summary>
        ///     Внутрениий список категорий
        /// </summary>
        private readonly List<string> _categories = new List<string>();
        /// <summary>
        ///     Внутренний список серий
        /// </summary>
        private readonly Dictionary<int, BrickDataSetSeria> _series = new Dictionary<int, BrickDataSetSeria>();
		/// <summary>
		///		Перечисление по айтемам данных
		/// </summary>
		public IEnumerable<DataItem> Items {
			get { return _items; }
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
        ///     Размер «лычки» в пикселях
        /// </summary>
        public int LabelHeight { get; private set; }
        /// <summary>
        ///     Серии
        /// </summary>
        public BrickDataSetSeria[] Series {
            get { return _series.Values.ToArray(); }
        }
        /// <summary>
        ///     Описывает набор данных для обсчета
        /// </summary>
        public BrickDataSet() : this(20) { }
        /// <summary>
        ///     Описывает набор данных для обсчета
        /// </summary>
        /// <param name="lableHeight">Размер лычки в пикселях</param>
        public BrickDataSet(int lableHeight) {
            Rows = new List<DataRow>();
            Preferences = new UserPreferences();
            LabelHeight = lableHeight;
        }
		/// <summary>
		/// 
		/// </summary>
		public virtual  BrickDataSet Calculate() {
			CalculateFirstScale();

			if (Rows.Any(_ => _.ScaleType == ScaleType.Second)) {
				CalculateSecondScale();
			}

			if(IsRequireLabelPositionCalculate()) {
				CalculateNormalizedValue();
				CalculateLabelPosition();
			}

		    return this;
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
            Task[] tasks = BuildColons().Select(CalculateLabelPositionAsync).ToArray();
            Task.WaitAll(tasks);
        }
        /// <summary>
        ///     Производит обсчёт расположения лэйблов внутри одной колонки
        /// </summary>
        /// <param name="colon">Представление колонки данных</param>
        private DataItemColon CalculateLabelPosition(DataItemColon colon) {
            colon.MinimizeTemperature();
            return colon;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="colon"></param>
        /// <returns></returns>
        private async Task<DataItemColon> CalculateLabelPositionAsync(DataItemColon colon) {
            return await Task<DataItemColon>.Factory.StartNew(() => CalculateLabelPosition(colon));
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
                if (FirstScale == null) {
                    var realMin = GetMin();
                    var realMax = GetMax();
                    var req = new BrickRequest {
                        SourceMinValue = realMin,
                        SourceMaxValue = realMax
                    };
                    req.Setup(Preferences.Y, Preferences.YMin, Preferences.YMax, Preferences.YTop.ToString(),
                              Preferences.YSignDelta.ToString());
                    var cat = new BrickCatalog();
                    var result = cat.GetBestVariant(req);
                    FirstScale = new Scale { Prepared = true, Min = result.ResultMinValue, Max = result.ResultMaxValue, DivLines = result.ResultDivCount };
                }

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
					var seria = Rows.FirstOrDefault(_ => _.ScaleType == scaleType);
					if (null == seria) return 0;
					return seria.Items.Select(_ => _.Max).Max();
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
        ///     Удаление указанной серии из жатасета
        /// </summary>
        /// <param name="seria">Искомая серия</param>
        public void Remove(BrickDataSetSeria seria) {
	        foreach (var item in seria) {
		        _items.Remove(item);
	        }
            _series.Remove(_series.FirstOrDefault(_ => _.Value == seria).Key);
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serianum"></param>
		/// <param name="rownum"></param>
		/// <param name="value"></param>
		public DataItem Add(int serianum, int rownum, decimal value) {
			return Add(serianum, rownum, value, false);
		}
        /// <summary>
        ///     Добавление категории
        /// </summary>
        /// <param name="label">Метка категории</param>
        public void AddCategory(string label) {
            if (!_categories.Contains(label)) {
                _categories.Add(label);
            }
        }
        /// <summary>
        ///     Установка имени серии
        /// </summary>
        /// <param name="serianum">Номер серии</param>
        /// <param name="name">Имя серии</param>
        public void SetSeriaName(int serianum, string name) {
            if (_series.ContainsKey(serianum)) {
                _series[serianum].Set("seriesname", name);
            }
        }
        /// <summary>
        ///     Установка мета-информации серии
        /// </summary>
        /// <param name="serianum">Номер серии</param>
        /// <param name="key">Ключ</param>
        /// <param name="value">Значение</param>
        public void SetSeriaMeta(int serianum, string key, string value) {
            if (_series.ContainsKey(serianum)) {
                _series[serianum].Set(key, value);
            }
        }
        /// <summary>
        ///     Получение мета-информации по ключу
        /// </summary>
        /// <param name="serianum">Номер серии</param>
        /// <param name="key">Ключ</param>
        /// <returns>Значение по ключу или <see cref="string.Empty"/></returns>
        public string GetSeriaMeta(int serianum, string key) {
            if (_series.ContainsKey(serianum)) {
                return _series[serianum].Get(key);
            }

            return string.Empty;
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serianum"></param>
		/// <param name="rownum"></param>
		/// <param name="value"></param>
		/// <param name="secondscale"></param>
		public DataItem Add(int serianum, int rownum, decimal value, bool secondscale) {
			var row = ResolveRow(serianum, rownum, secondscale?ScaleType.Second:ScaleType.First);
		    var item = new DataItem {Value = value, Index = row.Items.Count, LabelHeight = LabelHeight};
            if (!_series.ContainsKey(serianum)) {
               _series.Add(serianum, new BrickDataSetSeria(serianum));
            }
            if (!_series[serianum].Contains(row)) {
                _series[serianum].Add(row);
            }
			row.Items.Add(item);
			_items.Add(item);
		    return item;
		}
		/// <summary>
		/// Возвращает элемент данных по позиции
		/// </summary>
		public DataItem GetItem(int serianum, int rownum, int pos) {
			return GetItem(serianum, rownum, pos, false);
		}

		private readonly List<DataItem> _items = new List<DataItem>();
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
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator<DataItem> GetEnumerator() {
			return _items.GetEnumerator();
		}

		/// <summary>
	    /// 
	    /// </summary>
	    /// <returns></returns>
	    public override string ToString() {
	        return string.Join(";", this.GetSeries().Select(_ => _.ToString()));
	    }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static BrickDataSet Parse(string dataset) {
            throw new NotImplementedException();
        }
	}
}
