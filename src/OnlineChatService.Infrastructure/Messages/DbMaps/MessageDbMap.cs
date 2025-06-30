using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineChatService.Domain.Messages.Models;

namespace OnlineChatService.Infrastructure.Messages.DbMaps;

public class MessageDbMap : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("MessageId")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.Content)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.TimeStamp)
            .IsRequired();

        builder.Property(m => m.IsRead)
            .IsRequired();
        
        builder.Property(m => m.IsDeleted)
            .IsRequired();

        builder.HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}