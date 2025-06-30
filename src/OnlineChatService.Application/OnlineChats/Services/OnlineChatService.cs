using AWS.Messaging.Publishers.SNS;
using OnlineChatService.Application.Cache.Enums;
using OnlineChatService.Application.Cache.Interfaces;
using OnlineChatService.Application.OnlineChats.Interfaces;
using OnlineChatService.Domain.Chats.Interfaces;
using OnlineChatService.Domain.Chats.Models;
using OnlineChatService.Domain.Messages.Events;
using OnlineChatService.Domain.Messages.Interfaces;
using OnlineChatService.Domain.Messages.Models;
using OnlineChatService.Domain.Users.Interfaces;
using OnlineChatService.Domain.Users.Models;

namespace OnlineChatService.Application.OnlineChats.Services;

public class OnlineChatService : IOnlineChatService
{
    private readonly IChatsRepository _chatsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly ICacheProcessor _cacheProcessor;
    private readonly IMessagesRepository _messagesRepository;
    private readonly ISNSPublisher _snsPublisher;

    public OnlineChatService(IChatsRepository chatsRepository, IUsersRepository usersRepository,
        ICacheProcessor cacheProcessor, IMessagesRepository messagesRepository, ISNSPublisher snsPublisher)
    {
        _chatsRepository = chatsRepository;
        _usersRepository = usersRepository;
        _cacheProcessor = cacheProcessor;
        _messagesRepository = messagesRepository;
        _snsPublisher = snsPublisher;
    }

    public async Task<Chat> StartChat(Guid? chatId, List<Guid> userIds, CancellationToken cancellationToken)
    {
        Chat? chat = null;
        if (chatId is not null)
            chat = await _chatsRepository.Get(chatId.Value, cancellationToken);
        if (chat is not null)
            return chat;

        if (userIds.Count != 2)
            throw new ArgumentException("Users count must be 2", nameof(userIds));
        if (userIds.Any(a => a.Equals(Guid.Empty)))
            throw new ArgumentException("UserIds cannot be empty", nameof(userIds));
        List<User> users = await _usersRepository.GetList(userIds, cancellationToken);
        if (users.Count != 2)
            throw new ArgumentException("Users not found", nameof(userIds));

        chat = new Chat(users);
        await _chatsRepository.Save(chat, cancellationToken);

        return chat;
    }

    public async Task EndChat(Guid chatId, CancellationToken cancellationToken)
    {
        Chat? chat = await _chatsRepository.Get(chatId, cancellationToken);
        if (chat is null)
            throw new ArgumentException("Chat not found", nameof(chatId));

        List<Message>? messages =
            await _cacheProcessor.GetAsync<List<Message>>(CacheTypeEnums.Hybrid, chatId.ToString(), cancellationToken);

        if (messages is not null && messages.Any())
        {
            await _messagesRepository.SaveRange(messages, cancellationToken);
        }
    }

    public async Task SendMessage(Guid chatId, Guid userId, string content, CancellationToken cancellationToken)
    {
        if (chatId == Guid.Empty)
            throw new ArgumentException("Chat ID cannot be empty", nameof(chatId));
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(content) || content.Length > 100)
            throw new ArgumentException("Message content cannot be empty or length more than 100", nameof(content));

        Chat? chat = await _chatsRepository.Get(chatId, cancellationToken);
        if (chat is null)
            throw new ArgumentException("Chat not found", nameof(chatId));

        User? user = await _usersRepository.Get(userId, cancellationToken);
        if (user is null)
            throw new ArgumentException("User not found", nameof(userId));

        var message = new Message(content, chat, user);
        List<Message>? messages =
            await _cacheProcessor.GetAsync<List<Message>>(CacheTypeEnums.Hybrid, "Messages" + chatId.ToString(),
                cancellationToken) ?? new List<Message>();
        if (messages is null)
            messages = new List<Message>();

        messages.Add(message);
        await _cacheProcessor.SetAsync(CacheTypeEnums.Hybrid, "ChatMessages" + chatId.ToString(), messages,
            TimeSpan.FromHours(24), cancellationToken);
        await _cacheProcessor.SetAsync(CacheTypeEnums.Hybrid, "Messages" + message.Id.ToString(), messages,
            TimeSpan.FromHours(24), cancellationToken);
        await _snsPublisher.PublishAsync(new SendMessageEvent()
            {
                Message = message,
                ChatId = chatId
            },
            new SNSOptions()
            {
                MessageGroupId = chatId.ToString()
            });
    }

    public async Task SetOnlineUsers(List<Guid> userIds, CancellationToken cancellationToken)
    {
        if (userIds is null || !userIds.Any())
            throw new ArgumentException("User IDs cannot be null or empty", nameof(userIds));

        foreach (var userId in userIds)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            User? user = await _usersRepository.Get(userId, cancellationToken);
            if (user is null)
                throw new ArgumentException("User not found", nameof(userId));

            User? userCache = await _cacheProcessor.GetAsync<User>(CacheTypeEnums.Hybrid,
                "UserOffline" + userId.ToString(), cancellationToken);
            if (userCache is not null)
            {
                await _cacheProcessor.RemoveAsync(CacheTypeEnums.Hybrid, "UserOffline" + userId.ToString(),
                    cancellationToken);
            }

            await _cacheProcessor.SetAsync<User>(CacheTypeEnums.Hybrid, "UserOffline" + userId.ToString(), user,
                TimeSpan.FromHours(24));
        }
    }

    public async Task SetOfflineUser(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        User? user = await _usersRepository.Get(userId, cancellationToken);
        if (user is null)
            throw new ArgumentException("User not found", nameof(userId));

        User? userCache = await _cacheProcessor.GetAsync<User>(CacheTypeEnums.Hybrid, "UserOnline" + userId.ToString(),
            cancellationToken);
        if (userCache is not null)
        {
            await _cacheProcessor.RemoveAsync(CacheTypeEnums.Hybrid, "UserOnline" + userId.ToString(),
                cancellationToken);
        }

        await _cacheProcessor.SetAsync<User>(CacheTypeEnums.Hybrid, "UserOffline" + userId.ToString(), user,
            TimeSpan.FromHours(24));
    }
}