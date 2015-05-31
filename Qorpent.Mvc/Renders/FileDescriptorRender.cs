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
// PROJECT ORIGIN: Qorpent.Mvc/FileDescriptorRender.cs
#endregion
using System;
using System.IO;
using System.Web.Security;
using Qorpent.IO;
using Qorpent.Security;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Renders {
	/// <summary>
	/// 	Рендер для обработки и отправки в выводящий поток самого содержимого объекта <see cref="IFileDescriptor" />
	/// </summary>
	[Render("filedesc", Help = "Возвращает стандартные IFileDescriptor")]
	public class FileDescriptorRender : RenderBase {
		/// <summary>
		/// 	Отрисовывает переданный <see cref="IFileDescriptor" /> как контент с указанным MIME
		/// </summary>
		/// <param name="context"> </param>
		public override void Render(IMvcContext context) {
			var descriptor = context.ActionResult as IFileDescriptor;
			if (null == descriptor) {
				throw new QorpentException("Тип результата NULL или не совместим с FileDescriptorRender");
			}
			if (descriptor.Role.IsNotEmpty()) {
			    var auth = Application.Roles.IsInRole(context.User.Identity, descriptor.Role);
				if (!auth) {
					throw new QorpentSecurityException("Доступ к файлу не авторизован " + auth);
				}
			}
			if(descriptor.NeedDisposition) {
				var filename = descriptor.Name;
				var extension = Path.GetExtension(filename);
				if(string.IsNullOrWhiteSpace(extension)) {
					extension = MimeHelper.GetExtensionByMime(descriptor.MimeType);
					filename = filename + extension;
				}
				context.FileDisposition = filename;
			}
			
			context.ContentType = descriptor.MimeType;
			if(descriptor.IsStream) {
				context.WriteOutStream(descriptor.GetStream());
			}else {
				if (descriptor.Data != null) {
					context.WriteOutStream(new MemoryStream(descriptor.Data));
				}
				else {
					context.Output.Write(descriptor.Content);
				}
			}
		}

		/// <summary>
		/// 	Renders error, occured in given context
		/// </summary>
		/// <param name="error"> </param>
		/// <param name="context"> </param>
		public override void RenderError(Exception error, IMvcContext context) {
			context.ContentType = "text/plain";
			context.Output.Write(error.ToString());
		}
	}
}