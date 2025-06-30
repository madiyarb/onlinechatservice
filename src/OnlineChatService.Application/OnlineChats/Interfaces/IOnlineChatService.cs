using OnlineChatService.Domain.Chats.Models;
using OnlineChatService.Domain.Messages.Models;

namespace OnlineChatService.Application.OnlineChats.Interfaces;

public interface IOnlineChatService
{
    Task<Chat> StartChat(Guid? chatId, List<Guid> userIds,CancellationToken cancellationToken);
    Task EndChat(Guid chatId, CancellationToken cancellationToken);
    Task SendMessage(Guid chatId, Guid userId, string content, CancellationToken cancellationToken);
    Task SetOnlineUsers(List<Guid> userIds, CancellationToken cancellationToken);
    Task SetOfflineUser(Guid userId, CancellationToken cancellationToken);
}