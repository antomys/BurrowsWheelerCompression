using System;
using System.IO;
using CompressionLibrary.Bwt;

namespace Bwt.Demo {
    internal static class Program 
    {
        public static void Main()
        {
            var filePath = Console.ReadLine() ?? throw new ArgumentNullException();
            var file = File.ReadAllBytes(filePath);
            var bytes = CompressionLibrary.Bwt.Bwt.Transform(file);
            using var fs = new FileStream("2.txt", FileMode.Create);
            using var writer = new BinaryWriter(fs);
            writer.Write(bytes);
        }
    }
}