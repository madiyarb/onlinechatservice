using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineChatService.Application.Cache.Interfaces;
using OnlineChatService.Application.Email.Interfaces;
using OnlineChatService.Application.Messages.Interfaces;
using OnlineChatService.Domain.Chats.Interfaces;
using OnlineChatService.Domain.Messages.Interfaces;
using OnlineChatService.Domain.Users.Interfaces;
using OnlineChatService.Infrastructure.Cache.Processors;
using OnlineChatService.Infrastructure.Cache.Services;
using OnlineChatService.Infrastructure.Chats.Repositories;
using OnlineChatService.Infrastructure.Database.DbContexts;
using OnlineChatService.Infrastructure.Email;
using OnlineChatService.Infrastructure.Messages.QueryHandlers;
using OnlineChatService.Infrastructure.Messages.Repositories;
using OnlineChatService.Infrastructure.Users.Repositories;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace OnlineChatService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        #region Redis

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetSection("Redis:Configuration").Value;
            options.InstanceName = configuration.GetSection("Redis:InstanceName").Value;
        });

        #endregion

        #region HybridCache

        FusionCacheOptions opt = new FusionCacheOptions()
        {
            CacheKeyPrefix = "TaskManager"
        };
        services.AddFusionCache()
            .WithDistributedCache(_ =>
            {
                var options = new RedisCacheOptions
                {
                    Configuration = configuration.GetSection("Redis:Configuration").Value,
                    InstanceName = configuration.GetSection("Redis:InstanceName").Value,
                };

                return new RedisCache(options);
            })
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .AsHybridCache();

        #endregion
        
        #region Cache
        services.AddScoped<ICacheProcessor, CacheProcessor>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<ICacheService, HybridCacheService>();
        #endregion

        #region Database
        services.AddDbContext<OnlineChatDbContext>();
        #endregion
        
        #region Users
        services.AddScoped<IUsersRepository, UsersRepository>();
        #endregion
        
        #region Messages
        services.AddScoped<IMessagesRepository, MessagesRepository>();
        services.AddScoped<IMessageQueryHandler, MessageQueryHandler>();
        #endregion
        
        #region Chats
        services.AddScoped<IChatsRepository, ChatsRepository>();
        #endregion
        
        #region Email
        services.AddScoped<IEmailService, EmailService>();
        #endregion
    }
}