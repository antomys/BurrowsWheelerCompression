using System;
using System.IO;
using System.Threading.Tasks;
using CompressionLibrary.Bwt;
using CompressionLibrary.Lzw;
using Lzw.DemoWithBwt;

namespace BackCompression.Services
{
    public class CompressionService: ICompressionService
    {
        private readonly ICompressorAlgorithm _compressorAlgorithm;

        public CompressionService()
        {
            _compressorAlgorithm = new PbvCompressorLzw();
        }

        public string CompressLzw(string pInFile, string pOutFile)
        {
            _compressorAlgorithm.Compress(pInFile, pOutFile, out var fileNamePath);
            return fileNamePath;
        }

        public string DecompressLzw(string pInFile, string pOutFile)
        {
            _compressorAlgorithm.Decompress(pInFile, pOutFile, out var fileNamePath);
            return fileNamePath;
        }

        public async Task<string> CompressLzwBwt(string pInFile, string pOutFile)
        {
            var transformation = Bwt.Transform(await File.ReadAllBytesAsync(pInFile));
            var name = Guid.NewGuid() + ".tmp";
            await using var fileStream = new FileStream(name, FileMode.Create);
            await fileStream.WriteAsync(transformation);
            await fileStream.DisposeAsync();
                    
            _compressorAlgorithm.Compress(name, pOutFile, out var fullPath);
            File.Delete(name);

            return fullPath;
        }

        public async Task<string> DecompressLzwBwt(string pInFile, string pOutFile)
        {
            var name = Guid.NewGuid() + ".tmp";
            await Task.FromResult(_compressorAlgorithm.Decompress(pInFile, name, out var fullPath));
            var transformation = Bwt.InverseTransform(await File.ReadAllBytesAsync(fullPath));
            await using var fileStream = new FileStream(pOutFile, FileMode.Create);
            await fileStream.WriteAsync(transformation);
            await fileStream.DisposeAsync();
                    
           
            File.Delete(name);

            return Path.GetFullPath(pOutFile);
        }

        public void CompressHuffman(string pInFile, string pOutFile)
        {
            throw new System.NotImplementedException();
        }

        public void DecompressHuffman(string pInFile, string pOutFile)
        {
            throw new System.NotImplementedException();
        }
    }
}