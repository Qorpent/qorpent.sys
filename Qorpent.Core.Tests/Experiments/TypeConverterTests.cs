#undef SYSCONVERTTEST
//#undef OLDSYS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils;
using System.Linq.Expressions;
using Qorpent.Experiments.Utils;

#if OLDSYS
using Qorpent.Utils.Extensions;
#endif
namespace Qorpent.Bridge.Tests.Utils
{
    [TestFixture]
    public class TypeConverterTests {


        [TestCase((long)12, 12)]
        
        [TestCase(((long)int.MaxValue) + 1, 0)]
        [TestCase((uint)12, 12)]
        [TestCase(((uint)int.MaxValue) + 1, 0)]
        [TestCase((ulong)12, 12)]
        [TestCase(((ulong)int.MaxValue)+1, 0)]
        [TestCase((float)12, 12)]
        [TestCase(float.MaxValue , 0)]
        [TestCase(((double)int.MaxValue) + 1, 0)]
        [TestCase("", 0)]
        [TestCase(" ", 0)]
        [TestCase("\t", 0)]
        [TestCase(false, 0)]
        [TestCase(null, 0)]
        [TestCase("-101",-101)]
        [TestCase("-1.01",-1)]
        [TestCase("-1,101.01",-1101)]
        [TestCase(true, 1)]
        [TestCase(123, 123)]
        [TestCase((byte)123, 123)]
        [TestCase((short)123, 123)]
        [TestCase((ushort)123, 123)]
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

        [Test]
        public void DecimalsNotRounded() {
            Assert.AreEqual(52.45m,TypeConverter.ToDecimal("52.45"));
        }

