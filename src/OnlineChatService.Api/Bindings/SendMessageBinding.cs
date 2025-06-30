namespace OnlineChatService.Api.Bindings;

public record SendMessageBinding
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; }
}