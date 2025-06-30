using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlineChatService.Domain.Chats.Models;
using OnlineChatService.Domain.Messages.Models;
using OnlineChatService.Domain.Users.Models;
using OnlineChatService.Infrastructure.Chats.DbMaps;
using OnlineChatService.Infrastructure.Messages.DbMaps;
using OnlineChatService.Infrastructure.Users.DbMaps;

namespace OnlineChatService.Infrastructure.Database.DbContexts;

public class OnlineChatDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<User> Users { get; set; }

    public OnlineChatDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("ConnectionString"));
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ChatDbMap());
        builder.ApplyConfiguration(new MessageDbMap());
        builder.ApplyConfiguration(new UserDbMap());
    }
}