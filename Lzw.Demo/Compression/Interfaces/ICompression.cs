using System.IO;

namespace Lzw.Demo.Compression.Interfaces
{
    public interface ICompression
    {
        byte[] Compress(byte[] input);
        byte[] Decompress(byte[] input);
        void Compress(Stream input, Stream output);
        void Decompress(Stream input, Stream output);
        void Compress(BinaryReader reader, BinaryWriter writer);
        void Decompress(BinaryReader reader, BinaryWriter writer);
    }
}