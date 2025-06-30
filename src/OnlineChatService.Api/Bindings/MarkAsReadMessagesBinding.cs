namespace OnlineChatService.Api.Bindings;

public record MarkAsReadMessagesBinding
{
    public List<Guid> MessagesId { get; set; }
    public Guid ChatId { get; set; }
}