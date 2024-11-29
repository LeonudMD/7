using FluentValidation;
using System;
using System.Diagnostics.Contracts;
using System.Net;

namespace AuthService.API.Middlewares
{
        public class ExceptionHandlingMiddleware
        {
            private readonly RequestDelegate _next;

            public ExceptionHandlingMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (ValidationException ex)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";

                    var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });

                    await context.Response.WriteAsJsonAsync(new { Errors = errors });
                }
                catch (Exception ex)
                {
                    await HandleException(context, ex);
                }
            }

            private static Task HandleException(HttpContext context, Exception ex)
            {
                // Логика обработки исключений: запись в лог и формирование ответа
                var statusCode = HttpStatusCode.InternalServerError; // Статус по умолчанию

                // Пример кастомной обработки для разных типов исключений
                if (ex is ArgumentException)
                {
                    statusCode = HttpStatusCode.BadRequest;
                }

                var response = new
                {
                    error = ex.Message,
                    details = ex.StackTrace
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                return context.Response.WriteAsJsonAsync(response);
            }

        }
}
