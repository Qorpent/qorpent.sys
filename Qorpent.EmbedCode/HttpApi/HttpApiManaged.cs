using System;
using System.Runtime.InteropServices;
using System.Net;
using System.Security.Cryptography.X509Certificates;
#pragma warning disable 169
namespace Qorpent.Native.HttpApi
{

	/// <summary>
	/// Provides a barebones wrapper around the HTTPAPI. Only SSL certificate binding operations are provided.
	/// </summary>
	public class HttpApi : IDisposable
	{

		#region Extern Methods
		[DllImport("httpapi.dll", SetLastError = true)]
		private static extern Int32 HttpInitialize(HTTPAPI_VERSION Version, UInt32 Flags, IntPtr pReserved);

		[DllImport("httpapi.dll", SetLastError = true)]
		private static extern Int32 HttpSetServiceConfiguration(IntPtr ServiceIntPtr, HTTP_SERVICE_CONFIG_ID ConfigId,
																IntPtr pConfigInformation, Int32 ConfigInformationLength,
																IntPtr pOverlapped);

		[DllImport("httpapi.dll", SetLastError = true)]
		private static extern Int32 HttpDeleteServiceConfiguration(IntPtr ServiceIntPtr, HTTP_SERVICE_CONFIG_ID ConfigId,
																   IntPtr pConfigInformation,
																   Int32 ConfigInformationLength, IntPtr pOverlapped);

		[DllImport("httpapi.dll", SetLastError = true)]
		private static extern Int32 HttpQueryServiceConfiguration(IntPtr ServiceIntPtr, HTTP_SERVICE_CONFIG_ID ConfigId,
																  IntPtr pInputConfigInfo, Int32 InputConfigInfoLength,
																  IntPtr pOutputConfigInfo, Int32 OutputConfigInfoLength,
																  out Int32 pReturnLength, IntPtr pOverlapped);


		[DllImport("httpapi.dll", SetLastError = true)]
		private static extern Int32 HttpTerminate(UInt32 Flags, IntPtr pReserved);
		#endregion

		#region Nested Structs
		[StructLayout(LayoutKind.Sequential)]
		private struct HTTP_SERVICE_CONFIG_IP_LISTEN_PARAM
		{
			public UInt16 AddrLength;
			public IntPtr pAddress;
		}

