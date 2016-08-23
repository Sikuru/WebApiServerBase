using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApiServerBase.Proxies
{
	public static class ProxyRequest
	{
		public static string ParseAuthticationHeader(HttpRequestMessage request)
		{
			IEnumerable<string> headers;
			string auth_value = null;
			if (request.Headers.TryGetValues("Authorization", out headers))
			{
				auth_value = headers.FirstOrDefault();
				if (!string.IsNullOrEmpty(auth_value) && auth_value.Length > 8)
				{
					auth_value = auth_value.Substring(7);
				}
				else
				{
					auth_value = null;
				}
			}

			return auth_value;
		}

		public static string ParseDeviceUIDHeader(HttpRequestMessage request)
		{
			IEnumerable<string> headers;
			string device_uid = null;
			if (request.Headers.TryGetValues("DeviceUID", out headers))
			{
				device_uid = headers.FirstOrDefault();
				if (string.IsNullOrEmpty(device_uid))
				{
					device_uid = null;
				}
			}

			return device_uid;
		}

		public static bool ParseAccessTokenWithCheckDeviceUID(HttpRequestMessage request, out AccessTokenWithString token)
		{
			var token_string = ParseAuthticationHeader(request);
			Token.TryParseAccessTokenWithString(token_string, out token);

			var device_uid = ParseDeviceUIDHeader(request);
			var hash = SHA256Managed.Create().ComputeHash(Encoding.UTF8.GetBytes(device_uid));
			if (token.Token.SHAHashedDeviceUID.SequenceEqual(hash))
			{
				return true;
			}
			else
			{
				token = null;
				return false;
			}
		}

		public static void ParseRefreshToken(HttpRequestMessage request, out RefreshTokenWithString token)
		{
			var token_string = ParseAuthticationHeader(request);
			Token.TryParseRefreshTokenWithString(token_string, out token);
		}

		public static int GetContentLength(HttpRequestMessage request)
		{
			if (request.Properties.ContainsKey("MS_HttpContext"))
			{
				var http_request = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request;
				return http_request.ContentLength;
			}
			else if (HttpContext.Current != null)
			{
				return HttpContext.Current.Request.ContentLength;
			}

			return 0;
		}

		public static void GetClientIpAgent(out string client_ip, out string client_agent, HttpRequestMessage request)
		{
			client_ip = null;
			client_agent = null;

			if (request == null)
			{
				return;
			}

			if (request.Properties.ContainsKey("MS_HttpContext"))
			{
				var http_request = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request;
				client_ip = http_request.UserHostAddress;
				client_agent = http_request.UserAgent;
			}
			else if (HttpContext.Current != null)
			{
				client_ip = HttpContext.Current.Request.UserHostAddress;
				client_agent = HttpContext.Current.Request.UserAgent;
			}
		}

		public static string GetClientIp(HttpRequestMessage request)
		{
			if (request == null)
			{
				return null;
			}

			if (request.Properties.ContainsKey("MS_HttpContext"))
			{
				var http_request = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request;
				return http_request.UserHostAddress;
			}
			else if (HttpContext.Current != null)
			{
				return HttpContext.Current.Request.UserHostAddress;
			}

			return null;
		}

		//public static async Task<T> ParseBody<T>(HttpRequestMessage request) where T : RequestBase, new()
		//{
		//	byte[] request_body_bytes = await request.Content.ReadAsByteArrayAsync();
		//	if (request_body_bytes == null || request_body_bytes.Length == 0)
		//	{
		//		return null;
		//	}

		//	return BinConverter.ClassMaker<T>(request_body_bytes);
		//}
	}
}