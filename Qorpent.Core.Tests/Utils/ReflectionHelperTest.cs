#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : ReflectionHelperTest.cs
// Project: Qorpent.Utils.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests {
	[TestFixture]
	public class ReflectionHelperTest {
		[SetUp]
		public void setup() {
			_h = new ReflectionHelper();
			_t = typeof (membercontainer);
			_c = new membercontainer();
		}


		private ReflectionHelper _h;
		private Type _t;
		private membercontainer _c;
		private ValueMember _Case;
		private ValueMember _CaSe;
		private ValueMember _Name;
		private ValueMember _name;
		private ValueMember _IName;
		private ValueMember _iname;
		private ValueMember _Ro;
		private ValueMember _Wo;

		private ValueMember _Er
		                    ;

		private class membercontainer {
			public membercontainer() {
				Case = "Case";
				CaSe = "CaSe";
				Name = "Name";
				name = "name";
				IName = "IName";
				iname = "iname";
			}

			public string Name { get; set; }

			private string IName { get; set; }
			public string PubRPrivW { get; private set; }

			public string Ro {
				get { return "Ro"; }
			}

			public string Wo {
				set { }
			}

			public string Er {
				get { return "error"; }
				set { throw new Exception(); }
			}

			public override string ToString() {
				return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", Case, CaSe, Name, name, IName, iname, Ro, Er);
			}

			public readonly string CaSe;
			public readonly string Case;
			private readonly string iname;
			public readonly string name;
		}

		public void prepare(bool ignoreCase, bool publicOnly, bool readableOnly, bool writeableOnly) {
			_Case = _h.FindValueMember(_t, "Case", ignoreCase, publicOnly, readableOnly, writeableOnly);
			_CaSe = _h.FindValueMember(_t, "CaSe", ignoreCase, publicOnly, readableOnly, writeableOnly);
			_Name = _h.FindValueMember(_t, "Name", ignoreCase, publicOnly, readableOnly, writeableOnly);
			_name = _h.FindValueMember(_t, "name", ignoreCase, publicOnly, readableOnly, writeableOnly);
			_IName = _h.FindValueMember(_t, "IName", ignoreCase, publicOnly, readableOnly, writeableOnly);
			_iname = _h.FindValueMember(_t, "iname", ignoreCase, publicOnly, readableOnly, writeableOnly);
			_Ro = _h.FindValueMember(_t, "Ro", ignoreCase, publicOnly, readableOnly, writeableOnly);
			_Wo = _h.FindValueMember(_t, "Wo", ignoreCase, publicOnly, readableOnly, writeableOnly);
			_Er = _h.FindValueMember(_t, "Er", ignoreCase, publicOnly, readableOnly, writeableOnly);
		}

		[Test]
		public void Extensions_GetValue_Dictionary_Works() {
			var dict = new Dictionary<int, int>();
			dict[23] = 456;
			Assert.AreEqual(456, dict.GetValue<int>(23));
		}

		[Test]
		public void Extensions_GetValue_Works() {
			_c.SetValue("Name", "X");
			Assert.AreEqual("X", _c.GetValue("Name", "Y"));
		}


		[Test]
		public void Extensions_SetValue_Dictionary_Works() {
			var dict = new Dictionary<int, int>();
			dict.SetValue(23, 456);
			Assert.AreEqual(456, dict[23]);
		}

		[Test]
		public void Extensions_SetValue_Works() {
			_c.SetValue("Name", "X");
			Assert.AreEqual("X", _c.Name);
		}


		[Test]
		public void FindValueMember_Case_NonPublic_No_Access_Filter() {
			prepare(false, false, false, false);
			CollectionAssert.AllItemsAreNotNull(new[] {_Case, _CaSe, _Name, _name, _Ro, _Wo, _Er, _IName, _iname});
			Assert.AreNotEqual(_Case.Member, _CaSe.Member);
			Assert.AreNotEqual(_Name.Member, _name.Member);
			Assert.AreNotEqual(_IName.Member, _iname.Member);
		}

		[Test]
		public void FindValueMember_Case_NonPublic_ReadAndWrite_Filter() {
			prepare(false, false, true, true);
			CollectionAssert.AllItemsAreNotNull(new[] {_Case, _CaSe, _Name, _name, _Er, _IName, _iname});
			Assert.Null(_Ro);
			Assert.Null(_Wo);
		}


		[Test]
		public void FindValueMember_Case_NonPublic_ReadOnly_Filter() {
			prepare(false, false, true, false);
			CollectionAssert.AllItemsAreNotNull(new[] {_Case, _CaSe, _Name, _name, _Ro, _Er, _IName, _iname});
			Assert.Null(_Wo);
		}

		[Test]
		public void FindValueMember_Case_NonPublic_WriteOnly_Filter() {
			prepare(false, false, false, true);
			CollectionAssert.AllItemsAreNotNull(new[] {_Case, _CaSe, _Name, _name, _Er, _IName, _iname, _Wo});
			Assert.Null(_Ro);
		}


		[Test]
		public void FindValueMember_Case_Public_No_Access_Filter() {
			prepare(false, true, false, false);
			CollectionAssert.AllItemsAreNotNull(new[] {_Case, _CaSe, _Name, _name, _Ro, _Wo, _Er});
			Assert.Null(_IName);
			Assert.Null(_iname);
			Assert.AreNotEqual(_Case.Member, _CaSe.Member);
			Assert.AreNotEqual(_Name.Member, _name.Member);
		}

		[Test]
		public void FindValueMember_Case_Public_No_Access_Filter_Not_Found() {
			prepare(true, true, false, false);
			var _CASE = _h.FindValueMember(_t, "CASE", false, true);
			Assert.Null(_CASE);
		}

		[Test]
		public void FindValueMember_IgnoreCase_Public_No_Access_Filter() {
			prepare(true, true, false, false);
			var _CASE = _h.FindValueMember(_t, "CASE", true, true);
			Assert.NotNull(_CASE);
			Assert.AreEqual("CaSe", _CASE.Member.Name);
		}


		[Test]
		public void GetDictionaryTypes_Test() {
			var types = _h.GetDictionaryTypes(typeof (Dictionary<int, string>));
			Assert.AreEqual(typeof (int), types[0]);
			Assert.AreEqual(typeof (string), types[1]);
		}

		[Test]
		public void GetValue_Dictionary_Exists() {
			var dict = new Dictionary<string, string> {{"x", "xx"}};
			Assert.AreEqual("xx", _h.GetValue<string>(dict, "x"));
		}

		[Test]
		public void GetValue_Dictionary_Init() {
			IDictionary<int, int> dict = new Dictionary<int, int>();
			var result = _h.GetValue(dict, "23", 456, initialize: true);
			Assert.AreEqual(456, result);
			Assert.AreEqual(456, dict[23]);
		}

		[Test]
		public void GetValue_Dictionary_NoInit() {
			IDictionary<int, int> dict = new Dictionary<int, int>();
			var result = _h.GetValue(dict, "23", 456);
			Assert.AreEqual(456, result);
			Assert.False(dict.ContainsKey(23));
		}

		[Test]
		public void GetValue_Dictionary_NonExists_WithDef() {
			var dict = new Dictionary<string, string>();
			Assert.AreEqual("xx", _h.GetDictionaryValue(dict, "x", "xx"));
		}

		[Test]
		public void GetValue_Dictionary_NonExists_WithDefGen() {
			var dict = new Dictionary<string, string>();
			Assert.AreEqual("xx", _h.GetDictionaryValue(dict, "x", null, () => "xx"));
		}

		[Test]
		public void GetValue_IgnoreCase_NonPublic() {
			Assert.AreEqual("iname", _h.GetValue<string>(_c, "iname", publicOnly: false));
		}

		[Test]
		public void GetValue_IgnoreCase_Public() {
			Assert.AreEqual("name", _h.GetValue<string>(_c, "name"));
		}

		[Test]
		public void GetValue_TypeTransform_Public() {
			_c.Name = "365";
			Assert.AreEqual(365, _h.GetValue<int>(_c, "Name"));
		}

		[Test]
		public void IsDictionary_Test() {
			Assert.True(_h.IsDictionary(typeof (Dictionary<string, string>)));
			Assert.False(_h.IsDictionary(GetType()));
		}

		[Test]
		public void SetValue_Dictionary() {
			IDictionary<int, int> dict = new Dictionary<int, int>();
			_h.SetValue(dict, "23", "456");
			Assert.AreEqual(456, dict[23]);
		}

		[Test]
		public void SetValue_IgnoreCase_NonPublic() {
			prepare(false, false, false, false);
			_h.SetValue(_c, "INAME", "iname2", true, false, false, true, true);
			Assert.AreEqual("iname2", _IName.Get(_c));
		}

		[Test]
		public void SetValue_IgnoreCase_Public() {
			prepare(false, false, false, false);
			_h.SetValue(_c, "NAME", "NAME2", true, false, false, true, true);
			Assert.AreEqual("NAME2", _c.Name);
		}

		[Test]
		public void ValueMember_Can_CheckPrivacy() {
			var private_name_with_check = new ValueMember(_t.GetField("iname", BindingFlags.NonPublic | BindingFlags.Instance),
			                                              true);
			Assert.False(private_name_with_check.CanBeAssigned);
			Assert.False(private_name_with_check.CanBeRetrieved);
			var private_name_with_no_check = new ValueMember(
				_t.GetField("iname", BindingFlags.NonPublic | BindingFlags.Instance), false);
			Assert.True(private_name_with_no_check.CanBeAssigned);
			Assert.True(private_name_with_no_check.CanBeRetrieved);

			var PubRPrivW_with_check = new ValueMember(_t.GetProperty("PubRPrivW", BindingFlags.Public | BindingFlags.Instance),
			                                           true);
			Assert.False(PubRPrivW_with_check.CanBeAssigned);
			Assert.True(PubRPrivW_with_check.CanBeRetrieved);

			var PubRPrivW_with_no_check =
				new ValueMember(_t.GetProperty("PubRPrivW", BindingFlags.Public | BindingFlags.Instance), false);
			Assert.True(PubRPrivW_with_no_check.CanBeAssigned);
			Assert.True(PubRPrivW_with_no_check.CanBeRetrieved);
		}

		[Test]
		public void ValueMember_Can_Get() {
			prepare(false, false, false, false);
			Assert.AreEqual("Case", _Case.Get(_c));
			Assert.AreEqual("CaSe", _CaSe.Get(_c));
			Assert.AreEqual("Name", _Name.Get(_c));
			Assert.AreEqual("name", _name.Get(_c));
			Assert.AreEqual("IName", _IName.Get(_c));
			Assert.AreEqual("iname", _iname.Get(_c));

			Assert.True(_iname.CanBeRetrieved);
			Assert.False(_Wo.CanBeRetrieved);
			Assert.True(_Ro.CanBeRetrieved);

			Assert.AreEqual("Ro", _Ro.Get(_c));
			Assert.AreEqual("error", _Er.Get(_c));
		}

		[Test]
		public void ValueMember_Can_Set() {
			Assert.True(_iname.CanBeAssigned);
			Assert.True(_Wo.CanBeAssigned);
			Assert.False(_Ro.CanBeAssigned);
			prepare(false, false, false, false);

			_Case.Set(_c, "Case1");
			_CaSe.Set(_c, "CaSe1");
			_Name.Set(_c, "Name1");
			_name.Set(_c, "name1");
			_IName.Set(_c, "IName1");
			_iname.Set(_c, "iname1");
			_Wo.Set(_c, "Wo1");

			Assert.AreEqual("Case1", _Case.Get(_c));
			Assert.AreEqual("CaSe1", _CaSe.Get(_c));
			Assert.AreEqual("Name1", _Name.Get(_c));
			Assert.AreEqual("name1", _name.Get(_c));
			Assert.AreEqual("IName1", _IName.Get(_c));
			Assert.AreEqual("iname1", _iname.Get(_c));
		}
	}
}