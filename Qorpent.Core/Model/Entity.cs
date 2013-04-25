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
// PROJECT ORIGIN: Qorpent.Core/Entity.cs
#endregion
using System;

namespace Qorpent.Model {
	/// <summary>
	/// Simple <see cref="IEntity"/> implementation
	/// </summary>
	public class Entity : IEntity {
		/// <summary>
		/// PK ID in database terms
		/// </summary>
		public virtual int Id { get; set; }

		/// <summary>
		/// Unique memo-code
		/// </summary>
		public virtual string Code { get; set; }

		/// <summary>
		///Name of the entity
		/// </summary>
		public virtual string Name { get; set; }


		/// <summary>
		/// User-defined order index
		/// </summary>
		public virtual int Index { get; set; }


		/// <summary>
		///The TAG string
		/// </summary>
		public virtual string Tag { get; set; }


		/// <summary>
		/// User's comment string
		/// </summary>
		public virtual string Comment { get; set; }

		/// <summary>
		/// User's or system's time stamp
		/// </summary>
		public virtual DateTime Version { get; set; }
	}
}