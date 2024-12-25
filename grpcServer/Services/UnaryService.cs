using Grpc.Core;

namespace grpcServer.Services;

public class UnaryService : Unary.UnaryBase
{
    private readonly ILogger<UnaryService> _logger;
    public UnaryService(ILogger<UnaryService> logger)
    {
        _logger = logger;
    }

    public override Task<UnaryReply> SendMessage(UnaryRequest request, ServerCallContext context)
    {
        return Task.FromResult(new UnaryReply
        {
            Message = request.Name + " " + request.Message
        });
    }
}
