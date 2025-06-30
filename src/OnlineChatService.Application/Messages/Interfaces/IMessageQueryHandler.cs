using OnlineChatService.Domain.Messages.Models;

namespace OnlineChatService.Application.Messages.Interfaces;

public interface IMessageQueryHandler
{
    Task<IEnumerable<Message>> Handle(Guid chatId, int skip, int take,
        CancellationToken cancellationToken);

    Task<List<Message>> Handle(int skip, int take, CancellationToken cancellationToken);
}