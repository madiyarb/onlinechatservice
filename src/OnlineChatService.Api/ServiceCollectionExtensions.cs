using Amazon.ElastiCache;
using Amazon.SQS;
using OnlineChatService.Api.Handlers;
using OnlineChatService.Api.Options;
using OnlineChatService.Api.SignalR;
using OnlineChatService.Application.OnlineChats.Interfaces;
using OnlineChatService.Domain.Chats.Events;
using OnlineChatService.Domain.Messages.Events;
using OnlineChatService.Domain.Users.Commands;

namespace OnlineChatService.Api;

public static class ServiceCollectionExtensions
{
    public static void AddApi(this IServiceCollection services, WebApplicationBuilder builder)
    {
        #region AWS

        builder.Services.AddAWSService<IAmazonSQS>();
        builder.Services.AddAWSService<IAmazonElastiCache>();
        AWSResources awsResources = new AWSResources();
        builder.Configuration.GetSection(AWSResources.SectionName).Bind(awsResources);
        builder.Services.AddAWSMessageBus(bus =>
        {
            bus.AddSNSPublisher<EndChatEvent>(awsResources.OrderTopicArn);
            bus.AddSNSPublisher<SendMessageEvent>(awsResources.OrderTopicArn);
            bus.AddSNSPublisher<DeleteMessageEvent>(awsResources.OrderTopicArn);
            bus.AddSNSPublisher<SetOfflineUserEvent>(awsResources.OrderTopicArn);
            bus.AddSNSPublisher<UpdateMessageEvent>(awsResources.OrderTopicArn);
            bus.AddSNSPublisher<MarkAsReadMessageEvent>(awsResources.OrderTopicArn);
            bus.AddSNSPublisher<SendEmailCommand>(awsResources.OrderTopicArn);

            bus.AddSQSPoller(awsResources.OrderQueueUrl);
            bus.AddMessageHandler<EndChatEventHandler, EndChatEvent>();
            bus.AddMessageHandler<SendMessageEventHandler, SendMessageEvent>();
            bus.AddMessageHandler<DeleteMessageEventHandler, DeleteMessageEvent>();
            bus.AddMessageHandler<SetOfflineUserEventHandler, SetOfflineUserEvent>();
            bus.AddMessageHandler<MarkAsReadMessageEventHandler, MarkAsReadMessageEvent>();
            bus.AddMessageHandler<SendEmailCommandHandler, SendEmailCommand>();

            bus.ConfigureBackoffPolicy(cfg => cfg.UseCappedExponentialBackoff());
        });

        #endregion

        #region SignalR

        builder.Services.AddScoped<ISignalRNotificationService, SignalRNotificationService>();

        builder.Services.AddSignalR(options =>
        {
            options.MaximumReceiveMessageSize = 9223372036854775807;
            options.KeepAliveInterval = TimeSpan.FromMilliseconds(100);
            options.EnableDetailedErrors = true;
        });

        #endregion
    }
}