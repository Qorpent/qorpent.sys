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
// PROJECT ORIGIN: Qorpent.Core/ITypeResolver.cs
#endregion
using System;
using System.Collections;
using System.Collections.Generic;

namespace Qorpent.IoC {
	/// <summary>
	/// ��������� ����������� ��������
	/// </summary>
	public interface ITypeResolver {

		/// <summary>
		/// ��������� ��� ���������� �����
		/// </summary>
		int Idx { get; set; }
		/// <summary>
		/// 	���������� ������ ���������� ����. (����������)
		/// </summary>
		/// <typeparam name="T"> ��� ������� </typeparam>
		/// <param name="name"> ������������ ��� ����������, ���� ������� - ����� ����� ������������� ������ ����� � ����������� � �������� ������ </param>
		/// <param name="ctorArguments"> ��������� ��� ������ ������������, ���� �� ������� - ����� ����������� ����������� �� ���������. </param>
		/// <returns> ������ ���������� ���� ��� <c>null</c> , ���� ���������� ��������� �� ��������� </returns>
		/// <exception cref="ContainerException">��� ������� � ��������������� ��������</exception>
		/// <remarks>
		/// 	<invariant>������ ����� �������� ������� ��������� ���
		/// 		<see cref="Get" />
		/// 		, ��. ������������ ������� ������ ��� ����� ��������� ����������</invariant>
		/// </remarks>
		T Get<T>(string name = null, params object[] ctorArguments) where T : class;

		///<summary>
		///	���������� ������ ���������� ����. (������ �������� ����)
		///</summary>
		///<param name="type"> ��� ������� </param>
		///<param name="ctorArguments"> ��������� ��� ������ ������������, ���� �� ������� - ����� ����������� ����������� �� ���������. </param>
		///<param name="name"> ������������ ��� ����������, ���� ������� - ����� ����� ������������� ������ ����� � ����������� � �������� ������ </param>
		///<returns> ������ ���������� ���� ��� <c>null</c> , ���� ���������� ��������� �� ��������� </returns>
		///<exception cref="ContainerException">��� ������� � ��������������� ��������</exception>
		///<remarks>
		///	��� ������ ������, ��������� ������������ ����� � ����� ������������ � ������� ���������, ������������ ��������������� ������� �� ����� ������� 
		///	� ���. ���� ��������� �� ���������, �� ������������ null. <note>��� ��������� ���������
		///		                                                          <see cref="IContainer.ThrowErrorOnNotExistedComponent" />
		///		                                                          =
		///		                                                          <c>true</c>
		///		                                                          ,
		///		                                                          ������ �������� null ����� ������������� ��������� ����������
		///		                                                          <see cref="ContainerException" />
		///		                                                          .</note>
		///	<invariant>
		///		<h3>���������� �����������</h3>
		///		<list type="bullet">
		///			<item>� �������� ������ ������� ����� ��������� ��� �������� ��������� ���������� �������,  ��� � ������� �����/���������</item>
		///			<item>����� ������� ������� ����� ������������������ (��.
		///				<see cref="IComponentRegistry.Register" />
		///				,
		///				<see cref="ContainerFactory" />
		///				,
		///				<see cref="IComponentSource.GetComponents" />
		///				) �����������.</item>
		///			<item>���� ��� �� ������� �� ����� �������
		///				<strong>��������� ���</strong>
		///				���������� (� �� ����� ����������� � ������ "" ��� null)</item>
		///			<item>��� ��� �� ����� �����/������ ����� ���� ���������� ��������� �����������, �� ������������ ����� �������� ������������:
		///				<list type="number">
		///					<item>���� ���� �� ����������� ���� ������������ �� ���� ������� ��������������, �� �� ����� ��������� ����� ����, ������� ���������������� ����� �������������� ���������
		///						<note>�� ������ ������ ��� ������� ����������, ������ �� ���� ���� �������� �������� ����� ������ ���������� � �������, �����
		///							��� ����� �� �������������� �������� - ��� ������������� "����" ����������. �������������� ��� ����������� ����� ������� ��������
		///							�� "���������� ������ ���������� � ��������", � ��� "���������� ������� �������, � ����� ���� �������� ������������ �����".
		///							��� ������ ����� ������� ��������� �� ������� ��� ������� ��������� ����������� �� ���������� ������� ��� ������� ���������� ����� �������������,
		///							�� ��� ���� �� �� ������ �������������� ��� �������� ��� ������������ ������� ������� ��� ��������</note>
		///					</item>
		///					<item>���� � ���������� ���� ������ ��������� (��.
		///						<see cref="IComponentDefinition.Priority" />
		///						,
		///						<see cref="ContainerAttribute.Priority" />
		///						,
		///						<see cref="ContainerAttribute.Priority" />
		///						),
		///						�� ������������ ��������� �
		///						<strong>�������</strong>
		///						�����������</item>
		///					<item>��� ��������� ��� ���������� �����������, ������������ ��������� ������������������
		///						<strong>���������</strong>
		///					</item>
		///				</list>
		///			</item>
		///		</list>
		///		<h3>������������ �������� � ����� �����</h3>
		///		<para> ����� ����� ������������� � ������� ����������� ( <see cref="IComponentDefinition.Lifestyle" /> ). � ������������ �� ��� �������� � <see
		///	 cref="Lifestyle" /> </para>
		///		<para> ����� ����� ������ �� ��������� ���������� � ������ Get. </para>
		///		<list type="number">
		///			<listheader>������� ������ ����������</listheader>
		///			<item>����� ���������� (������� ����), ����� ���������� ����������� ����������</item>
		///			<item>������������� ��� �������� ���������� (������� �� ����� ����� ����������)</item>
		///			<item>���� ��� ����� ���������, �� ������������� ������������ ������ ������������</item>
		///			<item>��������� ���������� (����������� ���������� ������, ���������� ���������� ���������)</item>
		///			<item>������� ���������� ����������� ����</item>
		///		</list>
		///		<h3>�������� �����������</h3>
		///		���� ���� ����� ������������� �������� ������ ����������, �� ��� �������� ������������ �� ���������� ���������:
		///		<list type="number">
		///			<item>����������
		///				<see
		///					cref="Activator.CreateInstance(System.Type,System.Reflection.BindingFlags,System.Reflection.Binder,object[],System.Globalization.CultureInfo)" />
		///				,
		///				� ���������� ��������� ������������� � ���������
		///				<paramref name="ctorArguments" />
		///			</item>
		///			<item>����������� ������ ���������� ����������
		///				<see cref="IComponentDefinition.Parameters" />
		///				, ������ ������������ � ���������� ������ 
		///				(���� �������� �� ������� ��� ��� �� �������������, �� ������ �� ������������)</item>
		///			<item>������������ ������ �������� �� ������
		///				<see cref="ContainerComponentAttribute" />
		///			</item>
		///			<item>���� ��������� - ���������
		///				<see cref="IContainerBound" />
		///				(��. �����
		///				<see cref="ServiceBase" />
		///				), �� ������������
		///				������
		///				<see cref="IContainerBound.SetContainerContext" />
		///				, ��� ���������� ���������������� ��������� �� ���������</item>
		///		</list>
		///	</invariant>
		///</remarks>
		object Get(Type type, string name = null, params object[] ctorArguments);

