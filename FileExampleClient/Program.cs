using FileExample;
using Google.Protobuf;
using Grpc.Net.Client;

await Download();

static async Task Upload()
{
    var channel = GrpcChannel.ForAddress("https://localhost:7171");
    var client = new FileTransport.FileTransportClient(channel);

    string file = @"C:\Users\SeyyitAtim\Desktop\Projects\gRPC\gRPCExample\test.mp4";

    using FileStream fileStream = new FileStream(file, FileMode.Open);

    var content = new BytesContent
    {
        FileSize = fileStream.Length,
        ReadedByte = 0,
        Info = new FileExample.FileInfo()
        {
            FileName = Path.GetFileNameWithoutExtension(fileStream.Name),
            FileExtension = Path.GetExtension(fileStream.Name)
        }
    };

    var upload = client.FileUpload();

    byte[] buffer = new byte[2048];

    while ((content.ReadedByte = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
    {
        content.Buffer = ByteString.CopyFrom(buffer);
        await upload.RequestStream.WriteAsync(content);
    }

    await upload.RequestStream.CompleteAsync();
    fileStream.Close();
}

static async Task Download()
{
    var channel = GrpcChannel.ForAddress("https://localhost:7171");
    var client = new FileTransport.FileTransportClient(channel);

    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DownloadFiles");

    if (!Directory.Exists(path))
    {
        Directory.CreateDirectory(path);
    }

    var fileInfo = new FileExample.FileInfo()
    {
        FileName = "test",
        FileExtension = ".mp4"
    };

    FileStream fileStream = null;

    var request = client.FileDownload(fileInfo);

    CancellationTokenSource cancellationToken = new CancellationTokenSource();

    int count = 0;
    decimal chunkSize = 0;
    while (await request.ResponseStream.MoveNext(cancellationToken.Token))
    {
        if (count++ == 0)
        {
            fileStream = new FileStream($"{path}\\{request.ResponseStream.Current.Info.FileName}{request.ResponseStream.Current.Info.FileExtension}", FileMode.CreateNew);
            fileStream.SetLength(request.ResponseStream.Current.FileSize);

            fileStream.SetLength(request.ResponseStream.Current.FileSize);
        }

        var buffer = request.ResponseStream.Current.Buffer.ToByteArray();
        await fileStream.WriteAsync(buffer, 0, request.ResponseStream.Current.ReadedByte);


        System.Console.WriteLine($"{Math.Round(((chunkSize += request.ResponseStream.Current.ReadedByte) * 100) / request.ResponseStream.Current.FileSize)}%");
    }

    System.Console.WriteLine("yuklendi");
    await fileStream.DisposeAsync();
    fileStream.Close();
}
