using System;
using System.Linq;

namespace Qorpent.Utils.BrickScaleNormalizer {
	/// <summary>
	/// Подобранный вариант материала
	/// </summary>
	public class BrickVariant:IComparable<BrickVariant> {
		/// <summary>
		/// Проверяет идентичность вариантов
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected bool Equals(BrickVariant other) {
			return _brickCount == other._brickCount && BrickSize == other.BrickSize && Equals(Request, other.Request) && Equals(Catalog, other.Catalog);
		}
		/// <summary>
		/// Стандартный хэш код
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode() {
			unchecked {
				int hashCode = _brickCount;
				hashCode = (hashCode*397) ^ BrickSize;
				hashCode = (hashCode*397) ^ (Request != null ? Request.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ (Catalog != null ? Catalog.GetHashCode() : 0);
				return hashCode;
			}
		}

		/// <summary>
		/// Операция упорядочения по качеству
		/// </summary>
		/// <param name="other"></param>
		/// <returns>
		/// Выигравают более триммированные и/или более детализованные варианты
		/// </returns>
		public int CompareTo(BrickVariant other) {
			if (other.Equals(this)) return 0;

			if (0 != Request.MinPixelTop) {
				if (other.TopMarginePixels > this.BrickSizePixels) return -1;
				if (other.TopMarginePixels > Request.MinPixelTop*2 && this.TopMarginePixels < Request.MinPixelTop*2) return -1;
			}
			else {
				if (other.TopMarginePixels - this.TopMarginePixels > 10) return -1;
			}
			if (this.BrickMaxValue/other.BrickMaxValue < 0.8m) return -1;
			
			var thisqual = Array.IndexOf(Catalog.Bricks, this.BrickSize);
			var otherqual = Array.IndexOf(Catalog.Bricks, other.BrickSize);
			return thisqual.CompareTo(otherqual);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((BrickVariant) obj);
		}

		private int _brickCount =-1;

		/// <summary>
		/// Размер кирпича
		/// </summary>
		public int BrickSize { get; set; }


		/// <summary>
		/// Количество кирпичей
		/// </summary>
		/// <remarks>
		/// Либо явно выставляется, либо вычисляется
		/// </remarks>
		public int BrickCount {
			get {
				if (-1 == _brickCount) {
					_brickCount =  Convert.ToInt32(Math.Floor(Request.MaxValue / BrickSize))+1;
					while (TopMarginePixels < Request.MinPixelTop) {
						_brickCount++;
					}
				}
				return _brickCount;
			}
			set { _brickCount = value; }
		}
	

		/// <summary>
		/// Превышение над максом
		/// </summary>
		public decimal TopMargin {
			get { return BrickMaxValue - Request.MaxValue; }
		}
		/// <summary>
		/// Итоговая высота в кирпичах
		/// </summary>
		public int BrickMaxValue {
			get { return BrickSize*_brickCount; }
		}
		/// <summary>
		/// Возвращает true если можно без потерь для "качества" срезать еще кирпич сверху
		/// </summary>
		public bool IsTrimable {
			get { return ((TopMarginePixels - BrickSizePixels) >= Request.MinPixelTop); }
		}

		/// <summary>
		/// Превышение над максом в пикселях
		/// </summary>
		public int TopMarginePixels {
			get {
				var valsinPixels = Math.Abs(BrickMaxValue) / ((decimal)Request.Size);
				return (int)(TopMargin/valsinPixels);
			}
		}
		/// <summary>
		/// Превышение над максом в пикселях
		/// </summary>
		public int BrickSizePixels
		{
			get
			{
				var valsinPixels = ((decimal)BrickMaxValue) / ((decimal)Request.Size);
				return (int)(BrickSize / valsinPixels);
			}
		}
		/// <summary>
		/// Возможность разложить на кирпичи меньшего размера при сохранении общего размера
		/// </summary>
		public bool IsSplittable {
			get {
				if (BrickCount * 2 > Request.MaxDelimit) return false;
				return Catalog.Bricks.Any(brick => BrickSize == brick*2);
			}
		}
		/// <summary>
		/// Возможность увеличить до кирпичей большего размера при сохранении большего размера
		/// </summary>
		public bool Expandable {
			get {
				if (BrickCount/2 < Request.MinDelimit) return false;
				if (1 == BrickCount%2) return false;
				return Catalog.Bricks.Any(brick => BrickSize*2 == brick);
			}
		}
		/// <summary>
		/// Расщепляет вариант на более мелкие единицы (может вернуть клон)
		/// </summary>
		/// <returns></returns>
		public BrickVariant Split(bool clone = false) {
			if(!IsSplittable)throw new Exception("cannot split");
			var result = clone? (BrickVariant) this.MemberwiseClone() : this;
			result._brickCount = -1;
			result.BrickSize = this.BrickSize/2;
			return result;
		}
		/// <summary>
		/// отрезает по кирпичу сверху
		/// </summary>
		/// <returns></returns>
		public BrickVariant Trim(bool clone = false)
		{
			if (!IsTrimable) throw new Exception("cannot trim");
			var result = clone ? (BrickVariant)this.MemberwiseClone() : this;
			result._brickCount--;
			return result;
		}
		/// <summary>
		/// Укрупняет вариант на более крупные позиции
		/// </summary>
		/// <returns></returns>
		public BrickVariant Expand(bool clone = false)
		{
			if (!Expandable) throw new Exception("cannot expand");
			var result = clone ? (BrickVariant)this.MemberwiseClone() : this;
			result._brickCount = -1;
			result.BrickSize = this.BrickSize * 2;
			return result;
		}
		/// <summary>
		/// Конвертирует этот вариант к оптимальному
		/// </summary>
		/// <returns></returns>
		public BrickVariant Optimize(bool clone=false) {
			var current = clone ? this.MemberwiseClone() as BrickVariant : this;
			while (current.IsSplittable || current.IsTrimable) {
				if (current.IsSplittable) {
					current = current.Split();
				}
				else {
					current = current.Trim();
				}
			}
			return current;
		}

		/// <summary>
		/// Ссылка на исходный запрос
		/// </summary>
		public BrickRequest Request { get; set; }
		/// <summary>
		/// Ссылка на исходный катлог
		/// </summary>
		public BrickCatalog Catalog { get; set; }

		/// <summary>
		/// Итоговое минимальное значение с учетом масштаба и смещения
		/// </summary>
		public decimal ResultMinValue {
			get {
				var realbricksize = ResultBrickSize;
				
				if (Request.MinimalScaleBehavior== MiniamlScaleBehavior.MatchMin) {
					return Request.SourceMinValue;
				}
				var currentMin = 0m;
				if (currentMin > Request.SourceMinValue || Request.MinimalScaleBehavior==MiniamlScaleBehavior.FitMin) {
					while (currentMin > Request.SourceMinValue)
					{
						currentMin -= realbricksize;
					}	
					if (Request.MinimalScaleBehavior == MiniamlScaleBehavior.FitMin) {
						while (currentMin < Request.SourceMinValue ) {
							currentMin += realbricksize;
						}
						if (currentMin > Request.SourceMinValue) {
							currentMin -= realbricksize;
						}
					}
				}


				return currentMin;
			}
		}
		/// <summary>
		/// Итоговый размер кирпича
		/// </summary>
		public decimal ResultBrickSize {
			get { return BrickSize/Request.Scale; }
		}
		/// <summary>
		/// Итоговое максимальное значение по шкале
		/// </summary>
		public decimal ResultMaxValue {
			get {
				var realbricksize = ResultBrickSize;
				var minvalue = ResultMinValue;
				var realmaxval = ResultBrickSize*BrickCount + minvalue;
				while (realmaxval < Request.SourceMaxValue) {
					realmaxval += realbricksize;
				}
				while (realmaxval > (Request.SourceMaxValue+realbricksize)) {
					realmaxval -= realbricksize;
				}
				if (Request.MinPixelTop != 0) {
					var realpixelsperdata = (realmaxval - minvalue)/Request.Size;
					var realpixeltop = (realmaxval - Request.SourceMaxValue)/realpixelsperdata;
					if (realpixeltop < Request.MinPixelTop) {
						realmaxval += realbricksize;
					}

				}
				return realmaxval;
			}
		}
		/// <summary>
		/// Количество разделителей
		/// </summary>
		public int ResultDivCount {
			get {
				var result = (int)((ResultMaxValue - ResultMinValue)/ResultBrickSize);
				 result = result - 1;
				if (result <= 0) return 0;
				return result;
			}
		}
	}
}