		///<summary>
		///	���������� ��� ������� ��������� ���� (�����������)
		///</summary>
		///<typeparam name="T"> ��� ������� </typeparam>
		///<param name="ctorArguments"> ��������� ��� ������ ������������, ���� �� ������� - ����� ����������� ����������� �� ���������. </param>
		///<param name="name"> ������������ ��� ����������, ���� ������� - ����� ����� ������������� ������ ����� � ����������� � �������� ������ </param>
		///<returns> ��� ���������� ���������� ������� </returns>
		///<remarks>
		///	<invariant>����� All �������� ������ ��� ����������� � ������ �����
		///		<see cref="Lifestyle.Transient" />
		///		�
		///		<see cref="Lifestyle.Extension" />
		///		.
		///		<note>�� ��������� ����� ������� �������� ��� ���������� �������� � ������ ������ �����</note>
		///		<para> ��������� ����������� ��������� ���������� ��� ������ � �������� ����������� ������� � ������������ � <see
		///	 cref="Get" /> </para>
		///	</invariant>
		///</remarks>
		IEnumerable<T> All<T>(string name = null, params object[] ctorArguments) where T : class;

		/// <summary>
		/// 	���������� ��� ������� ��������� ���� (������ �������� ����)
		/// </summary>
		/// <param name="ctorArguments"> ��������� ��� ������ ������������, ���� �� ������� - ����� ����������� ����������� �� ���������. </param>
		/// <param name="type"> ��� ������� </param>
		/// <param name="name"> ������������ ��� ����������, ���� ������� - ����� ����� ������������� ������ ����� � ����������� � �������� ������ </param>
		/// <returns> ��� ���������� ���������� ������� </returns>
		/// <remarks>
		/// 	<invariant>����� All �������� ������ ��� ����������� � ������ �����
		/// 		<see cref="Lifestyle.Transient" />
		/// 		�
		/// 		<see cref="Lifestyle.Extension" />
		/// 		.
		/// 		<note>�� ��������� ����� ������� �������� ��� ���������� �������� � ������ ������ �����</note>
		/// 	</invariant>
		/// </remarks>
		IEnumerable All(Type type, string name = null, params object[] ctorArguments);
		
		/// <summary>
		/// 	���������� ������ ����������
		/// </summary>
		/// <param name="obj"> ������, ����� �������� ����������� </param>
		/// <remarks>
		/// 	<invariant>����� ��������� ���������� ������������ ����������� ������� � ��������
		/// 		<see cref="IContainerBound.OnContainerRelease" />
		/// 		�����,
		/// 		�������� ��� ������ ���
		/// 		<see cref="Lifestyle.Pooled" />
		/// 		�
		/// 		<see cref="Lifestyle.PerThread" />
		/// 		, ��� ��������� ������ ����� ������������</invariant>
		/// </remarks>
		void Release(object obj);

	    IEnumerable All2(Type type, string name, params object[] ctorArguments);
	}
}