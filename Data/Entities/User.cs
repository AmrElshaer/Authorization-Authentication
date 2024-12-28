using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authorization.Data.Entities;

public class User
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public  ICollection<Role> Roles { get; set; }
    public string? IdentityUserId { get; set; }
}
public class UserConfiguration:IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.Property(u => u.Name).HasMaxLength(255);
        builder.HasMany(u => u.Roles)
            .WithMany(r=>r.Users)
            .UsingEntity<UserRole>();
        builder.HasData(new User()
        {
            Id = 1,
            Name = "Admin user",
        });
    }
}