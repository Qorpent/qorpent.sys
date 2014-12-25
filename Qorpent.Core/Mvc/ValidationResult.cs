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
// PROJECT ORIGIN: Qorpent.Core/ValidationResult.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Applications;
using Qorpent.Log;

namespace Qorpent.Mvc{
    /// <summary>
    /// Инкапсуляция запроса на валидизацию
    /// </summary>
    public class ValidationResult{
        /// <summary>
        /// Признак валидности
        /// </summary>
        public bool IsValid { get; set; }
        private IList<string> messages = new List<string>();

        /// <summary>
        /// Сообщения
        /// </summary>
        public IList<string> Messages{
            get { return messages; }
            
        }

        /// <summary>
        /// Ошибка валидации
        /// </summary>
        public Exception Error { get; set; }
        /// <summary>
        /// Сгеренировать ошибку валидации
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ValidationException"></exception>
        public ValidationResult Throw() {
            if (IsValid) return this;
            throw new ValidationException(string.Join("\r\n",messages.ToArray()),Error);
            
        }

        private static IUserLog log = Application.Current.LogManager.GetLog(typeof (ValidationResult).FullName,null);
        /// <summary>
        /// Установить статус результата в Fail
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ValidationResult Fail(string message){
            log.Warn("fails with " + message);
            IsValid = false;
            Messages.Add(message);
            return this;
        }
		/// <summary>
		/// 
		/// </summary>
	    public static ValidationResult OK = new ValidationResult {IsValid = true};
    }
}