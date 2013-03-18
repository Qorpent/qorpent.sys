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

		/// <summary>
		/// 	Прямой идентификатор родителя
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
		/// 	Ссылка на родительский объект
		/// </summary>
		public T Parent {
			get { return _parent; }
			set { _parent = value; }
		}

		/// <summary>
		/// 	Коллекция дочерних объектов
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
			var selfcode = Code;
			if (string.IsNullOrWhiteSpace(selfcode)) {
				throw new Exception("cannot evaluate path when code is not defined");
			}
			if (null == _parent) return "/" + selfcode + "/";
			return _parent.Path + selfcode + "/";
		}

		/// <summary>
		///		Checks if children existed
		/// </summary>
		/// <returns></returns>
		public bool HasChildren() {
			return null != _children;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool HasParent() {
			return null != _parent;
		}
	}
}