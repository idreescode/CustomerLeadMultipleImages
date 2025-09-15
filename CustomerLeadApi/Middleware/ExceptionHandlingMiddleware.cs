using CustomerLeadApi.Common;
using System.Net;
using System.Text.Json;

namespace CustomerLeadApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            ApiResponse response;

            switch (exception)
            {
                case ArgumentNullException nullEx:
                    response = ApiResponse.ErrorResult("Required parameter is null", nullEx.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case ArgumentException argEx:
                    response = ApiResponse.ErrorResult("Invalid argument", argEx.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case UnauthorizedAccessException:
                    response = ApiResponse.ErrorResult("Unauthorized access");
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;
                case KeyNotFoundException:
                    response = ApiResponse.ErrorResult("Resource not found");
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                default:
                    response = ApiResponse.ErrorResult("An error occurred while processing your request");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
