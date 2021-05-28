using System;
using System.IO;
using System.Threading.Tasks;
using BackCompression.Services.Interfaces;
using CompressionLibrary.Bwt;
using CompressionLibrary.Huffman;
using CompressionLibrary.Lzw;
using CompressionLibrary.Mtf;
using Lzw.DemoWithBwt;
using Microsoft.Extensions.Logging;

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

        public async Task<string> LzwBwtCompress(string pInFile, string pOutFile)
        {
            var bytes = await File.ReadAllBytesAsync(pInFile);
            var bw = await Bwt.Transform(bytes);
            var mtf = await Task.FromResult(MoveToFrontCoding.Encode(bw));

            await using (var fs = new FileStream(pInFile, FileMode.Create))
            {
                await using (var writer = new BinaryWriter(fs))
                {
                    writer.Write(mtf);
                }
            }

            await Task.FromResult(_compressorAlgorithm
                .Compress(pInFile, pOutFile, out var fileNamePath));
            return fileNamePath;
        }
        
        public async Task<string> LzwBwtDecompress(string pInFile, string pOutFile)
        {
            
            await Task.FromResult(_compressorAlgorithm
                .Decompress(pInFile, pOutFile, out var fileNamePath));
           
            var bytes = await File.ReadAllBytesAsync(fileNamePath);
            var mtf = await Task.FromResult(MoveToFrontCoding.Decode(bytes));
            var bw = await Bwt.InverseTransform(mtf);

            await using var fs = new FileStream(fileNamePath, FileMode.Create);
            await using var writer = new BinaryWriter(fs);
            writer.Write(bw);

            return fileNamePath;
        }

        public string DecompressLzw(string pInFile, string pOutFile)
        {
            _compressorAlgorithm.Decompress(pInFile, pOutFile, out var fileNamePath);
            return fileNamePath;
        }

        public async Task<string> CompressBwt(string pInFile, string pOutFile)
        {
            var bytes = await File.ReadAllBytesAsync(pInFile);
            var transformation = await HuffmanBwt.Compress(bytes);
            await using var fs = new FileStream(pOutFile, FileMode.Create);
            await using var writer = new BinaryWriter(fs);
            writer.Write(transformation);
           
            return Path.GetFullPath(pOutFile);
        }
        public async Task<string> DecompressBwt(string pInFile, string pOutFile)
        {
            var bytes = await File.ReadAllBytesAsync(pInFile);
            var transformation = await HuffmanBwt.Decompress(bytes);
            await using var fs = new FileStream(pOutFile, FileMode.Create);
            await using var writer = new BinaryWriter(fs);
            writer.Write(transformation);

            return Path.GetFullPath(pOutFile);
        }

        public async Task<string> CompressLzwBwt(string pInFile, string pOutFile)
        {
            var transformation = await Bwt.Transform(await File.ReadAllBytesAsync(pInFile));
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
            var transformation = await Bwt.InverseTransform(await File.ReadAllBytesAsync(fullPath));
            await using var fileStream = new FileStream(pOutFile, FileMode.Create);
            await fileStream.WriteAsync(transformation);
            await fileStream.DisposeAsync();
            
            File.Delete(name);
            return Path.GetFullPath(pOutFile);
        }

        public string CompressHuffman(string pInFile, string pOutFile)
        {
            MainAlgorithms.CompressFile(pInFile,pOutFile, out var fullPath);
            return fullPath;
        }

        public string DecompressHuffman(string pInFile, string pOutFile)
        {
            MainAlgorithms.DecompressFile(pInFile,pOutFile, out var fullPath);
            return fullPath;
        }
        
        
    }
}