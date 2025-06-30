using OnlineChatService.Domain.Chats.Models;

namespace OnlineChatService.Domain.Users.Models;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public List<Chat> Chats { get; private set; } = new List<Chat>();

    public User(string email)
    {
        Id = Guid.NewGuid();
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));
        Email = email;
    }
    
    public void AddChat(Chat chat)
    {
        if (chat is null)
            throw new ArgumentNullException(nameof(chat), "Chat cannot be null");
        if (Chats.Any(c => c.Id == chat.Id))
            throw new InvalidOperationException("Chat with this ID already exists in user's chat list");
        
        Chats.Add(chat);
    }

}