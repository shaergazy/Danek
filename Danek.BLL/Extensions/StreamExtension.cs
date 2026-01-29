using System.IO;
using System.Threading.Tasks;

namespace Danek.BLL.Extesions;

public static class StreamExtension
{

    public static async Task<Stream> Clone(this Stream source)
    {
        var pos = source.Position;
        source.Seek(0, SeekOrigin.Begin);
        var destination = new MemoryStream();
        await source.CopyToAsync(destination);
        destination.Seek(0, SeekOrigin.Begin);
        source.Position = pos;

        return destination;
    }

    public static string ReadStreamInChunks(this Stream stream)
    {
        const int readChunkBufferLength = 4096;

        stream.Seek(0, SeekOrigin.Begin);

        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream);

        var readChunk = new char[readChunkBufferLength];
        int readChunkLength;

        do
        {
            readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
            textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0);
        stream.Seek(0, SeekOrigin.Begin);

        return textWriter.ToString();
    }

    public static byte[] ToBytes(this Stream stream)
    {
        var ms = new MemoryStream();
        stream.CopyTo(ms);

        return ms.ToArray();
    }
}
