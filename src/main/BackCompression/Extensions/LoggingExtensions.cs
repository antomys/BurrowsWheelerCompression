using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace BackCompression.Extensions;

public static partial class LoggingExtensions
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "[{Date}] {Compression} Time: {ElapsedTime} ms")]
    public static partial void TimeElapsed(
        this ILogger logger,
        in DateTimeOffset date,
        string compression,
        in double elapsedTime);

    public static void CompressionRate(
        this ILogger logger,
        string inputFile,
        string outputFile)
    {
        var inputFileSize = new FileInfo(inputFile).Length;
        var outputFileSize = new FileInfo(outputFile).Length;

        var difference = Math.Abs(inputFileSize - outputFileSize);
        var percentage = difference / (inputFileSize / 100.0);

        if (outputFileSize > inputFileSize)
        {
            percentage *= -1;
        }

        logger.CompressionRate(DateTimeOffset.UtcNow, inputFileSize, outputFileSize, difference, percentage);
    }

    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = """
                  [{Date}]: Input file size: {InputSize};
                  Output file size: {OutputSize};
                  Difference: {Difference}
                  Compression rate: {Percentage}
                  """)]
    private static partial void CompressionRate(
        this ILogger logger,
        in DateTimeOffset date,
        in long inputSize,
        in long outputSize,
        in long difference,
        in double percentage);
}
