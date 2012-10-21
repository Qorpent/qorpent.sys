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
// Original file : md5serializer.cs
// Project: Qorpent.Serialization
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Qorpent.IoC;

namespace Qorpent.Serialization {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle.Transient, ServiceType = typeof (ISerializer), Name = "md5.serializer")]
	public class Md5Serializer : Serializer {
		/// <summary>
		/// 	Creates the impl.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected override ISerializerImpl CreateImpl(string name, object value) {
			return new JsonSerializerImpl();
		}

		/// <summary>
		/// 	Serializes the specified name.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <param name="output"> The output. </param>
		/// <remarks>
		/// </remarks>
		public override void Serialize(string name, object value, TextWriter output) {
			var sw = new StringWriter();
			base.Serialize(name, value, sw);
			var data = sw.ToString();
			var md5 = Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(data)));
			output.Write(md5);
		}
	}
}