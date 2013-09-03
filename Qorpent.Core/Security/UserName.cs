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
// PROJECT ORIGIN: Qorpent.Core/UserName.cs
#endregion
using System;
using System.Security.Principal;

namespace Qorpent.Security{
    /// <summary>
    ///   Parses usual windows identity names and split it into
    ///   domain and name slots and defines if it's local
    ///   identity
    /// </summary>
    public sealed class UserName{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        public UserName(string username){
            username = username.Replace("/", "\\").ToLower();
            FullName = username;
            if (username.Contains("\\")){
                Domain = username.Split('\\')[0];
                Name = username.Split('\\')[1];
                if (Domain == Environment.MachineName.ToLower()){
                    IsLocal = true;
                }
            }
            else{
                Name = username;
                Domain = Environment.MachineName;
                FullName = (Domain + "\\" + Name).ToLower();
                IsLocal = true;
            }
        }
        /// <summary>
        /// Приводит имя к "локальному" написанию
        /// </summary>
        public string LocalizedName{
            get{
                if(IsLocal){
                    return "local\\" + Name;
                }else{
                    return FullName;
                }
            }
        }
        /// <summary>
        /// Создает UserName из принципала
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static UserName For(IPrincipal principal){
            return For(principal.Identity.Name);
        }
		/// <summary>
		/// Создает UserName из идентичности
		/// </summary>
		/// <param name="identity"></param>
		/// <returns></returns>
        public static UserName For(IIdentity identity){
            return For(identity.Name);
        }
		/// <summary>
		/// Создает UserName из строки с именем пользователя
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
        public static UserName For(string username){
            return new UserName(username);
        }
		/// <summary>
		/// Полное имя
		/// </summary>
        public string FullName { get; private set; }
		/// <summary>
		/// Домен
		/// </summary>
        public string Domain { get; private set; }
		/// <summary>
		/// Имя
		/// </summary>
        public string Name { get; private set; }
		/// <summary>
		/// Признак локального пользователя
		/// </summary>
        public bool IsLocal { get; private set; }
    }
}