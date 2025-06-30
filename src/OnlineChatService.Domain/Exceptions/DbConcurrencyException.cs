using System.Runtime.CompilerServices;

namespace OnlineChatService.Domain.Exceptions;

public class DbConcurrencyException : Exception
{
    public DbConcurrencyException(Exception innerException)
    {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(54, 1);
        interpolatedStringHandler.AppendLiteral("Some concurrent request had time to update the state: ");
        interpolatedStringHandler.AppendFormatted<Exception>(innerException);
    }
}