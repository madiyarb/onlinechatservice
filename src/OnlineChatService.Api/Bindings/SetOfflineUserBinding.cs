namespace OnlineChatService.Api.Bindings;

public record SetOfflineUserBinding
{
    public Guid UserId { get; set; }
}