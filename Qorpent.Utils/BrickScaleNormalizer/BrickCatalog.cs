using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.BrickScaleNormalizer
{
	/// <summary>
	/// Класс - контейнер "кирпичей" - высот DIVLINE
	/// </summary>
	public  class BrickCatalog {
		/// <summary>
		/// Размеры кирпичей по умолчанию
		/// </summary>
		public static readonly int[] DefaultBricks = new[] {
			1,  2,5, 3,  4, 
			10, 20,  25,50,  30, 40, 15,
			100, 200, 250, 500, 300, 400,  150,
			1000,2000,2500,5000,3000, 4000, 1500, 
			10000, 20000 , 25000,50000,30000, 40000, 15000,  
		};

		/// <summary>
		/// Максимумы по количеству дивлайнов на размер в пикселах по умолчанию
		/// </summary>
		///<remarks>Позиция эквивалентна кол-ву дивлайнов. 
		/// Для числа пикселей нужно найти максимальное число меньше или равное
		/// элементу массива - индекс массива будет являться максимальным числом 
		/// разделителей</remarks>
		public static readonly int[] BricksMaxDefault = new[] {
			0,100,200,0,0,300,400,0,500,0,600
		};
		/// <summary>
		/// Рекомендованные минимумы по количеству дивлайнов на размер в пикселах
		/// </summary>
		///<remarks>Позиция эквивалентна кол-ву дивлайнов. 
		/// Для числа пикселей нужно найти минимальное число больше или равное
		/// элементу массива - индекс массива будет являться рекомендованным минимальным числом 
		/// разделителей</remarks>
		public static readonly int[] RecomendedBricksMinDefault = new[] {
			100,200,300,600,800
		};


		private int[] _bricks = null;
		private int[] _maxdelimit = null;
		private int[] _mindelimit = null;
		/// <summary>
		/// Акцессор к каталогу кирпичей
		/// </summary>
		public int[] DelimiterMax {
			get { return _maxdelimit ?? BricksMaxDefault; }
			set { _maxdelimit = value; }
		}
		/// <summary>
		/// Акцессор к каталогу кирпичей
		/// </summary>
		public int[] RecomendedDelimiterMin {
			get { return _mindelimit ?? RecomendedBricksMinDefault; }
			set { _mindelimit = value; }
		}
		/// <summary>
		/// Акцессор к каталогу кирпичей
		/// </summary>
		public int[] Bricks {
			get { return _bricks ?? DefaultBricks; }
			set { _bricks = value; }
		}

		/// <summary>
		/// Подбирает лучший вариант "кладки"
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public BrickVariant GetBestVariant(BrickRequest request) {
			var result = GetAllVariants(request).Select(_ => _.Optimize()).OrderBy(_ => _, new BrickVariantComparer()).FirstOrDefault();
			if (null == result) {
				throw new Exception("cannot decide valid variant");
			}
			if (!WellFormedScale(result)) {
				result = NormalizeOnlyMaxValue(request);
			}
			return result;
		}

		private static bool WellFormedScale(BrickVariant result) {
			if (result.BrickCount <= 2) {
				if ((result.Request.MaxValue/result.BrickMaxValue) <= 0.8m) {
					return false;
				}
			}
			return true;
		}

		private BrickVariant NormalizeOnlyMaxValue(BrickRequest request) {
			var initialsize = request.Size;
			request.MaxDelimit = 0;
			request.MinDelimit = 0;
			request.Size = 800;
			var result = GetAllVariants(request).Select(_ => _.Optimize()).OrderBy(_ => _, new BrickVariantComparer()).FirstOrDefault();
			result.BrickSize = result.BrickMaxValue;
			result.BrickCount = 1;
			request.Size = initialsize;
			return result;
		}

		/// <summary>
		/// Возвращает все допустимые варианты
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		private IEnumerable<BrickVariant> GetAllVariants(BrickRequest request) {
			if (request.MaxDelimit <= 0) {
				request.MaxDelimit = DecideBestMaxDelimit(request.Size);
			}
			if (request.MinDelimit <= 0) {
				request.MinDelimit = DecideBestMinDelimit(request.Size);
			}
			
			foreach (var i in Bricks) {
				var variant = new BrickVariant {BrickSize = i, Request = request, Catalog = this};
				if (variant.BrickCount <= request.MaxDelimit && variant.BrickCount>=request.MinDelimit) {
					yield return variant;
				}
			}
		}

		private int DecideBestMinDelimit(decimal maxValue) {
			for (var i = 0; i < RecomendedDelimiterMin.Length; i++)
			{
				var size = RecomendedDelimiterMin[i];
				if (0 == size) continue;
				if (maxValue <= size) return i;
			}
			return RecomendedDelimiterMin.Length;
		}
		/// <summary>
		/// Определяет ограничение верхних разделителей
		/// </summary>
		/// <param name="maxValue"></param>
		/// <returns></returns>
		private int DecideBestMaxDelimit(decimal maxValue) {
			for (var i = DelimiterMax.Length - 1; i >= 0; i--) {
				var size = DelimiterMax[i];
				if(0==size)continue;
				if (maxValue >= size) return i;
			}
			return 0;
		}
	}
}
