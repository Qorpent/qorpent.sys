using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.IO.DirtyVersion;
using Qorpent.IO.DirtyVersion.Helpers;

namespace Qorpent.IO.Tests.DirtyVersion
{
	[TestFixture]
	public class HahserTest
	{
		[TestCase("./not-same-folter/file1.txt")]
		[TestCase("./same folder/file1.txt")]
		[TestCase("./file1.txt")]
		public void FileNameHashTest(string filename) {
			var result = new Hasher().GetHash(filename);
			var result2 = new Hasher().GetHash(filename);
			var result3 = new Hasher().GetHash(filename+'\0');
			Console.WriteLine(result);
			// первое требование - хэш должен быть
			Assert.False(string.IsNullOrWhiteSpace(result));
			// второе требование - хэш должен иметь фиксированный размер
			Assert.AreEqual(Const.HashSize,result.Length);
			// третье требование - хэш должен быть совместим с файловой системой
			Assert.True(result.All(char.IsLetterOrDigit));
			// четвертое требование - хэш должен быть воспроизводимым
			Assert.AreEqual(result,result2);
			// пятое требование - хэш должен уникальным
			Assert.AreNotEqual(result, result3);
		}
	}
}
