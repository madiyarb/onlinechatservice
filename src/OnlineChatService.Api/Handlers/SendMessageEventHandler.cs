using AWS.Messaging;
using OnlineChatService.Application.OnlineChats.Interfaces;
using OnlineChatService.Domain.Messages.Events;

namespace OnlineChatService.Api.Handlers;

public class SendMessageEventHandler(ISignalRNotificationService signalRNotificationService) : IMessageHandler<SendMessageEvent>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<SendMessageEvent> messageEnvelope, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var eventMessage = messageEnvelope.Message;
            await signalRNotificationService.Notify(eventMessage.Message, eventMessage.ChatId.ToString(), cancellationToken);
            return MessageProcessStatus.Success();
        }
        catch (Exception e)
        {
            return MessageProcessStatus.Failed();
            throw;
        }
    }
}