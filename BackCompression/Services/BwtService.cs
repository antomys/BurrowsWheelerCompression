using System;
using System.IO;
using System.Threading.Tasks;
using BackCompression.Services.Interfaces;

namespace BackCompression.Services
{
    public class BwtService:IBwtService
    {
        public async Task<string> Transform(string fileName)
        {
            var bytes = CompressionLibrary.Bwt.Bwt.Transform(await File.ReadAllBytesAsync(fileName));

            var outputName = "BWT " + Guid.NewGuid() + ".bwt";
            await using var fileStream = new FileStream(outputName, FileMode.Create);
            await fileStream.WriteAsync(bytes);
            
            return Path.GetFullPath(outputName);
        }

        public async Task<string> InverseTransform(string fileName)
        {
            var bytes = CompressionLibrary.Bwt.Bwt.InverseTransform(await File.ReadAllBytesAsync(fileName));
           
            var outputName = "InvBWT " + Guid.NewGuid();
            await using var fileStream = new FileStream(outputName, FileMode.Create);
            await fileStream.WriteAsync(bytes);
            
            return Path.GetFullPath(outputName);
        }
    }
}