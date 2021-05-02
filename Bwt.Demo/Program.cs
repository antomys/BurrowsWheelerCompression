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
            var bytes = CompressionLibrary.Bwt.Bwt.InverseTransform(file);
            using var fs = new FileStream("2.pdf", FileMode.Create);
            using var writer = new BinaryWriter(fs);
            writer.Write(bytes);
        }
    }
}