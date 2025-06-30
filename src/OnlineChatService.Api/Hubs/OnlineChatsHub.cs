using Microsoft.AspNetCore.SignalR;
using OnlineChatService.Application.Cache.Interfaces;
using OnlineChatService.Application.OnlineChats.Interfaces;
using OnlineChatService.Domain.Chats.Interfaces;
using OnlineChatService.Domain.Chats.Models;
using OnlineChatService.Domain.Users.Interfaces;
using OnlineChatService.Domain.Users.Models;

namespace OnlineChatService.Api.Hubs;

public sealed class OnlineChatsHub : Hub
{
    private readonly ICacheProcessor _cacheProcessor;
    private readonly IChatsRepository _chatsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IOnlineChatService _onlineChatService;

    public OnlineChatsHub(ICacheProcessor cacheProcessor, IChatsRepository chatsRepository, IUsersRepository usersRepository, IOnlineChatService onlineChatService)
    {
        _cacheProcessor = cacheProcessor;
        _chatsRepository = chatsRepository;
        _usersRepository = usersRepository;
        _onlineChatService = onlineChatService;
    }

    /// <summary>
    /// OnConnectedAsync
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        string chatId = httpContext?.Request.Query["chatId"];
        string userId = httpContext?.Request.Query["userId"];
        string handlerUserId = httpContext?.Request.Query["handlerUserId"];
        
        
        if (string.IsNullOrEmpty(userId))
        {
            await Clients.Caller.SendAsync("OnConnectionError", "User ID is missing.");
            Context.Abort(); 
            return;
        }
        
        if (!Guid.TryParse(userId, out Guid userIdGuid))
        {
            await Clients.Caller.SendAsync("OnConnectionError", "User ID is incorrect.");
            Context.Abort();
            return;
        }
        
        if (!await _usersRepository.Any(userIdGuid, default))
        {
            await Clients.Caller.SendAsync("OnConnectionError", "User is not found.");
            Context.Abort(); 
            return;
        }
        
        if (string.IsNullOrEmpty(handlerUserId))
        {
            await Clients.Caller.SendAsync("OnConnectionError", "User ID is missing.");
            Context.Abort();
            return;
        }
        
        if (!Guid.TryParse(handlerUserId, out Guid handlerUserIdGuid))
        {
            await Clients.Caller.SendAsync("OnConnectionError", "Handler User ID is incorrect.");
            Context.Abort(); 
            return;
        }
        
        if (!await _usersRepository.Any(handlerUserIdGuid, default))
        {
            await Clients.Caller.SendAsync("OnConnectionError", "Handler User is not found.");
            Context.Abort(); 
            return;
        }

        Guid chatIdGuid = Guid.Empty;
        if (!string.IsNullOrEmpty(chatId) && !Guid.TryParse(chatId, out chatIdGuid))
        {
            await Clients.Caller.SendAsync("OnConnectionError", "Chat ID is incorrect.");
            Context.Abort(); 
            return;
        }
        
        if (!await _chatsRepository.Any(chatIdGuid, default))
        {
            await Clients.Caller.SendAsync("OnConnectionError", "Chat is not found.");
            Context.Abort(); 
            return;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(chatId))
            {
                List<Guid> userIds = new() { userIdGuid, handlerUserIdGuid };
                await _onlineChatService.StartChat(null, userIds, default);
            }
            else
            {
                List<Guid> userIds = new() { userIdGuid, handlerUserIdGuid };
                await _onlineChatService.StartChat(chatIdGuid, userIds, default);
            }
        }
        catch (Exception e)
        {
            await Clients.Caller.SendAsync("OnConnectionError", e.Message);
            Context.Abort(); 
            return;
        }

        await _onlineChatService.SetOnlineUsers(new List<Guid>(){userIdGuid, handlerUserIdGuid}, default);
        await Groups.AddToGroupAsync(Context.ConnectionId, $"ChatId-{chatId}");
        await base.OnConnectedAsync();
    }
}
