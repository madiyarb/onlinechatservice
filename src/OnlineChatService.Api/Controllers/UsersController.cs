using Ardalis.Result;
using AWS.Messaging.Publishers.SNS;
using Microsoft.AspNetCore.Mvc;
using OnlineChatService.Api.Bindings;
using OnlineChatService.Application.Cache.Enums;
using OnlineChatService.Domain.Messages.Events;
using OnlineChatService.Domain.Users.Interfaces;
using OnlineChatService.Domain.Users.Models;

namespace OnlineChatService.Api.Controllers;

public class UsersController : ControllerBase
{
    
    /// <summary>
    /// Set Offline user
    /// </summary>
    /// <param name="binding"></param>
    /// <param name="usersRepository"></param>
    /// <param name="snsPublisher"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("api/user/offline")]
    public async Task<IActionResult> SetOfflineUser(
        [FromBody] SetOfflineUserBinding binding,
        [FromServices] IUsersRepository usersRepository,
        [FromServices] ISNSPublisher snsPublisher,
        CancellationToken cancellationToken
    )
    {
        User? user = await usersRepository.Get(binding.UserId, cancellationToken);

        if (user is null )
        {
            return NotFound(Result.NotFound("User not found."));
        }
        
        await snsPublisher.PublishAsync(new SetOfflineUserEvent()
            {
                UserId = binding.UserId
            },
            new SNSOptions()
            {
                MessageGroupId = binding.UserId.ToString()
            });


        return Ok(Result.Success());
    }
}