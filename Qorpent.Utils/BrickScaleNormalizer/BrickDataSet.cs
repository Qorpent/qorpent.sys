using System;
using System.Collections.Generic;
using System.Linq;

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
			if(IsRequireLabelPositionCalculate())
			{
				CalculateLabelPosition();
			}
		}

		private bool IsRequireLabelPositionCalculate() {
			return 0!=(Preferences.SeriaCalcMode & (SeriaCalcMode.SeriaLinear|SeriaCalcMode.CrossSeriaLinear))  && Rows.Select(_=>_.RowNumber).Distinct().Count()>1;
		}

		private void CalculateLabelPosition() {
			//throw new NotImplementedException();
		}

		private void CalculateSecondScale() {
			if (0 == Preferences.SYFixMin && 0 == Preferences.SYFixMin && 0 == Preferences.SYFixDiv)
			{
				var realMin = GetMin(ScaleType.Second);
				var realMax = GetMax(ScaleType.Second);
				var req = new BrickRequest();
				req.SourceMinValue = realMin;
				req.SourceMaxValue = realMax;
				req.Setup(Preferences.SY, Preferences.SYMin, Preferences.SYMax, Preferences.SYTop.ToString(),
						  Preferences.SYSignDelta.ToString());
				var cat = new BrickCatalog();
				var result = cat.GetBestVariant(req);
				this.SecondScale = new Scale { Prepared = true, Min = result.ResultMinValue, Max = result.ResultMaxValue, DivLines = result.ResultDivCount };
			}
			else
			{
				this.SecondScale = new Scale();
			}
		}

		private void CalculateFirstScale() {
			if (0 == Preferences.YFixMin && 0 == Preferences.YFixMin && 0 == Preferences.YFixDiv) {
				var realMin = GetMin(ScaleType.First);
				var realMax = GetMax(ScaleType.First);
				var req = new BrickRequest();
				req.SourceMinValue = realMin;
				req.SourceMaxValue = realMax;
				req.Setup(Preferences.Y, Preferences.YMin, Preferences.YMax, Preferences.YTop.ToString(),
				          Preferences.YSignDelta.ToString());
				var cat = new BrickCatalog();
				var result = cat.GetBestVariant(req);
				this.FirstScale = new Scale{Prepared = true, Min = result.ResultMinValue,Max = result.ResultMaxValue,DivLines = result.ResultDivCount};
			}
			else {
				this.FirstScale = new Scale();
			}
			
			
		}

		private bool _isNormalizedRecord = false;
		private BrickDataSet _normalizedSet = null;
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
				if (null == _normalizedSet) {
					_normalizedSet = GetNormalizedRecord();
				}
				return _normalizedSet.GetMax(scaleType);
		}

		private BrickDataSet GetNormalizedRecord() {
			var result = new BrickDataSet {_isNormalizedRecord = true};
			foreach (var scaleType in new[]{ScaleType.First,ScaleType.Second}) {
				var rows = Rows.Where(_ => _.ScaleType == scaleType).ToArray();
				if (rows.Length == 0) continue;
				var maxCount = rows.Select(_ => _.Items.Count).Max();
				var scaleRow = new DataRow();
				scaleRow.ScaleType = scaleType;
				result.Rows.Add(scaleRow);
				bool sumcs = 0 != (Preferences.SeriaCalcMode & SeriaCalcMode.CrossSeriaSum);
				bool sumser = 0 != (Preferences.SeriaCalcMode & SeriaCalcMode.SeriaSum);
				for (var i = 0; i < maxCount; i++) {
					scaleRow.Items.Add(new DataItem {
						NegMin = sumcs?0: decimal.MaxValue,
						PosMax = sumcs?0:decimal.MinValue,
						PosMin = decimal.MaxValue,
						NegMax = decimal.MinValue
					});
				}
				var serias = rows.GroupBy(_ => _.SeriaNumber);
				for (var i = 0; i < maxCount; i++)
				{
				
					var currentPosMax = sumser ? 0 : decimal.MinValue;
					var currentPosMin =  decimal.MaxValue;
					var currentNegMax =  decimal.MinValue;
					var currentNegMin = sumser ? 0 : decimal.MaxValue;

					foreach (var seria in serias)
					{
						var seriaPosMax = sumser ? 0 : decimal.MinValue;
						var seriaNegMin = sumser ? 0 : decimal.MaxValue;
						foreach (var row in seria) {
							decimal val = 0;
							if (i < row.Items.Count) {
								val = row.Items[i].Value;
							}
							if (sumser)
							{
								if (val >= 0) {
									if (sumcs) {
										currentPosMax += val;
									}
									seriaPosMax += val;
								}
								else {
									if (sumcs) {
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
						if (sumser ) {

							currentNegMax = Math.Max(currentNegMax, seriaNegMin);
							currentPosMin = Math.Min(currentPosMin, seriaPosMax);
							if (!sumcs) {
								currentPosMax = Math.Max(currentPosMax, seriaPosMax);
								currentNegMin = Math.Min(currentNegMin, seriaNegMin);
							}
						}
						
					}
					if (sumcs)
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


		/// <summary>
		/// Получить реальное полное минимальное значение по всему набору данных с учетом стеков и рядов
		/// </summary>
		/// <param name="scaleType"></param>
		/// <returns></returns>
		public decimal GetMin(ScaleType scaleType = ScaleType.First) {
				if (_isNormalizedRecord) {
					var _seria = Rows.FirstOrDefault(_ => _.ScaleType == scaleType);
					if (null == _seria) return 0;
					return _seria.Items.Select(_ => _.Min).Min();
				}
				if (null == _normalizedSet) {
					_normalizedSet = GetNormalizedRecord();
				}
			return _normalizedSet.GetMin(scaleType);
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
			row.Items.Add(new DataItem{Value = value});
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
}
