using System.Diagnostics;
using System.Reflection;
using WebAPP.Utilities;

namespace WebAPP.MiddlewareFactory
{
    public class LoggerMiddlleware
    {
        private readonly RequestDelegate _next;
        private readonly Dictionary<string, object> _scope;
        public LoggerMiddlleware(RequestDelegate next)
        {
            _next = next;
            _scope = [];

            Assembly? assemby = Assembly.GetExecutingAssembly();
            if (assemby is not null)
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assemby.Location);
                _scope.Add("Version", $"{versionInfo.FileMajorPart}.{versionInfo.FileMinorPart}.{versionInfo.FileBuildPart}");
            }
        }

        public async Task InvokeAsync(HttpContext context, ILogger logger)
        {
            try
            {
                using (logger.BeginScope(_scope)) await _next(context);
            }
            catch (Exception ex)
            {
                WriteLog.WriteErrorLog(logger, _scope, ex.GetType().Name);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(ex.Message);
            }
        }
    }
}
