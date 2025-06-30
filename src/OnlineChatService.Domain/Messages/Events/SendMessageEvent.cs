using OnlineChatService.Domain.Messages.Models;

namespace OnlineChatService.Domain.Messages.Events;

public class SendMessageEvent
{
    public string Type { get; set; } = nameof(SendMessageEvent);
    public Message Message { get; set; }
    public Guid ChatId { get; set; }
}