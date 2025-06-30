using Microsoft.Extensions.DependencyInjection;
using OnlineChatService.Application.OnlineChats.Interfaces;

namespace OnlineChatService.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        #region Online Chat Service
        services.AddScoped<IOnlineChatService, OnlineChats.Services.OnlineChatService>();
        #endregion

    }
}