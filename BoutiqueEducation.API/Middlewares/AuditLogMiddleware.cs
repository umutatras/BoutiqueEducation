using BoutiqueEducation.DataAccess.Context;
using BoutiqueEducation.Entity.Entities;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace BoutiqueEducation.API.Middlewares;

public sealed class AuditLogMiddleware
{
    private readonly RequestDelegate _next;

    public AuditLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        // Request body okumayı etkinleştiriyoruz, aksi taktirde buffer olmadığı için Exception fırlar
        context.Request.EnableBuffering();
        string requestBody = "";

        if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
        {
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0; // İsteğin controllerlara bozulmadan gidebilmesi için başa sarılır
            }
        }

        await _next(context); // İstek Controller tarafına gönderilir ve işlenmesi beklenir
        stopwatch.Stop();

        try
        {
            // Transient scope bir servis içerisinden veritabanı yollamak için (Middleware scope sorunu yasamamak adına)
            using var scope = context.RequestServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Guid? userId = null;
            var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid parsedUserId))
            {
                userId = parsedUserId;
            }

            var auditLog = new AuditLog
            {
                UserId = userId,
                UserName = context.User?.Identity?.Name,
                HttpMethod = context.Request.Method,
                Path = context.Request.Path,
                QueryString = context.Request.QueryString.ToString(),
                RequestBody = string.IsNullOrWhiteSpace(requestBody) ? null : requestBody,
                StatusCode = context.Response.StatusCode,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                ExecutionDurationMs = stopwatch.ElapsedMilliseconds
            };

            dbContext.Set<AuditLog>().Add(auditLog);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Loglama sırasında bir sorun olursa ana akışı bozmaması için try catch
            Console.WriteLine("Audit Loglanırken hata oluştu: " + ex.Message);
        }
    }
}
