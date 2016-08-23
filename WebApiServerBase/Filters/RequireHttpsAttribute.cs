using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApiServerBase.Filters
{
	public class RequireHttpsAttribute : AuthorizationFilterAttribute
	{
		public override void OnAuthorization(HttpActionContext actionContext)
		{
			// HTTPS 이외 응답 거절
			//if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
			//{
			//	actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
			//	{
			//		ReasonPhrase = "HTTPS REQUIRED"
			//	};
			//}

			if (!WebCore.IsInitialized)
			{
				// 서버 초기화 전 응답 거절
				actionContext.Response = new HttpResponseMessage(HttpStatusCode.NotAcceptable)
				{
					ReasonPhrase = "SERVER IS NOT READY"
				};
			}
			else
			{
				base.OnAuthorization(actionContext);
			}
		}
	}
}