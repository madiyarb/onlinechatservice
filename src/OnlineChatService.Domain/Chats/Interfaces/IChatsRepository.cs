using OnlineChatService.Domain.Chats.Models;

namespace OnlineChatService.Domain.Chats.Interfaces;

public interface IChatsRepository
{
    Task Save(Chat entity, CancellationToken cancellationToken);
    Task<Chat?> Get(Guid id, CancellationToken cancellationToken);
    Task<Chat?> GetByUsers(List<Guid> userIds, CancellationToken cancellationToken);
    Task<bool> Any(Guid id, CancellationToken cancellationToken);
}