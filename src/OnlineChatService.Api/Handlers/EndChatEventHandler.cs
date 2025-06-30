using AWS.Messaging;
using OnlineChatService.Application.OnlineChats.Interfaces;
using OnlineChatService.Domain.Chats.Events;

namespace OnlineChatService.Api.Handlers;

public class EndChatEventHandler(IOnlineChatService onlineChatService) : IMessageHandler<EndChatEvent>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<EndChatEvent> messageEnvelope,
        CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var eventMessage = messageEnvelope.Message;
            await onlineChatService.EndChat(eventMessage.ChatId, cancellationToken);
            return MessageProcessStatus.Success();
        }
        catch (Exception e)
        {
            return MessageProcessStatus.Failed();
            throw;
        }
    }
}