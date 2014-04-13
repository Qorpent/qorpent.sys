﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Utils.XDiff;

namespace Qorpent.Utils.Tests.XDiff
{
	[TestFixture]
	public class XDiff2MainTest
	{

		[Test]
		[Explicit]
		public void LargeXmlPerformanceTest()
		{
			var x1 = new XElement("x1");
			var x2 = new XElement("x2");
			var rnd = new Random();
			for (var i = 0; i < 1000; i++)
			{
				var id1 = i + 3;
				var id2 = 1000 - i + 4;
				var n1 = rnd.Next(1000000);
				var n2 = rnd.Next(1000000);
				x1.Add(new XElement("x", new XAttribute("id", id1)
					, new XAttribute("name", n1)
					, new XAttribute("name1", n1)
					, new XAttribute("name2", n1)
					, new XAttribute("name3", n1)
					));

				x2.Add(new XElement("x", new XAttribute("id", id2)
					, new XAttribute("name", n2)
					, new XAttribute("name1", n2)
					, new XAttribute("nam2", n2)
					, new XAttribute("nam3", n2)
					, new XAttribute("name4", n2)
					));
			}
			Console.WriteLine("xml built");


			var gen = new XDiffGenerator();
			var sw = Stopwatch.StartNew();
			Console.WriteLine("NO ASYNC ...");
			for (var i = 0; i < 10; i++)
			{
				var diff = gen.GetDiff(x1, x2).ToArray();
				//Console.WriteLine(diff.LogToString());
				Console.WriteLine(i + " " + diff.Length);
			}
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
			Console.WriteLine("ASYNC ...");
			sw = Stopwatch.StartNew();
			for (var i = 0; i < 10; i++)
			{
				var diff = gen.GetDiff2(x1, x2).ToArray();
				Console.WriteLine(i + " " + diff.Length);
			}
			sw.Stop();
			Console.WriteLine(sw.Elapsed);
		}

		[Test]
		public void CanDetectChanges(){
			var b = XElement.Parse("<a><b code='1' x='1' name='a'/></a>");
			var n = XElement.Parse("<a><b code='1' x='1' name='b'/></a>");
			Assert.True(new XDiffGenerator().IsDifferent2(b,n));
		}

		[Test]
		public void CanDetectEquality()
		{
			var b = XElement.Parse("<a><b code='1' x='1' name='a'  /></a>");
			var n = XElement.Parse("<a><b code='1' x='1'   name='a'/></a>");
			Assert.False(new XDiffGenerator().IsDifferent2(b, n));
		}

		[Test]
		public void CanDetectEqualityWithoutOrdering()
		{
			var b = XElement.Parse("<a><b code='1' x='1' name='a'/><b code='2'  name='x' x='z'/></a>");
			var n = XElement.Parse("<a><b code='2'  name='x' x='z'/><b code='1'  name='a' x='1'/></a>");
			Assert.False(new XDiffGenerator().IsDifferent2(b, n));
		}

