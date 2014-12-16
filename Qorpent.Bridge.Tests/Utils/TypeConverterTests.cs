#undef SYSCONVERTTEST
#undef OLDSYS
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils;
#if OLDSYS
using Qorpent.Utils.Extensions;
#endif
namespace Qorpent.Bridge.Tests.Utils
{
    [TestFixture]
    public class TypeConverterTests {

        [TestCase(false, 0)]
        [TestCase(true, 1)]
        [TestCase(123, 123)]
        [TestCase(123d, 123)]
        [TestCase(123.4d, 123)]
        [TestCase(123.6d, 124)]
        [TestCase(long.MaxValue, 0)]
        [TestCase(long.MinValue, 0)]
        [TestCase(123L, 123)]
        [TestCase("123", 123)]
        [TestCase("-123", -123)]
        [TestCase(" - 12 3 ", -123)]
        [TestCase(" + 12 3 ", 123)]
        [TestCase(" ++ 12 3 ", 0)]
        [TestCase(" 13,13 ", 13)]
        [TestCase(" 13,13,12 ", 131312)]
        [TestCase(" 13,13.32 ", 1313)]
        [TestCase(" 13,13.92 ", 1314)]
        [TestCase(" 13.92 ", 14)]
        [TestCase(StringSplitOptions.RemoveEmptyEntries,1)]
        public void BaseToIntResolving(object source, int result) {
            Assert.AreEqual(result,TypeConverter.ToInt(source));
        }

        [TestCase(long.MinValue)]
        [TestCase(long.MaxValue)]
        [TestCase("dt")]
        [TestCase("obj")]
        [TestCase("++10")]
        [TestCase("+-10")]
        [TestCase("10.232,22")]
        [TestCase("10.232.22")]
        public void UnsafeMode(object source) {
            if (source == "dt") source = DateTime.MinValue;
            if(source=="obj")source = new object();
            Assert.Throws<ArgumentException>(()=>TypeConverter.ToInt(source, safe: false));
        }
     

        [TestCase("1970-02-01", 2678400)]
        [TestCase("1890-02-01",0)]
        [TestCase("2010-02-01", 1264982400)]
        [TestCase("2044-02-01",0)]
        public void DateTimeToIntResolving(string date, int result) {
            var dt = DateTime.Parse(date);
           Assert.AreEqual(result,TypeConverter.ToInt(dt));
        }

        private const int timestampSize = 10000000;
        readonly static DateTime UnixTime = new DateTime(1970, 1, 1);
        [Explicit]
        [Test]
        public void Timestamp() {
            var dataToTest = new object[timestampSize];
            for (var i = 0; i < timestampSize; i++) {
                #if !SYSCONVERTTEST
                if (i%20 == 0) { //every 10 - datetime
                    dataToTest[i] = UnixTime.AddSeconds(i);
                }

                else 
                #endif
                if (i%19 == 0) {
                    dataToTest[i] = (ulong) i;
                }
                else if (i % 18 == 0)
                {
                    dataToTest[i] = (long)i;
                }
                else if (i % 17 == 0 && i<short.MaxValue)
                {
                    dataToTest[i] = (short)i;
                }
                else if (i % 16 == 0) {
                    dataToTest[i] = false;
                }
                else if (i % 15 == 0)
                {
                    dataToTest[i] = true;
                }
                else if (i % 14 == 0)
                {
                    dataToTest[i] = (double)i;
                }
#if !SYSCONVERTTEST
                else if (i % 13 == 0)
                {
                    dataToTest[i] = i+"."+i;
                }

                else if (i % 12 == 0)
                {
                    dataToTest[i] = "-"+10+","+i + " . " + i;
                }

                else if (i % 11 == 0)
                {
                    dataToTest[i] = new object();
                }
                else if (i % 10 == 0) {
                    dataToTest[i] = null;
                }
#endif
                else if (i % 9 == 0)
                {
                    dataToTest[i] =ConsoleColor.Red;
                }
                else if (i%8 > 4) {
                    dataToTest[i] = i.ToString(CultureInfo.InvariantCulture);
                }
                else {
                    dataToTest[i] = i;
                }
            }
            Console.WriteLine("ToInt probes count: " + timestampSize);
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < timestampSize; i++) {
                var result = TypeConverter.ToInt(dataToTest[i]);
            }
            sw.Stop();
            Console.WriteLine("Serial BRIDGE: "+sw.ElapsedMilliseconds);

            sw = Stopwatch.StartNew();
            dataToTest.AsParallel().ForAll(_ => {
                var result = TypeConverter.ToInt(_);
            });
            sw.Stop();
            Console.WriteLine("Parallel BRIDGE: " + sw.ElapsedMilliseconds);

#if SYSCONVERTTEST
            sw = Stopwatch.StartNew();
            for (var i = 0; i < timestampSize; i++) {
                var result = Convert.ToInt32(dataToTest[i]);
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
#endif
            
            
#if OLDSYS
             sw = Stopwatch.StartNew();
            for (var i = 0; i < timestampSize; i++) {
                var result = CoreExtensions.ToInt(dataToTest[i]);
            }
            sw.Stop();
            Console.WriteLine("Serial OLDSYS: " + sw.ElapsedMilliseconds);

            sw = Stopwatch.StartNew();
            dataToTest.AsParallel().ForAll(_ =>
            {
                var result = CoreExtensions.ToInt(_);
            });
            sw.Stop();
            Console.WriteLine("Parallel OLDSYS: " + sw.ElapsedMilliseconds);
#endif
        }
    }
}
