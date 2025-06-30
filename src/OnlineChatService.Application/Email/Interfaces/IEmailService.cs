using OnlineChatService.Domain.Messages.Models;
using OnlineChatService.Domain.Users.Models;

namespace OnlineChatService.Application.Email.Interfaces;

public interface IEmailService
{
    Task Send(User user, Message message);
}