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
// PROJECT ORIGIN: Qorpent.Core/ILogger.cs
#endregion
namespace Qorpent.Log {
    /// <summary>
    /// 	������� ���������� �������
    /// </summary>
	public interface ILogger {
		/// <summary>
		/// 	������� ������������
		/// </summary>
		LogLevel Level { get; set; }
		/// <summary>
		///     ���������, ��� ������ ����� �������������� (���� <see cref="bool"/> True), ���� � false
		/// </summary>
		bool Available { get; set; }
		/// <summary>
		/// 	��������-�������� ��� �������
		/// </summary>
		string Name { get; set; }
        /// <summary>
        ///     �������� ��������� � �������� ���� �������. ���� �������� �� �����������, �� �����
        ///     ������������ �������� ��������� �� ���������
        /// </summary>
		InternalLoggerErrorBehavior ErrorBehavior { get; set; }
		/// <summary>
		///     ���������, ��� ������ ������ �������� � ����������� ���������
		/// </summary>
		/// <param name="context">��������</param>
		/// <returns>True, ���� ��������</returns>
		bool IsApplyable(object context);
		/// <summary>
		///     �������� ����������� ������ <see cref="LogMessage"/> � ������� ���
		/// </summary>
		/// <param name="message"> </param>
		void StartWrite(LogMessage message);
        /// <summary>
        ///     �������������� ����� ��������� � �������
        /// </summary>
		void Join();
	}
}