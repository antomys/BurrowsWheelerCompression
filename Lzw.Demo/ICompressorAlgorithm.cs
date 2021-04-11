namespace Lzw.Demo
{ 
    public interface ICompressorAlgorithm
    {
        bool Compress(string pIntputFileName, string pOutputFileName);
        bool Decompress(string pIntputFileName, string pOutputFileName);
    }
}
