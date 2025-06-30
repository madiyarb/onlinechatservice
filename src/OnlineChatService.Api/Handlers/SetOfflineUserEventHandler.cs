using AWS.Messaging;
using OnlineChatService.Application.OnlineChats.Interfaces;
using OnlineChatService.Domain.Messages.Events;

namespace OnlineChatService.Api.Handlers;

public class SetOfflineUserEventHandler(IOnlineChatService onlineChatService) : IMessageHandler<SetOfflineUserEvent>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<SetOfflineUserEvent> messageEnvelope, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var eventMessage = messageEnvelope.Message;
            await onlineChatService.SetOfflineUser(eventMessage.UserId, cancellationToken);
            return MessageProcessStatus.Success();
        }
        catch (Exception e)
        {
            return MessageProcessStatus.Failed();
            throw;
        }
    }
}