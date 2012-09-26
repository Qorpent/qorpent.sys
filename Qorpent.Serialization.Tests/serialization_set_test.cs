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
// Original file : serialization_set_test.cs
// Project: Qorpent.Serialization.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests {
	[TestFixture]
	public class serialization_set_test {
		[Serialize]
		public interface itest {
			[SerializeNotNullOnly] int nip { get; set; }

			[IgnoreSerialize] int iip { get; set; }

			int ip { get; set; }
		}

		public class titest : itest {
			public titest self { get; set; }


			public int nip { get; set; }
			public int iip { get; set; }
			public int ip { get; set; }
		}

		public class test {
			public int intpn { get; set; }

			[SerializeNotNullOnly] public int intp { get; set; }

			public string strpn { get; set; }

			[SerializeNotNullOnly] public string strp { get; set; }


			public test2 spn { get; set; }

			[SerializeNotNullOnly] public test2 sp { get; set; }

			public test3 nspn { get; set; }

			[SerializeNotNullOnly] public test3 nsp { get; set; }

			[Serialize] public test3 snspn { get; set; }
			[IgnoreSerialize] public int iintfn;
			[SerializeNotNullOnly] public int intf;
			public int intfn;
			public int[] intfna;
			[SerializeNotNullOnly] public test3 nsf;
			public test3 nsfn;
			[SerializeNotNullOnly] public test2 sf;
			public test2 sfn;
			[SerializeNotNullOnly] public string strf;
			public string strfn;
		}

		[Serialize]
		public class test2 {}

		public class test3 {}

		[Test]
		public void bug_0_int_was_treated_as_serializable() {
			var test = new test();
			var item = new SerializableItem(test.GetType().GetField("intf"), test);
			Assert.False(item.IsSerializable);
		}

		[Test]
		public void can_retrieve_setting_from_interface() {
			var items = SerializableItem.GetSerializableItems(new titest());
			Assert.Null(items.FirstOrDefault(x => x.Name == "nip"), "nip");
			Assert.Null(items.FirstOrDefault(x => x.Name == "iip"), "iip");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "ip"), "ip");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "self"), "self");
		}

		[Test]
		public void ignore_items_are_ignored() {
			var items = SerializableItem.GetSerializableItems(new test());
			Assert.Null(items.FirstOrDefault(x => x.Name == "iintf"), "iintf");
		}

		[Test]
		public void not_nullable_final_fields_anp_properties_ignored_on_nulls() {
			var items = SerializableItem.GetSerializableItems(new test());
			Assert.Null(items.FirstOrDefault(x => x.Name == "intf"), "intf");
			Assert.Null(items.FirstOrDefault(x => x.Name == "strf"), "strf");
			Assert.Null(items.FirstOrDefault(x => x.Name == "sf"), "strf");
			Assert.Null(items.FirstOrDefault(x => x.Name == "intp"), "intp");
			Assert.Null(items.FirstOrDefault(x => x.Name == "strp"), "strp");
			Assert.Null(items.FirstOrDefault(x => x.Name == "sp"), "strp");
		}

		[Test]
		public void not_nullable_final_fields_anp_properties_not_ignored_if_not_nulls() {
			var items =
				SerializableItem.GetSerializableItems(new test
					{
						intf = 1,
						strf = "1",
						sf = new test2(),
						intp = 1,
						strp = "1",
						sp = new test2()
					});
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "intf"), "intf");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "strf"), "strf");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "sf"), "sf");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "intp"), "intp");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "strp"), "strp");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "sp"), "strp");
		}

		[Test]
		public void not_serializable_classes_ignored_except_of_member_level_serialization_setted_on() {
			var items = SerializableItem.GetSerializableItems(new test());
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "snspn"), "snspn");
			Assert.Null(items.FirstOrDefault(x => x.Name == "nspn"), "nspn");
		}

		[Test]
		public void nullable_final_fields_anp_properties_always_occures() {
			var items = SerializableItem.GetSerializableItems(new test());
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "intfn"), "intfn");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "intfna"), "intfna");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "strfn"), "strfn");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "sfn"), "strfn");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "intpn"), "intpn");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "strpn"), "strpn");
			Assert.NotNull(items.FirstOrDefault(x => x.Name == "spn"), "strpn");
		}
	}
}