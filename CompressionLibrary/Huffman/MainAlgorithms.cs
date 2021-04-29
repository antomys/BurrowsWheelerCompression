using System;

namespace CompressionLibrary.Huffman
{
    public static class MainAlgorithms
    {
        public static void CompressFile (string filename, string fileOutName, out string filePath)
        {
            try {
                Compressor.HuffConsole(filename, fileOutName, out var file);
                filePath = file;
            }
            catch (Exception e) 
            {
                filePath = null;
                PrintHelper.Err("Unable to compress the file due to the error: " + e.Message);
            }
        }
        public static void DecompressFile (string filename, string fileOutName, out string filePath) {
            try {
                Compressor.UnHuffConsole(filename, fileOutName, out var file);
                filePath = file;
            }
            catch (Exception e) 
            {
                filePath = null;
                PrintHelper.Err("Unable to decompress the file due to the error: " + e.Message);
            }
        }
    }
}