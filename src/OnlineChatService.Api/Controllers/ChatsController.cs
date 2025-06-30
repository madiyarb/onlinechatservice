using Ardalis.Result;
using AWS.Messaging.Publishers.SNS;
using Microsoft.AspNetCore.Mvc;
using OnlineChatService.Api.Bindings;
using OnlineChatService.Application.OnlineChats.Interfaces;
using OnlineChatService.Domain.Chats.Events;

namespace OnlineChatService.Api.Controllers;

public class ChatsController : ControllerBase
{
    /// <summary>
    /// Start chat
    /// </summary>
    /// <param name="binding"></param>
    /// <param name="onlineChatService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("api/chats/start")]
    public async Task<IActionResult> StartChat(
        [FromBody] StartChatBinding binding,
        [FromServices] IOnlineChatService onlineChatService,
        CancellationToken cancellationToken
    )
    {
        var result = await onlineChatService.StartChat(binding.ChatId, binding.UserIds, cancellationToken);
        return Ok(Result.Success(result));
    }

    /// <summary>
    /// End chat
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="snsPublisher"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("api/chats/end/{chatId}")]
    public async Task<IActionResult> EndChat(
        [FromRoute] Guid chatId,
        [FromServices] ISNSPublisher snsPublisher,
        CancellationToken cancellationToken
    )
    {
        await snsPublisher.PublishAsync(new EndChatEvent()
            {
                ChatId = chatId,
            },
            new SNSOptions()
            {
                MessageGroupId = chatId.ToString()
            });
        return Ok(Result.Success());
    }
}