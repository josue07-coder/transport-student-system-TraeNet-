using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Transport.Domain.Exceptions;

namespace Transport.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Validation failed for {Method} {Path}. CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path.Value,
                    context.TraceIdentifier);

                await HandleValidationException(context, ex);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Domain rule failed for {Method} {Path}. CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path.Value,
                    context.TraceIdentifier);

                await HandleException(context, GetDomainStatusCode(ex.Message), ex.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning(
                    "Concurrency conflict for {Method} {Path}. CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path.Value,
                    context.TraceIdentifier);

                await HandleException(
                    context,
                    HttpStatusCode.Conflict,
                    "El registro fue modificado por otro proceso. Actualice la información e intente nuevamente.");
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                _logger.LogWarning(
                    ex,
                    "Unique constraint conflict for {Method} {Path}. CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path.Value,
                    context.TraceIdentifier);

                await HandleException(
                    context,
                    HttpStatusCode.Conflict,
                    "La operación no pudo completarse porque ya existe un registro con los mismos datos únicos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception for {Method} {Path}. CorrelationId: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path.Value,
                    context.TraceIdentifier);

                await HandleException(context, HttpStatusCode.InternalServerError, "Ocurrió un error interno en el servidor.");
            }
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException exception)
        {
            var message = exception.InnerException?.Message ?? exception.Message;
            return message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase)
                || message.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
                || message.Contains("duplicada", StringComparison.OrdinalIgnoreCase)
                || message.Contains("IX_", StringComparison.OrdinalIgnoreCase)
                || message.Contains("AK_", StringComparison.OrdinalIgnoreCase);
        }

        private static HttpStatusCode GetDomainStatusCode(string message)
        {
            if (message.Contains("No tiene permiso", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("No autorizado", StringComparison.OrdinalIgnoreCase))
                return HttpStatusCode.Forbidden;

            if (message.Contains("no encontrado", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("no encontrada", StringComparison.OrdinalIgnoreCase))
                return HttpStatusCode.NotFound;

            if (message.Contains("credenciales", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("contraseña inválidos", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("contraseña invalidos", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("no autenticado", StringComparison.OrdinalIgnoreCase))
                return HttpStatusCode.Unauthorized;

            return HttpStatusCode.BadRequest;
        }

        private static async Task HandleValidationException(HttpContext context, ValidationException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = new
            {
                success = false,
                error = "Validation failed",
                traceId = context.TraceIdentifier,
                correlationId = context.TraceIdentifier,
                errors = exception.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray())
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }

        private static async Task HandleException(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                success = false,
                error = message,
                traceId = context.TraceIdentifier,
                correlationId = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
