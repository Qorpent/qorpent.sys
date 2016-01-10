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
// PROJECT ORIGIN: Qorpent.Core/RequireResetAttribute.cs
#endregion
using System;
using System.Linq;
using System.Security.Principal;
using Qorpent.Applications;
using Qorpent.Utils.Extensions;

namespace Qorpent.Events {
    /// <summary>
    /// 	Помечает класс для автоматической поддержки Reset-инфраструктуры, класс должен быть унаследован от <see
    /// 	 cref="IResetable" />
    /// </summary>
    /// <remarks>
    /// 	<see cref="ServiceBase" /> поддерживает автоматическую настройку на Reset исходя из этого атрибута и 
    /// 	использует его в собственном <see cref="IApplicationBound.SetApplication" />
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RequireResetAttribute : Attribute {
        /// <summary>
        /// 	Стандартный конструктор атрибута
        /// </summary>
        public RequireResetAttribute() {
            All = true;
            UseClassNameAsOption = true;
        }

        /// <summary>
        /// 	Если установлена данная роль, то для выполнения данного ресетера требуется наличие прав при вызове
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 	True (по умолчанию) - сервис будет реагировать на опцию сброса "все"
        /// </summary>
        public bool All { get; set; }

        /// <summary>
        /// 	True (по умолчанию) - имя класса является первичной опцией при оценке опций сброса (с игнором регистра)
        /// </summary>
        public bool UseClassNameAsOption { get; set; }

        /// <summary>
        /// 	Дополнительные опции, на которые будет реагировать класс (может быть пустым)
        /// </summary>
        public string[] Options { get; set; }

        public bool IsMatch(Type type,string opt) {
            var opts = opt.ToLowerInvariant().SmartSplit();
            if (All && (string.IsNullOrWhiteSpace(opt) || opts.Contains("all"))) return true;
            if (UseClassNameAsOption && opts.Contains(type.Name.ToLowerInvariant())) return true;
            if (opts.Any(_ => Options.Contains(_))) return true;
            return false;
        }
    }
}