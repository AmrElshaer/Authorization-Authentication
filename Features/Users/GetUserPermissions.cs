using Authorization.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Features.Users;

public record GetUserPermissionsQuery(string IdentityUserId) : IRequest<GetUserPermissionsResponse>;
public record GetUserPermissionsResponse(HashSet<string> Permissions);
public class GetUserPermissionsQueryHandler(ApplicationDbContext context) : IRequestHandler<GetUserPermissionsQuery,GetUserPermissionsResponse>
{
    public async Task<GetUserPermissionsResponse> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        var user = await context.Users.Include(u => u.Roles)
            .ThenInclude(r=>r.Permissions)
            .FirstOrDefaultAsync(u => u.IdentityUserId == request.IdentityUserId,cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"User with id '{request.IdentityUserId}' not found.");
        }

        var permissions = user.Roles.SelectMany(r => r.Permissions).Select(p => p.Name)
            .ToHashSet();
        return new GetUserPermissionsResponse(permissions);
    }
}
