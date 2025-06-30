using Microsoft.EntityFrameworkCore;
using OnlineChatService.Application.Messages.Interfaces;
using OnlineChatService.Domain.Messages.Models;
using OnlineChatService.Infrastructure.Database.DbContexts;

namespace OnlineChatService.Infrastructure.Messages.QueryHandlers;

public class MessageQueryHandler : IMessageQueryHandler
{
    private readonly OnlineChatDbContext _context;

    public MessageQueryHandler(OnlineChatDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Message>> Handle(Guid chatId, int skip, int take,
        CancellationToken cancellationToken)
    {
        return await _context.Messages
            .AsNoTracking()
            .Where(m => m.ChatId == chatId)
            .OrderByDescending(m => m.TimeStamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<Message>> Handle(int skip, int take, CancellationToken cancellationToken)
    {
        return await _context.Messages
            .Include(a=>a.User)
            .AsNoTracking()
            .Where(m => m.IsRead == false && m.TimeStamp <= DateTimeOffset.Now - TimeSpan.FromMinutes(5))
            .OrderBy(m => m.TimeStamp)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }
}