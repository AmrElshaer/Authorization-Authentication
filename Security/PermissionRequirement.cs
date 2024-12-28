using Microsoft.AspNetCore.Authorization;

namespace Authorization.Security;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}