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
// PROJECT ORIGIN: Qorpent.Core/IImpersonationProvider.cs
#endregion
using System.Security.Principal;

namespace Qorpent.Security {
	/// <summary>
	/// 	Служба обеспечения темпоральной имперсонации учетной записи для внутреннего контекста
	/// </summary>
	public interface IImpersonationProvider {
		/// <summary>
		/// 	Регистриует имперсонацию одной учетной записи в другую
		/// </summary>
		/// <param name="srcUser"> </param>
		/// <param name="resultUser"> </param>
		void Impersonate(IPrincipal srcUser, IPrincipal resultUser);

		/// <summary>
		/// 	True - указанная запись имеет имперсонированный эквивалент
		/// </summary>
		/// <param name="usr"> </param>
		/// <returns> </returns>
		bool IsImpersonated(IPrincipal usr);

		/// <summary>
		/// 	Возвращает имперсонированную верисию учетной записи
		/// </summary>
		/// <param name="usr"> </param>
		/// <returns> </returns>
		IPrincipal GetImpersonation(IPrincipal usr);
	}
}