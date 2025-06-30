using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using OnlineChatService.Api.Hubs;
using OnlineChatService.Application.OnlineChats.Interfaces;

namespace OnlineChatService.Api.SignalR;
public sealed class SignalRNotificationService : ISignalRNotificationService
{
    private readonly IHubContext<OnlineChatsHub> _userHab;
    private static JsonSerializerOptions _serializerOptions;
    public SignalRNotificationService(IHubContext<OnlineChatsHub> userHab)
    {
        _userHab = userHab;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
    
    public async Task Notify<T>(T message, string chatId, CancellationToken cancellationToken, string method = null)
    {
        if (string.IsNullOrEmpty(method))
        {
            method = typeof(T).Name;//if method not set use Typeof
        }
        await _userHab.Clients.Group($"ChatId-{chatId}")
            .SendAsync(method, JsonSerializer.Serialize(message, _serializerOptions), cancellationToken);
    }
}