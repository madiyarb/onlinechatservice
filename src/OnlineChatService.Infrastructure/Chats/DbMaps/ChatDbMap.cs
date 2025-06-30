using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineChatService.Domain.Chats.Models;

namespace OnlineChatService.Infrastructure.Chats.DbMaps;

public class ChatDbMap : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("Chats");
        builder.HasKey(jt => jt.Id);
        builder.Property(jt => jt.Id).HasColumnName("ChatId")
            .ValueGeneratedOnAdd();
        
        builder
            .HasMany(u => u.Users)
            .WithMany(c => c.Chats)
            .UsingEntity(j => j.ToTable("ChatUsers"));
        
        builder.HasMany(cd => cd.Messages)
            .WithOne(cdr => cdr.Chat)
            .HasForeignKey(cdr => cdr.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}