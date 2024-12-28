using Ardalis.SmartEnum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authorization.Data.Entities;

public class Permission
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public  ICollection<Role>? Roles { get; set; }
}
public class PermissionEnum(string name, int value) : SmartEnum<PermissionEnum>(name, value)
{
    public static readonly PermissionEnum ReadUser = new PermissionEnum("ReadUser", 1);
    public static readonly PermissionEnum CreateUser = new PermissionEnum("CreateUser", 2);
    public static readonly PermissionEnum ReadWeathers = new PermissionEnum("ReadWeathers", 3);
}

public class PermissionConfiguration:IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.HasMany<Role>(r => r.Roles)
            .WithMany()
            .UsingEntity<RolePermission>();
        builder.HasData(PermissionEnum.List.Select(r => new Permission
        {
            Id = r,
            Name = r.Name
        }));
    }
}