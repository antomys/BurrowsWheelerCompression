using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CompressionLibrary.Bwt;
using CompressionLibrary.Huffman;
using CompressionLibrary.Lzw;
using ICSharpCode.SharpZipLib.BZip2;

namespace Lzw.DemoWithBwt
{
    public class PbvCompressor
    {
        private readonly ICompressorAlgorithm _compressorAlgorithm;

        private ICompressorAlgorithm CompressorAlgorithm
        {
            // allows us to set the algorithm at runtime, also lets us set the algorithm dynamically if we create a factory class
            init => _compressorAlgorithm = value;
        }

        private PbvCompressor()
        {
            // setting this by default but we could create a factory class to let us set the algorithm based on arguments passed to the main method
            // IE. "Compress.exe zip -c blah.txt" would use the zip algorithm as opposed to the default one.
            CompressorAlgorithm = new PbvCompressorLzw();
        }
        
        private async Task Start(string argument, string pInFile, string pOutFile)
        {
            var newOutFile = FileNameSelector.GetFileName(pOutFile);

            if (newOutFile == null)
            {
                Console.WriteLine("Exiting...");
                Environment.Exit(1);
            }

            switch (argument)
            {
                case "-c":
                    _compressorAlgorithm.Compress(pInFile, pOutFile, out _);
                    GetCompressionRate(pInFile, pOutFile);
                    break;
                case "-lzbwc":
                {
                    var transformation = await Bwt.Transform(await File.ReadAllBytesAsync(pInFile));
                    var name = Guid.NewGuid() + ".tmp";
                    await using var fileStream = new FileStream(name, FileMode.Create);
                    await fileStream.WriteAsync(transformation);
                    await fileStream.DisposeAsync();
                    
                    _compressorAlgorithm.Compress(name, pOutFile, out _);
                    
                    GetCompressionRate(pInFile, pOutFile);
                    File.Delete(name);
                    break;
                }
                case "-bwtc":
                {
                    var bytes = await File.ReadAllBytesAsync(pInFile);
                    var transformation = await HuffmanBwt.Compress(bytes);
                    await using var fs = new FileStream(pOutFile, FileMode.Create);
                    await using var writer = new BinaryWriter(fs);
                    writer.Write(transformation);
                    await writer.DisposeAsync();
                    
                    GetCompressionRate(pInFile, pOutFile);
                    break;
                }
                case "-bwtd":
                {
                    var bytes = await File.ReadAllBytesAsync(pInFile);
                    var transformation = await HuffmanBwt.Decompress(bytes);
                    await using var fs = new FileStream(pOutFile, FileMode.Create);
                    await using var writer = new BinaryWriter(fs);
                    writer.Write(transformation);
                    await writer.DisposeAsync();
                    
                    GetCompressionRate(pInFile, pOutFile);
                    break;
                }
                case "-lzbwd":
                {
                    var name = Guid.NewGuid() + ".tmp";
                    _compressorAlgorithm.Decompress(pInFile, name, out _);
                    
                    var transformation = await Bwt.InverseTransform(await File.ReadAllBytesAsync(name));
                    
                    await using var fileStream = new FileStream(pOutFile, FileMode.Create);
                    await fileStream.WriteAsync(transformation);
                    await fileStream.DisposeAsync();

                    GetCompressionRate(pInFile, pOutFile);
                    File.Delete(name);
                    break;
                }
                case "-hc":
                {
                    MainAlgorithms.CompressFile(pInFile,pOutFile, out _);
                    GetCompressionRate(pInFile, pOutFile);
                    break;
                }
                case "-hd":
                {
                    MainAlgorithms.DecompressFile(pInFile,pOutFile, out _);
                    GetCompressionRate(pInFile, pOutFile);
                    break;
                }
                case "-d":
                    _compressorAlgorithm.Decompress(pInFile, pOutFile, out _);
                    GetCompressionRate(pInFile, pOutFile);
                    break;
                case "-bc":
                {
                    await using var fileToBeZippedAsStream = File.OpenRead(pInFile);
                    await using var zipTargetAsStream = File.Create(pOutFile);
                    try
                    {
                        BZip2.Compress(fileToBeZippedAsStream, zipTargetAsStream, true, 4096);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    GetCompressionRate(pInFile, pOutFile);
                    break;
                }
            }
        }

        private static void GetCompressionRate(string inputFile, string outputFile)
        {
            var inputFileSize = new FileInfo(inputFile).Length;
            var outputFileSize = new FileInfo(outputFile).Length; 
            var difference = Math.Abs(inputFileSize - outputFileSize);
            var percentage = difference / (inputFileSize / 100.0);
            
            if (outputFileSize > inputFileSize)
                percentage *= -1;

            Console.WriteLine( $"[{DateTime.Now}]: Input file size: {inputFileSize};\n" +
                               $"Output file size: {outputFileSize};\n" +
                               $"Difference: {difference}\n" +
                               $"Compression rate: {percentage}");
           
                    
        }

        private static async Task Main(string[] args)
        {
            var validCommands = new Regex("-[cdCDbwtcBWTCbwtdBWTDhcHChdHDbcBClzbwclzbwd]");

            if (args.Length != 3)
            {
                Console.WriteLine("Too many or too few arguments! Exiting.");
                return;
            }

            if (validCommands.IsMatch(args[0])) //if valid arguments given proceed
            {
                var watch = Stopwatch.StartNew();
                var pc = new PbvCompressor();
                await pc.Start(args[0].ToLower(), args[1], args[2]);
                watch.Stop();
                Console.WriteLine($"Time: {watch.ElapsedMilliseconds} ms");
            }
            else
                Console.WriteLine("Invalid argument command given. Exiting.");
        }
    }
}