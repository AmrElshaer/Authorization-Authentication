using System.Security.Claims;
using Authorization.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace Authorization.Security;

public class CustomClaimsTransformation(IServiceProvider serviceProvider):IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.HasClaim(c => c.Type == CustomClaims.Permission))
        {
            return principal;
        }

        using IServiceScope scope = serviceProvider.CreateScope();

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var identityId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var result = await sender.Send(
            new GetUserPermissionsQuery(identityId));
      
        if (principal.Identity is not ClaimsIdentity identity)
        {
            return principal;
        }

        foreach (var permission in result.Permissions)
        {
            identity.AddClaim(
                new Claim(CustomClaims.Permission, permission));
        }

        return principal;
    }
}