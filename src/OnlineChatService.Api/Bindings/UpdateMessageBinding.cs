namespace OnlineChatService.Api.Bindings;

public record UpdateMessageBinding
{
    public Guid MessageId { get; set; }
    public Guid ChatId { get; set; }
    public string Content { get; set; }
}