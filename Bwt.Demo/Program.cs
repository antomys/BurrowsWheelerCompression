using System;
using System.IO;

namespace Bwt.Demo {
    internal static class Program 
    {
        public static void Main()
        {
            var filePath = Console.ReadLine() ?? throw new ArgumentNullException();
            var file = File.ReadAllBytes(filePath);
            var bytes = Bwt.Bwt.InverseTransform(file);
            using var fs = new FileStream("2.txt", FileMode.Create);
            using var writer = new BinaryWriter(fs);
            writer.Write(bytes);
        }
    }
}