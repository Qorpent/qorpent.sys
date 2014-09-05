using System.IO;
using System.Net.Security;

namespace Qorpent.IO.Net{
	/// <summary>
	/// Publicates underlined stream to public scope
	/// </summary>
	public class SslWithUnderlinedStream : SslStream{
		/// <summary>
		/// Опубликованный поток
		/// </summary>
		public Stream Underlined { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="innerStream"></param>
		public SslWithUnderlinedStream(Stream innerStream) : base(innerStream){
			Underlined = innerStream;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="innerStream"></param>
		/// <param name="leaveInnerStreamOpen"></param>
		public SslWithUnderlinedStream(Stream innerStream, bool leaveInnerStreamOpen) : base(innerStream, leaveInnerStreamOpen){
			Underlined = innerStream;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="innerStream"></param>
		/// <param name="leaveInnerStreamOpen"></param>
		/// <param name="userCertificateValidationCallback"></param>
		public SslWithUnderlinedStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback) : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback){
			Underlined = innerStream;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="innerStream"></param>
		/// <param name="leaveInnerStreamOpen"></param>
		/// <param name="userCertificateValidationCallback"></param>
		/// <param name="userCertificateSelectionCallback"></param>
		public SslWithUnderlinedStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback) : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback){
			Underlined = innerStream;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="innerStream"></param>
		/// <param name="leaveInnerStreamOpen"></param>
		/// <param name="userCertificateValidationCallback"></param>
		/// <param name="userCertificateSelectionCallback"></param>
		/// <param name="encryptionPolicy"></param>
		public SslWithUnderlinedStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback, EncryptionPolicy encryptionPolicy) : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback, encryptionPolicy){
			Underlined = innerStream;
		}
	}
}