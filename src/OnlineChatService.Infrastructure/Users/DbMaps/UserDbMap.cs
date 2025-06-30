using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineChatService.Domain.Users.Models;

namespace OnlineChatService.Infrastructure.Users.DbMaps;

public class UserDbMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("UserId")
            .ValueGeneratedOnAdd();

        builder
            .HasMany(u => u.Chats)
            .WithMany(c => c.Users)
            .UsingEntity(j => j.ToTable("ChatUsers"));
    }
}