﻿using System;
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
        ///     Текущий индекс элемента данных, вставленного в датасет
        /// </summary>
	    private int _currentDataItemIndex;
        /// <summary>
        ///     Внутренний список серий
        /// </summary>
        private readonly Dictionary<int, List<BrickDataSetSeria>> _sets = new Dictionary<int, List<BrickDataSetSeria>>();
		/// <summary>
		///     Рассчитанный размер первой шкалы
		/// </summary>
		public Scale FirstScale { get; set; }
		/// <summary>
		///     Рассчитанный вариант второй шкалы
		/// </summary>
		public Scale SecondScale { get; set; }
	    /// <summary>
	    ///     Ряды данных внутри датасета
	    /// </summary>
	    public IEnumerable<DataRow> Rows {
            get { return _sets.Values.SelectMany(_ => _).SelectMany(_ => _.Rows).ToList(); }
	    }
        /// <summary>
        ///     Перечисление элементов данных
        /// </summary>
	    public IEnumerable<DataItem> DataItems {
            get { return _sets.Values.SelectMany(_ => _).SelectMany(_ => _); }
	    }
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
            get { return _sets.Values.SelectMany(_ => _).ToArray(); }
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
                    req.Setup(Preferences.Y, Preferences.YMin, Preferences.YMax, Preferences.YTop.ToString(), Preferences.YSignDelta.ToString());
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
				//result.Rows.Add(scaleRow); я хер знает что с этим говнищем теперь делать
                result.EnsureSeria(0).Add(scaleRow);
				var serias = rows.GroupBy(_ => _.SeriaNumber).ToArray();
				for (var i = 0; i < maxCount; i++)
				{
				
					var currentPosMax = 0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum) ? 0 : decimal.MinValue;
					var currentPosMin =  decimal.MaxValue;
					var currentNegMax =  decimal.MinValue;
					var currentNegMin = 0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum) ? 0 : decimal.MaxValue;

					foreach (var seria in serias) {
						var seriaPosMax = 0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum) ? 0 : decimal.MinValue;
						var seriaNegMin = 0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum) ? 0 : decimal.MaxValue;
						foreach (var row in seria) {
							decimal val = 0;
							if (i < row.Items.Count) {
								val = row.Items[i].Value;
							}
							if (0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum)) {
								if (val >= 0) {
									if (0 != (Preferences.SeriaCalcMode & SeriaCalcMode.CrossSeriaSum)) {
										currentPosMax += val;
									}
									seriaPosMax += val;
								} else {
									if (0 != (Preferences.SeriaCalcMode & SeriaCalcMode.CrossSeriaSum)) {
										currentNegMin += val;
									}
									seriaNegMin += val;
								}
								
							} else {
								if (val >= 0) {
									currentPosMax = Math.Max(val,currentPosMax);
									currentPosMin = Math.Min(val, currentPosMin);
								} else {
									currentNegMin = Math.Min(val,currentNegMin);
									currentNegMax = Math.Max(val, currentNegMax);
								}
							}
						}
						if (0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum)) {
							currentNegMax = Math.Max(currentNegMax, seriaNegMin);
							currentPosMin = Math.Min(currentPosMin, seriaPosMax);
							if (0 == (Preferences.SeriaCalcMode & SeriaCalcMode.CrossSeriaSum)) {
								currentPosMax = Math.Max(currentPosMax, seriaPosMax);
								currentNegMin = Math.Min(currentNegMin, seriaNegMin);
							}
						}
						
					}
					if (0 != (Preferences.SeriaCalcMode & SeriaCalcMode.CrossSeriaSum)) {
						scaleRow.Items[i].PosMax += currentPosMax;
						scaleRow.Items[i].NegMin += currentNegMin;
					} else {
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
			var scaleRow = new DataRow {ScaleType = scaleType};
            for (var i = 0; i < maxCount; i++) {
				scaleRow.Add(new DataItem {
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
            foreach (var set in _sets) {
                set.Value.Remove(seria);
            }
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
	    /// 
	    /// </summary>
	    /// <param name="serianum"></param>
	    /// <param name="rownum"></param>
	    /// <param name="value"></param>
	    /// <param name="secondscale"></param>
	    /// <param name="setnumber"></param>
	    public DataItem Add(int serianum, int rownum, decimal value, bool secondscale, int setnumber = 0) {
            var item = new DataItem { Value = value, LabelHeight = LabelHeight, DatasetIndex = _currentDataItemIndex };
		    _currentDataItemIndex++;
            Insert(setnumber, serianum, rownum, secondscale ? ScaleType.Second : ScaleType.First, item);
		    return item;
		}
	    /// <summary>
	    ///     Вставка элемента данных в датасет
	    /// </summary>
	    /// <param name="setnumber">Номер сета</param>
	    /// <param name="serianum">Номер серии</param>
	    /// <param name="rownum">Номер ряда</param>
	    /// <param name="scaleType">Тип шкалы, к которой относится элемент данных</param>
	    /// <param name="dataItem">Элемент данных</param>
	    public void Insert(int setnumber, int serianum, int rownum, ScaleType scaleType, DataItem dataItem) {
            EnsureRow(serianum, rownum, scaleType, setnumber).Add(dataItem);
        }

	    /// <summary>
	    ///     Убеждается в наличии серии и производит добавление в случае отсутствия
	    /// </summary>
	    /// <param name="serianum">Номер серии</param>
	    /// <param name="setnumber"></param>
	    /// <returns>Серия с указанным идентификатором</returns>
	    public BrickDataSetSeria EnsureSeria(int serianum, int setnumber = 0) {
            if (!_sets.ContainsKey(setnumber)) {
                _sets.Add(setnumber, new List<BrickDataSetSeria>());
            }
	        var seria = _sets[setnumber].FirstOrDefault(_ => _.SeriaNumber == serianum);
            if (seria == null) {
                seria = new BrickDataSetSeria(serianum);
                _sets[setnumber].Add(seria);
            }
	        return seria;
	    }

	    /// <summary>
	    ///     Убеждается в наличии ряда в указанной серии и производит инициализацию при необходимости
	    /// </summary>
	    /// <param name="serianum">Номер серии</param>
	    /// <param name="rownum">Номер ряда внутри серии</param>
	    /// <param name="scaleType">Тип шкалы</param>
	    /// <param name="setnumber"></param>
	    /// <returns>Представления ряда данных</returns>
	    public DataRow EnsureRow(int serianum, int rownum, ScaleType scaleType, int setnumber = 0) {
            return EnsureSeria(serianum, setnumber).EnsureRow(rownum, scaleType);
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
			var row = EnsureRow(serianum, rownum, secondscale ? ScaleType.Second : ScaleType.First);
			if (pos < row.Items.Count) return row.Items[pos];
			return new DataItem();
		}
		/// <summary>
	    ///     Приведение <see cref="BrickDataSet"/> к <see cref="string"/>
	    /// </summary>
	    /// <returns>Строковое представление <see cref="BrickDataSet"/></returns>
	    public override string ToString() {
	        return string.Join(";", this.GetSeries().Select(_ => _.ToString()));
	    }
	}
}
