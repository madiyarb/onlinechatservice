using AWS.Messaging;
using OnlineChatService.Application.Cache.Enums;
using OnlineChatService.Application.Cache.Interfaces;
using OnlineChatService.Application.Email.Interfaces;
using OnlineChatService.Domain.Messages.Events;
using OnlineChatService.Domain.Messages.Models;
using OnlineChatService.Domain.Users.Commands;

namespace OnlineChatService.Api.Handlers;

public class SendEmailCommandHandler(IEmailService emailService, ICacheProcessor cacheProcessor)  : IMessageHandler<SendEmailCommand>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<SendEmailCommand> messageEnvelope, CancellationToken cancellationToken = new CancellationToken())
    {
        var eventMessage = messageEnvelope.Message;
        Message? messageCache = await cacheProcessor.GetAsync<Message>(CacheTypeEnums.Hybrid, "Email"+eventMessage.Message.Id.ToString(), cancellationToken);
        if (messageCache is not null)
        {
            return MessageProcessStatus.Success();
        }
        else
        {
            await emailService.Send(eventMessage.Message.User, eventMessage.Message);
            return MessageProcessStatus.Success();
        }
    }
}