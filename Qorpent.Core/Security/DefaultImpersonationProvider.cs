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
// PROJECT ORIGIN: Qorpent.Core/DefaultImpersonationProvider.cs
#endregion
using System.Collections.Generic;
using System.Security.Principal;
using Qorpent.IoC;

namespace Qorpent.Security {
	/// <summary>
	/// 	����������� ���������� <see cref="IImpersonationProvider" />
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton)]
	public class DefaultImpersonationProvider : IImpersonationProvider {
		/// <summary>
		/// 	����������� ������������ ����� ������� ������ � ������
		/// </summary>
		/// <param name="srcUser"> </param>
		/// <param name="resultUser"> null - ������� ������������ </param>
		public void Impersonate(IPrincipal srcUser, IPrincipal resultUser) {
			lock (this) {
				var key = GetKey(srcUser);
				_impersonationMap[key] = resultUser;
			}
		}

		/// <summary>
		/// 	True - ��������� ������ ����� ����������������� ����������
		/// </summary>
		/// <param name="usr"> </param>
		/// <returns> </returns>
		public bool IsImpersonated(IPrincipal usr) {
			lock (this) {
				var key = GetKey(usr);
				if (!_impersonationMap.ContainsKey(key)) {
					return false;
				}
				if (null == _impersonationMap[key]) {
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// 	���������� ����������������� ������� ������� ������
		/// </summary>
		/// <param name="usr"> </param>
		/// <returns> </returns>
		public IPrincipal GetImpersonation(IPrincipal usr) {
			lock (this) {
				if (IsImpersonated(usr)) {
					return _impersonationMap[GetKey(usr)];
				}
				return usr;
			}
		}


		private static string GetKey(IPrincipal usr) {
			var key = usr.Identity.Name.Replace("\\", "/").ToUpperInvariant();
			return key;
		}

		private readonly Dictionary<string, IPrincipal> _impersonationMap = new Dictionary<string, IPrincipal>();
	}
}