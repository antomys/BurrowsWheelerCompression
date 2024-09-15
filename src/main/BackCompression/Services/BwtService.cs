using System;
using System.IO;
using System.Threading.Tasks;
using BackCompression.Services.Interfaces;
using BurrowsWheelerTransform;

namespace BackCompression.Services;

public sealed class BwtService : IBwtService
{
    public async ValueTask<string> Transform(string fileName)
    {
        var bytes = await Bwt.Transform(await File.ReadAllBytesAsync(fileName));

        var outputName = "BWT " + Guid.NewGuid() + ".bwt";
        await using var fileStream = new FileStream(outputName, FileMode.Create);
        await fileStream.WriteAsync(bytes);

        return Path.GetFullPath(outputName);
    }

    public async ValueTask<string> InverseTransform(string fileName)
    {
        var bytes = await Bwt.InverseTransform(await File.ReadAllBytesAsync(fileName));

        var outputName = "InvBWT " + Guid.NewGuid();
        await using var fileStream = new FileStream(outputName, FileMode.Create);
        await fileStream.WriteAsync(bytes);

        return Path.GetFullPath(outputName);
    }
}
