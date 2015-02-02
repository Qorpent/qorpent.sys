using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Qorpent.Bridge.Tests.Utils
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    [Explicit]
    public class TryUnsafe
    {
        /*
        [Test]
        public unsafe void IsIntAssigneeQuickerWithUnsafe() {
            //ответ - НЕТ, это геморой, не ускоряющий ничего
            var i1 = 0;
            var i2 = 0;
            var i3 = 0;
            var i4 = 0;
            var _i1 = &i1;
            var _i2 = &i2;
            var _i3 = &i3;
            var _i4 = &i4;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 500000000; i++) {
                *_i1 = i;
                *_i2 = *_i1+1;
                *_i3 = *_i2+2;
                *_i4 = *_i3+3;
                *_i1 = *_i4;
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            sw = Stopwatch.StartNew();
            for (var i = 0; i < 500000000; i++)
            {
                i1 = i;
                i2 = i1 + 1;
                i3 = i2 + 2;
                i4 = i3 + 3;
                i1 = i4;
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

        }
         */
        /*
        [Test]
        public  unsafe  void CalculateUnsafe() {
            // и таки да, работа с *char быстрее чем со String (на чтение)
            var sw = Stopwatch.StartNew();
            var testString = "Ф1b2c3d4";
            int i = 0;
            int j =0;
            int length = testString.Length;
            int result = 0;
            for (  i = 0; i <= 100000000; i++) {
                fixed (char* pchar = testString) {
                    var cursor = pchar;
                     result = 0;
                        j = 0;
                        while (j < length) {
                            if (*cursor >= 48 && *cursor <= 57) {
                                result += *cursor - 48;
                            }
                            cursor += 1;
                            j += 1;
                        }
                        if (result != 10) {
                            throw new Exception("fail "+result);
                        }
                    
                }
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }*/

        [Test]
        public void BestCountForEnumerable() {
            
        }
     
        [Test]
        public  void CalculateSafe() {
            var sw = Stopwatch.StartNew();
            var testString = "Ф1b2c3d4";

            int length = testString.Length;
            for (var i = 0; i <= 100000000; i++) {
                var res = 0;
                var idx = 0;
                while (idx < length) {
                    if (testString[idx] >= 48 && testString[idx] <= 57) {
                        res += testString[idx] - 48;
                    }
                    idx++;
                }
                if (res != 10) {
                    throw new Exception("fail " + res);
                }

            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

    
    }
}
