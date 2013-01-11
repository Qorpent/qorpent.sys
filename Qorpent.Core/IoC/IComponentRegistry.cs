namespace Qorpent.IoC {
	/// <summary>
	/// ��������� ������� �����������
	/// </summary>
	public interface IComponentRegistry : IComponentSource {
		/// <summary>
		/// 	������������ ����� ��������� � ����������
		/// </summary>
		/// <param name="component"> ��������� ��� ����������� </param>
		/// <remarks>
		/// 	�� ������, �� �� ������� ������������ ���������� ����� ��������� ��� ������ <see cref="EmptyComponent" />, <see
		/// 	 cref="IContainer.NewExtension{TService}" />,
		/// 	<see cref="NewComponent{TService,TImplementation}" />, ������ ������������� ������ ������� �������� ���� ���������� �� ������������� ����� ������ - ����������
		/// 	<see cref="IComponentDefinition" />.<br />
		/// 	<invariant>� Qorpent ����� ����������� �� ��������� ������, � �������� � ����, �������������� ���
		/// 		<see cref="Unregister" />
		/// 		��� ������� �� ���������� �� ��������, 
		/// 		� ������������ � ����� ������ ������.
		/// 		<br />
		/// 		���������� ����������� ��� ���� ������� �� ������ "����� ����������" ������ �� ������� ��������� � ����������, ����� � ���� ����������
		/// 		<see cref="IComponentDefinition.Priority" />
		/// 		.
		/// 		<br />
		/// 		��������������
		/// 		<see cref="IComponentSource.GetComponents" />
		/// 		���������� ��� ������ ���� �����������.
		/// 		<br />
		/// 		��� �������� � ����������
		/// 		<see cref="Lifestyle.ContainerExtension" />
		/// 		, ��������� ���������������� �� ����������� �
		/// 		<see cref="IContainer.RegisterExtension" />
		/// 	</invariant>
		/// </remarks>
		void Register(IComponentDefinition component);

		/// <summary>
		/// 	�������� ����������� ����������
		/// </summary>
		/// <param name="component"> ���������, ������� ������ ���� ����� �� ���������� </param>
		/// <remarks>
		/// 	<note>�������� ���, ����� ����� ������ �������������� � ���������� ����������, ������ ��� ������������� � ������� ������ ������������,
		/// 		���������� ���������� ����������� ����������, ��� ��� � ���������� �������� ������������� ��������� ������ ���� ��������� �� �������� �����������
		/// 		������������� ������� � �����������, ����� ��� �������� ����� ��������� ����� �������� � ����������� �������� � ������ ����������� �����
		/// 		������ ������� ���������� ��������</note>
		/// </remarks>
		void Unregister(IComponentDefinition component);

		/// <summary>
		/// 	���������� �����-��������� ���������� �� ���������
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// 	���� ��������� ����� ������� � ��������� <see cref="IContainer" />, � ��
		/// 	������������ ����� � ����� ����������, ��� ��� �������� ��������� ��������� � ������������� ����������
		/// 	� �������������� ������������� ��� ������ <see cref="ITypeResolver.Get{T}" /> ��������� ������������
		/// </remarks>
		IContainerLoader GetLoader();

		/// <summary>
		/// 	������� ������ ��������� ��� ��������� (����������� �� ������������!)
		/// </summary>
		/// <returns> ������ ��������� <see cref="IComponentDefinition" /> </returns>
		/// <remarks>
		/// 	����������� ���� ����� � ����� ��������� <see cref="Register" />
		/// </remarks>
		IComponentDefinition EmptyComponent();

		/// <summary>
		/// 	������� ����� ��������� � ���������� �����������
		/// </summary>
		/// <param name="lifestyle"> ���� ����� ���������� </param>
		/// <param name="name"> ��� </param>
		/// <param name="priority"> �������������� </param>
		/// <param name="implementation"> ������� ��������� </param>
		/// <typeparam name="TService"> ��� ������� </typeparam>
		/// <typeparam name="TImplementation"> ��� ���������� </typeparam>
		/// <returns> ����-����������� ��������� <see cref="IComponentDefinition" /> </returns>
		/// <remarks>
		/// 	����������� ���� ����� � ����� ��������� <see cref="Register" />
		/// </remarks>
		IComponentDefinition NewComponent<TService, TImplementation>(Lifestyle lifestyle = Lifestyle.Transient,
		                                                             string name = "", int priority = 10000,
		                                                             TImplementation implementation = null)
			where TService : class where TImplementation : class, TService, new();
	}
}