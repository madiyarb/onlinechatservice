using Ardalis.Result;
using AWS.Messaging.Publishers.SNS;
using Microsoft.AspNetCore.Mvc;
using OnlineChatService.Api.Bindings;
using OnlineChatService.Api.Handlers;
using OnlineChatService.Application.Cache.Enums;
using OnlineChatService.Application.Cache.Interfaces;
using OnlineChatService.Application.Messages.Interfaces;
using OnlineChatService.Application.OnlineChats.Interfaces;
using OnlineChatService.Domain.Messages.Events;
using OnlineChatService.Domain.Messages.Interfaces;
using OnlineChatService.Domain.Messages.Models;
using OnlineChatService.Domain.Users.Models;

namespace OnlineChatService.Api.Controllers;

public class MessagesController : ControllerBase
{
    /// <summary>
    /// Get messages
    /// </summary>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="chatId"></param>
    /// <param name="queryHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("api/messages/{skip}/{take}")]
    public async Task<IActionResult> GetMessages(
        [FromRoute] int skip,
        [FromRoute] int take,
        [FromQuery] Guid chatId,
        [FromServices] IMessageQueryHandler queryHandler,
        CancellationToken cancellationToken
    )
    {
        var messages = await queryHandler.Handle(chatId, skip, take, cancellationToken);
        return Ok(Result.Success(messages));
    }


    /// <summary>
    /// Send message
    /// </summary>
    /// <param name="binding"></param>
    /// <param name="onlineChatService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("api/messages/send")]
    public async Task<IActionResult> SendMessage(
        [FromBody] SendMessageBinding binding,
        [FromServices] IOnlineChatService onlineChatService,
        CancellationToken cancellationToken
    )
    {
        await onlineChatService.SendMessage(binding.ChatId, binding.UserId, binding.Content, cancellationToken);
        return Ok(Result.Success());
    }

    
    /// <summary>
    /// Update Content
    /// </summary>
    /// <param name="binding"></param>
    /// <param name="cacheProcessor"></param>
    /// <param name="repository"></param>
    /// <param name="snsPublisher"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("api/messages")]
    public async Task<IActionResult> UpdateMessage(
        [FromBody] UpdateMessageBinding binding,
        [FromServices] ICacheProcessor cacheProcessor,
        [FromServices] IMessagesRepository repository,
        [FromServices] ISNSPublisher snsPublisher,
        CancellationToken cancellationToken
    )
    {
        Message? messageCache =
            await cacheProcessor.GetAsync<Message>(CacheTypeEnums.Hybrid, "Messages" + binding.MessageId);
        Message? messageDb = await repository.Get(binding.MessageId, cancellationToken);

        if (messageCache is null && messageDb is null)
        {
            return NotFound(Result.NotFound("Message not found."));
        }
        
        await snsPublisher.PublishAsync(new UpdateMessageEvent()
            {
                Content = binding.Content,
                ChatId = binding.ChatId,
                MessageId = binding.MessageId,
            },
            new SNSOptions()
            {
                MessageGroupId = binding.MessageId.ToString()
            });


        return Ok(Result.Success());
    }
    
    /// <summary>
    /// Delete message
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="chatId"></param>
    /// <param name="cacheProcessor"></param>
    /// <param name="repository"></param>
    /// <param name="snsPublisher"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("api/messages")]
    public async Task<IActionResult> DeleteMessage(
        [FromRoute] Guid messageId,
        [FromRoute] Guid chatId,
        [FromServices] ICacheProcessor cacheProcessor,
        [FromServices] IMessagesRepository repository,
        [FromServices] ISNSPublisher snsPublisher,
        CancellationToken cancellationToken
    )
    {
        Message? messageCache =
            await cacheProcessor.GetAsync<Message>(CacheTypeEnums.Hybrid, "Messages" + messageId);
        Message? messageDb = await repository.Get(messageId, cancellationToken);

        if (messageCache is null && messageDb is null)
        {
            return NotFound(Result.NotFound("Message not found."));
        }

        
        await snsPublisher.PublishAsync(new DeleteMessageEvent()
            {
                ChatId = chatId,
                MessageId = messageId,
            },
            new SNSOptions()
            {
                MessageGroupId = messageId.ToString()
            });


        return Ok(Result.Success());
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="binding"></param>
    /// <param name="cacheProcessor"></param>
    /// <param name="repository"></param>
    /// <param name="snsPublisher"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("api/messages/read")]
    public async Task<IActionResult> MarkAsReadMessages(
        [FromBody] MarkAsReadMessagesBinding binding,
        [FromServices] ICacheProcessor cacheProcessor,
        [FromServices] IMessagesRepository repository,
        [FromServices] ISNSPublisher snsPublisher,
        CancellationToken cancellationToken
    )
    {
        foreach (var messageId in binding.MessagesId)
        {
            Message? messageCache =
                await cacheProcessor.GetAsync<Message>(CacheTypeEnums.Hybrid, "Messages" + messageId);
            Message? messageDb = await repository.Get(messageId, cancellationToken);

            if (messageCache is null && messageDb is null)
            {
                continue;
            }
        
            await snsPublisher.PublishAsync(new MarkAsReadMessageEvent()
                {
                    MessageId = messageId,
                    ChatId = binding.ChatId,
                },
                new SNSOptions()
                {
                    MessageGroupId = messageId.ToString()
                });
        }


        return Ok(Result.Success());
    }
    
    
  /// <summary>
  /// GetUnReadMessages
  /// </summary>
  /// <param name="skip"></param>
  /// <param name="take"></param>
  /// <param name="queryHandler"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
    [HttpGet("api/messages/unread/{skip}/{take}")]
    public async Task<IActionResult> GetUnReadMessages(
        [FromRoute] int skip,
        [FromRoute] int take,
        [FromServices] IMessageQueryHandler queryHandler,
        [FromServices] ICacheProcessor cacheProcessor,
        CancellationToken cancellationToken
    )
    {
        
        var messages = await queryHandler.Handle(skip, take, cancellationToken);

        var result = new List<Message>();

        foreach (var message in messages)
        {
            var user = await cacheProcessor.GetAsync<User>(
                CacheTypeEnums.Hybrid, 
                "UserOffline" + message.UserId.ToString(), 
                cancellationToken
            );

            if (user is not null)
            {
                result.Add(message);
            }
        }

        return Ok(Result.Success(result));

    }
}