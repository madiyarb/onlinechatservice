using OnlineChatService.Application.Email.Interfaces;
using OnlineChatService.Domain.Messages.Models;
using OnlineChatService.Domain.Users.Commands;
using OnlineChatService.Domain.Users.Models;

namespace OnlineChatService.Infrastructure.Email;

public class EmailService : IEmailService
{
    public async Task Send(User user, Message message)
    {
        throw new NotImplementedException();
    }
}