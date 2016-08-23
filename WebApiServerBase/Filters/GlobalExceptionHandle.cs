using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace WebApiServerBase.Filters
{
	public class GlobalExceptionHandler : ExceptionHandler
	{
		public async override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
		{
			// TODO: 예외 추가 처리
			System.Diagnostics.Trace.WriteLine(context.Exception.ToString());

			await base.HandleAsync(context, cancellationToken);
		}
	}

	public class GlobalExceptionLogger : ExceptionLogger
	{
		public async override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
		{
            // TODO: 예외 로그 처리
            System.Diagnostics.Trace.WriteLine(context.Exception.ToString());

			await base.LogAsync(context, cancellationToken);
		}
	}
}