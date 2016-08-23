using ServerCore;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiServerBase.Proxies
{
	public static class ProxyResponse
	{
		private static HttpResponseMessage HttpResponse(HttpRequestMessage request, byte[] response_bytes)
		{
			return HttpResponse(request, HttpStatusCode.OK, response_bytes);
		}

		private static HttpResponseMessage HttpResponse(HttpRequestMessage request, HttpStatusCode http_status_code, byte[] response_bytes)
		{
			//var response = request.CreateResponse(http_status_code, sc, new MediaTypeHeaderValue("application/x-www-form-urlencoded"));
			var response = new HttpResponseMessage(http_status_code);
			if (response_bytes != null)
			{
				response.Content = new StreamContent(new MemoryStream(response_bytes));
			}

			response.Headers.CacheControl = new CacheControlHeaderValue()
			{
				NoCache = true,
				NoStore = true,
				MustRevalidate = true,
			};
			return response;
		}

		public static HttpResponseMessage HttpResponseStatusCode(HttpRequestMessage request, HttpStatusCode status_code, DateTime start_time_utc)
		{
			var response = request.CreateResponse(status_code);
			response.Headers.CacheControl = new CacheControlHeaderValue()
			{
				NoCache = true,
				NoStore = true,
				MustRevalidate = true
			};

			return response;
		}

		//public static HttpResponseMessage HttpResponse<T>(HttpRequestMessage request, T response_body, DateTime start_time_utc, AccessTokenWithString token, int result_code) where T : ResponseBase
		//{
		//	if (response_body == null)
		//	{
		//		return HttpResponse(request, HttpStatusCode.InternalServerError, null);
		//	}

		//	//DelegatorLog.ApiAsync(200, request.RequestUri, start_time_utc, ProxyRequest.GetClientIp(request), token != null ? token.Token.AccountUID : 0, response_body.ResultCode);
		//	return HttpResponse(request, BinConverter.BinMaker(response_body));
		//}

		//public static HttpResponseMessage HttpResponseInternalException<T>(HttpRequestMessage request, ServerCore.ServerException server_exception, DateTime start_time_utc, AccessTokenWithString token) where T : ResponseBase, new()
		//{
		//	LogTrace.Error(string.Format("HttpResponseInternalException<{0}> ; {1}({2}) {3}", typeof(T).ToString(),
		//		server_exception.ResultCode.ToString(), server_exception.Message, server_exception.ToString()));

		//	T response_body = new T();
		//	response_body.ResultCode = server_exception.ResultCode;

		//	//DelegatorLog.ApiAsync(200, request.RequestUri, start_time_utc, ProxyRequest.GetClientIp(request), token != null ? token.Token.AccountUID : 0, response_body.ResultCode);
		//	return HttpResponse(request, BinConverter.BinMaker(response_body));
		//}
	}
}