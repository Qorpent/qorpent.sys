using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Charts;
using Qorpent.Utils.BrickScaleNormalizer;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests.BrickScale {
	[Explicit]
	[TestFixture]
	public class DataColonBestVariantHelperTests {
		/// <summary>
		///		Максимальное значение по шкале
		/// </summary>
		const decimal ScaleMax = 100;
		/// <summary>
		///		Минимальное значение по шкале
		/// </summary>
		const decimal ScaleMin = 0;
		/// <summary>
		///		Высота шкалы
		/// </summary>
		const decimal Height = 100;
		/// <summary>
		///		Порядок расположения данных в шкале
		/// </summary>
		const ColonDataItemOrder Order = ColonDataItemOrder.Real;
		/// <summary>
		///		Высота лычки
		/// </summary>
		private const decimal LabelHeight = 27;
		/// <summary>
		///		Эземпляр хэлпера
		/// </summary>
		private DataColonLabelHelper _colonLabelHelper;
		[SetUp]
		public void SetUp() {
			_colonLabelHelper = new DataColonLabelHelper {
				Height = Height,
				LabelHeight = LabelHeight,
				Order = Order,
				ScaleMax = ScaleMax,
				ScaleMin = ScaleMin
			};
		}
		[TestCase(new[] {52, 48}, new[] {LabelPosition.Above, LabelPosition.Below}, new int[] {})]
		[TestCase(new[] {52, 48, 0}, new[] {LabelPosition.Above, LabelPosition.Below, LabelPosition.Hidden}, new[] {2})]
		public void IsCorrectDecision(decimal[] input, LabelPosition[] expected, int[] hideIndexes) {
			var index = 0;
			foreach (var value in input) {
				var dataItem = new DataItem {Value = value};
				dataItem.NormalizedValue = BrickDataSetHelper.GetNormalizedValue(ScaleMin, ScaleMax, Height, dataItem.Value);
				_colonLabelHelper.Add(dataItem);
				if (index.IsIn(hideIndexes)) {
					dataItem.Hide = true;
				}
				index++;
			}
			_colonLabelHelper.EnsureBestLabels();
			var orderedPositions = _colonLabelHelper.GetOrderedItems().ToArray();
			Assert.AreEqual(orderedPositions.Length, expected.Length);
			for (var i = 0; i < expected.Length; i++) {
				Assert.AreEqual(expected[i], orderedPositions[i]);
			}
		}
	}
	[TestFixture(Description = "Фикстура для тестирования корректности получениея всех возможных вариантов лычек")]
	public class DataColonGetPossibleVariantsTests {
		/// <summary>
		///		Эземпляр хэлпера
		/// </summary>
		private DataColonLabelHelper _colonLabelHelper;
		/// <summary>
		///		Установка теста
		/// </summary>
		[SetUp]
		public void SetUp() {
			_colonLabelHelper = new DataColonLabelHelper();
		}
		[TestCase(new[] {10, 20, 30}, 27, 0)]
		[TestCase(new[] {10, 20, -1}, 27, 1)]
		[TestCase(new[] {10, -1}, 9, 1)]
		public void IsCorrectRotation(int[] values, int expectedCount, int hiddenInEachItemExpectedCount) {
			foreach (var value in values) {
				var dataItem = new DataItem();
				if (value < 0) {
					dataItem.Hide = true;
				}
				_colonLabelHelper.Add(dataItem);
			}
			var rotation = _colonLabelHelper.GetPossibleVariants().ToArray();
			Assert.AreEqual(expectedCount, rotation.Length);
			Assert.IsTrue(rotation.All(_ => _.Count(__ => __ == LabelPosition.Hidden) == hiddenInEachItemExpectedCount));
		}
	}
	[TestFixture]
	public class DataColonLabelHelperOrderTests {
		/// <summary>
		///		Эземпляр хэлпера
		/// </summary>
		private DataColonLabelHelper _colonLabelHelper;
		[SetUp]
		public void SetUp() {
			_colonLabelHelper = new DataColonLabelHelper {
				new DataItem {Value = 50},
				new DataItem {Value = 40},
				new DataItem {Value = 70}
			};
		}
		[Test]
		public void IsCorrectAsSuppliedOrder() {
			_colonLabelHelper.Order = ColonDataItemOrder.AsSupplied;
			var ordered = _colonLabelHelper.GetOrderedItems().ToArray();
			Assert.AreEqual(3, ordered.Length);
			Assert.AreEqual(50, ordered[0].Value);
			Assert.AreEqual(40, ordered[1].Value);
			Assert.AreEqual(70, ordered[2].Value);
		}
		[Test]
		public void IsCorrectInvertedOrder() {
			_colonLabelHelper.Order = ColonDataItemOrder.Inverted;
			var ordered = _colonLabelHelper.GetOrderedItems().ToArray();
			Assert.AreEqual(3, ordered.Length);
			Assert.AreEqual(70, ordered[0].Value);
			Assert.AreEqual(50, ordered[1].Value);
			Assert.AreEqual(40, ordered[2].Value);
		}
		[Test]
		public void IsCorrectRealOrder() {
			_colonLabelHelper.Order = ColonDataItemOrder.Real;
			var ordered = _colonLabelHelper.GetOrderedItems().ToArray();
			Assert.AreEqual(3, ordered.Length);
			Assert.AreEqual(40, ordered[0].Value);
			Assert.AreEqual(50, ordered[1].Value);
			Assert.AreEqual(70, ordered[2].Value);
		}
	}





















    /// <summary>
    ///     Фикстура, тестирующая разнесение лэйблов в графике
    /// </summary>
    [TestFixture]
    public class BrickDataSetLabelNormalizationTests : BrickDataSetTestBase {
        [Test]
        public void RealCase1Test() {
            var brick = ChartBuilder.ParseDatasets("3918,3602,4009;3568,3509,3771;3531,3492,3898;3198,3840,4122;3365,3345,3863;4100,3840,4122").ToBrickDataset();
            brick.Preferences = new UserPreferences {Height = 100, SeriaCalcMode = SeriaCalcMode.Linear};
            brick.Calculate();
            var colons = brick.GetColons();
        }

        [Test]
        public void CanHideLabelWhenTwoLabelsDiffersAboutOnePixel() {
            var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 200);
            ds.Add(1, 1, 10);
            ds.Add(2, 1, 10);
            ds.Calculate();
            Assert.AreEqual(10, ds.Rows.ToArray()[0].Items[0].Value);
            Assert.AreEqual(10, ds.Rows.ToArray()[1].Items[0].Value);
            Assert.IsTrue(
                ((ds.Rows.ToArray()[0].Items[0].LabelPosition.HasFlag(LabelPosition.Hidden) && !ds.Rows.ToArray()[1].Items[0].LabelPosition.HasFlag(LabelPosition.Hidden)))
                    ||
                ((ds.Rows.ToArray()[1].Items[0].LabelPosition.HasFlag(LabelPosition.Hidden) && !ds.Rows.ToArray()[0].Items[0].LabelPosition.HasFlag(LabelPosition.Hidden)))
            );
        }
        [Test]
        public void SimpleLabelNormalizationTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 1, 50);
            ds.Add(1, 1, 40);
            ds.Add(2, 1, 50);
            ds.Add(2, 1, 55);
            ds.Add(2, 1, 41);
            ds.Calculate();
            Console.WriteLine(ds.Rows.ToArray()[0].Items[0].LabelPosition + ", " + ds.Rows.ToArray()[1].Items[0].LabelPosition);
            Console.WriteLine(ds.Rows.ToArray()[0].Items[1].LabelPosition + ", " + ds.Rows.ToArray()[1].Items[1].LabelPosition);
            Console.WriteLine(ds.Rows.ToArray()[0].Items[2].LabelPosition + ", " + ds.Rows.ToArray()[1].Items[2].LabelPosition);
            Assert.AreEqual(10, ds.Rows.ToArray()[0].Items[0].Value);
            Assert.AreEqual(50, ds.Rows.ToArray()[0].Items[1].Value);
            Assert.AreEqual(40, ds.Rows.ToArray()[0].Items[2].Value);
            Assert.AreEqual(50, ds.Rows.ToArray()[1].Items[0].Value);
            Assert.AreEqual(55, ds.Rows.ToArray()[1].Items[1].Value);
            Assert.AreEqual(41, ds.Rows.ToArray()[1].Items[2].Value);
            // Первая пара слишком далеко друг от друга, так что они Auto
            Assert.AreEqual(LabelPosition.Auto, ds.Rows.ToArray()[0].Items[0].LabelPosition);
            Assert.AreEqual(LabelPosition.Auto, ds.Rows.ToArray()[1].Items[0].LabelPosition);
            // Вторая пара уже близко, так что лэйбел меньшего значения снизу, большего — вверху
            Assert.AreEqual(LabelPosition.Below, ds.Rows.ToArray()[0].Items[1].LabelPosition);
            Assert.AreEqual(LabelPosition.Above, ds.Rows.ToArray()[1].Items[1].LabelPosition);
            // Вначале я думал, что алгоритм должен отключить один из лэйблов, но он смог развести, так что правлю тест
            Assert.AreEqual(LabelPosition.Below, ds.Rows.ToArray()[0].Items[2].LabelPosition);
            Assert.AreEqual(LabelPosition.Above, ds.Rows.ToArray()[1].Items[2].LabelPosition);
        }
        /// <summary>
        ///     Это реально лютый тест. По моим представлениям, отработал даже лучше, чем я предполагал.
        /// </summary>
        [Test]
        public void HardLabelNormalizationTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 2, 15);
            ds.Add(1, 1, 20);
            ds.Add(1, 2, 25);
            ds.Add(1, 1, 30);
            ds.Add(1, 2, 35);
            ds.Add(2, 1, 20);
            ds.Add(2, 2, 150);
            ds.Add(2, 1, 70);
            ds.Add(2, 2, 250);
            ds.Add(2, 1, 300);
            ds.Add(2, 2, 350);
            ds.Calculate();
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(LabelPosition.Auto, colons[0].Items[0].LabelPosition);
            Assert.AreEqual(LabelPosition.Below, colons[0].Items[1].LabelPosition);
            Assert.AreEqual(LabelPosition.Hidden, colons[0].Items[2].LabelPosition);
            Assert.AreEqual(LabelPosition.Auto, colons[0].Items[3].LabelPosition);
            Assert.AreEqual(LabelPosition.Below, colons[1].Items[0].LabelPosition);
            Assert.AreEqual(LabelPosition.Above, colons[1].Items[1].LabelPosition);
            Assert.AreEqual(LabelPosition.Above, colons[1].Items[2].LabelPosition);
            Assert.AreEqual(LabelPosition.Auto, colons[1].Items[3].LabelPosition);
            Assert.AreEqual(LabelPosition.Below, colons[2].Items[0].LabelPosition);
            Assert.AreEqual(LabelPosition.Above, colons[2].Items[1].LabelPosition);
            Assert.AreEqual(LabelPosition.Auto, colons[2].Items[2].LabelPosition);
            Assert.AreEqual(LabelPosition.Above, colons[2].Items[3].LabelPosition);
        }
        /// <summary>
        ///     Тест выражает правильность сбора колонок в односерийном простом графике типа «линия»
        /// </summary>
        [Test]
        public void SimpleChartBuildColonsTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.Linear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 1, 20);
            ds.Add(1, 1, 30);
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(3, colons.Length);
            Assert.AreEqual(1, colons[0].Items.Length);
            Assert.AreEqual(1, colons[1].Items.Length);
            Assert.AreEqual(1, colons[2].Items.Length);
            Assert.AreEqual(10, colons[0].Items[0].Value);
            Assert.AreEqual(20, colons[1].Items[0].Value);
            Assert.AreEqual(30, colons[2].Items[0].Value);
        }
        /// <summary>
        ///     Тест выражает то, что в графике с двумя сериями (по три значения в каждой) соберётся три колонки с двумя <see cref="DataItem"/>
        ///     внутри и правильном порядке
        /// </summary>
        [Test]
        public void MultiLineChartBuildColonsTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 1, 20);
            ds.Add(1, 1, 30);
            ds.Add(2, 1, 100);
            ds.Add(2, 1, 200);
            ds.Add(2, 1, 300);
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(3, colons.Length);
            Assert.AreEqual(2, colons[0].Items.Length);
            Assert.AreEqual(2, colons[1].Items.Length);
            Assert.AreEqual(2, colons[2].Items.Length);
            Assert.AreEqual(10, colons[0].Items[0].Value);
            Assert.AreEqual(20, colons[1].Items[0].Value);
            Assert.AreEqual(30, colons[2].Items[0].Value);
            Assert.AreEqual(100, colons[0].Items[1].Value);
            Assert.AreEqual(200, colons[1].Items[1].Value);
            Assert.AreEqual(300, colons[2].Items[1].Value);
        }
        /// <summary>
        ///     Тест выражает то, что в мультисерийном графике с 2 рядами соберутся 3 колонки с четырьмя значениями в каждой:
        ///     1с1р,1с2р,2с1р,2с2р (с — серия, р — ряд)
        /// </summary>
        [Test]
        public void MiltilineChartWithRowsBuildColonsTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 200);
            ds.Add(1, 1, 10);
            ds.Add(1, 2, 15);
            ds.Add(1, 1, 20);
            ds.Add(1, 2, 25);
            ds.Add(1, 1, 30);
            ds.Add(1, 2, 35);
            ds.Add(2, 1, 100);
            ds.Add(2, 2, 150);
            ds.Add(2, 1, 200);
            ds.Add(2, 2, 250);
            ds.Add(2, 1, 300);
            ds.Add(2, 2, 350);
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(3, colons.Length);
            Assert.AreEqual(4, colons[0].Items.Length);
            Assert.AreEqual(4, colons[1].Items.Length);
            Assert.AreEqual(4, colons[2].Items.Length);
            Assert.AreEqual(10, colons[0].Items[0].Value);
            Assert.AreEqual(20, colons[1].Items[0].Value);
            Assert.AreEqual(30, colons[2].Items[0].Value);
            Assert.AreEqual(15, colons[0].Items[1].Value);
            Assert.AreEqual(25, colons[1].Items[1].Value);
            Assert.AreEqual(35, colons[2].Items[1].Value);
            Assert.AreEqual(100, colons[0].Items[2].Value);
            Assert.AreEqual(200, colons[1].Items[2].Value);
            Assert.AreEqual(300, colons[2].Items[2].Value);
            Assert.AreEqual(150, colons[0].Items[3].Value);
            Assert.AreEqual(250, colons[1].Items[3].Value);
            Assert.AreEqual(350, colons[2].Items[3].Value);
        }
        /// <summary>
        ///     Тест выражает возможность собрать колонки по графику с двумя Y-ками
        /// </summary>
        [Test]
        public void SingleLineTwoScaleBuildColonsTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.Linear, 200);
            ds.Add(1, 1, 10, false);
            ds.Add(1, 1, 20, false);
            ds.Add(1, 1, 15, true);
            ds.Add(1, 1, 25, true);
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(2, colons.Length);
            Assert.AreEqual(2, colons[0].Items.Length);
            Assert.AreEqual(2, colons[1].Items.Length);
            Assert.AreEqual(10, colons[0].Items[0].Value);
            Assert.AreEqual(20, colons[1].Items[0].Value);
            Assert.AreEqual(15, colons[0].Items[1].Value);
            Assert.AreEqual(25, colons[1].Items[1].Value);
        }
        /// <summary>
        ///     Тест выражает то, что при работе с двумя осями полностью поддежривается логика работы с Row
        /// </summary>
        [Test]
        public void SingleLineTwoScaleWithRowsBuildColonsTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.Linear, 200);
            ds.Add(1, 1, 10, false);
            ds.Add(1, 2, 11, false);
            ds.Add(1, 1, 20, false);
            ds.Add(1, 2, 21, false);
            ds.Add(1, 1, 15, true);
            ds.Add(1, 2, 16, true);
            ds.Add(1, 1, 25, true);
            ds.Add(1, 2, 26, true);
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(2, colons.Length);
            Assert.AreEqual(4, colons[0].Items.Length);
            Assert.AreEqual(4, colons[1].Items.Length);
            Assert.AreEqual(10, colons[0].Items[0].Value);
            Assert.AreEqual(20, colons[1].Items[0].Value);
            Assert.AreEqual(11, colons[0].Items[1].Value);
            Assert.AreEqual(21, colons[1].Items[1].Value);
            Assert.AreEqual(15, colons[0].Items[2].Value);
            Assert.AreEqual(25, colons[1].Items[2].Value);
            Assert.AreEqual(16, colons[0].Items[3].Value);
            Assert.AreEqual(26, colons[1].Items[3].Value);
        }
        /// <summary>
        ///     Тест показывает то, что поддерживается две оси с множеством серий и рядов
        /// </summary>
        [Test]
        public void MultiLineTwoScaleWithRowsBuildColonsTest() {
            var ds = GetEmptyDataSet(SeriaCalcMode.Linear, 200);
            ds.Add(1, 1, 10, false);
            ds.Add(1, 2, 11, false);
            ds.Add(1, 1, 20, false);
            ds.Add(1, 2, 21, false);
            ds.Add(2, 1, 12, false);
            ds.Add(2, 2, 13, false);
            ds.Add(2, 1, 22, false);
            ds.Add(2, 2, 23, false);
            ds.Add(1, 1, 15, true);
            ds.Add(1, 2, 16, true);
            ds.Add(1, 1, 25, true);
            ds.Add(1, 2, 26, true);
            ds.Add(2, 1, 17, true);
            ds.Add(2, 2, 18, true); 
            ds.Add(2, 1, 27, true);
            ds.Add(2, 2, 28, true);
            var colons = ds.BuildColons().ToArray();
            Assert.AreEqual(2, colons.Length);
            Assert.AreEqual(8, colons[0].Items.Length);
            Assert.AreEqual(8, colons[1].Items.Length);
            Assert.AreEqual(10, colons[0].Items[0].Value);
            Assert.AreEqual(20, colons[1].Items[0].Value);
            Assert.AreEqual(11, colons[0].Items[1].Value);
            Assert.AreEqual(21, colons[1].Items[1].Value);
            Assert.AreEqual(12, colons[0].Items[2].Value);
            Assert.AreEqual(22, colons[1].Items[2].Value);
            Assert.AreEqual(13, colons[0].Items[3].Value);
            Assert.AreEqual(23, colons[1].Items[3].Value);
            Assert.AreEqual(15, colons[0].Items[4].Value);
            Assert.AreEqual(25, colons[1].Items[4].Value);
            Assert.AreEqual(16, colons[0].Items[5].Value);
            Assert.AreEqual(26, colons[1].Items[5].Value);
            Assert.AreEqual(17, colons[0].Items[6].Value);
            Assert.AreEqual(27, colons[1].Items[6].Value);
            Assert.AreEqual(18, colons[0].Items[7].Value);
            Assert.AreEqual(28, colons[1].Items[7].Value);
        }
    }
	[Explicit]
	public class ProductionTests : BrickDataSetTestBase {
		/// <summary>
		///		Версия графика ПРОДАЖИ -> Динамика цен и себестоимость -> Цена и себестоимость реализации золото в слитках по данным на 03.04.2014 16:00
		///		на базе ecot
		/// </summary>
		public BrickDataSet EcotSalesSebestAgBdsVersion {
			get {
				var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 325);
				ds.Add(0, 0, 1600000);
				ds.Add(0, 0, 1609995);
				ds.Add(0, 0, 1407646);
				ds.Add(0, 0, 1408383);
				ds.Add(0, 0, 1300002);
				ds.Add(0, 0, 1420464);
				ds.Add(0, 0, 1200000);
				ds.Add(1, 0, 1557686);
				ds.Add(1, 0, 1688037);
				ds.Add(1, 0, 1713031);
				ds.Add(1, 0, 1555403);
				ds.Add(1, 0, 1115985);
				ds.Add(1, 0, 1496643);
				ds.Add(1, 0, 1184710);
				ds.Add(2, 0, 1387940);
				ds.Add(2, 0, 1506492);
				ds.Add(2, 0, 1542788);
				ds.Add(2, 0, 1391386);
				ds.Add(2, 0, 976984);
				ds.Add(2, 0, 1334491);
				ds.Add(2, 0, 1011155);
				return ds;
			}
		}
		/// <summary>
		///		Версия графика ПРОДАЖИ -> Динамика цен и себестоимость -> Цена и себестоимость реализации золото в слитках по данным на 03.04.2014 16:00
		///		на базе zdev
		/// </summary>
		public BrickDataSet ZdevSalesSebestAgBdsVersion {
			get {
				var ds = GetEmptyDataSet(SeriaCalcMode.SeriaLinear, 325);
				ds.Add(0, 0, 1600000);
				ds.Add(0, 0, 1609995);
				ds.Add(0, 0, 1407646);
				ds.Add(0, 0, 1408383);
				ds.Add(0, 0, 1300002);
				ds.Add(0, 0, 1420464);
				ds.Add(0, 0, 1200000);
				ds.Add(0, 1, 1557686);
				ds.Add(0, 1, 1688037);
				ds.Add(0, 1, 1713031);
				ds.Add(0, 1, 1555403);
				ds.Add(0, 1, 1115985);
				ds.Add(0, 1, 1496643);
				ds.Add(0, 1, 1184710);
				ds.Add(0, 2, 1387940);
				ds.Add(0, 2, 1506492);
				ds.Add(0, 2, 1542788);
				ds.Add(0, 2, 1391386);
				ds.Add(0, 2, 976984);
				ds.Add(0, 2, 1334491);
				ds.Add(0, 2, 1011155);
				return ds;
			}
		}
		[TestCase("ecot", "0")]
		[TestCase("zdev", "0")]
		[TestCase("ecot", "auto")]
		[TestCase("zdev", "auto")]
		public void SebestAgSlitkiSeriaAsDataset(string versionCode, string ymin) {
			BrickDataSet ds;
			if (versionCode == "ecot") {
				ds = EcotSalesSebestAgBdsVersion;
			} else if (versionCode == "zdev") {
				ds = ZdevSalesSebestAgBdsVersion;
			} else {
				throw new NotSupportedException();
			}
			ds.Preferences.Height = 325;
			ds.Preferences.YMin = ymin;
			ds.Calculate();
			var colons = ds.BuildColons().ToArray();

			Console.WriteLine("Прочая информация о датасете:");
			Console.WriteLine("LabelHeight: {0}", ds.LabelHeight);
			ds.PrintPreferences();
			

			colons[0].AsserIsCorrectLabelPosition(1600000, LabelPosition.Above);
			colons[0].AsserIsCorrectLabelPosition(1557686, LabelPosition.Below);
			colons[0].AsserIsCorrectLabelPosition(1387940, LabelPosition.Below);


			colons[1].AsserIsCorrectLabelPosition(1609995, LabelPosition.Below);
			colons[1].AsserIsCorrectLabelPosition(1688037, LabelPosition.Above);
			colons[1].AsserIsCorrectLabelPosition(1506492, LabelPosition.Below);

			
			colons[2].AsserIsCorrectLabelPosition(1407646, LabelPosition.Auto);
			colons[2].AsserIsCorrectLabelPosition(1713031, LabelPosition.Above);
			colons[2].AsserIsCorrectLabelPosition(1542788, LabelPosition.Above);


			colons[3].AsserIsCorrectLabelPosition(1408383, LabelPosition.Above);
			colons[3].AsserIsCorrectLabelPosition(1555403, LabelPosition.Above);
			colons[3].AsserIsCorrectLabelPosition(1391386, LabelPosition.Below);


			colons[4].AsserIsCorrectLabelPosition(1300002, LabelPosition.Auto);
			colons[4].AsserIsCorrectLabelPosition(1115985, LabelPosition.Below);
			colons[4].AsserIsCorrectLabelPosition(976984, LabelPosition.Below);


			colons[5].AsserIsCorrectLabelPosition(1420464, LabelPosition.Below);
			colons[5].AsserIsCorrectLabelPosition(1496643, LabelPosition.Above);
			colons[5].AsserIsCorrectLabelPosition(1334491, LabelPosition.Below);


			colons[6].AsserIsCorrectLabelPosition(1200000, LabelPosition.Above);
			colons[6].AsserIsCorrectLabelPosition(1184710, LabelPosition.Below);
			colons[6].AsserIsCorrectLabelPosition(1011155, LabelPosition.Below);
		}
		[Test]
		public void SebestAgSlitkiSingleColonTest() {
			var colon = new DataItemColon {Min = 800000, Max = 1800000, Height = 175};
			colon.Add(1609995);
			colon.Add(1688037);
			colon.Add(1506492);
			colon.MinimizeTemperature();
			colon.PrintColon();
			colon.AsserIsCorrectLabelPosition(1506492, LabelPosition.Below);
			colon.AsserIsCorrectLabelPosition(1688037, LabelPosition.Above);
			colon.AsserIsCorrectLabelPosition(1609995, LabelPosition.Auto);
		}
	}

	internal static class BrickDatasetTestBaseHelper {
		public static void AsserIsCorrectLabelPosition(this DataItemColon colon, decimal value, LabelPosition labelPosition) {
			var dataItem = colon.FirstOrDefault(_ => _.Value == value);
			Assert.IsNotNull(dataItem);
			Assert.AreEqual(labelPosition, dataItem.LabelPosition);
		 }
		 public static string DescribeColon(this DataItemColon colon) {
			 var result = string.Empty;
			 foreach (var item in colon) {
				 result += string.Format("Value: {0}, LabelPosition: {1}\n", item.Value, item.LabelPosition);
			 }
			 return result;
		 }
		 public static void PrintColon(this DataItemColon colon) {
			Console.WriteLine("Min: {0}, Max: {1}", colon.Min, colon.Max);
			 Console.WriteLine(colon.DescribeColon());
		 }
	}
    public class BrickDataSetTestBase {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="calcMode"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected BrickDataSet GetEmptyDataSet(SeriaCalcMode calcMode, int height) {
            return new BrickDataSet {
                Preferences = new UserPreferences {
                    SeriaCalcMode = calcMode,
                    Height = height,
                }
            };
        }
    }
}