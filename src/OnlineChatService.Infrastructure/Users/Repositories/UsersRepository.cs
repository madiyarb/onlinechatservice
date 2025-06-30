using Microsoft.EntityFrameworkCore;
using OnlineChatService.Domain.Exceptions;
using OnlineChatService.Domain.Users.Interfaces;
using OnlineChatService.Domain.Users.Models;
using OnlineChatService.Infrastructure.Database.DbContexts;

namespace OnlineChatService.Infrastructure.Users.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly OnlineChatDbContext _context;

    public UsersRepository(OnlineChatDbContext context)
    {
        _context = context;
    }

    public async Task Save(User entity, CancellationToken cancellationToken)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            await _context.Users.AddAsync(entity, cancellationToken);
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
    
    public async Task<User?> Get(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users
            .SingleOrDefaultAsync(channel => channel.Id == id, cancellationToken);
    }
    
    public async Task<List<User>> GetList(List<Guid> ids, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Where(channel => ids.Contains(channel.Id)).ToListAsync(cancellationToken);
    }
    
    public async Task<bool> Any(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Chats.AnyAsync(c => c.Id == id, cancellationToken);
    }
}