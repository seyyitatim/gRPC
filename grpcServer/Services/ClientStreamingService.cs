using Grpc.Core;
using System.Threading;

namespace grpcServer.Services;

public class ClientStreamingService : ClientStreaming.ClientStreamingBase
{
    private readonly ILogger<ClientStreamingService> _logger;
    public ClientStreamingService(ILogger<ClientStreamingService> logger)
    {
        _logger = logger;
    }

    public override async Task<ClientStreamingReply> SendMessage(IAsyncStreamReader<ClientStreamingRequest> requestStream, ServerCallContext context)
    {
        while (await requestStream.MoveNext(context.CancellationToken))
        {
            System.Console.WriteLine(requestStream.Current.Message);
        }

        return new()
        {
            Message = "Veri alinmistir."
        };
    }
}
