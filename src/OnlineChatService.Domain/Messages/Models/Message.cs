using OnlineChatService.Domain.Chats.Models;
using OnlineChatService.Domain.Users.Models;

namespace OnlineChatService.Domain.Messages.Models;

public class Message
{
    public Guid Id { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public Guid ChatId { get; set; }
    public Chat Chat { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public DateTimeOffset TimeStamp { get; private set; } = DateTimeOffset.Now;
    public bool IsDeleted { get; private set; }
    public bool IsRead { get; private set; }


    private Message()
    {
        
    }

    public Message(string content, Chat chat, User user)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be null or empty", nameof(content));
        if (content.Length > 100)
            throw new ArgumentException("Content cannot be longer than 100 characters", nameof(content));
        if (chat == null)
            throw new ArgumentNullException(nameof(chat), "Chat cannot be null");
        if (user == null)
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        
        Id = Guid.NewGuid();
        Content = content;
        TimeStamp = DateTimeOffset.Now;
        ChatId = chat.Id;
        Chat = chat;
        UserId = user.Id;
        User = user;
        IsRead = false;
        IsDeleted = false;
    }
    
    public void MarkAsRead()
    {
        IsRead = true;
    }
    
    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
    
    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be null or empty", nameof(content));
        if (content.Length > 100)
            throw new ArgumentException("Content cannot be longer than 100 characters", nameof(content));

        Content = content;
    }
}