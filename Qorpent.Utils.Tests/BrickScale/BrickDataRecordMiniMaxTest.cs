using NUnit.Framework;
using Qorpent.Utils.BrickScaleNormalizer;

namespace Qorpent.Utils.Tests.BrickScale {
    /// <summary>
	/// Данные тесты проверяют корректность расчета минимакса шкал для промежуточного представления данных графика
	/// </summary>
	[TestFixture]
	public class BrickDataSetMiniMaxTest {
		[Test]
		public void SingleRow_Only_Positive_Test() {
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.Linear;
			dr.Add(1,1,11);
			dr.Add(1,1,13);
			dr.Add(1,1,9.8m);
			dr.Add(1,1,15m);
			dr.Add(1,1,17m);
			dr.Add(1,1,10m);
			Assert.AreEqual(9.8m,dr.GetMin());
			Assert.AreEqual(17,dr.GetMax());
		}

		


		[Test]
		public void SingleRow_With_Negative_Test()
		{
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.Linear;
			dr.Add(1, 1, 11);
			dr.Add(1, 1, 13);
			dr.Add(1, 1, 9.8m);
			dr.Add(1, 1, 15m);
			dr.Add(1, 1, 17m);
			dr.Add(1, 1, -10m);
			Assert.AreEqual(-10m, dr.GetMin());
			Assert.AreEqual(17, dr.GetMax());
		}

		[Test]
		public void SingleRow_Only_Negative_Test()
		{
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.Linear;
			dr.Add(1, 1, -11);
			dr.Add(1, 1, -13);
			dr.Add(1, 1, -9.8m);
			dr.Add(1, 1, -15m);
			dr.Add(1, 1, -17m);
			dr.Add(1, 1, -10m);
			Assert.AreEqual(-17m, dr.GetMin());
			Assert.AreEqual(-9.8m, dr.GetMax());
		}

		[Test]
		public void BUG_invalid_max_value() {
			var di = new DataItem {
				NegMax = decimal.MinValue,
				NegMin = decimal.MaxValue,
				PosMax = decimal.MinValue,
				PosMin = decimal.MaxValue
			};
			di.NegMax = -9.8m;
			di.NegMin = -9.8m;
			Assert.AreEqual(-9.8m,di.Max);
		}


		[Test]
		public void BUG_invalid_max_value_2()
		{
			var di = new DataItem
			{
				NegMax = decimal.MinValue,
				NegMin = decimal.MaxValue,
				PosMax = decimal.MinValue,
				PosMin = decimal.MaxValue
			};
			di.PosMax = 3m;
			di.NegMin = decimal.MaxValue;
			Assert.AreEqual(3m, di.Min);
			Assert.AreEqual(3m, di.Max);
		}


		[Test]
		public void MultiSeria_Positive_And_Negative_One_Seria_Two_Rows() {
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.Linear;
			dr.Add(1, 1, 11.5m);
			dr.Add(1, 1, -13);
			dr.Add(1, 1, 9.8m);
			dr.Add(1, 1, -15m);
			dr.Add(1, 1, -17.2m);
			dr.Add(1, 1, -10m);

			dr.Add(1, 2, 11.2m);
			dr.Add(1, 2, -13.1m);
			dr.Add(1, 2, 9.8m);
			dr.Add(1, 2, -15m);
			dr.Add(1, 2, -17.4m);
			dr.Add(1, 2, -10m);
			Assert.AreEqual(-17.4m, dr.GetMin());
			Assert.AreEqual(11.5m, dr.GetMax());
		}



		[Test]
		public void MultiSeria_Two_Scale()
		{
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.Linear;
			dr.Add(1, 1, 11.5m);
			dr.Add(1, 1, -13);
			dr.Add(1, 1, 9.8m);
			dr.Add(1, 1, -15m);
			dr.Add(1, 1, -17.2m);
			dr.Add(1, 1, -10m);

			dr.Add(2, 2, 11.2m,true);
			dr.Add(2, 2, -13.1m,true);
			dr.Add(2, 2, 9.8m,true);
			dr.Add(2, 2, -15m,true);
			dr.Add(2, 2, -17.4m,true);
			dr.Add(2, 2, -10m,true);
			Assert.AreEqual(-17.2m, dr.GetMin());
			Assert.AreEqual(11.5m, dr.GetMax());

			Assert.AreEqual(-17.4m, dr.GetMin(ScaleType.Second));
			Assert.AreEqual(11.2m, dr.GetMax(ScaleType.Second));
		}


