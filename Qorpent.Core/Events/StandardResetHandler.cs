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
// PROJECT ORIGIN: Qorpent.Core/StandardResetHandler.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Model;
using Qorpent.Serialization;

namespace Qorpent.Events {
	/// <summary>
	/// 	Стандартный ресетер, настраиваемый на <see cref="IResetable" />
	/// </summary>
	[Serialize]
	public class StandardResetHandler : EventHandlerBase<ResetEvent>, IWithRole {
		/// <summary>
		/// 	Создает стандартный ресетер
		/// </summary>
		/// <param name="target"> Целевой объект для сброса </param>
		/// <param name="requireResetAttributeInfo"> Информация о параметрах сброса </param>
		public StandardResetHandler(IResetable target, RequireResetAttribute requireResetAttributeInfo = null) {
			_target = target;
			_info = requireResetAttributeInfo ??
			        _target.GetType().GetCustomAttributes(typeof (RequireResetAttribute), true).OfType<RequireResetAttribute>().
				        FirstOrDefault() ?? new RequireResetAttribute();
		}

		/// <summary>
		/// 	Имя целевого типа
		/// </summary>
		[Serialize] public string TargetTypeName {
			get { return _target.GetType().FullName; }
		}

		/// <summary>
		/// 	Информация об объекте на текущий момент (касающейся очистки)
		/// </summary>
		[Serialize] [SerializeNotNullOnly] public object PreResetInfo {
			get { return _target.GetPreResetInfo(); }
		}

		/// <summary>
		/// 	Признак вызова при опции All
		/// </summary>
		[Serialize] public bool AllIsSupported {
			get { return _info.All; }
		}

		/// <summary>
		/// 	Список всех поддерживаемых опций
		/// </summary>
		[Serialize] public string[] SupportedOptions {
			get {
				var result = new List<string>();
				if (AllIsSupported) {
					result.Add("all");
				}
				if (_info.UseClassNameAsOption) {
					result.Add(_target.GetType().Name.ToLowerInvariant());
				}
				if (null != _info.Options) {
					result.AddRange(_info.Options);
				}
				return result.ToArray();
			}
		}


		/// <summary>
		/// 	Роль (список ролей), которые необходимы для доступа (используются данные из <see cref="RequireResetAttribute.Role" />)
		/// </summary>
		public string Role {
			get { return _info.Role; }
			set { throw new NotSupportedException("роль не может быть изменена для StandardResetHandler"); }
		}


		/// <summary>
		/// 	Проверяет наличие включенных опций и вызывает <see cref="IResetable.Reset" /> при совпадении, анализ ведется на основе <see
		/// 	 cref="RequireResetAttribute" />
		/// </summary>
		/// <param name="e"> </param>
		public override void Process(ResetEvent e) {
			if (Proceed(e.Data)) {
				var customResult = _target.Reset(e.Data);
				var result = new ResetResultInfo {Name = _target.ToString(), Info = customResult};
				e.Result.InvokeList.Add(result);
			}
		}

		private bool Proceed(ResetEventData data) {
			if (_info.All && data.All) {
				return true;
			}
			if (_info.UseClassNameAsOption) {
				if (data.IsSet(_target.GetType().Name.ToLower(), false)) {
					return true;
				}
			}
			if (_info.Options != null) {
				return _info.Options.Any(option => data.IsSet(option, false));
			}
			return false;
		}

		private readonly RequireResetAttribute _info;
		private readonly IResetable _target;
	}
}