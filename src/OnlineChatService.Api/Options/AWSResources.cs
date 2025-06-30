namespace OnlineChatService.Api.Options;

public class AWSResources
{
    public const string SectionName  = nameof(AWSResources);
    public string OrderQueueUrl { get; set; }
    public string OrderTopicArn { get; set; }
}