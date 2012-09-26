#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : MvcFactory.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
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

		/// <summary>
		/// 	Reference to QView monitor instance
		/// </summary>
		[Inject] public IQViewMonitor QViewMonitor { get; set; }


		/// <summary>
		/// 	Registers all mvcfactory-managed items from given assembly
		/// </summary>
		/// <param name="assembly"> </param>
		/// <returns> </returns>
		public IMvcFactory Register(Assembly assembly) {
			var types = from t in assembly.GetTypes()
			            where
				            (typeof (IAction).IsAssignableFrom(t) ||
				             typeof (IRender).IsAssignableFrom(t) ||
				             typeof (IQView).IsAssignableFrom(t)) &&
				            !t.IsAbstract
			            select t;
			foreach (var type in types) {
				Register(type);
			}
			return this;
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
			lock (this) {
				if (0 != (type | MvcObjectType.Action)) {
					if (name.IsNotEmpty()) {
						actionPool.Remove(NormalizeActionName(name));
					}
					else {
						actionPool.Clear();
					}
				}
				if (0 != (type | MvcObjectType.Render)) {
					if (name.IsNotEmpty()) {
						renderPool.Remove(NormalizeRenderName(name));
					}
					else {
						renderPool.Clear();
					}
				}
				if (0 != (type | MvcObjectType.View)) {
					if (name.IsNotEmpty()) {
						viewPool.Remove(name);
						viewContainerNameCache.Remove(name);
					}
					else {
						viewPool.Clear();
						viewContainerNameCache.Clear();
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
			lock (this) {
				var result = TryResolveActionFromPool(context.ActionName);
				if (null == result) {
					result = GenerateNewActionDescriptor(context.ActionName);
				}

				return result;
			}
		}

		/// <summary>
		/// 	Releases action after context used
		/// </summary>
		/// <param name="context"> </param>
		public void ReleaseAction(IMvcContext context) {
			lock (this) {
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
			lock (this) {
				var result = TryResolveRenderFromPool(context.RenderName);
				if (null == result) {
					result = GenerateNewRenderDescriptor(context.RenderName);
				}

				return result;
			}
		}

		/// <summary>
		/// 	Release render  after usage
		/// </summary>
		/// <param name="context"> </param>
		public void ReleaseRender(IMvcContext context) {
			lock (this) {
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
			lock (this) {
				name = NormalizeQviewName(name);
				var result = TryResolveViewPool(name);
				if (null == result) {
					result = GenerateNewView(name);
				}

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
			if(viewname.IsEmpty()) return false;
			var name = NormalizeQviewName(viewname);
			if (viewContainerNameCache.ContainsKey(name)) {
				return true;
			}
			foreach (var viewLevel in ViewLevels) {
				var componentname = name + "." + viewLevel + ".view";
				var view = ResolveService<IQView>(componentname);
				if (null != view) {
					ReleaseView(name, view);
					viewContainerNameCache[viewname] = name;
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
			ClearCaches();
			return null;
		}

		/// <summary>
		/// 	Возвращает размер кэшей
		/// </summary>
		/// <returns> </returns>
		public override object GetPreResetInfo() {
			return new {actions = actionPool.Count, renders = renderPool.Count, views = viewPool.Count};
		}


		/// <summary>
		/// 	called on object after creation in IoC with current component context
		/// 	object can perform container bound logic here
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <param name="component"> The component. </param>
		/// <remarks>
		/// </remarks>
		public override void SetContainerContext(IContainer container, IComponentDefinition component) {
			base.SetContainerContext(container, component);

			//Register(Assembly.GetExecutingAssembly());

			if (null != QViewMonitor) {
				QViewMonitor.SetFactory(this);
				QViewMonitor.Startup();
				QViewMonitor.StartMonitoring();
			}
		}

		private ActionDescriptor GenerateNewActionDescriptor(string actionName) {
			var name = NormalizeActionName(actionName);
			var implementation = ResolveService<IAction>(name);
			if (null == implementation) {
				throw new ActionNotFoundException("cannot find " + name);
			}
			var result = new ActionDescriptor(implementation) {Factory = this};
			if (implementation is IContextualAction) {
				((IContextualAction) implementation).SetDescriptor(result);
			}
			return result;
		}

		private IQView GenerateNewView(string viewname) {
			IQView view = null;
			if (viewContainerNameCache.ContainsKey(viewname)) {
				view = ResolveService<IQView>(viewContainerNameCache[viewname]);
			}
			else {
				foreach (var viewLevel in ViewLevels) {
					var name = viewname + "." + viewLevel + ".view";
					view = ResolveService<IQView>(name);
					if (null != view) {
						viewContainerNameCache[viewname] = name;
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
			if (actionPool.ContainsKey(name) && actionPool[name].Count != 0) {
				return actionPool[name].Pop();
			}
			return null;
		}

		private IQView TryResolveViewPool(string viewname) {
			if (viewPool.ContainsKey(viewname) && viewPool[viewname].Count != 0) {
				return viewPool[viewname].Pop();
			}
			return null;
		}

		private void PushViewToPool(string viewname, IQView view) {
			if (!viewPool.ContainsKey(viewname)) {
				viewPool[viewname] = new Stack<IQView>();
			}
			viewPool[viewname].Push(view);
		}

		private void PushActionToPool(ActionDescriptor action) {
			var name = NormalizeRenderName(action.Name);
			if (!actionPool.ContainsKey(name)) {
				actionPool[name] = new Stack<ActionDescriptor>();
			}
			actionPool[name].Push(action);
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
			if (renderPool.ContainsKey(name) && renderPool[name].Count != 0) {
				return renderPool[name].Pop();
			}
			return null;
		}

		private void PushRenderPool(RenderDescriptor renderDescriptor) {
			var name = NormalizeRenderName(renderDescriptor.Name);
			if (!renderPool.ContainsKey(name)) {
				renderPool[name] = new Stack<RenderDescriptor>();
			}
			renderPool[name].Push(renderDescriptor);
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
			if (name == null ) {
				throw new ArgumentNullException("name");
			}
			var result = name;
			if (name.StartsWith("/")) {
				result = name.Substring(1);
			}
			return result;
		}

		private readonly IDictionary<string, Stack<ActionDescriptor>> actionPool =
			new Dictionary<string, Stack<ActionDescriptor>>();

		private readonly IDictionary<string, Stack<RenderDescriptor>> renderPool =
			new Dictionary<string, Stack<RenderDescriptor>>();

		private readonly IDictionary<string, string> viewContainerNameCache = new Dictionary<string, string>();
		private readonly IDictionary<string, Stack<IQView>> viewPool = new Dictionary<string, Stack<IQView>>();
	}
}