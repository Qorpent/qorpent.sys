// Copyright 2007-2011 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
//  Supported by Media Technology LTD 
//   
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  
// MODIFICATIONS HAVE BEEN MADE TO THIS FILE

using System.Security.Principal;
using Qorpent.Applications;
using Qorpent.IoC;
using Qorpent.Model;
using Qorpent.Security;

//using Comdiv.QWeb;

namespace Qorpent.Utils.Extensions{
    ///<summary>
    ///</summary>
    public static class SecurityExtensions{
        /// <summary>
        /// ������ �������������
        /// </summary>
        public static object sync = new object();
        private static IPrincipalSource _principalSource;
        private static IContainer _container;
        internal static string __GetRole(this object obj)
        {
            if (null == obj) return null;
            if (obj is IWithRole) return ((IWithRole)obj).Role;
            return null;
        }

        private static IContainer Container{
            get { return _container ?? (_container = Application.Current.Container); }
            set { _container = value; }
        }

       

        /// <summary>
        /// ��������� ����������� �������� ������������
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static bool IsForUser(this IWithRole roles, IPrincipal principal = null ) {
            if(roles==null) return true;
            if(string.IsNullOrWhiteSpace(roles.Role)) return true;
            principal = principal ?? Application.Current.Principal.CurrentUser;
            if(Application.Current.Roles.IsAdmin(principal)) return true;
            foreach (var role in roles.Role.SmartSplit()) {
                if(Application.Current.Roles.IsInRole(principal,role)) return true;
            }
            return false;
        }
        

        /// <summary>
        /// �������� ���������� ������ � ������ ������������ � ����������
        /// </summary>
        /// <param name="username"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static IPrincipal toPrincipal(this string username, params string[] roles){
            var un = UserName.For(username);
            return new GenericPrincipal(new GenericIdentity(un.LocalizedName),roles ?? new string[]{});
        }

        ///<summary>
        ///</summary>
        private static IPrincipalSource PrincipalSource{
            get{
                if (_principalSource == null) {
	                _principalSource = Application.Current.Principal;
                }
                return _principalSource;
            }
            set { _principalSource = value; }
        }


        private static IPrincipal getCurrent(){
            return PrincipalSource.CurrentUser;
        }

        /// <summary>
        /// ������� ���������������� ������� ������ �������� ������������
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static bool IsAdmin(this IRoleResolver resolver){
            return IsAdmin(resolver, getCurrent());
        }

	    /// <summary>
	    /// ������� ���������������� ������� ������
	    /// </summary>
	    /// <param name="resolver"></param>
	    /// <param name="user"> </param>
	    /// <returns></returns>
	    public static bool IsAdmin(this IRoleResolver resolver, IPrincipal user){
            return resolver.IsInRole(user, "ADMIN");
        }

          /// <summary>
          /// �������� �������� ���� �������� ������������
          /// </summary>
          /// <param name="resolver"></param>
          /// <param name="role"></param>
          /// <param name="adminanyrole"></param>
          /// <returns></returns>
          public static bool IsInRole(this IRoleResolver resolver, string role, bool adminanyrole = true ){
            return resolver.IsInRole(getCurrent(), role,adminanyrole);
        }

	    private const string lSlash = "\\";
	    private const string rSlash = "/";
		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="userName"></param>
		  /// <param name="useDomain"></param>
		  /// <returns></returns>
		  public static string GetLoginNamePart(this string userName, bool useDomain)
		  {
			  if (userName.IsEmpty()) return "";
			  if (useDomain) return userName;
			  if (userName.Contains(lSlash) || userName.Contains(rSlash)) return userName.Split('\\', '/')[1];
			  return userName;
		  }

	    private const string LocalDomainName = "local";
		  /// <summary>
		  /// 
		  /// </summary>
		  /// <param name="userName"></param>
		  /// <returns></returns>
		  public static string GetDomainNamePart(this string userName)
		  {
			  if (userName.IsEmpty()) return "";
			  if (userName.Contains(lSlash) || userName.Contains(rSlash))
				  return userName.Split('\\', '/')[0];
			  return LocalDomainName;
		  }

      
    }
}