		[Test]
		public void CanFindNewElement(){
			var b = XElement.Parse("<a><b code='1' x='1' name='a'/></a>");
			var n = XElement.Parse("<a><b code='2'  name='x' x='z'/><b code='1'  name='a' x='1'/></a>");
			var result = GetResult(b, n);
			Assert.AreEqual(@"CreateElement n0
	NewestElement : (<b code='2' name='x' x='z' />)", result);
		}

		[Test]
		public void CanFindNewAttribute(){
			var b = XElement.Parse("<a><b code='1' x='1' name='a'/></a>");
			var n = XElement.Parse("<a><b code='1' a='3'  name='a' x='1'/></a>");
			var result = GetResult(b, n);
			Assert.AreEqual(@"CreateAttribute n0
	BasisElement name=b code=1
	NewestAttribute a :3", result);
		}

		[Test]
		public void CanFindDeletedAttribute()
		{
			var b = XElement.Parse("<a><b code='1' a='3'  name='a' x='1'/></a>");
			var n = XElement.Parse("<a><b code='1' x='1' name='a'/></a>");
			
			var result = GetResult(b, n);
			Assert.AreEqual(@"DeleteAttribute n0
	BasisElement name=b code=1
	BasisAttribute a :3", result);
		}

		[Test]
		public void CanFindDeletedElement()
		{
			var b = XElement.Parse("<a><b code='2'  name='x' x='z'/><b code='1'  name='a' x='1'/></a>");
			var n = XElement.Parse("<a><b code='1' x='1' name='a'/></a>");

			var result = GetResult(b, n);
			Assert.AreEqual(@"DeleteElement n0
	BasisElement name=b code=2", result);
		}

		[Test]
		public void DoesNotDeleteIdCanFindDeletedElement()
		{
			var b = XElement.Parse("<a><z id='1' code='2'/></a>");
			var n = XElement.Parse("<a><z code='2'/></a>");

			var result = GetResult(b, n);
			Assert.AreEqual(@"", result);
		}

		[Test]
		public void DoesNotDeleteCodeCanFindDeletedElement()
		{
			var b = XElement.Parse("<a><z id='1' code='2'/></a>");
			var n = XElement.Parse("<a><z id='1' /></a>");

			var result = GetResult(b, n);
			Assert.AreEqual(@"", result);
		}


		[Test]
		public void CodesAreChangedAgainstIdInUsualMode()
		{
			var b = XElement.Parse("<a><z id='1' code='2'/><z id='3' code='4'/></a>");
			var n = XElement.Parse("<a><z id='3' set-code='2'/><z id='1' set-code='4'/></a>");

			var result = GetResult(b, n);
			Assert.AreEqual(@"ChangeAttribute n0
	BasisElement name=z id=1
	NewestAttribute code :4
ChangeAttribute n1
	BasisElement name=z id=3
	NewestAttribute code :2".Length, result.Length);
		}



		[Test]
		public void CanFilterActions()
		{
			var b = XElement.Parse("<a><z id='1' code='2'/><z id='3' code='4'/></a>");
			var n = XElement.Parse("<a><z id='3' set-code='2'/><z id='1' set-code='4'/><z id='5'/></a>");

			var result = GetResult(b, n);
			Assert.AreEqual(@"CreateElement n0
	NewestElement : (<z id='5' />)
ChangeAttribute n1
	BasisElement name=z id=1
	NewestAttribute code :4
ChangeAttribute n2
	BasisElement name=z id=3
	NewestAttribute code :2".Length, result.Length);
			Console.WriteLine();
			var opts = new XDiffOptions();
			opts.IncludeActions = opts.IncludeActions & ~XDiffAction.CreateElement;
			result = GetResult(b, n, opts);
			Assert.AreEqual(@"ChangeAttribute n0
	BasisElement name=z id=1
	NewestAttribute code :4
ChangeAttribute n1
	BasisElement name=z id=3
	NewestAttribute code :2".Length, result.Length);
			Console.WriteLine();
			opts = new XDiffOptions();
			opts.IncludeActions = opts.IncludeActions & ~XDiffAction.ChangeAttribute;
			result = GetResult(b, n, opts);
			Assert.AreEqual(@"CreateElement n0
	NewestElement : (<z id='5' />)".Length, result.Length);
		}

		[Test]
		public void CanPreventActions()
		{
			var b = XElement.Parse("<a><z id='1' code='2'/><z id='3' code='4'/></a>");
			var n = XElement.Parse("<a><z id='3' code='2'/><z id='1' code='4'/><z id='5'/></a>");
			var opts = new XDiffOptions();
			opts.ErrorActions = opts.ErrorActions | XDiffAction.CreateElement;
			Assert.Throws<Exception>(()=> GetResult(b, n, opts));
		}


		[Test]
		public void CanAppplyPatch()
		{
			var b = XElement.Parse("<a><z id='1' code='2' name='2'/><z id='3' code='3' name='4'/></a>");
			var n = XElement.Parse("<a><z id='3' set-code='4' name='2'/><z id='1' set-code='3' name='4'/></a>");
			Assert.True(new XDiffGenerator().IsDifferent2(b, n));
			var result = new XDiffGenerator().GetDiff2(b,n).Apply(b);
			Console.WriteLine(result.ToString());
			Assert.False(new XDiffGenerator().IsDifferent2(b,n));
		}

		[Test]
		public void CanAppplyPatchHierarchcallyWithNameChanges()
		{
			var b = XElement.Parse("<a><z code='2' name='y'/><z id='3' code='4'><x id='5' name='x'/></z></a>");
			var n = XElement.Parse("<a><u code='2' name='y'><y id='5' name='x'/></u><z id='3' code='4'></z></a>");
			var opts = new XDiffOptions{IsHierarchy = true, IsNameIndepended = true};
			GetResult(b, n, opts);
			Assert.True(new XDiffGenerator(opts).IsDifferent2(b, n));
			var result = new XDiffGenerator(opts).GetDiff2(b, n).Apply(b,opts);
			Console.WriteLine(result.ToString());
			Assert.False(new XDiffGenerator(opts).IsDifferent2(b, n));
		}

		[Test]
		public void IdUpdating()
		{
			var b = XElement.Parse("<a><z id='1' code='2'/><z id='3' code='4'/></a>");
			var n = XElement.Parse("<a><z set-id='3' code='2'/><z set-id='1' code='4'/></a>");

			var result = GetResult(b, n);
			Assert.AreEqual(@"ChangeAttribute n0
	BasisElement name=z id=1
	NewestAttribute id :3
ChangeAttribute n1
	BasisElement name=z id=3
	NewestAttribute id :1".Length, result.Length);
		}

		[Test]
		public void CanFindChangedElementValue()
		{
			var b = XElement.Parse("<a><b code='1'  name='a' x='1'/></a>");
			var n = XElement.Parse("<a><b code='1' x='1' name='a'>aaa</b></a>");

			var result = GetResult(b, n);
			Assert.AreEqual(@"ChangeElement n0
	BasisElement name=b code=1
	NewValue : aaa", result);


		}


		[Test]
		[Ignore("Invalid behavior")]
		public void CanFindChangedElementValueToEmpty()
		{
			var b = XElement.Parse("<a><b code='1' x='1' name='a'>aaa</b></a>");
			var n = XElement.Parse("<a><b code='1'  name='a' x='1'/></a>");
			

			var result = GetResult(b, n);
			Assert.AreEqual(@"ChangeElement n0
	BasisElement name=b code=1
	NewValue : ''", result);

		}

		

		[Test]
		public void BehaviorWithNamesDistinctMode()
		{
			var b = XElement.Parse("<a><z code='1' x='1' name='a'>aaa</z></a>");
			var n = XElement.Parse("<a><y code='1' x='1' name='a'>aaa</y></a>");


			var result = GetResult(b, n);
			Assert.AreEqual(@"CreateElement n0
	NewestElement : (<y code='1' x='1' name='a'>aaa</y>)
DeleteElement n1
	BasisElement name=z code=1", result);

		}

		[Test]
		public void BehaviorWithNamesSameMode()
		{
			var b = XElement.Parse("<a><z code='1' x='1' name='a'>aaa</z></a>");
			var n = XElement.Parse("<a><y code='1' x='1' name='a'>aaa</y></a>");


			var result = GetResult(b, n, new XDiffOptions{IsNameIndepended = true});
			Assert.AreEqual(@"RenameElement n0
	BasisElement name=z code=1
	NewValue : y", result);

		}

		[Test]
		public void BehaviorWithoutTrees()
		{
			var b = XElement.Parse("<a><z code='1'><y code='2'/></z></a>");
			var n = XElement.Parse("<a><z code='1'><y code='2'/><s code='3'/></z></a>");


			var result = GetResult(b, n);
			Assert.AreEqual(@"ChangeElement n0
	BasisElement name=z code=1
	NewValue : '<y code=\'2\' />\r\n<s code=\'3\' />'", result);

		}


		[Test]
		public void BehaviorWithTrees()
		{
			var b = XElement.Parse("<a><z code='1'><y code='2'/></z></a>");
			var n = XElement.Parse("<a><z code='1'><y code='2'/><s code='3'/></z></a>");


			var result = GetResult(b, n, new XDiffOptions{IsHierarchy = true});
			Assert.AreEqual(@"CreateElement n0
	NewestElement : (<s code='3' __parent='z-code-1' />)", result);

		}

		[Test]	
		[Ignore("same items in patch are merged")]
		public void ErrorOnDoubleDetectionInNew()
		{
			var b = XElement.Parse("<a><z code='1'/></a>");
			var n = XElement.Parse("<a><z code='1'/><z code='1'/></a>");
			Assert.Throws<Exception>(() => new XDiffGenerator().IsDifferent2(b, n));

		}
		[Test]
		public void ErrorOnAmbigousDetectionInNew()
		{
			var b = XElement.Parse("<a><z id='1' code='1'/></a>");
			var n = XElement.Parse("<a><z id='1' code='1'/><z id='2' code='1'/></a>");
			Assert.Throws<Exception>(() => new XDiffGenerator().IsDifferent2(b, n));

		}

		[Test]
		public void ErrorOnDoubleDetectionInOld()
		{
			var b = XElement.Parse("<a><z code='1'/><z code='1'/></a>");
			var n = XElement.Parse("<a><z code='1'/></a>");
			Assert.Throws<Exception>(() => new XDiffGenerator().IsDifferent2(b, n));

		}

		[Test]
		public void ErrorOnDoubleDetectionInNewWithIdsAndCode()
		{
			var b = XElement.Parse("<a><z id='1' code='1'/></a>");
			var n = XElement.Parse("<a><z code='1'/><z id='1' code='2'/></a>");
			Assert.Throws<Exception>(() => new XDiffGenerator().IsDifferent2(b, n));

		}


		[Test]
		public void CanChangeCodeWithSet()
		{
			var b = XElement.Parse("<a><z code='1'/></a>");
			var n = XElement.Parse("<a><z code='1' set-code='2'/></a>");


			var result = GetResult(b, n, new XDiffOptions());
			Assert.AreEqual(@"ChangeAttribute n0
	BasisElement name=z code=1
	NewestAttribute code :2", result);

		}
	

		[Test]
		public void PositionChangeBetweenNodes()
		{
			var b = XElement.Parse("<a><z code='1'><y code='2'/><s code='3'/></z><z code='4'></z></a>");
			var n = XElement.Parse("<a><z code='1'><y code='2'/></z><z code='4'><s code='3'/></z></a>");


			var result = GetResult(b, n, new XDiffOptions { IsHierarchy = true });
			Assert.AreEqual(@"ChangeHierarchyPosition n0
	BasisElement name=s code=3 parent=z-code-1
	NewValue : z-code-4", result);

		}
		[Test]
		public void PositionChangeToRoot()
		{
			var b = XElement.Parse("<a><z code='1'><y code='2'/><s code='3'/></z></a>");
			var n = XElement.Parse("<a><z code='1'><y code='2'/></z><s code='3'/></a>");


			var result = GetResult(b, n, new XDiffOptions { IsHierarchy = true });
			Assert.AreEqual(@"ChangeHierarchyPosition n0
	BasisElement name=s code=3 parent=z-code-1
	NewValue : ''", result);

		}



		private static string GetResult(XElement b, XElement n, XDiffOptions opts = null){
			var result = new XDiffGenerator(opts).GetDiff2(b, n).LogToString().Replace("\"", "'").Trim();
			Console.WriteLine(result);
			return result;
		}
	}
}