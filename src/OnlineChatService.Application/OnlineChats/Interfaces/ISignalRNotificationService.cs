namespace OnlineChatService.Application.OnlineChats.Interfaces;

public interface ISignalRNotificationService
{
    public Task Notify<T>(T message, string chatId, CancellationToken cancellationToken,
        string method = null);
}