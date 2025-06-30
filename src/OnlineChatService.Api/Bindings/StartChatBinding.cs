namespace OnlineChatService.Api.Bindings;

public record StartChatBinding
{
    public List<Guid> UserIds { get; set; }
    public Guid? ChatId { get; set; }
}