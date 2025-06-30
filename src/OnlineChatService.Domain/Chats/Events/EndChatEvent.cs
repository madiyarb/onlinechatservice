namespace OnlineChatService.Domain.Chats.Events;

public record EndChatEvent
{
    public string Type { get; set; } = nameof(EndChatEvent);
    public Guid ChatId { get; set; }
}