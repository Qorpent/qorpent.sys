using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;
using Qorpent.Events;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Mvc;
using Qorpent.Security.Watchdog;
using Qorpent.Utils.Collections;
using Qorpent.Utils.Extensions;


namespace Qorpent.Security
{
	/// <summary>
	/// Разроешает роли относительно XML/BXL файлов конфигурации с элементами role, user
	/// </summary>
	[ContainerComponent(Lifestyle.Extension,"filebased.roleresolver.provider",ServiceType =typeof(IRoleResolverExtension))]
	public class FileBasedRoleProvider:ServiceBase,IRoleResolverExtension
	{
		/// <summary>
		/// Файловая система
		/// </summary>
		[Inject] public IFileNameResolver FileNameResolver { get;set; }

		/// <summary>
		/// Прямой реестр ролей и пользователей
		/// </summary>
		public XElement DirectXml { get; set; }

		/// <summary>
		/// 	An index of object
		/// </summary>
		public int Idx { get; set; }

		/// <summary>Resolves roles with config files</summary>
		///  <param name="principal"> </param>
		/// <param name="role"> </param>
		/// <param name="exact"> </param>
		/// <param name="callcontext"> </param>
		/// <param name="customcontext"> </param>
		/// <returns> </returns>
		public bool IsInRole(IPrincipal principal, string role, bool exact = false, IMvcContext callcontext = null, object customcontext = null) {
			if(!_initialized) initialize();			
			var usr = "USER:" + principal.Identity.Name.ToUpper();
			var roletofind = role.ToUpper();
			#if PARANOID
			if (roletofind=="ADMIN") return false;
			#endif
			var exactresult = map.ForwardAll(usr).Any(x => x == roletofind);
			#if !PARANOID
			//in paranoid mode we have not check admins
			if(!exactresult && roletofind!="ADMIN" && !exact) {
				return map.ForwardAll(usr).Any(x => x == "ADMIN");
			}
			#endif
			return exactresult;
		}

		private void initialize() {
			Reset(null);
			if(null==DirectXml) {
				var rolefiles = FileNameResolver.ResolveAll(
					new FileSearchQuery
						{
							ExistedOnly = true,
							PathType = FileSearchResultType.FullPath,
							ProbeFiles = new[] {"*.rolemap.xml", "*.rolemap.bxl"},
							ProbePaths = new[] {"~/", "~/.config", "~/bin", "~/sys", "~/usr"}
						});
				foreach (var rolefile in rolefiles) {
					var xml = XmlExtensions.GetXmlFromAny(rolefile);
					ReadXml(xml);
				}
			}else {
				ReadXml(DirectXml);
			}

			_initialized = true;
		}

		private void ReadXml(XElement xml) {
			var rolemaps = xml.Elements("role");
			var usermaps = xml.Elements("user");
			foreach (var element in rolemaps) {
				var desc = element.Describe();
				var subroles = desc.Name.SmartSplit();
				foreach (var subrole in subroles) {
					#if PARANOID
					if(Paranoid.Provider.IsSecureRole(subrole))continue;
					#endif
					map.Add(desc.Code.ToUpper(), subrole.ToUpper());
				}
			}
			foreach (var element in usermaps) {
				var desc = element.Describe();
				var subroles = desc.Name.SmartSplit();
				foreach (var subrole in subroles)
				{
#if PARANOID
					if(Paranoid.Provider.IsSecureRole(subrole))continue;
#endif
					map.Add("USER:" + desc.Code.ToUpper(), subrole.ToUpper());
				}
			}
		}

		readonly StringMap map  = new StringMap();
		private bool _initialized = false;

		/// <summary>
		/// 	Вызывается при вызове Reset
		/// </summary>
		/// <param name="data"> </param>
		/// <returns> любой объект - будет включен в состав результатов <see cref="ResetEventResult" /> </returns>
		/// <remarks>
		/// 	При использовании стандартной настройки из <see cref="ServiceBase" /> не требует фильтрации опций,
		/// 	настраивается на основе атрибута <see cref="RequireResetAttribute" />
		/// </remarks>
		public override object Reset(Events.ResetEventData data)
		{
			map.Clear();
			_initialized = false;
			return null;
		}
	}
}
