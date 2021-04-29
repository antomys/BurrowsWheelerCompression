using System;

namespace CompressionLibrary.Huffman
{
    public static class MainAlgorithms
    {
        public static void CompressFile (string filename, string fileOutName) {
            try {
                Compressor.HuffConsole (filename, fileOutName);
            }
            catch (Exception e) {
                PrintHelper.Err("Unable to compress the file due to the error: " + e.Message);
            }
        }
        public static void DecompressFile (string filename, string fileOutName) {
            try {
                Compressor.UnHuffConsole (filename, fileOutName);
            }
            catch (Exception e) {
                PrintHelper.Err("Unable to decompress the file due to the error: " + e.Message);
            }
        }
    }
}