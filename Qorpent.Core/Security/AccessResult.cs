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
// PROJECT ORIGIN: Qorpent.Core/AccessResult.cs
#endregion
using System.Collections.Generic;

namespace Qorpent.Security {
	/// <summary>
	/// 	��������� ���������� ������� ���� �� ������
	/// </summary>
	public class AccessResult {
		/// <summary>
		/// 	����������� ���������
		/// </summary>
		public AccessResultType ResultType { get; set; }

		/// <summary>
		/// 	�������� ������ �� �������� �������
		/// </summary>
		public IAccessProvider Provider { get; set; }

		/// <summary>
		/// 	��������� ��������� � ����������
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 	�������� ���-����������
		/// </summary>
		public IList<AccessResult> SubResults {
			get { return _subResults ?? (_subResults = new List<AccessResult>()); }
			set { _subResults = value; }
		}

		/// <summary>
		/// 	�������������� � True ���� ��������� �� <see cref="AccessResultType.Deny" />
		/// </summary>
		/// <param name="accessResult"> </param>
		/// <remarks>
		/// 	<see cref="AccessResultType.NotDefined" /> ����� ������������� ��� True �� ��������� ��������
		/// 	����� ������ � ���������� ������� ����������� ��-�� ����������� ���������� ���������� Access
		/// </remarks>
		/// <returns> </returns>
		public static implicit operator bool(AccessResult accessResult) {
			if (null == accessResult) {
				return true; //null - ���� true
			}
			return accessResult.ResultType != AccessResultType.Deny;
		}

		/// <summary>
		/// 	True - �������������� � AccessResult.Allow, False - � Deny
		/// </summary>
		/// <param name="boolResult"> </param>
		/// <returns> </returns>
		public static implicit operator AccessResult(bool boolResult) {
			if (boolResult) {
				return new AccessResult {ResultType = AccessResultType.Allow};
			}
			return new AccessResult {ResultType = AccessResultType.Deny};
		}


		/// <summary>
		/// 	������������ ������ � ��������� �������, ������ ������ ���������� � Allow , Deny
		/// </summary>
		/// <param name="quickDescriptor"> ������� ��������� ������� </param>
		/// <returns> </returns>
		public static implicit operator AccessResult(string quickDescriptor) {
			if (quickDescriptor == null) {
				return new AccessResult {ResultType = AccessResultType.NotDefined, Description = "null descriptor"};
			}
			var result = new AccessResult();
			if (quickDescriptor.StartsWith("Allow")) {
				result.ResultType = AccessResultType.Allow;
				result.Description = quickDescriptor.Substring(5).Trim();
			}
			else if (quickDescriptor.StartsWith("Deny")) {
				result.ResultType = AccessResultType.Deny;
				result.Description = quickDescriptor.Substring(4).Trim();
			}
			else {
				result.ResultType = AccessResultType.NotDefined;
				result.Description = quickDescriptor;
			}
			return result;
		}

		private IList<AccessResult> _subResults;
	}
}