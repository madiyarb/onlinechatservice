using OnlineChatService.Domain.Messages.Models;
using OnlineChatService.Domain.Users.Models;

namespace OnlineChatService.Domain.Chats.Models;

public class Chat
{
    public Guid Id { get; private set; }
    public List<User> Users { get; private set; } = new List<User>();
    public List<Message> Messages { get; set; } = new List<Message>();

    private Chat()
    {
        
    }

    public Chat(List<User> users)
    {
        if (users is null)
            throw new ArgumentNullException(nameof(users));

        if (users.Count != 2)
            throw new ArgumentException("Users count must be 2");
        
        if (users.Any(u => u is null))
            throw new ArgumentException("Users cannot be null");
        
        Id = Guid.NewGuid();
        Users = users;
        Messages = new List<Message>();
    }

    public void AddMessages(List<Message> messages)
    {
        Messages.AddRange(messages);
    }
}