		/// <summary>
		/// 
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct HTTP_SERVICE_CONFIG_SSL_SET
		{
			/// <summary>
			/// 
			/// </summary>
			public HTTP_SERVICE_CONFIG_SSL_KEY KeyDesc;
			/// <summary>
			/// 
			/// </summary>
			public HTTP_SERVICE_CONFIG_SSL_PARAM ParamDesc;
		}
		/// <summary>
		/// 
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct HTTP_SERVICE_CONFIG_SSL_KEY
		{
			/// <summary>
			/// 
			/// </summary>
			public IntPtr pIpPort;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
#pragma warning disable 1591
		public struct HTTP_SERVICE_CONFIG_SSL_PARAM

		{
			public Int32 SslHashLength;
			public IntPtr pSslHash;
			public Guid AppId;
			[MarshalAs(UnmanagedType.LPWStr)]
			public String pSslCertStoreName;
			public UInt32 DefaultCertCheckMode;
			public Int32 DefaultRevocationFreshnessTime;
			public Int32 DefaultRevocationUrlRetrievalTimeout;
			[MarshalAs(UnmanagedType.LPWStr)]
			public String pDefaultSslCtlIdentifier;
			[MarshalAs(UnmanagedType.LPWStr)]
			public String pDefaultSslCtlStoreName;
			public UInt32 DefaultFlags;
		}
#pragma warning restore 1591
		[StructLayout(LayoutKind.Sequential)]
		private struct HTTP_SERVICE_CONFIG_SSL_QUERY
		{
			public HTTP_SERVICE_CONFIG_QUERY_TYPE QueryDesc;
			public HTTP_SERVICE_CONFIG_SSL_KEY KeyDesc;
			public UInt32 dwToken;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		private struct HTTPAPI_VERSION
		{
			public UInt16 HttpApiMajorVersion;
			public UInt16 HttpApiMinorVersion;

			public HTTPAPI_VERSION(UInt16 major_version, UInt16 minor_version)
			{
				HttpApiMajorVersion = major_version;
				HttpApiMinorVersion = minor_version;
			}
		}
		#endregion

		#region Enums
		private enum HTTP_SERVICE_CONFIG_ID
		{
			HttpServiceConfigIPListenList = 0,
			HttpServiceConfigSSLCertInfo = 1,
			HttpServiceConfigUrlAclInfo = 2,
			HttpServiceConfigMax = 3
		}

		private enum HTTP_SERVICE_CONFIG_QUERY_TYPE
		{
			HttpServiceConfigQueryExact = 0,
			HttpServiceConfigQueryNext = 1,
			HttpServiceConfigQueryMax = 2
		}
		#endregion

		#region Constants
		private const Int32 HTTP_INITIALIZE_CONFIG = 0x00000002;
		private const Int32 HTTP_SERVICE_CONFIG_SSL_FLAG_NONE = 0x00000000;
		private const Int32 HTTP_SERVICE_CONFIG_SSL_FLAG_USE_DS_MAPPER = 0x00000001;
		private const Int32 HTTP_SERVICE_CONFIG_SSL_FLAG_NEGOTIATE_CLIENT_CERT = 0x00000002;

		private const Int32 HTTP_SERVICE_CONFIG_SSL_FLAG_NO_RAW_FILTER = 0x00000004;


		private const Int32 ERROR_SUCCESS = 0;
		private const Int32 ERROR_FILE_NOT_FOUND = 2;
		private const Int32 ERROR_ACCESS_DENIED = 5;
		private const Int32 ERROR_INVALID_PARAMETER = 87;
		private const Int32 ERROR_INSUFFICIENT_BUFFER = 122;
		private const Int32 ERROR_ALREADY_EXISTS = 183;
		private const Int32 ERROR_NO_SUCH_LOGON_SESSION = 1312;
		private const Int32 ERROR_INVALID_HANDLE = 1609;
		#endregion

		#region Fields
		private Boolean disposed = false;
		private readonly Guid application_id;
		#endregion

		#region Properties
		
#pragma warning disable 1584,1711,1572,1581,1580
		/// <summary>
		/// Gets the application identifier being used by the <see cref="T:HttpApi"/> object.
	 /// </summary>
#pragma warning restore 1584,1711,1572,1581,1580
		
		public Guid ApplicationId
		{
			get { return application_id; }
		}
		#endregion

		#region Constructors
		private HttpApi(Guid application_id)
		{
			//
			// Initialize the HTTPAPI runtime...
			Int32 result = HttpInitialize(GetApiVersion(), HTTP_INITIALIZE_CONFIG, IntPtr.Zero);
			if (result != ERROR_SUCCESS)
				throw new HttpApiException(
					result,
					String.Format("The HTTPAPI runtime could not be initialized due to a {0:x} error.", result)
				);

			//
			// Store parameters...
			this.application_id = application_id == Guid.Empty ? Guid.NewGuid() : application_id;
		}
		/// <summary>
		/// 
		/// </summary>
		~HttpApi()
		{
			Dispose(false);
		}
		#endregion

		#region Public Static Methods
		/// <summary>
		/// Opens a HTTPAPI session with which operations can be executed.
		/// </summary>
		/// <returns>A <see cref="HttpApi"/> object that represents the session.</returns>
		public static HttpApi OpenSession()
		{
			return new HttpApi(Guid.NewGuid());
		}

		/// <summary>
		/// Opens a HTTPAPI session with which operations can be executed.
		/// </summary>
		/// <param name="application_id">The unique identifier of the application.</param>
		/// <returns>A <see cref="HttpApi"/> object that represents the session.</returns>
		public static HttpApi OpenSession(Guid application_id)
		{
			return new HttpApi(application_id);
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Binds a SSL certificate to a IP:Port endpoint.
		/// </summary>
		/// <param name="ip_port">The IP:Port pair that represents the binding endpoint.</param>
		/// <param name="cert_hash">The unique hash (usually MD5 or SHA1) of the certificate in the local machine's certificate store.</param>
		public void BindSslEndpoint(IPEndPoint ip_port, Byte[] cert_hash)
		{
			//
			// Check pre-conditions...
			ThrowIfDisposed();

			Int32 result = ERROR_SUCCESS;
			InvokeConfigSslSet(ip_port, cert_hash, (p_cfg_info, size) =>
			{
				result = HttpSetServiceConfiguration(
					IntPtr.Zero,
					HTTP_SERVICE_CONFIG_ID.HttpServiceConfigSSLCertInfo,
					p_cfg_info,
					size,
					IntPtr.Zero);
			});

			HandleResult(result, ip_port);
		}

		/// <summary>
		/// Queries for the existence of a SSL binding endpoint.
		/// </summary>
		/// <param name="ip_port">The IP:Port pair that represents the binding endpoint.</param>
		/// <returns>True if the binding endpoint exists or false if it does not.</returns>
		public HTTP_SERVICE_CONFIG_SSL_SET QuerySslEndpoint(IPEndPoint ip_port)
		{
			//
			// Check pre-conditions...
			ThrowIfDisposed();

			HTTP_SERVICE_CONFIG_SSL_SET ssl_set = new HTTP_SERVICE_CONFIG_SSL_SET();
			InvokeConfigSslQuery(ip_port, (p_in_cfg_info, in_size) =>
			{

				Int32 result;
				Int32 ret_size = 0;

				//
				// First call will always fail with ERROR_INSUFFICIENT_BUFFER, but we merely use it to ask how much buffer space we need for the second call...
				result = HttpQueryServiceConfiguration(
							  IntPtr.Zero,
							  HTTP_SERVICE_CONFIG_ID.HttpServiceConfigSSLCertInfo,
							  p_in_cfg_info,
							  in_size,
							  IntPtr.Zero,
							  ret_size,
							  out ret_size,
							  IntPtr.Zero);

				if (result == ERROR_FILE_NOT_FOUND)
					return;

				if (result != ERROR_INSUFFICIENT_BUFFER)
					throw new InvalidOperationException(String.Format("HttpQueryServiceConfiguration() call unexpectedly returned 0x{0:x}.", result));

				Int32 out_size = ret_size;
				IntPtr p_out_cfg_info = IntPtr.Zero;
				try
				{
					//
					// Allocate memory buffer where the query results will be output...
					p_out_cfg_info = Marshal.AllocCoTaskMem(out_size);

					//
					// Invoke the query on the HTTPAPI...
					if ((result = HttpQueryServiceConfiguration(
						IntPtr.Zero,
						HTTP_SERVICE_CONFIG_ID.HttpServiceConfigSSLCertInfo,
						p_in_cfg_info,
						in_size,
						p_out_cfg_info,
						out_size,
						out ret_size,
						IntPtr.Zero
						)) != ERROR_SUCCESS)
						throw new InvalidOperationException(String.Format("HttpQueryServiceConfiguration() call unexpectedly returned 0x{0:x}.", result));

					//
					// Store the returned object...
					ssl_set =
						(HTTP_SERVICE_CONFIG_SSL_SET)
						Marshal.PtrToStructure(p_out_cfg_info, typeof(HTTP_SERVICE_CONFIG_SSL_SET));

				}
				finally
				{
					//
					// Free the memory buffer...
					if (p_out_cfg_info != IntPtr.Zero)
						Marshal.FreeCoTaskMem(p_out_cfg_info);
				}

			});
			/*
			if (ssl_set.ParamDesc.AppId != Guid.Empty)
			{
				existing_application_id = ssl_set.ParamDesc.AppId;
				return true;
			}
			else
			{
				existing_application_id = Guid.Empty;
				return false;
			}
			 */
			return ssl_set;
		}

		/// <summary>
		/// Unbinds a SSL certificate from a IP:Port endpoint.
		/// </summary>
		/// <param name="ip_port">The IP:Port pair that represents the binding endpoint.</param>
		public void UnbindSslEndpoint(IPEndPoint ip_port)
		{
			//
			// Check pre-conditions...
			ThrowIfDisposed();

			Int32 result = ERROR_SUCCESS;
			InvokeConfigSslSet(ip_port, (p_cfg_info, size) =>
			{
				result = HttpDeleteServiceConfiguration(
					IntPtr.Zero,
					HTTP_SERVICE_CONFIG_ID.HttpServiceConfigSSLCertInfo,
					p_cfg_info,
					size,
					IntPtr.Zero);
			});

			HandleResult(result, ip_port);
		}
		#endregion

		#region Private Methods
		private void HandleResult(Int32 result, IPEndPoint ip_port)
		{
			switch (result)
			{

				case ERROR_SUCCESS:
					break;

				case ERROR_ACCESS_DENIED:
					throw new HttpApiException(
						result,
						"The current user does not have sufficient privileges to invoke the HTTPAPI operation."
					);

				case ERROR_ALREADY_EXISTS:
					throw new HttpApiException(
						result,
						String.Format("An existing SSL certificate is already bound to \"{0}\".", ip_port)
					);

				case ERROR_NO_SUCH_LOGON_SESSION:
					throw new HttpApiException(
						result,
						"There is a problem with the X.509 certificate. It may be invalid or the private key may not be available."
					);

				case ERROR_FILE_NOT_FOUND:
					throw new HttpApiException(
						result,
						String.Format("The binding \"{0}\" does not exist or is invalid.", ip_port)
					);

				case ERROR_INVALID_PARAMETER:
					throw new HttpApiException(
						result,
						"One or more parameters are invalid."
					);

				default:
					throw new HttpApiException(
						result,
						String.Format("An unexpected HTTPAPI error ({0:x}) occured.", result)
					);

			}//switch
		}

		private void InvokeConfigSslAction<T>(T ssl_cfg, Action<IntPtr, Int32> action)
		{
			//
			// Check pre-conditions...
			if (action == null)
				throw new ArgumentNullException("action");

			IntPtr p_cfg_info = IntPtr.Zero;
			try
			{
				//
				// Allocate unmanaged memory buffer to copy the structure to...
				Int32 size = Marshal.SizeOf(typeof(T));
				p_cfg_info = Marshal.AllocCoTaskMem(size);

				//
				// Copy the structure to the allocated memory buffer...
				Marshal.StructureToPtr(ssl_cfg, p_cfg_info, false);

				//
				// Invoke the action method/delegate...
				action(p_cfg_info, size);

			}
			finally
			{
				//
				// Free the memory buffer...
				if (p_cfg_info != IntPtr.Zero)
					Marshal.FreeCoTaskMem(p_cfg_info);
			}
		}

		private HTTP_SERVICE_CONFIG_SSL_SET InvokeConfigSslSet(IPEndPoint ip_port, Action<IntPtr, Int32> action)
		{
			//
			// Create the SSL key (the IP/Port binding)...
			GCHandle n_sockaddr = GetSocketAddress(ip_port);
			var ssl_key = new HTTP_SERVICE_CONFIG_SSL_KEY()
			{
				pIpPort = n_sockaddr.AddrOfPinnedObject()
			};

			//
			// Create the SSL setting the combines the key and parameters...
			var ssl_set = new HTTP_SERVICE_CONFIG_SSL_SET()
			{
				KeyDesc = ssl_key
			};

			//
			// Invoke the action...
			if (action != null)
				InvokeConfigSslAction(ssl_set, action);

			//
			// Return the created setting for chaining...
			return ssl_set;
		}

		private HTTP_SERVICE_CONFIG_SSL_SET InvokeConfigSslSet(IPEndPoint ip_port, Byte[] cert_hash, Action<IntPtr, Int32> action)
		{
			//
			// Create the basic SSL setting structure...
			var ssl_set = InvokeConfigSslSet(ip_port, null);

			//
			// Create the SSL parameters for the key...
			GCHandle n_cert_hash = GCHandle.Alloc(cert_hash, GCHandleType.Pinned);
			var ssl_param = new HTTP_SERVICE_CONFIG_SSL_PARAM()
			{
				AppId = application_id,
				DefaultCertCheckMode = 0,
				DefaultFlags = HTTP_SERVICE_CONFIG_SSL_FLAG_NONE,
				DefaultRevocationFreshnessTime = 0,
				DefaultRevocationUrlRetrievalTimeout = 0,
				pSslCertStoreName = StoreName.My.ToString(),
				pSslHash = n_cert_hash.AddrOfPinnedObject(),
				SslHashLength = cert_hash.Length
			};

			//
			// Set the SSL parameters in the setting structure...
			ssl_set.ParamDesc = ssl_param;

			//
			// Invoke the action...
			if (action != null)
				InvokeConfigSslAction(ssl_set, action);

			//
			// Return the created setting for chaining...
			return ssl_set;
		}

		private HTTP_SERVICE_CONFIG_SSL_QUERY InvokeConfigSslQuery(IPEndPoint ip_port, Action<IntPtr, Int32> action)
		{
			//
			// Create the SSL key (the IP/Port binding)...
			GCHandle n_sockaddr = GetSocketAddress(ip_port);
			var ssl_key = new HTTP_SERVICE_CONFIG_SSL_KEY()
			{
				pIpPort = n_sockaddr.AddrOfPinnedObject()
			};

			//
			// Create the SSL query...
			var ssl_query = new HTTP_SERVICE_CONFIG_SSL_QUERY()
			{
				QueryDesc = HTTP_SERVICE_CONFIG_QUERY_TYPE.HttpServiceConfigQueryExact,
				KeyDesc = ssl_key
			};

			//
			// Invoke the action...
			if (action != null)
				InvokeConfigSslAction(ssl_query, action);

			//
			// Return the created query for chaining...
			return ssl_query;
		}

		private void ThrowIfDisposed()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().Name);
		}
		#endregion

		#region Private Static Methods
		private static HTTPAPI_VERSION GetApiVersion()
		{
			return new HTTPAPI_VERSION(1, 0);
		}

		private static GCHandle GetSocketAddress(IPEndPoint ip_port)
		{
			SocketAddress sa = ip_port.Serialize();
			Byte[] bytes = new Byte[sa.Size];

			for (Int32 i = 0; i < sa.Size; ++i)
				bytes[i] = sa[i];

			return GCHandle.Alloc(bytes, GCHandleType.Pinned);
		}
		#endregion
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		#region IDisposable Members
		protected virtual void Dispose(Boolean disposing)
		{
			if (disposed)
				return;

			if (disposing)
				GC.SuppressFinalize(this);

			//
			// Terminate the HTTPAPI session...
			HttpTerminate(HTTP_INITIALIZE_CONFIG, IntPtr.Zero);

			//
			// Set flag...
			disposed = true;
		}
		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}
		#endregion

	}

	internal class HttpApiException : Exception{
		public HttpApiException(int result, string format){
			this.Result = result;
			this.Format = format;
		}

		protected int Result { get; set; }

		protected string Format { get; set; }

		
	}

//class

}//namespace
#pragma warning restore 169