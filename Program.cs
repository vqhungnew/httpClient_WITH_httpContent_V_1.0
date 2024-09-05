using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyHttpContentExample
{
    public class MyContent : HttpContent
    {
        private readonly string _data;

        public MyContent(string data)
        {
            _data = data;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
            => stream.WriteAsync(Encoding.UTF8.GetBytes(_data)).AsTask();

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
            => stream.WriteAsync(Encoding.UTF8.GetBytes(_data), cancellationToken).AsTask();

        protected override void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
            => stream.Write(Encoding.UTF8.GetBytes(_data));

        protected override Task<Stream> CreateContentReadStreamAsync()
            => Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes(_data)));

        protected override Task<Stream> CreateContentReadStreamAsync(CancellationToken cancellationToken)
            => Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes(_data))).WaitAsync(cancellationToken);

        protected override Stream CreateContentReadStream(CancellationToken cancellationToken)
            => new MemoryStream(Encoding.UTF8.GetBytes(_data));
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var data = "Hello, World!";
            var myContent = new MyContent(data);

            using (var client = new HttpClient())
            {
                // Example of sending a POST request with custom content
                var response = await client.PostAsync("https://httpbin.org/post", myContent);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Response Status Code: " + response.StatusCode);
                Console.WriteLine("Response Body: " + responseBody);
            }
        }
    }
}
