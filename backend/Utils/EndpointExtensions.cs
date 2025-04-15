using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace CosmoCargo.Utils
{
    public static class EndpointExtensions
    {
        public static void Get(this WebApplication app, [StringSyntax("Route")] string pattern, Delegate handler, string[] roles)
        {
            app
                .MapGet(pattern, handler)
                .RequireAuthorization(policy => policy.RequireRole(roles));
        }

        public static void Post(this WebApplication app, [StringSyntax("Route")] string pattern, Delegate handler, string[] roles)
        {
            app
                .MapPost(pattern, handler)
                .RequireAuthorization(policy => policy.RequireRole(roles));
        }
    }
}
