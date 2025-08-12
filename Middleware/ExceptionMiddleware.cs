﻿using System.Net;
using System.Text.Json;

namespace HotByteAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                object response;

                if (_env.IsDevelopment())
                {
                    response = new
                    {
                        statusCode = context.Response.StatusCode,
                        message = ex.Message,
                        stackTrace = ex.StackTrace ?? "No stack trace available"
                    };
                }
                else
                {
                    response = new
                    {
                        statusCode = context.Response.StatusCode,
                        message = "An unexpected error occurred."
                    };
                }

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
