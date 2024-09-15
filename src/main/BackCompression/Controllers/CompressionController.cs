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

namespace BackCompression.Controllers;

[ApiController]
[DisableRequestSizeLimit]
[RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
[Route("[controller]/[action]")]
public sealed class CompressionController(
    ILogger<CompressionController> logger,
    ICompressionService compressionService,
    IBwtService bwtService)
    : ControllerBase
{
    private static readonly Stopwatch _stopwatch = new();

    [HttpPost]
    public async Task<IActionResult> BurrowsWheelerTransform(IFormFile formFile, bool transform = true)
    {
        _stopwatch.Restart();
        var inputFile = await WriteFile(formFile);

        var outputFile = transform
            ? await bwtService.Transform(inputFile)
            : await bwtService.InverseTransform(inputFile);

        _stopwatch.Stop();

        logger.TimeElapsed(DateTime.Now, "BWT", _stopwatch.ElapsedMilliseconds / 1000.0);
        logger.CompressionRate(inputFile, outputFile);

        return await GenerateDownloadLink(outputFile);
    }

    [HttpPost]
    public async Task<IActionResult> LzwVanilla(IFormFile formFile, bool compress = true)
    {
        var inputFile = await WriteFile(formFile);

        _stopwatch.Restart();
        var outputFile = compress
            ? compressionService.CompressLzw(inputFile, Path.GetFileNameWithoutExtension(inputFile) + ".lzw")
            : compressionService.DecompressLzw(inputFile, Path.GetFileNameWithoutExtension(inputFile));

        _stopwatch.Stop();

        logger.TimeElapsed(DateTime.Now, "Default LZW", _stopwatch.ElapsedMilliseconds / 1000.0);
        logger.CompressionRate(inputFile, outputFile);

        return await GenerateDownloadLink(outputFile);
    }

    [HttpPost]
    public async Task<IActionResult> LzwBwt(IFormFile formFile, bool compress = true)
    {
        var inputFile = await WriteFile(formFile);

        _stopwatch.Restart();
        var outputFile = compress
            ? await compressionService.CompressLzwBwt(inputFile, Path.GetFileNameWithoutExtension(inputFile) + ".lzwb")
            : await compressionService.DecompressLzwBwt(inputFile, Path.GetFileNameWithoutExtension(inputFile));

        _stopwatch.Stop();

        logger.TimeElapsed(DateTime.Now, "LZW + BWT", _stopwatch.ElapsedMilliseconds / 1000.0);
        logger.CompressionRate(inputFile, outputFile);

        return await GenerateDownloadLink(outputFile);
    }

    [HttpPost]
    public async Task<IActionResult> BwtCompression(IFormFile formFile, bool compress = true)
    {
        var inputFile = await WriteFile(formFile);

        _stopwatch.Restart();
        var outputFile = compress
            ? await compressionService.CompressBwt(inputFile, Path.GetFileNameWithoutExtension(inputFile) + ".bwc")
            : await compressionService.DecompressBwt(inputFile, Path.GetFileNameWithoutExtension(inputFile));

        _stopwatch.Stop();

        logger.TimeElapsed(DateTime.Now, "BWT", _stopwatch.ElapsedMilliseconds / 1000.0);
        logger.CompressionRate(inputFile, outputFile);

        return await GenerateDownloadLink(outputFile);
    }

    [HttpPost]
    public async Task<IActionResult> Huffman(IFormFile formFile, bool compress = true)
    {
        var inputFile = await WriteFile(formFile);

        _stopwatch.Restart();
        var outputFile = compress
            ? compressionService.CompressHuffman(inputFile, Path.GetFileNameWithoutExtension(inputFile) + ".huff")
            : compressionService.DecompressHuffman(inputFile, Path.GetFileNameWithoutExtension(inputFile));

        _stopwatch.Stop();

        logger.TimeElapsed(DateTime.Now, "Huffman", _stopwatch.ElapsedMilliseconds / 1000.0);
        logger.CompressionRate(inputFile, outputFile);

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
