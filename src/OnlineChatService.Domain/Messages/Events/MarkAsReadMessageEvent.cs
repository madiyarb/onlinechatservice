namespace OnlineChatService.Domain.Messages.Events;

public class MarkAsReadMessageEvent
{
    public string Type { get; set; } = nameof(MarkAsReadMessageEvent);
    public Guid MessageId { get; set; }
    public Guid ChatId { get; set; }
}