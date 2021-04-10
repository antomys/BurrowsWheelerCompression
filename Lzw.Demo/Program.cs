using System;
using System.IO;
using Lzw.Demo.Compression;

namespace Lzw.Demo
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var compressor = new LzwCompression();

            using var fileStream = new FileStream("1.txt", FileMode.Open, FileAccess.Read);
            using var binaryReader = new BinaryReader(fileStream);
            using var createFile = new FileStream("1.lzw", FileMode.CreateNew);
            using var binaryWriter = new BinaryWriter(createFile);

            //todo: use threads or tasks
            compressor.Compress(binaryReader, binaryWriter);
            //compressor.Decompress(binaryReader, binaryWriter);

        }
    }
}