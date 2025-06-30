using Microsoft.EntityFrameworkCore;
using OnlineChatService.Domain.Chats.Interfaces;
using OnlineChatService.Domain.Chats.Models;
using OnlineChatService.Domain.Exceptions;
using OnlineChatService.Infrastructure.Database.DbContexts;

namespace OnlineChatService.Infrastructure.Chats.Repositories;

public class ChatsRepository : IChatsRepository
{
    private readonly OnlineChatDbContext _context;

    public ChatsRepository(OnlineChatDbContext context)
    {
        _context = context;
    }

    public async Task Save(Chat entity, CancellationToken cancellationToken)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            await _context.Chats.AddAsync(entity, cancellationToken);
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new DbConcurrencyException(ex);
        }
    }
    
    public async Task<Chat?> Get(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Chats
            .SingleOrDefaultAsync(channel => channel.Id == id, cancellationToken);
    }
    
    public async Task<Chat?> GetByUsers(List<Guid> userIds, CancellationToken cancellationToken)
    {
        return await _context.Chats
            .Include(c => c.Users)
            .Where(c => userIds.All(id => c.Users.Select(u => u.Id).Contains(id)))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> Any(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Chats.AnyAsync(c => c.Id == id, cancellationToken);
    }
}