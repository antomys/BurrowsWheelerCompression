using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CompressionLibrary.Bwt;
using CompressionLibrary.Lzw;

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
                    _compressorAlgorithm.Compress(pInFile, pOutFile);
                    break;
                case "-bwtc":
                {
                    var transformation = Bwt.Transform(await File.ReadAllBytesAsync(pInFile));
                    var name = Guid.NewGuid() + ".tmp";
                    await using var fileStream = new FileStream(name, FileMode.Create);
                    await fileStream.WriteAsync(transformation);
                    await fileStream.DisposeAsync();
                    
                    _compressorAlgorithm.Compress(name, pOutFile);
                    File.Delete(name);
                    break;
                }
                case "-bwtd":
                {
                    var transformation = Bwt.InverseTransform(await File.ReadAllBytesAsync(pInFile));
                    var name = Guid.NewGuid() + ".tmp";
                    await using var fileStream = new FileStream(name, FileMode.Create);
                    await fileStream.WriteAsync(transformation);
                    await fileStream.DisposeAsync();
                   
                    _compressorAlgorithm.Decompress(name, pOutFile);
                    
                    File.Delete(name);
                    break;
                }
                case "-d":
                    _compressorAlgorithm.Decompress(pInFile, pOutFile);
                    break;
            }
        }

        private static async Task Main(string[] args)
        {
            var validCommands = new Regex("-[cdCDbwtcBWTCbwtdBWTD]");

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