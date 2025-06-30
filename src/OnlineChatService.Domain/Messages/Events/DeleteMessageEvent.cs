namespace OnlineChatService.Domain.Messages.Events;

public class DeleteMessageEvent
{
    public string Type { get; set; } = nameof(DeleteMessageEvent);
    public Guid MessageId { get; set; }
    public Guid ChatId { get; set; }
}