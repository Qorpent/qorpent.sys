using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Qorpent.IO.Protocols;

namespace Qorpent.IO.Net{
	/// <summary>
	/// Класс, осуществляющий низкоуровневую работу по обеспечению выполнения запросов HTTP
	/// </summary>
	/// <remarks>Привязан к определенному EndPoint, исходя из истории запросов может использовать
	/// Keep-Alive, присбоях в соединении</remarks>
	public class HttpSocket:IDisposable{
		private readonly IPEndPoint _endpoint;
		private readonly bool _secure;
		private Stream _stream;
		private Socket _socket;
		private bool _reuse = false;

		private BufferManager _bufferManager;



		/// <summary>
		/// Автоматическая загрузка ресурса целиком (удобно в сценарии с Dispose при отпраке 
		/// </summary>
		public bool AutoLoad { get; set; }
		/// <summary>
		/// Автоматически закрывать после полной загрузки
		/// </summary>
		public bool CloseAfterLoad { get; set; }

		/// <summary>
		/// Буфер протоколов
		/// </summary>
		public BufferManager BufferManager{
			get { return _bufferManager ?? (_bufferManager = new BufferManager()); }
			set { _bufferManager = value; }
		}
		HttpProtocol protocol =new HttpProtocol();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			return (Secure ? "https" : "http") + "//" + Endpoint;
		}
		/// <summary>
		/// Создает HttpSocket для указанной точки 
		/// </summary>
		/// <param name="endpoint">удаленный адрес</param>
		/// <param name="secure">признак использования SSL</param>
		public HttpSocket(IPEndPoint endpoint, bool secure = false){
			_endpoint = endpoint;
			_secure = secure;
			AllowNonTrustedCertificates = true;
			
		}
		/// <summary>
		/// Удаленный адрес
		/// </summary>
		public IPEndPoint Endpoint{
			get { return _endpoint; }
		}
		/// <summary>
		/// Признак защищенного соединения
		/// </summary>
		public bool Secure{
			get { return _secure; }
		}
		/// <summary>
		/// Признак разрешения не доверенных сертификатов
		/// </summary>
		public bool AllowNonTrustedCertificates { get; set; }

		/// <summary>
		/// Выполняет запрос и возвращает результат
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public HttpResponse2 Call(HttpRequest request){
			try{
				Prepare();
				Handshake(request);
				Send(request);
				return Recieve();
			}
			catch{
				Close();
				throw;
			}
		}
		/// <summary>
		/// Выполняет запрос и возвращает результат
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public HttpResponse2 Call3(HttpRequest request)
		{
			try
			{
				Prepare();
				Handshake(request);
				Send(request);
				return Recieve3();
			}
			catch
			{
				Close();
				throw;
			}
		}

		private HttpResponse2 Recieve(){
			var result = BufferManager.Read(_stream, protocol, true);
			if(!result.Ok)throw new IOException("Error in protocol",result.Error);
			return protocol.Response;
		}

		private HttpResponseReader3 reader;
		private HttpResponse2 Recieve3(){
			reader = reader ?? new HttpResponseReader3();
			return reader.Read(_stream);
		}


		private void Send(HttpRequest request){
			var httpwriter = new HttpRequestWriter(_stream);
			httpwriter.Write(request);
		}

		private void Handshake(HttpRequest request){
			if (Secure){
				((SslStream)_stream).AuthenticateAsClient(request.Uri.Host);
			}
		}

		private void Prepare(){
			_socket = EnsureAvailableSocket();
			_stream = EnsureStream();
		}

		private Socket EnsureAvailableSocket(){
			if (!_reuse){
				Close();
			}
			_socket = new Socket(SocketType.Stream,ProtocolType.Tcp);
			_socket.Connect(Endpoint);
			return _socket;
		}

		private void Close(){
			if (null != _stream){
				_stream.Dispose();
			}
			if (null != _socket){
				_socket.Dispose();
			}
			_socket = null;
			_stream = null;
		}


		private Stream EnsureStream(){
			if (_reuse && null!=_stream) return _stream;
			if (null != _stream){
				_stream.Dispose();
			}
			Stream result = new NetworkStream(_socket){ReadTimeout = 30,WriteTimeout = 30};
			
			if (Secure){
				result = new SslWithUnderlinedStream(result,true,UserCertificateValidationCallback);
			}
			return result;
		}
		/// <summary>
		/// Событие проверки сертификата
		/// </summary>
		public event RemoteCertificateValidationCallback OnCheckCertifity;

		private bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors){
			if (null != OnCheckCertifity){
				return OnCheckCertifity(sender, certificate, chain, sslPolicyErrors);
			}
			return AllowNonTrustedCertificates;
		}
		/// <summary>
		/// Уничтожает сокет и поток
		/// </summary>
		public void Dispose(){
			if (null != _stream) _stream.Close();
			if (null != _socket) _socket.Dispose();
		}
	}
}