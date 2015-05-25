using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Qorpent.Host.Security{
	internal class Encryptor : IEncryptProvider{
		private readonly SymmetricAlgorithm _cryptoService = new TripleDESCryptoServiceProvider();
		public byte[] Key;
		public byte[] Vector;

	    public Encryptor(){
		}

		public Encryptor(string keysrc){
			InitializeKey(keysrc);
		}

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

		private void InitializeKey(string keysrc){
           
			if (string.IsNullOrWhiteSpace(keysrc)) {
			    keysrc = Guid.NewGuid().ToString();
			}
            keysrc += Environment.MachineName + DateTime.Today.ToLongDateString();
            Console.WriteLine("keysrc: "+keysrc);
			while (keysrc.Length <= 32){
				keysrc += keysrc;
			}
			Key = Encoding.UTF8.GetBytes(keysrc.Substring(0, 16));
			Vector = Encoding.UTF8.GetBytes(keysrc.Substring(16));
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