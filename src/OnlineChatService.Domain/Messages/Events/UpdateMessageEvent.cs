namespace OnlineChatService.Domain.Messages.Events;

public class UpdateMessageEvent
{
    public string Type { get; set; } = nameof(UpdateMessageEvent);
    public Guid MessageId { get; set; }
    public Guid ChatId { get; set; }
    public string Content { get; set; }
}