using OnlineChatService.Domain.Messages.Models;

namespace OnlineChatService.Domain.Users.Commands;

public class SendEmailCommand
{
    public string Type { get; set; } = nameof(SendEmailCommand);
    public Message Message { get; set; }
}