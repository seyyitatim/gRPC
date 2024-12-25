using Grpc.Core;

namespace grpcServer.Services;

public class ServerStreamingService : ServerStreaming.ServerStreamingBase
{
    private readonly ILogger<ServerStreamingService> _logger;
    public ServerStreamingService(ILogger<ServerStreamingService> logger)
    {
        _logger = logger;
    }

    public override async Task SendMessage(ServerStreamingRequest request, IServerStreamWriter<ServerStreamingReply> responseStream, ServerCallContext context)
    {
        System.Console.WriteLine(request.Message);

        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(1000);

            await responseStream.WriteAsync(new()
            {
                Message = "Merhaba " + i
            });

            System.Console.WriteLine("Mesaj gonderildi");
        }
    }
}
