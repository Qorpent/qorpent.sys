using System.Collections.Generic;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO{
	/// <summary>
	/// �������� ������������� IWebFileResolver
	/// </summary>
	[ContainerComponent(Lifestyle.Transient,ServiceType = typeof(IWebFileResolver))]
	public class WebFileResolver : IWebFileResolver{
		/// <summary>
		/// 
		/// </summary>
		public WebFileResolver(){
			Providers = new List<IWebFileProvider>();
			Cache = new Dictionary<string, IWebFileRecord>();
		}
		/// <summary>
		/// ��������� ���-�����������
		/// </summary>
		[Inject]
		public IList<IWebFileProvider> Providers { get; private set; }


		/// <summary>
		/// ������������ ����� ��������� ����� �� �������
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		public IWebFileRecord Find(string search){
			var nsearch = ("/" + search).NormalizePath();
			if (this.Prefix != "/"){
				if (!nsearch.StartsWith(Prefix + "/")) return null;
				nsearch = nsearch.Substring(Prefix.Length, nsearch.Length - Prefix.Length);
			}
			if(!Cache.ContainsKey(search)){
				IWebFileRecord result = null;
				foreach (var p in Providers){
					if(!p.IsMatch(nsearch))continue;
					result = p.Find(nsearch);
					if (null != result){
						break;	
					}
				}
				if (null == result){
					foreach (var p in Providers)
					{
						if (!p.IsMatch(nsearch)) continue;
						result = p.Find(nsearch,WebFileSerachMode.IgnorePath);
						if (null != result)
						{
							break;
						}
					}
				}
				Cache[search] = result;
			}
			return Cache[search];
		}

		/// <summary>
		/// ������� ��� ����������
		/// </summary>
		public void Clear(){
			Cache.Clear();
		}

		/// <summary>
		/// ������������ ���������� ������
		/// </summary>
		/// <param name="provider"></param>
		public void Register(IWebFileProvider provider){
			Providers.Add(provider);
		}

		/// <summary>
		/// ��� ����� ������������ ���������� ������
		/// </summary>
		public IDictionary<string, IWebFileRecord> Cache { get; private set; }
		private string _prefix;

		/// <summary>
		/// ����� ������� ���������� ����������
		/// </summary>
		public string Prefix
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_prefix)) _prefix = "/";
				return _prefix;
			}
			set
			{
				_prefix = ("/" + value).NormalizePath();
			}
		}

		
	}
}