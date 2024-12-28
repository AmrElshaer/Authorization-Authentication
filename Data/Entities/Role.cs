using System.Collections;
using Ardalis.SmartEnum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authorization.Data.Entities;

public class Role
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public ICollection<Permission> Permissions { get; set; } = default!;
    public ICollection<User> Users { get; set; }= default!;
}

public class RoleEnum(int value, string name) : SmartEnum<RoleEnum>(name, value)
{
    public static readonly RoleEnum Admin = new (1,"Admin");
    public static readonly RoleEnum User = new (2, "User");
}
public class RoleConfiguration:IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.HasMany<Permission>(r => r.Permissions)
            .WithMany()
            .UsingEntity<RolePermission>();
        builder.HasMany<User>(r => r.Users)
            .WithMany(u => u.Roles)
            .UsingEntity<UserRole>();
        builder.HasData(RoleEnum.List.Select(r => new Role
        {
            Id = r,
            Name = r.Name
        }));
    }
}