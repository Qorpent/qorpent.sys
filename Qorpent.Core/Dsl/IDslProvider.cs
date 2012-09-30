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
// Original file : IDslProvider.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Dsl {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public interface IDslProvider {
		/// <summary>
		/// 	Опциональный поставщик логики определения ресурсов, связывается по языку
		/// </summary>
		IDslResourceProvider ResourceProvider { get; set; }

		/// <summary>
		/// 	Опциональный класс дополнительно обработки XML
		/// </summary>
		IDslXmlPreprocessor XmlPreprocessor { get; set; }

		/// <summary>
		/// 	Initializes the specified project.
		/// </summary>
		/// <param name="project"> The project. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		DslContext Initialize(DslProject project);

		/// <summary>
		/// 	Preprocesses the specified context.
		/// </summary>
		/// <param name="context"> The context. </param>
		/// <remarks>
		/// </remarks>
		void Preprocess(DslContext context);

		/// <summary>
		/// 	Compiles the specified context.
		/// </summary>
		/// <param name="context"> The context. </param>
		/// <remarks>
		/// </remarks>
		void Compile(DslContext context);

		/// <summary>
		/// 	Runs the specified project.
		/// </summary>
		/// <param name="project"> The project. </param>
		/// <remarks>
		/// </remarks>
		DslContext Run(DslProject project);
	}
}