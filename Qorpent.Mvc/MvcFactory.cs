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
// PROJECT ORIGIN: Qorpent.Mvc/MvcFactory.cs
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Qorpent.Events;
using Qorpent.IoC;
using Qorpent.Mvc.Binding;
using Qorpent.Mvc.QView;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Default Mvc actions and renders factory
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton)]
	[RequireReset(Role = "DEVELOPER", Options = new[] {QorpentConst.MvcFactoryResetCode})]
	public class MvcFactory : ServiceBase, IMvcFactory {
		private static readonly string[] ViewLevels = new[] {"usr", "mod", "sys", "root", "code"};
#if PARANOID
		static MvcFactory() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif


		/// <summary>
		/// 	Registers all mvcfactory-managed items from given assembly
		/// </summary>
		/// <param name="assembly"> </param>
		/// <returns> </returns>
		public IMvcFactory Register(Assembly assembly ) {
			try {
				var types = from t in assembly.GetTypes()
				            where
					            (typeof (IAction).IsAssignableFrom(t) ||
					             typeof (IRender).IsAssignableFrom(t) ||
					             typeof (IQView).IsAssignableFrom(t)) &&
					            !t.IsAbstract
				            select t;
				foreach (var type in types) {
				    if (null != type.GetCustomAttribute<ContainerComponentAttribute>(true)) {
				        Register(type);
				    }
				}
				return this;
			}catch(ReflectionTypeLoadException e) {
				var strings = e.LoaderExceptions.Select(x => x.ToString());
				throw new Exception("assembly loader exception in "+assembly.FullName+ "\r\n"+string.Join("\r\n=====\r\n",strings));
			}
		}

		/// <summary>
		/// 	Создает новый экземпляр обработчика
		/// </summary>
		/// <returns> </returns>
		public IMvcHandler CreateHandler() {
			return ResolveService<IMvcHandler>();
		}

		/// <summary>
		/// 	Создает контекст относительно URL
		/// </summary>
		/// <param name="url"> </param>
		/// <returns> </returns>
		public IMvcContext CreateContext(string url = null) {
			return ResolveService<IMvcContext>(null, url);
		}

		/// <summary>
		/// 	Wrapper for container setup for Actions, Renders, Views
		/// </summary>
		/// <param name="type"> </param>
		/// <returns> </returns>
		public IMvcFactory Register(Type type) {
			if (typeof (IAction).IsAssignableFrom(type)) {
				RegisterAction(type);
			}
			else if (typeof (IRender).IsAssignableFrom(type)) {
				RegisterRender(type);
			}
			else if (typeof (IQView).IsAssignableFrom(type)) {
				RegisterView(type);
			}
			else {
				throw new MvcExecption("not supported type " + type);
			}
			return this;
		}

		/// <summary>
		/// 	clears all resolution caches
		/// </summary>
		public void ClearCaches(MvcObjectType type = MvcObjectType.All, string name = null) {
			lock (Sync) {
				if (0 != (type | MvcObjectType.Action)) {
					if (name.IsNotEmpty()) {
						_actionPool.Remove(NormalizeActionName(name));
					}
					else {
						_actionPool.Clear();
					}
				}
				if (0 != (type | MvcObjectType.Render)) {
					if (name.IsNotEmpty()) {
						_renderPool.Remove(NormalizeRenderName(name));
					}
					else {
						_renderPool.Clear();
					}
				}
				if (0 != (type | MvcObjectType.View)) {
					if (name.IsNotEmpty()) {
						Debug.Assert(name != null, "name != null");
						if (_viewPool.ContainsKey(name)) {
							_viewPool.Remove(name);
						}
						_viewContainerNameCache.Remove(name);
					}
					else {
						_viewPool.Clear();
						_viewContainerNameCache.Clear();
					}
				}
			}
		}

		/// <summary>
		/// 	Creates action descriptor for context
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		public ActionDescriptor GetAction(IMvcContext context) {
			lock (Sync) {
				var result = TryResolveActionFromPool(context.ActionName) ?? GenerateNewActionDescriptor(context.ActionName);

				return result;
			}
		}

		/// <summary>
		/// 	Releases action after context used
		/// </summary>
		/// <param name="context"> </param>
		public void ReleaseAction(IMvcContext context) {
			lock (Sync) {
				if (null != context.ActionDescriptor) {
					PushActionToPool(context.ActionDescriptor);
				}
			}
		}

		/// <summary>
		/// 	Creates render for context
		/// </summary>
		/// <param name="context"> </param>
		/// <returns> </returns>
		public RenderDescriptor GetRender(IMvcContext context) {
			lock (Sync) {
				var result = TryResolveRenderFromPool(context.RenderName) ?? GenerateNewRenderDescriptor(context.RenderName);

				return result;
			}
		}

		/// <summary>
		/// 	Release render  after usage
		/// </summary>
		/// <param name="context"> </param>
		public void ReleaseRender(IMvcContext context) {
			lock (Sync) {
				if (null != context.RenderDescriptor) {
					PushRenderPool(context.RenderDescriptor);
				}
			}
		}

		/// <summary>
		/// 	Return action binder (ioc based service)
		/// </summary>
		/// <returns> </returns>
		public IActionBinder GetBinder() {
			return ResolveService<IActionBinder>() ?? new DefaultActionBinder();
		}

		/// <summary>
		/// 	Get IQView instance for given name
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public IQView GetView(string name) {
			lock (Sync) {
				name = NormalizeQviewName(name);
				var result = TryResolveViewPool(name) ?? GenerateNewView(name);

				return result;
			}
		}

		/// <summary>
		/// 	Releases used IQView
		/// </summary>
		/// <param name="viewname"> </param>
		/// <param name="view"> </param>
		public void ReleaseView(string viewname, IQView view) {
			PushViewToPool(viewname, view);
		}

		/// <summary>
		/// 	Проверяет наличие вида и кэширует его экземпляр
		/// </summary>
		/// <param name="viewname"> </param>
		/// <returns> </returns>
		public bool ViewExists(string viewname) {
			if (viewname.IsEmpty()) {
				return false;
			}
			var name = NormalizeQviewName(viewname);
			if (_viewContainerNameCache.ContainsKey(name)) {
				return true;
			}
			foreach (var viewLevel in ViewLevels) {
				var componentname = name + "." + viewLevel + ".view";
				var view = ResolveService<IQView>(componentname);
				if (null != view) {
					ReleaseView(name, view);
					_viewContainerNameCache[viewname] = name;
					return true;
				}
			}
			return false;
		}


		private void RegisterView(Type type) {
			var name = QViewAttribute.GetName(type);
			var level = QViewAttribute.GetLevel(type);
			var componentname = name.ToLower() + "." + level.ToStr().ToLower() + ".view";
			var component = Container.EmptyComponent();
			component.ServiceType = typeof (IQView);
			component.ImplementationType = type;
			component.Name = componentname;
			component.Lifestyle = Lifestyle.Transient;
			Container.Register(component);
			ClearCaches(MvcObjectType.View, name);
		}

		private void RegisterRender(Type type) {
			var name = NormalizeRenderName(RenderAttribute.GetName(type));
			var component = Container.EmptyComponent();
			component.Name = name;
			component.ServiceType = typeof (IRender);
			component.ImplementationType = type;
			component.Lifestyle = Lifestyle.Transient;
			Container.Register(component);
			ClearCaches(MvcObjectType.Render, name);
		}

		private void RegisterAction(Type type) {
			var name = NormalizeActionName(ActionAttribute.GetName(type));
			var component = Container.EmptyComponent();
			component.Name = name;
			component.ServiceType = typeof (IAction);
			component.ImplementationType = type;
			component.Lifestyle = Lifestyle.Transient;
			Container.Register(component);
			ClearCaches(MvcObjectType.Action, name);
		}

		/// <summary>
		/// 	Сбрасывает все локальные кэши, не модифицируется по параметрам события
		/// </summary>
		/// <param name="data"> </param>
		/// <returns> </returns>
		public override object Reset(ResetEventData data) {
			lock (Sync) {
				ClearCaches();
				return null;
			}
		}

		/// <summary>
		/// 	Возвращает размер кэшей
		/// </summary>
		/// <returns> </returns>
		public override object GetPreResetInfo() {
			return new {actions = _actionPool.Count, renders = _renderPool.Count, views = _viewPool.Count};
		}


	    public object GetMetric(string name) {
	        if (name == "action.pool.count") {
	            return _actionPool.Values.Sum(_=>_.Count);
	        }
	        return null;
	    }

		private ActionDescriptor GenerateNewActionDescriptor(string actionName) {
			var name = NormalizeActionName(actionName);
			var implementation = ResolveService<IAction>(name);
			if (null == implementation) {
				throw new ActionNotFoundException("cannot find " + name);
			}
			var result = new ActionDescriptor(implementation) {Factory = this};
			var contextualAction = implementation as IContextualAction;
			if (contextualAction != null) {
				contextualAction.SetDescriptor(result);
			}
			return result;
		}

		private IQView GenerateNewView(string viewname) {
			IQView view = null;
			if (_viewContainerNameCache.ContainsKey(viewname)) {
				view = ResolveService<IQView>(_viewContainerNameCache[viewname]);
			}
			else {
				foreach (var viewLevel in ViewLevels) {
					var name = viewname + "." + viewLevel + ".view";
					view = ResolveService<IQView>(name);
					if (null != view) {
						_viewContainerNameCache[viewname] = name;
						break;
					}
				}
			}
			if (null == view) {
				throw new ViewNotFoundException(viewname);
			}

			return view;
		}

		private ActionDescriptor TryResolveActionFromPool(string actionName) {
			var name = NormalizeActionName(actionName);
			if (_actionPool.ContainsKey(name) && _actionPool[name].Count != 0) {
				return _actionPool[name].Pop();
			}
			return null;
		}

		private IQView TryResolveViewPool(string viewname) {
			if (_viewPool.ContainsKey(viewname) && _viewPool[viewname].Count != 0) {
				return _viewPool[viewname].Pop();
			}
			return null;
		}

		private void PushViewToPool(string viewname, IQView view) {
			if (!_viewPool.ContainsKey(viewname)) {
				_viewPool[viewname] = new Stack<IQView>();
			}
			_viewPool[viewname].Push(view);
		}

		private void PushActionToPool(ActionDescriptor action) {
			var name = NormalizeActionName(action.Name);
			if (!_actionPool.ContainsKey(name)) {
				_actionPool[name] = new Stack<ActionDescriptor>();
			}
			_actionPool[name].Push(action);
		}

		private RenderDescriptor GenerateNewRenderDescriptor(string renderName) {
			var name = NormalizeRenderName(renderName);
			var implementation = ResolveService<IRender>(name);
			if (null == implementation) {
				throw new RenderNotFoundException("cannot find " + name);
			}
			var result = new RenderDescriptor(implementation) {Factory = this};
			return result;
		}

		private RenderDescriptor TryResolveRenderFromPool(string renderName) {
			var name = NormalizeRenderName(renderName);
			if (_renderPool.ContainsKey(name) && _renderPool[name].Count != 0) {
				return _renderPool[name].Pop();
			}
			return null;
		}

		private void PushRenderPool(RenderDescriptor renderDescriptor) {
			var name = NormalizeRenderName(renderDescriptor.Name);
			if (!_renderPool.ContainsKey(name)) {
				_renderPool[name] = new Stack<RenderDescriptor>();
			}
			_renderPool[name].Push(renderDescriptor);
		}

		/// <summary>
		/// 	Normilize given action name to match cache/container naming
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public static string NormalizeActionName(string name) {
			var result = name.ToLower();
			if (!result.EndsWith(".action")) {
				result += ".action";
			}
			return result;
		}

		/// <summary>
		/// 	Normilize given render name to match cache/container naming
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public static string NormalizeRenderName(string name) {
			var result = name.ToLower();
			if (!result.EndsWith(".render")) {
				result += ".render";
			}
			return result;
		}


		/// <summary>
		/// 	Normalize given qview name
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public static string NormalizeQviewName(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			var result = name;
			if (name.StartsWith("/")) {
				result = name.Substring(1);
			}
			return result;
		}

		private readonly IDictionary<string, Stack<ActionDescriptor>> _actionPool =
			new Dictionary<string, Stack<ActionDescriptor>>();

		private readonly IDictionary<string, Stack<RenderDescriptor>> _renderPool =
			new Dictionary<string, Stack<RenderDescriptor>>();

		private readonly IDictionary<string, string> _viewContainerNameCache = new Dictionary<string, string>();
		private readonly IDictionary<string, Stack<IQView>> _viewPool = new Dictionary<string, Stack<IQView>>();
	}
}