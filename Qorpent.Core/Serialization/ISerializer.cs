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
// PROJECT ORIGIN: Qorpent.Core/ISerializer.cs
#endregion
using System.IO;

namespace Qorpent.Serialization {



	/// <summary>
	/// Описывает объект-сериализатор, превращающий входной объект в поток симвлолв нужного формата
	/// </summary>
	public interface ISerializer {
        /// <summary>
        /// Сериализует переданный объект в текстовой поток
        /// </summary>
        /// <param name="name"> Имя сериализуемого объекта</param>
        /// <param name="value">Сериализуемый объект </param>
        /// <param name="output">Целевой текстововй поток</param>
        /// <remarks>
        /// Такое определение интерфейса предполагает, что сериализация производится в поток,
        /// но при этом мы не предполагаем бинарной сериализации, так как бинарная сериализация
        /// не является типовым сценарием для коммутриуемх API
        /// </remarks>
		void Serialize(string name, object value, TextWriter output);
	}
}