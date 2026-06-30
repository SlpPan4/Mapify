using MapifyBackend.database_files;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MapifyBackend.Utility.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireAdminApiKeyAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        AdminApiKeyService apiKeyService =
            context.HttpContext.RequestServices.GetRequiredService<AdminApiKeyService>();

        string? apiKey = context.HttpContext.Request.Headers[AdminApiKeyService.HeaderName].FirstOrDefault();
        bool isValid = await apiKeyService.IsValidAdminKey(apiKey);

        if (!isValid)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                error = "Missing or invalid admin API key"
            });
        }
    }
}
