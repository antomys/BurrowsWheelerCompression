using System;
using System.IO;

namespace BackCompression.Extensions
{
    public static class StatisticsExtension
    {
        public static string GetCompressionRate(string inputFile, string outputFile)
        {
            var inputFileSize = new FileInfo(inputFile).Length;
            var outputFileSize = new FileInfo(outputFile).Length;

            var difference = Math.Abs(inputFileSize - outputFileSize);
            var percentage = difference / (inputFileSize / 100.0);

            if (outputFileSize > inputFileSize)
                percentage *= -1;

            return $"[{DateTime.Now}]: Input file size: {inputFileSize};\n" +
                   $"Output file size: {outputFileSize};\n" +
                   $"Difference: {difference}\n" +
                   $"Compression rate: {percentage}";
        }
    }
}