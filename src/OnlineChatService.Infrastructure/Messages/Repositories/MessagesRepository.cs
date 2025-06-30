using Microsoft.EntityFrameworkCore;
using OnlineChatService.Domain.Exceptions;
using OnlineChatService.Domain.Messages.Interfaces;
using OnlineChatService.Domain.Messages.Models;
using OnlineChatService.Infrastructure.Database.DbContexts;

namespace OnlineChatService.Infrastructure.Messages.Repositories;

public class MessagesRepository : IMessagesRepository
{
    private readonly OnlineChatDbContext _context;

    public MessagesRepository(OnlineChatDbContext context)
    {
        _context = context;
    }

    public async Task Save(Message entity, CancellationToken cancellationToken)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            await _context.Messages.AddAsync(entity, cancellationToken);
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
    
    public async Task SaveRange(List<Message> entities, CancellationToken cancellationToken)
    {
        foreach (var entity in entities)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                await _context.Messages.AddAsync(entity, cancellationToken);
            }
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
    
    public async Task<Message?> Get(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Messages
            .SingleOrDefaultAsync(channel => channel.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetRange(Guid chatId, int skip, int take, CancellationToken cancellationToken)
    {
        return await _context.Messages
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.TimeStamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetUnReadMessages(int skip, int take, CancellationToken cancellationToken)
    {
        return await _context.Messages
            .Where(m => m.IsRead == false)
            .OrderBy(m => m.TimeStamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}