using Grpc.Core;
using System.Threading;

namespace grpcServer.Services;

public class CBiDirectionalStreamingService : BiDirectionalStreaming.BiDirectionalStreamingBase
{
    private readonly ILogger<CBiDirectionalStreamingService> _logger;
    public CBiDirectionalStreamingService(ILogger<CBiDirectionalStreamingService> logger)
    {
        _logger = logger;
    }

    public override async Task SendMessage(IAsyncStreamReader<BiDirectionalStreamingRequest> requestStream, IServerStreamWriter<BiDirectionalStreamingReply> responseStream, ServerCallContext context)
    {
        var task = Task.Run(async () =>
        {

            while (await requestStream.MoveNext(context.CancellationToken))
            {
                System.Console.WriteLine(requestStream.Current.Message);
            }
        });

        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(1000);

            await responseStream.WriteAsync(new()
            {
                Message = "Server tarafindan mesaj " + i
            });
        }

        await task;
    }
}
