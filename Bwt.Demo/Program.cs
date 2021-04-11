using System.IO;
using System.Reflection.Metadata;

namespace Bwt.Demo {
    internal static class Program 
    {
        public static void Main(string[] args)
        {
            var file = File.ReadAllBytes(@"C:\Users\Volokhovych\Documents\RiderProj\DiplomaWork\Lzw.DemoWithBwt\bin\Debug\net5.0\1.txt");
            var bytes = Lzw.DemoWithBwt.Bwt.Bwt.Transform(file);
            using var fs = new FileStream("2.bwt", FileMode.Create);
            using var writer = new BinaryWriter(fs);
            writer.Write(bytes);
        }
    }
}