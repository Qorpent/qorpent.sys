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
// Original file : JsRaw.cs
// Project: Qorpent.Serialization
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Serialization {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public struct JsRaw {
		/// <summary>
		/// 	Initializes a new instance of the <see cref="JsRaw" /> struct.
		/// </summary>
		/// <param name="js"> The js. </param>
		/// <remarks>
		/// </remarks>
		public JsRaw(string js) {
			this.js = js;
		}

		/// <summary>
		/// 	Performs an implicit conversion from <see cref="Qorpent.Serialization.JsRaw" /> to <see cref="System.String" />.
		/// </summary>
		/// <param name="js"> The js. </param>
		/// <returns> The result of the conversion. </returns>
		/// <remarks>
		/// </remarks>
		public static implicit operator string(JsRaw js) {
			return js.js;
		}

		/// <summary>
		/// 	Performs an explicit conversion from <see cref="System.String" /> to <see cref="Qorpent.Serialization.JsRaw" />.
		/// </summary>
		/// <param name="js"> The js. </param>
		/// <returns> The result of the conversion. </returns>
		/// <remarks>
		/// </remarks>
		public static explicit operator JsRaw(string js) {
			return new JsRaw(js);
		}

		/// <summary>
		/// 	Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns> A <see cref="System.String" /> that represents this instance. </returns>
		/// <remarks>
		/// </remarks>
		public override string ToString() {
			return js;
		}

		/// <summary>
		/// </summary>
		private readonly string js;
	}
}