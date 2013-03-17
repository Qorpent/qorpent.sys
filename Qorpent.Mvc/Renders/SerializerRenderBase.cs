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
// PROJECT ORIGIN: Qorpent.Mvc/SerializerRenderBase.cs
#endregion
using System;
using Qorpent.Serialization;

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// 	Base class for usual by-serialize renders
	/// </summary>
	public abstract class SerializerRenderBase : RenderBase {
		/// <summary>
		/// 	return format to be used
		/// </summary>
		/// <returns> </returns>
		protected abstract SerializationFormat GetFormat();

		/// <summary>
		/// 	output mime type
		/// </summary>
		/// <returns> </returns>
		protected abstract string GetContentType();

		/// <summary>
		/// 	Renders given context
		/// </summary>
		/// <param name="context"> </param>
		public override void Render(IMvcContext context) {
			context.ContentType = GetContentType();
			if (null == context.ActionResult) {
				context.Output.Write("null");
			}
			else {
				GetSerializer().Serialize("result", context.ActionResult, context.Output);
			}
		}

		/// <summary>
		/// 	Renders error, occured in given context
		/// </summary>
		/// <param name="error"> </param>
		/// <param name="context"> </param>
		public override void RenderError(Exception error, IMvcContext context) {
			GetSerializer().Serialize("error", error, context.Output);
		}

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		protected ISerializer GetSerializer() {
			return ResolveService<ISerializerFactory>().GetSerializer(GetFormat());
		}
	}
}