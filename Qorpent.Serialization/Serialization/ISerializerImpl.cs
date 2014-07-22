#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Serialization/ISerializerImpl.cs
#endregion
using System.IO;

namespace Qorpent.Serialization {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public interface ISerializerImpl {
		/// <summary>
		/// 	Gets or sets the output.
		/// </summary>
		/// <value> The output. </value>
		/// <remarks>
		/// </remarks>
		TextWriter Output { get; set; }

		/// <summary>
		/// 	Begins the specified name.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <remarks>
		/// </remarks>
		void Begin(string name);

		/// <summary>
		/// 	Ends this instance.
		/// </summary>
		/// <remarks>
		/// </remarks>
		void End();

		/// <summary>
		/// 	Begins the object.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <remarks>
		/// </remarks>
		void BeginObject(string name);

		/// <summary>
		/// 	Ends the object.
		/// </summary>
		/// <remarks>
		/// </remarks>
		void EndObject();

		/// <summary>
		/// 	Begins the object item.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="isfinal"> if set to <c>true</c> [isfinal]. </param>
		/// <remarks>
		/// </remarks>
		void BeginObjectItem(string name, bool isfinal);

		/// <summary>
		/// 	Ends the object item.
		/// </summary>
		/// <param name="islast"> if set to <c>true</c> [islast]. </param>
		/// <remarks>
		/// </remarks>
		void EndObjectItem(bool islast);

		/// <summary>
		/// 	Writes the final.
		/// </summary>
		/// <param name="value"> The value. </param>
		/// <remarks>
		/// </remarks>
		void WriteFinal(object value);

		/// <summary>
		/// 	Begins the dictionary.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <remarks>
		/// </remarks>
		void BeginDictionary(string name);

		/// <summary>
		/// 	Ends the dictionary.
		/// </summary>
		/// <remarks>
		/// </remarks>
		void EndDictionary();

		/// <summary>
		/// 	Begins the dictionary entry.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <remarks>
		/// </remarks>
		void BeginDictionaryEntry(string name);

		/// <summary>
		/// 	Ends the dictionary entry.
		/// </summary>
		/// <param name="islast"> if set to <c>true</c> [islast]. </param>
		/// <remarks>
		/// </remarks>
		void EndDictionaryEntry(bool islast);

		/// <summary>
		/// 	Begins the array.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="length"></param>
		/// <remarks>
		/// </remarks>
		void BeginArray(string name,int length);

		/// <summary>
		/// 	Ends the array.
		/// </summary>
		/// <remarks>
		/// </remarks>
		void EndArray();

		/// <summary>
		/// 	Begins the array entry.
		/// </summary>
		/// <param name="idx"> The idx. </param>
		/// <param name="name"></param>
		/// <param name="noindex"></param>
		/// <remarks>
		/// </remarks>
		void BeginArrayEntry(int idx, string name = "item", bool noindex = false);

		/// <summary>
		/// 	Ends the array entry.
		/// </summary>
		/// <param name="last"> if set to <c>true</c> [last]. </param>
		/// <param name="noindex"></param>
		/// <remarks>
		/// </remarks>
		void EndArrayEntry(bool last, bool noindex = false);

		/// <summary>
		/// 	Flushes this instance.
		/// </summary>
		/// <remarks>
		/// </remarks>
		void Flush();
	}
}