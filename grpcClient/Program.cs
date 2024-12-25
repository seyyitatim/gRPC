// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using grpcServer;

var channel = GrpcChannel.ForAddress("https://localhost:7232");


// Unary
//var messageClient = new Message.MessageClient(channel);
//var messageResponse = await messageClient.SendMessageAsync(new()
//{
//    Name = "Seyyit",
//    Message = "bu unary test mesajidir"
//});

//System.Console.WriteLine(messageResponse.Message);


// Server Streaming
//var serverStreamingClient = new ServerStreaming.ServerStreamingClient(channel);
//var response = serverStreamingClient.SendMessage(new()
//{
//    Message = "bu server streaming test mesajidir"
//});

//CancellationTokenSource cancellationToken = new CancellationTokenSource();

//while (await response.ResponseStream.MoveNext(cancellationToken.Token))
//{
//    System.Console.WriteLine(response.ResponseStream.Current.Message);
//}


// Client Streaming
//var clientStreamingClient = new ClientStreaming.ClientStreamingClient(channel);

//var request = clientStreamingClient.SendMessage();

//for (int i = 0; i < 10; i++)
//{
//    await Task.Delay(200);

//    await request.RequestStream.WriteAsync(new()
//    {
//        Message = "Message " + i
//    });

//}

//await request.RequestStream.CompleteAsync();
//System.Console.WriteLine("Islem tamam");


// Bi Directional Streaming
var biDirectionalStreamingClient = new BiDirectionalStreaming.BiDirectionalStreamingClient(channel);
var request = biDirectionalStreamingClient.SendMessage();

var task = Task.Run(async () =>
{
    for (int i = 0; i < 10; i++)
    {
        await Task.Delay(100);
        request.RequestStream.WriteAsync(new()
        {
            Message = "client tarafindan mesaj " + i
        });

    }
});

CancellationTokenSource cancellationToken = new CancellationTokenSource();

while (await request.ResponseStream.MoveNext(cancellationToken.Token))
{
    System.Console.WriteLine(request.ResponseStream.Current.Message);
}

await task;
await request.RequestStream.CompleteAsync();