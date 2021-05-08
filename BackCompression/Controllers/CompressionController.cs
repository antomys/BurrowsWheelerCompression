using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using BackCompression.Extensions;
using BackCompression.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using static BackCompression.Extensions.FileExtension;

namespace BackCompression.Controllers
{
    [ApiController]
    [DisableRequestSizeLimit]
    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    [Route("[controller]/[action]")]
    public class CompressionController : ControllerBase
    {
        private readonly ILogger<CompressionController> _logger;
        private readonly ICompressionService _compressionService;
        private readonly IBwtService _bwtService;
        private readonly Stopwatch _stopwatch;

        public CompressionController(
            ILogger<CompressionController> logger, 
            ICompressionService compressionService, 
            IBwtService bwtService)
        {
            _logger = logger;
            _compressionService = compressionService;
            _bwtService = bwtService;
            _stopwatch = new Stopwatch();
        }

        [HttpPost]
        public async Task<IActionResult> BurrowsWheelerTransform(IFormFile formFile, bool transform = true)
        {
            _stopwatch.Restart();
            var inputFile = await WriteFile(formFile);

            var outputFile = transform
                ? await _bwtService.Transform(inputFile)
                : await _bwtService.InverseTransform(inputFile);
            
            _stopwatch.Stop();
            _logger.LogInformation($"[{DateTime.Now}] BWT Time: {_stopwatch.ElapsedMilliseconds / 1000.0} ms");
            _logger.LogInformation(StatisticsExtension.GetCompressionRate(inputFile,outputFile));
            
            return await GenerateDownloadLink(outputFile);
        }

        [HttpPost]
        public async Task<IActionResult> LzwVanilla(IFormFile formFile, bool compress = true)
        {
            var inputFile = await WriteFile(formFile);

            _stopwatch.Restart();
            var outputFile = compress ?
                _compressionService.CompressLzw(inputFile, Path.GetFileNameWithoutExtension(inputFile) + ".lzw") :
                _compressionService.DecompressLzw(inputFile, Path.GetFileNameWithoutExtension(inputFile));
            
            _stopwatch.Stop();
            _logger.LogInformation($"[{DateTime.Now}] LZW Vanilla Time: {_stopwatch.ElapsedMilliseconds / 1000.0} ms");
            _logger.LogInformation(StatisticsExtension.GetCompressionRate(inputFile,outputFile));

            return await GenerateDownloadLink(outputFile);
        }

        [HttpPost]
        public async Task<IActionResult> LzwBwt(IFormFile formFile, bool compress = true)
        {
            var inputFile = await WriteFile(formFile);

            _stopwatch.Restart();
            var outputFile = compress ?
                await _compressionService.CompressLzwBwt(inputFile, Path.GetFileNameWithoutExtension(inputFile) + ".lzwb") :
                await _compressionService.DecompressLzwBwt(inputFile, Path.GetFileNameWithoutExtension(inputFile));
            
            _stopwatch.Stop();
            _logger.LogInformation($"[{DateTime.Now}] Lzw+Bwt Time: {_stopwatch.ElapsedMilliseconds / 1000.0} ms");
            _logger.LogInformation(StatisticsExtension.GetCompressionRate(inputFile,outputFile));

            return await GenerateDownloadLink(outputFile);
        }
        [HttpPost]
        public async Task<IActionResult> BwCompression(IFormFile formFile, bool compress = true)
        {
            var inputFile = await WriteFile(formFile);

            _stopwatch.Restart();
            var outputFile = compress ?
                await _compressionService.CompressBwt(inputFile, Path.GetFileNameWithoutExtension(inputFile) + ".bwc") :
                await _compressionService.DecompressBwt(inputFile, Path.GetFileNameWithoutExtension(inputFile));
            
            _stopwatch.Stop();
            _logger.LogInformation($"[{DateTime.Now}] Lzw+Bwt Time: {_stopwatch.ElapsedMilliseconds / 1000.0} ms");
            _logger.LogInformation(StatisticsExtension.GetCompressionRate(inputFile,outputFile));

            return await GenerateDownloadLink(outputFile);
        }
        [HttpPost]
        public async Task<IActionResult> Huffman(IFormFile formFile, bool compress = true)
        {
            var inputFile = await WriteFile(formFile);

            _stopwatch.Restart();
            var outputFile = compress ?
                _compressionService.CompressHuffman(inputFile, Path.GetFileNameWithoutExtension(inputFile) + ".huff") :
                _compressionService.DecompressHuffman(inputFile, Path.GetFileNameWithoutExtension(inputFile));
            
            _stopwatch.Stop();
            _logger.LogInformation($"[{DateTime.Now}] Huffman Time: {_stopwatch.ElapsedMilliseconds / 1000.0} ms");
            _logger.LogInformation(StatisticsExtension.GetCompressionRate(inputFile,outputFile));

            return await GenerateDownloadLink(outputFile);
        }
        private async Task<FileContentResult> GenerateDownloadLink(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
    
            var bytes = await System.IO.File.ReadAllBytesAsync(fileName);
            return File(bytes, contentType, Path.GetFileName(fileName));
        }
    }
}