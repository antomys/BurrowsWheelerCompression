namespace CompressionLibrary.Lzw
{ 
    public interface ICompressorAlgorithm
    {
        bool Compress(string pInputFileName, string pOutputFileName, out string fileNamePath);
        bool Decompress(string pInputFileName, string pOutputFileName, out string fileNamePath);
    }
}
