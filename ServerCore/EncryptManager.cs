using System;
using System.Security.Cryptography;

namespace ServerCore
{
	internal class EncryptManager
	{
		private RSACryptoServiceProvider _rsa;

		public EncryptManager(string private_key)
		{
			_rsa = new RSACryptoServiceProvider(DefaultValue.SERVER_RSA_KEY_SIZE);
			_rsa.ImportCspBlob(Convert.FromBase64String(private_key));
		}

		public byte[] Encrypt(byte[] data)
		{
			lock (_rsa)
			{
				return _rsa.Encrypt(data, true);
			}
		}

		public byte[] Decrypt(byte[] data)
		{
			lock (_rsa)
			{
				return _rsa.Decrypt(data, true);
			}
		}
	}
}
