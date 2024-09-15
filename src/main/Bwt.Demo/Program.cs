using System;
using System.IO;
using System.Threading.Tasks;

namespace Bwt.Demo;

internal static class Program
{
    public static async Task Main()
    {
        var filePath = Console.ReadLine() ?? throw new ArgumentNullException();
        var file = await File.ReadAllBytesAsync(filePath);
        var bytes = await BurrowsWheelerTransform.Bwt.Transform(file);
        await using var fs = new FileStream("2.bwt", FileMode.Create);
        await using var writer = new BinaryWriter(fs);
        writer.Write(bytes);
    }
}
