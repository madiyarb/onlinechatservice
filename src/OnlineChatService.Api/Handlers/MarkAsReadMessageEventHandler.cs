using AWS.Messaging;
using OnlineChatService.Application.Cache.Enums;
using OnlineChatService.Application.Cache.Interfaces;
using OnlineChatService.Application.OnlineChats.Interfaces;
using OnlineChatService.Domain.Messages.Events;
using OnlineChatService.Domain.Messages.Interfaces;
using OnlineChatService.Domain.Messages.Models;

namespace OnlineChatService.Api.Handlers;

public class MarkAsReadMessageEventHandler(ICacheProcessor cacheProcessor, IMessagesRepository messagesRepository, ISignalRNotificationService signalRNotification) : IMessageHandler<MarkAsReadMessageEvent>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<MarkAsReadMessageEvent> messageEnvelope, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var eventMessage = messageEnvelope.Message;
            Message? messageCache =
                await cacheProcessor.GetAsync<Message>(CacheTypeEnums.Hybrid, "Messages" + eventMessage.MessageId);
            Message? messageDb = await messagesRepository.Get(eventMessage.MessageId, cancellationToken);
            
            if (messageDb is not null)
            {
                messageDb.MarkAsRead();
                await messagesRepository.Save(messageDb, cancellationToken);
            }

            if (messageCache is not null)
            {
                messageDb.MarkAsRead();
                await cacheProcessor.RemoveAsync(CacheTypeEnums.Hybrid, "Messages" + eventMessage.MessageId);
                await cacheProcessor.RemoveAsync(CacheTypeEnums.Hybrid, "ChatMessages" + eventMessage.ChatId);
                await cacheProcessor.SetAsync(CacheTypeEnums.Hybrid, "Messages" + eventMessage.MessageId, messageCache,
                    TimeSpan.FromHours(24),
                    cancellationToken);
                await cacheProcessor.SetAsync(CacheTypeEnums.Hybrid, "ChatMessages" + eventMessage.MessageId,
                    messageCache, TimeSpan.FromHours(24),
                    cancellationToken);
            }
            
            await signalRNotification.Notify(eventMessage, eventMessage.ChatId.ToString(), cancellationToken);

            return MessageProcessStatus.Success();
        }
        catch (Exception e)
        {
            return MessageProcessStatus.Failed();
            throw;
        }
    }
}