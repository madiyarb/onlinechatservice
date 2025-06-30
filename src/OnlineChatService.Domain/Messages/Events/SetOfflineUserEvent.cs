namespace OnlineChatService.Domain.Messages.Events;

public class SetOfflineUserEvent
{
    public string Type { get; set; } = nameof(SetOfflineUserEvent);
    public Guid UserId { get; set; }
}