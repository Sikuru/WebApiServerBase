using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace WebApiServerBase
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            // XML 응답 포매터 삭제
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // 예외 처리 필터
            config.Services.Replace(typeof(IExceptionHandler), new Filters.GlobalExceptionHandler());
            // 예외 로그 필터
            config.Services.Add(typeof(IExceptionLogger), new Filters.GlobalExceptionLogger());
        }
    }
}
