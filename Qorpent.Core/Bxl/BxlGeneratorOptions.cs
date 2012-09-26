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
// Original file : BxlGeneratorOptions.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Bxl {
	/// <summary>
	/// 	Options for generating BXL from XML
	/// </summary>
	public class BxlGeneratorOptions {
		/// <summary>
		/// 	Set of highly prioritized attributes, whose order will be applyed before apphabetical
		/// </summary>
		public string[] FirstPlaceAttributes = new[] {"id", "code", "name"};

		/// <summary>
		/// 	Set of attributes which always must be rendered at same line as element start
		/// </summary>
		public string[] InlineAlwaysAttributes = new[] {"id", "code", "name", "_file", "_line", "idx"};

		/// <summary>
		/// 	If it's possible - renders attributes at same line as element starts
		/// </summary>
		public bool InlineAttributesByDefault;

		/// <summary>
		/// 	Set of attributes which must always be rendered at own line
		/// </summary>
		public string[] NewlineAlwaysAttributes = new string[] {};

		/// <summary>
		/// 	Prevents generation of root element (just 1-st level descendants will be rendered)
		/// </summary>
		public bool NoRootElement;

		/// <summary>
		/// 	Set of attributes, which must be skipped during generation
		/// </summary>
		public string[] SkipAttributes = new string[] {};

		/// <summary>
		/// 	Forces usage of safe tripple quotes strings on values rendering
		/// </summary>
		public bool UseTrippleQuotOnValues;
	}
}