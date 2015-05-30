using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Qorpent.BSharp.Builder;
using Qorpent.Host;

namespace qorpent.v2.security.encryption{
	public class Encryptor : IEncryptProvider{
		private readonly SymmetricAlgorithm _cryptoService = new TripleDESCryptoServiceProvider();
		public byte[] Key;
		public byte[] Vector;
        

	    public Encryptor():this(Guid.NewGuid().ToString()){
		}

		public Encryptor(string keysrc, bool withmachineanddate = true) {
		    this.KeySource = keysrc;
			InitializeKey(keysrc,withmachineanddate);
		}

	    public string KeySource { get;private set; }

	    // maybe use AesCryptoServiceProvider instead?

		// vector and key have to match between encryption and decryption
		public string Encrypt(string text){
			byte[] data = Encoding.UTF8.GetBytes(text);
			return Encrypt(data);
		}

		/// <summary>
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public string Encrypt(byte[] data){
			var result = Uri.EscapeDataString(Convert.ToBase64String(Transform(data, _cryptoService.CreateEncryptor(Key, Vector))));
		    return result;
		}

		// vector and key have to match between encryption and decryption
		public string Decrypt(string text){
			byte[] data = DecryptData(text);
			return Encoding.UTF8.GetString(data);
		}

	    public byte[] Decrypt(byte[] encrypted, byte[] privateKey) {
            var cert = new X509Certificate2(privateKey);
	        var key = (RSACryptoServiceProvider) cert.PrivateKey;
	        return key.Decrypt(encrypted, false);
	    }

	    public byte[] Encrypt(byte[] data, byte[] publicKey) {
            var cert = new X509Certificate2(publicKey);
            var key = (RSACryptoServiceProvider)cert.PublicKey.Key;
            var edata = key.Encrypt(data, false);
	        return edata;
	    }

	    public bool CheckSign(byte[] data, byte[] sign, byte[] certificate) {
            var cert = new X509Certificate2(certificate);
            var key = (RSACryptoServiceProvider)cert.PublicKey.Key;
            return key.VerifyData(data, CryptoConfig.MapNameToOID("SHA1"), sign);
	    }

	    public byte[] SignData(byte[] data, byte[] certificate) {
            var cert = new X509Certificate2(certificate);
	        var key = (RSACryptoServiceProvider) cert.PrivateKey;
            return key.SignData(data, 0, data.Length, CryptoConfig.MapNameToOID("SHA1"));
	    }

		/// <summary>
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public byte[] DecryptData(string text) {
		    var str = Convert.FromBase64String(Uri.UnescapeDataString(text));
		    
			byte[] data = Transform(str, _cryptoService.CreateDecryptor(Key, Vector));
			return data;
		}

		public void Initialize(IHostServer server) {
			InitializeKey(server.Config.EncryptBasis);
		}

		private void InitializeKey(string keysrc, bool withmachineanddate = true){
           
			if (string.IsNullOrWhiteSpace(keysrc)) {
			    keysrc = Guid.NewGuid().ToString();
			}
		    if (withmachineanddate) {
		        keysrc += Environment.MachineName + DateTime.Today.ToLongDateString();
		    }
		    while (keysrc.Length <= 32){
				keysrc += keysrc;
			}
		    var bytes = Encoding.UTF8.GetBytes(keysrc);
		    Key = bytes.Take(16).ToArray();
		    Vector = bytes.Skip(16).ToArray();
		}

		private byte[] Transform(byte[] input, ICryptoTransform cryptoTransform){
			var stream = new MemoryStream();
			var cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write);
			cryptoStream.Write(input, 0, input.Length);
			cryptoStream.FlushFinalBlock();
			return stream.ToArray();
		}
	}
}