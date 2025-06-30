using OnlineChatService.Domain.Users.Models;

namespace OnlineChatService.Domain.Users.Interfaces;

public interface IUsersRepository
{
    Task Save(User entity, CancellationToken cancellationToken);
    Task<User?> Get(Guid id, CancellationToken cancellationToken);
    Task<List<User>> GetList(List<Guid> ids, CancellationToken cancellationToken);
    Task<bool> Any(Guid id, CancellationToken cancellationToken);
}