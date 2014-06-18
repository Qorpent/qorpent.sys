using System.Threading.Tasks;

namespace Qorpent.Host.Exe.SimpleSockets{
	/// <summary>
	/// Интерфейс хэндлера
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="R"></typeparam>
	public interface ISimpleSocketHandler<T,R>{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		Task Execute(SimpleSocketRequest<T, R> request);
	}
}