		[Test]
		public void MultiSeria_Positive_And_Negative_Two_Seria_Two_Rows()
		{
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.Linear;
			dr.Add(1, 1, 11.5m);
			dr.Add(1, 1, -13);
			dr.Add(1, 1, 9.8m);
			dr.Add(1, 1, -15m);
			dr.Add(1, 1, -17.2m);
			dr.Add(1, 1, -10m);

			dr.Add(2, 2, 11.2m);
			dr.Add(2, 2, -13.1m);
			dr.Add(2, 2, 9.8m);
			dr.Add(2, 2, -15m);
			dr.Add(2, 2, -17.4m);
			dr.Add(2, 2, -10m);
			Assert.AreEqual(-17.4m, dr.GetMin());
			Assert.AreEqual(11.5m, dr.GetMax());
		}

		[Test]
		public void MultiSeria_Positive_And_Negative_Two_Seria_Four_Rows()
		{
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.Linear;
			dr.Add(1, 1, 11.5m);
			dr.Add(1, 1, -13);
			dr.Add(1, 1, 9.8m);
			dr.Add(1, 2, -15m);
			dr.Add(1, 2, -17.2m);
			dr.Add(1, 2, -10m);

			dr.Add(2, 3, 11.2m);
			dr.Add(2, 3, -13.1m);
			dr.Add(2, 3, 9.8m);
			dr.Add(2, 4, -15m);
			dr.Add(2, 4, -17.4m);
			dr.Add(2, 4, -10m);
			Assert.AreEqual(-17.4m, dr.GetMin());
			Assert.AreEqual(11.5m, dr.GetMax());
		}

		[Test]
		public void Simple_Stacked_One_Seria_Two_Rows_Only_Pos() {
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.Stacked;
			dr.Add(1, 1, 1);
			dr.Add(1, 1, 2);
			dr.Add(1, 1, 3);
			dr.Add(1, 1, 4);
			dr.Add(1, 2, 2);
			dr.Add(1, 2, 5);
			dr.Add(1, 2, 6);
			dr.Add(1, 2, 8);
			Assert.AreEqual(3m, dr.GetMin());
			Assert.AreEqual(12m, dr.GetMax());
		}


		[Test]
		public void Simple_Stacked_One_Seria_Three_Rows_With_Negative()
		{
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.Stacked;
			dr.Add(1, 1, 1);
			dr.Add(1, 1, 2);
			dr.Add(1, 1, 3);
			dr.Add(1, 1, 4);

			dr.Add(1, 2, -2);
			dr.Add(1, 2, 5);
			dr.Add(1, 2, -6);
			dr.Add(1, 2, -1);

			dr.Add(1, 3, 2);
			dr.Add(1, 3, -5);
			dr.Add(1, 3, 6);
			dr.Add(1, 3, -8);
			Assert.AreEqual(-9m, dr.GetMin());
			Assert.AreEqual(9m, dr.GetMax());
		}

		[Test]
		public void Simple_Stacked_Two_Seria_Two_Rows_Only_Pos()
		{
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.Stacked;
			dr.Add(1, 1, 1);
			dr.Add(1, 1, 2);
			dr.Add(1, 1, 3);
			dr.Add(1, 1, 4);
			dr.Add(2, 2, 2);
			dr.Add(2, 2, 5);
			dr.Add(2, 2, 6);
			dr.Add(2, 2, 8);
			Assert.AreEqual(3m, dr.GetMin());
			Assert.AreEqual(12m, dr.GetMax());
		}


	


		[Test]
		public void MultiSeria_Stacked_Only_Pos()
		{
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.MultiSeriaStacked;
			dr.Add(1, 1, 1);
			dr.Add(1, 1, 2);
			dr.Add(1, 2, 3);
			dr.Add(1, 2, 4);
			dr.Add(2, 3, 2);
			dr.Add(2, 3, 5);
			dr.Add(2, 4, 6);
			dr.Add(2, 4, 8);
			Assert.AreEqual(4m, dr.GetMin());
			Assert.AreEqual(13m, dr.GetMax());
		}

		[Test]
		public void MultiSeria_Stacked_Neg_And_Pos()
		{
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.MultiSeriaStacked;
			dr.Add(1, 1, 1);
			dr.Add(1, 1, 2);
			dr.Add(1, 2, -3);
			dr.Add(1, 2, 4);
			dr.Add(2, 3, 2);
			dr.Add(2, 3, 5);
			dr.Add(2, 4, 7);
			dr.Add(2, 4, -8);
			Assert.AreEqual(-8m, dr.GetMin());
			Assert.AreEqual(9m, dr.GetMax());
		}

		[Test]
		public void MultiSeria_Stacked_Neg_And_Pos_Minimal_1()
		{
			var dr = new BrickDataSet();
			dr.Preferences.SeriaCalcMode = SeriaCalcMode.MultiSeriaStacked;
			dr.Add(1, 1, 1);
			dr.Add(2, 2, -3);
			
			Assert.AreEqual(-3m, dr.GetMin());
			Assert.AreEqual(1m, dr.GetMax());
		}
	}
}