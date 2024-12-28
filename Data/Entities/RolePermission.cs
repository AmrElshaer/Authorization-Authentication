using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authorization.Data.Entities;

public class RolePermission
{
    public int RoleId { get; init; }
    public int PermissionId { get; init;}
}

public class RolePermissionConfiguration:IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermission");
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
        builder.HasData(Create(RoleEnum.Admin, PermissionEnum.ReadUser),
            Create(RoleEnum.Admin, PermissionEnum.CreateUser),
            Create(RoleEnum.User, PermissionEnum.ReadUser),
            Create(RoleEnum.Admin, PermissionEnum.ReadWeathers));
    }

    private static RolePermission Create(RoleEnum role,PermissionEnum permission)
    {
        return new RolePermission
        {
            PermissionId = permission,
            RoleId = role
        };
    }
}