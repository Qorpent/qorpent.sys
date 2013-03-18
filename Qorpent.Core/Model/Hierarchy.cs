using System;
using System.Collections.Generic;

namespace Qorpent.Model {
	/// <summary>
	/// <see cref="Entity"/> that implements hierarchy pattern
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public  class Hierarchy<T> : Entity, IWithHierarchy<T> where T:class,IWithId,IWithCode,IWithHierarchy<T> {
		private ICollection<T> _children;
		private int? _parentId;
		private T _parent;
		private string _path;
		private string _parentCode;

		/// <summary>
		/// 	������ ������������� ��������
		/// </summary>
		/// <exception cref="Exception">cannot set ParentId when <see cref="Parent"/> is defined</exception>
		public int? ParentId {
			get {
				return null != _parent ? _parent.Id : _parentId;
			}
			set {
				if (null != _parent) {
					throw new Exception("cannot set ParentId when Parent is defined");
				}
				_parentId = value;
			}
		}


		/// <summary>
		///		CODE of parent
		/// </summary>
		/// <exception cref="Exception">cannot set ParentCode when <see cref="Parent"/> is defined</exception>
		public string ParentCode {
			get { return null != _parent ? _parent.Code : _parentCode; }
			set {
				if (null != _parent)
				{
					throw new Exception("cannot set ParentCode when Parent is defined");
				}
				_parentCode = value;
			}
		}

		/// <summary>
		/// 	������ �� ������������ ������
		/// </summary>
		public T Parent {
			get {
				return _parent;
			}
			set {
				_parent = value;
			}
		}

		/// <summary>
		/// 	��������� �������� ��������
		/// </summary>
		public ICollection<T> Children {
			get { return _children ??(_children= new List<T>()); }
			set { _children = value; }
		}

		/// <summary>
		/// 	Cached path of entity
		/// </summary>
		public string Path {
			get { return _path ?? (_path =EvaluatePath()); }
			set { _path = value; }
		}

		/// <summary>
		/// Evaluates default path of entity
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception">cannot evaluate path when code is not defined</exception>
		protected virtual string EvaluatePath() {
			return this.EvaluateDefaultPath();
		}

		/// <summary>
		///		Checks if children existed
		/// </summary>
		/// <returns></returns>
		public bool HasChildren() {
			return null != _children && 0!=_children.Count;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool HasParent() {
			return null != _parent;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsParentDefined() {
			return  _parentId.HasValue || null!=_parent || !string.IsNullOrWhiteSpace(_parentCode);
		}
	}
}