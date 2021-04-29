using System.Threading.Tasks;

namespace BackCompression.Services
{
    public interface ICompressionService
    {
        string CompressLzw(string pInFile, string pOutFile);
        string DecompressLzw(string pInFile, string pOutFile);
        Task<string> CompressLzwBwt(string pInFile, string pOutFile);
        Task<string> DecompressLzwBwt(string pInFile, string pOutFile);
        string CompressHuffman(string pInFile, string pOutFile);
        string DecompressHuffman(string pInFile, string pOutFile);
    }
}