        [Test]
        public void ValidZeroParsing() {
            Assert.AreEqual(303,TypeConverter.ToInt("303"));
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


        [TestCase(2678400,"1970-02-01")]
        [TestCase("1970-02-01", "1970-02-01")]
        [TestCase(1264982400, "2010-02-01")]
        [TestCase(0, "1900-01-01",Description = "Это точка неоднозначного преобразования, старайтесь избегать преобразований число-дата и обратно")]
        public void DateTimeConversion(object date, string result)
        {
            Assert.AreEqual(DateTime.ParseExact(result,new []{"yyyy-MM-dd HH:mm:ss","yyyy-MM-dd"},CultureInfo.InvariantCulture,DateTimeStyles.AllowWhiteSpaces), TypeConverter.ToDate(date));
        }

        private const int timestampSize = 1000000;
        readonly static DateTime UnixTime = new DateTime(1970, 1, 1);
        [Explicit]
        [Test]
        public void ToIntTimestamp() {
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

        [Explicit]
        [Test]
        public void ToDecimalTimestamp()
        {
            var dataToTest = new object[timestampSize];
            for (var i = 0; i < timestampSize; i++)
            {
#if !SYSCONVERTTEST
                if (i % 20 == 0)
                { //every 10 - datetime
                    dataToTest[i] = UnixTime.AddSeconds(i);
                }

                else
#endif
                    if (i % 19 == 0)
                    {
                        dataToTest[i] = (ulong)i;
                    }
                    else if (i % 18 == 0)
                    {
                        dataToTest[i] = (long)i;
                    }
                    else if (i % 17 == 0 && i < short.MaxValue)
                    {
                        dataToTest[i] = (short)i;
                    }
                    else if (i % 16 == 0)
                    {
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
                        dataToTest[i] = i + "." + i;
                    }

                    else if (i % 12 == 0)
                    {
                        dataToTest[i] = "-" + 10 + "," + i + " . " + i;
                    }

                    else if (i % 11 == 0)
                    {
                        dataToTest[i] = new object();
                    }
                    else if (i % 10 == 0)
                    {
                        dataToTest[i] = null;
                    }
#endif
                    else if (i % 9 == 0)
                    {
                        dataToTest[i] = ConsoleColor.Red;
                    }
                    else if (i % 8 > 4)
                    {
                        dataToTest[i] = i.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        dataToTest[i] = i;
                    }
            }
            Console.WriteLine("ToInt probes count: " + timestampSize);
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < timestampSize; i++)
            {
                var result = TypeConverter.ToDecimal(dataToTest[i]);
            }
            sw.Stop();
            Console.WriteLine("Serial BRIDGE: " + sw.ElapsedMilliseconds);

            sw = Stopwatch.StartNew();
            dataToTest.AsParallel().ForAll(_ =>
            {
                var result = TypeConverter.ToDecimal(_);
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
            for (var i = 0; i < timestampSize; i++)
            {
                var result = CoreExtensions.ToDecimal(dataToTest[i],true);
            }
            sw.Stop();
            Console.WriteLine("Serial OLDSYS: " + sw.ElapsedMilliseconds);

            sw = Stopwatch.StartNew();
            dataToTest.AsParallel().ForAll(_ =>
            {
                var result = CoreExtensions.ToDecimal(_, true);
            });
            sw.Stop();
            Console.WriteLine("Parallel OLDSYS: " + sw.ElapsedMilliseconds);
#endif
        }

        [TestCase(1,true)]
        [TestCase(ConsoleColor.Red,true)]
        [TestCase((ConsoleColor)0,false)]
        [TestCase(1.1,true)]
        [TestCase(-1,true)]
        [TestCase(0,false)]
        [TestCase("False",false)]
        [TestCase("FALSE",false)]
        [TestCase("1",true)]
        [TestCase("",false)]
        [TestCase("   ",false)]
        [TestCase("xxx",true)]
        [TestCase("0",false)]
        public void BoolMainTest(object src, bool result) {
            Assert.AreEqual(result,TypeConverter.ToBool(src));
        }

#if OLDSYS
        [TestCase(1, true)]
        [TestCase(ConsoleColor.Red, true)]
        [TestCase((ConsoleColor)0, false)]
        [TestCase(1.1, true)]
        [TestCase(-1, true)]
        [TestCase(0, false)]
        [TestCase("False", false)]
        [TestCase("FALSE", false)]
        [TestCase("1", true)]
        [TestCase("", false)]
        [TestCase("   ", false)]
        [TestCase("xxx", true)]
        [TestCase("0", false)]
        public void OldToBoolMainTest(object src, bool result)
        {
            Assert.AreEqual(result, CoreExtensions.ToBool(src));
        }

        [Test]
        public void OldToBooleanWithStruct()
        {
            Assert.True(CoreExtensions.ToBool(new X()));
        }


        [Test]
        public void OldDateTimeToBoolTest()
        {
            Assert.True(CoreExtensions.ToBool(new DateTime(1901, 1, 1)));
            Assert.True(CoreExtensions.ToBool(new DateTime(2099, 1, 1)));
            Assert.False(CoreExtensions.ToBool(new DateTime(1900, 1, 1)));
            Assert.False(CoreExtensions.ToBool(new DateTime(3000, 1, 1)));
            Assert.False(CoreExtensions.ToBool(new DateTime(1899, 1, 1)));
            Assert.False(CoreExtensions.ToBool(new DateTime(3001, 1, 1)));
        }

        [Test]
        public void OldCollecitonToBoolTest()
        {
            Assert.True(CoreExtensions.ToBool(new Dictionary<string, string> { { "x", "y" } }));
            Assert.False(CoreExtensions.ToBool(new Dictionary<string, string>()));
            Assert.True(CoreExtensions.ToBool(new List<string> { "x" }));
            Assert.False(CoreExtensions.ToBool(new List<string>()));
            Assert.True(CoreExtensions.ToBool(new[] { "x" }));
            Assert.False(CoreExtensions.ToBool(new string[] { }));
        }
#endif

        private struct X {
            public int Y;
        }


        [Test]
        [Explicit]
        public void DoesExpressionsRecompile() {
            var ex1 = Expression.Lambda(Expression.Constant(1)).Compile();
            var ex2 = Expression.Lambda(Expression.Constant(1)).Compile();
            Assert.AreEqual(ex1.Method,ex2.Method);
        }

        [Test]
        public void ToBooleanWithStruct() {
            Assert.True(TypeConverter.ToBool(new X()));
        }

        [Test]
        public void DateTimeToBoolTest() {
            Assert.True(TypeConverter.ToBool(new DateTime(1901,1,1)));
            Assert.True(TypeConverter.ToBool(new DateTime(2099,1,1)));
            Assert.False(TypeConverter.ToBool(new DateTime(1900,1,1)));
            Assert.False(TypeConverter.ToBool(new DateTime(3000,1,1)));
            Assert.False(TypeConverter.ToBool(new DateTime(1899,1,1)));
            Assert.False(TypeConverter.ToBool(new DateTime(3001,1,1)));
        }

        [Test]
        public void CollecitonToBoolTest() {
            Assert.True(TypeConverter.ToBool(new Dictionary<string,string>{{"x","y"}}));
            Assert.False(TypeConverter.ToBool(new Dictionary<string,string>()));
            Assert.True(TypeConverter.ToBool(new List<string> { "x"}));
            Assert.False(TypeConverter.ToBool(new List<string>()));
            Assert.True(TypeConverter.ToBool(new []{"x"}));
            Assert.False(TypeConverter.ToBool(new string[]{}));
        }
        [Test]
        [Explicit]
        public void ToBoolTimestamp()
        {
            var dataToTest = new object[timestampSize];
            for (var i = 0; i < timestampSize; i++)
            {
#if !SYSCONVERTTEST
                if (i % 20 == 0)
                { //every 10 - datetime
                    dataToTest[i] = UnixTime.AddSeconds(i);
                }

                else
#endif
                    if (i % 19 == 0)
                    {
                        dataToTest[i] = DateTime.MinValue;
                    }
                    else if (i % 18 == 0)
                    {
                        dataToTest[i] = (long)i;
                    }
                    else if (i % 17 == 0 && i < short.MaxValue)
                    {
                        dataToTest[i] = (short)i;
                    }
                    else if (i % 16 == 0)
                    {
                        dataToTest[i] = "1";
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
                        dataToTest[i] = "0";
                    }

                    else if (i % 12 == 0) {
                        dataToTest[i] = new Dictionary<string, string> {{"x", "y"}};
                    }

                    else if (i % 11 == 0)
                    {
                        dataToTest[i] = new object();
                    }
                    else if (i % 10 == 0)
                    {
                        dataToTest[i] = null;
                    }
#endif
                    else if (i % 9 == 0)
                    {
                        dataToTest[i] = ConsoleColor.Red;
                    }
                    else if (i % 8 > 4)
                    {
                        dataToTest[i] = true;
                    }
                    else
                    {
                        dataToTest[i] = i;
                    }
            }
            Console.WriteLine("ToBool probes count: " + timestampSize);
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < timestampSize; i++)
            {
                var result = TypeConverter.ToBool(dataToTest[i]);
            }
            sw.Stop();
            Console.WriteLine("Serial BRIDGE: " + sw.ElapsedMilliseconds);

            sw = Stopwatch.StartNew();
            dataToTest.AsParallel().ForAll(_ =>
            {
                var result = TypeConverter.ToBool(_);
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
            for (var i = 0; i < timestampSize; i++)
            {
                var result = CoreExtensions.ToBool(dataToTest[i]);
            }
            sw.Stop();
            Console.WriteLine("Serial OLDSYS: " + sw.ElapsedMilliseconds);

            sw = Stopwatch.StartNew();
            dataToTest.AsParallel().ForAll(_ =>
            {
                var result = CoreExtensions.ToBool(_);
            });
            sw.Stop();
            Console.WriteLine("Parallel OLDSYS: " + sw.ElapsedMilliseconds);
#endif
        }


        
        [Test]
        public void CanUnifyAllDictionariesToStrToObj() {
            var dict = new Dictionary<int, decimal> {{10, 10.4m}, {20, 20.5m}};
            var udict = TypeConverter.ToDict(dict);
            Assert.AreEqual(10.4m,udict["10"]);
            Assert.AreEqual(20.5m,udict["20"]);
        }

        [Test]
        public void CanConvertStringToDictionary() {
            var dict = "10=10.4;20=20.5";
            var udict = TypeConverter.ToDict(dict);
            Assert.AreEqual("10.4", udict["10"]);
            Assert.AreEqual("20.5", udict["20"]);
        }

        [Test]
        public void CanConvertStringToDictionaryUrlEncode()
        {
            var dict = "10=%2C+%2B&20=20.5";
            var udict = TypeConverter.ToDict(dict,itemdelimiter:'&',escapechar:'\0',urlescape:true);
            Assert.AreEqual(", +", udict["10"]);
            Assert.AreEqual("20.5", udict["20"]);
        }


        [Test]
        public void CanConvertClassToDict() {
            var dict = new {x = 1, y = 2};
            var udict = TypeConverter.ToDict(dict);
            Assert.AreEqual(1, udict["x"]);
            Assert.AreEqual(2, udict["y"]);
        }

        [Test]
        public void CanConvertArrayToDict()
        {
            var dict = new []{ "a","b" };
            var udict = TypeConverter.ToDict(dict);
            Assert.AreEqual("a", udict["0"]);
            Assert.AreEqual("b", udict["1"]);
        }

        [Explicit]
        [Test]
        public void ToDictPerformance() {
            var dict = new {x = 1, y = 2, z = 3};
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 1000000; i++) {
                var d = dict.ToDict();
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            sw = Stopwatch.StartNew();
            for (var i = 0; i < 1000000; i++)
            {
                var d = TypeConverter.ToDict(dict);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

        [Explicit]
        [Test]
        public void ToDictSSPerformance() {
            var dict = new Dictionary<string, string>() {{"x", "1"}, {"y", "2"}, {"z", "3"}};
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 1000000; i++)
            {
                var d = dict.ToDict();
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            sw = Stopwatch.StartNew();
            for (var i = 0; i < 1000000; i++)
            {
                var d = TypeConverter.ToDict(dict);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }
        [Explicit]
        [Test]
        public void ToDictSOPerformance()
        {
            var dict = new Dictionary<string, object>() { { "x", "1" }, { "y", "2" }, { "z", "3" } };
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 1000000; i++)
            {
                var d = dict.ToDict();
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            sw = Stopwatch.StartNew();
            for (var i = 0; i < 1000000; i++)
            {
                var d = TypeConverter.ToDict(dict);
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }
    }
}
