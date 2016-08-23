using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
	public class RefreshToken : BinConvertable
	{
		public long AccountUID { get; set; }
		/// <summary>
		/// 토큰 카운트. 1부터 사용합니다. OTP 처럼 일부 오차값은 허용합니다. (현재는 +-1 범위만 허용)
		/// 리플레시 토큰에서 카운트값은 이전 토큰과 비교용으로만 사용합니다.
		/// </summary>
		public int TokenCount { get; set; }
	}

	public class RefreshTokenWithString
	{
		public RefreshToken Token { get; set; }
		public string TokenString { get; set; }
	}

	public class AccessToken : RefreshToken
	{
		public byte[] SHAHashedDeviceUID { get; set; }
	}

	public class AccessTokenWithString
	{
		public AccessToken Token { get; set; }
		public string TokenString { get; set; }
	}

	public static class Token
	{
		public static RefreshToken IssueRefreshToken(long account_uid, int token_count)
		{
			return new RefreshToken()
			{
				AccountUID = account_uid,
				TokenCount = token_count
			};
		}

		public static RefreshTokenWithString IssueRefreshTokenWithString(long account_uid, int token_count)
		{
			var refresh_token = new RefreshToken()
			{
				AccountUID = account_uid,
				TokenCount = token_count
			};

			return new RefreshTokenWithString()
			{
				Token = refresh_token,
				TokenString = TokenToString(refresh_token)
			};
		}

		public static string TokenToString(RefreshToken token)
		{
			return Convert.ToBase64String(Core.EncryptManager.Encrypt(BinConverter.BinMaker(token)));
		}

		public static bool TryParseRefreshToken(string token_string, out RefreshToken token)
		{
			token = null;
			if (string.IsNullOrEmpty(token_string))
			{
				return false;
			}

			try
			{
				byte[] decrypt_bytes = Core.EncryptManager.Decrypt(Convert.FromBase64String(token_string));
				token = BinConverter.ClassMaker<RefreshToken>(decrypt_bytes);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool TryParseRefreshTokenWithString(string token_string, out RefreshTokenWithString token)
		{
			token = null;
			if (string.IsNullOrEmpty(token_string))
			{
				return false;
			}
			try
			{
				byte[] decrypt_bytes = Core.EncryptManager.Decrypt(Convert.FromBase64String(token_string));
				token = new RefreshTokenWithString()
				{
					Token = BinConverter.ClassMaker<RefreshToken>(decrypt_bytes),
					TokenString = token_string
				};

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static AccessToken IssueAccessToken(long account_uid, int token_count, string device_uid)
		{
			return new AccessToken()
			{
				AccountUID = account_uid,
				TokenCount = token_count,
				SHAHashedDeviceUID = SHA256Managed.Create().ComputeHash(Encoding.UTF8.GetBytes(device_uid))
			};
		}

		public static AccessTokenWithString IssueAccessTokenWithString(long account_uid, int token_count, string device_uid)
		{
			var access_token = new AccessToken()
			{
				AccountUID = account_uid,
				TokenCount = token_count,
				SHAHashedDeviceUID = SHA256Managed.Create().ComputeHash(Encoding.UTF8.GetBytes(device_uid))
			};

			return new AccessTokenWithString()
			{
				Token = access_token,
				TokenString = TokenToString(access_token)
			};
		}

		public static AccessTokenWithString IssueAccessTokenWithString(long account_uid, int token_count, byte[] device_uid_hash)
		{
			var access_token = new AccessToken()
			{
				AccountUID = account_uid,
				TokenCount = token_count,
				SHAHashedDeviceUID = device_uid_hash
			};

			return new AccessTokenWithString()
			{
				Token = access_token,
				TokenString = TokenToString(access_token)
			};
		}

		public static string TokenToString(AccessToken token)
		{
			return Convert.ToBase64String(Core.EncryptManager.Encrypt(BinConverter.BinMaker(token)));
		}

		public static bool TryParseAccessToken(string token_string, out AccessToken token)
		{
			token = null;
			if (string.IsNullOrEmpty(token_string))
			{
				return false;
			}

			try
			{
				byte[] decrypt_bytes = Core.EncryptManager.Decrypt(Convert.FromBase64String(token_string));
				token = BinConverter.ClassMaker<AccessToken>(decrypt_bytes);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool TryParseAccessTokenWithString(string token_string, out AccessTokenWithString token)
		{
			token = null;
			if (string.IsNullOrEmpty(token_string))
			{
				return false;
			}

			try
			{
				byte[] decrypt_bytes = Core.EncryptManager.Decrypt(Convert.FromBase64String(token_string));
				token = new AccessTokenWithString()
				{
					Token = BinConverter.ClassMaker<AccessToken>(decrypt_bytes),
					TokenString = token_string
				};
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
