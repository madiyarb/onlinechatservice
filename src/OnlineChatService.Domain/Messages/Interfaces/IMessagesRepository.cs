using OnlineChatService.Domain.Messages.Models;

namespace OnlineChatService.Domain.Messages.Interfaces;

public interface IMessagesRepository
{
    Task Save(Message entity, CancellationToken cancellationToken);
    Task SaveRange(List<Message> entities, CancellationToken cancellationToken);
    Task<Message?> Get(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Message>> GetRange(Guid chatId, int skip, int take, CancellationToken cancellationToken);
    Task<IEnumerable<Message>> GetUnReadMessages(int skip, int take, CancellationToken cancellationToken);
}