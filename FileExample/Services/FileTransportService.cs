using FileExample;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace FileExample.Services
{
    public class FileTransportService : FileTransport.FileTransportBase
    {
        public readonly IWebHostEnvironment webHostEnvironment;
        public FileTransportService(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public override async Task FileDownload(FileInfo request, IServerStreamWriter<BytesContent> responseStream, ServerCallContext context)
        {
            string path = Path.Combine(webHostEnvironment.WebRootPath, "files");

            using FileStream fileStream = new FileStream($"{path}/{request.FileName}{request.FileExtension}", FileMode.Open, FileAccess.Read);

            byte[] buffer = new byte[2048];

            BytesContent content = new BytesContent()
            {
                FileSize = fileStream.Length,
                Info = new FileInfo() { FileName = Path.GetFileNameWithoutExtension(fileStream.Name), FileExtension = Path.GetExtension(fileStream.Name) },
                ReadedByte = 0
            };

            while ((content.ReadedByte = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                content.Buffer = ByteString.CopyFrom(buffer);
                await responseStream.WriteAsync(content);
            }

            fileStream.Close();
        }

        public override async Task<Empty> FileUpload(IAsyncStreamReader<BytesContent> requestStream, ServerCallContext context)
        {
            string path = Path.Combine(webHostEnvironment.WebRootPath, "files");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            FileStream fileStream = null;

            try
            {
                int count = 0;
                decimal chunkSize = 0;

                while (await requestStream.MoveNext())
                {
                    if (count++ == 0)
                    {
                        fileStream = new FileStream($"{path}/{requestStream.Current.Info.FileName}{requestStream.Current.Info.FileExtension}", FileMode.CreateNew);
                        fileStream.SetLength(requestStream.Current.FileSize);
                    }

                    var buffer = requestStream.Current.Buffer.ToByteArray();
                    await fileStream.WriteAsync(buffer, 0, buffer.Length);

                    System.Console.WriteLine($"{Math.Round(((chunkSize += requestStream.Current.ReadedByte) * 100) / requestStream.Current.FileSize)}%");
                }
            }
            catch (Exception)
            {
                System.Console.WriteLine("hata alindi dostum");
            }

            await fileStream.DisposeAsync();
            fileStream.Close();
            return new Empty();
        }
    }
}
