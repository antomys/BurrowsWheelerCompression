using System;
using System.IO;
using System.Threading.Tasks;
using BackCompression.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using static BackCompression.WriteFileExtension;

namespace BackCompression.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CompressionController : ControllerBase
    {
        private readonly ILogger<CompressionController> _logger;
        private readonly ICompressionService _compressionService;

        public CompressionController(ILogger<CompressionController> logger, ICompressionService compressionService)
        {
            _logger = logger;
            _compressionService = compressionService;
        }

        [HttpPost]
        public async Task<IActionResult> BurrowsWheelerTransform(IFormFile formFile)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> LzwVanilla(IFormFile formFile)
        {
            var inputFile = await WriteFile(formFile);

            var outputFile =
                _compressionService.CompressLzw(inputFile, Path.GetFileNameWithoutExtension(inputFile) + ".lzw");
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(outputFile, out var contentType))
            {
                contentType = "application/octet-stream";
            }
    
            var bytes = await System.IO.File.ReadAllBytesAsync(outputFile);
            return File(bytes, contentType, Path.GetFileName(outputFile));
        }
        
        [HttpPost]
        public IActionResult LzwBwt(IFormFile formFile)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        public IActionResult Huffman(IFormFile formFile)
        {
            throw new NotImplementedException();
        }
    }
}