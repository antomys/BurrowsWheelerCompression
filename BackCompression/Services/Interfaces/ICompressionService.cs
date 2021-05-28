using System.Threading.Tasks;

namespace BackCompression.Services.Interfaces
{
    public interface ICompressionService
    {
        string CompressLzw(string pInFile, string pOutFile);
        string DecompressLzw(string pInFile, string pOutFile);
        Task<string> CompressLzwBwt(string pInFile, string pOutFile);
        Task<string> LzwBwtCompress(string pInFile, string pOutFile);
        Task<string> LzwBwtDecompress(string pInFile, string pOutFile);
        Task<string> DecompressLzwBwt(string pInFile, string pOutFile);
        Task<string> CompressBwt(string pInFile, string pOutFile);
        Task<string> DecompressBwt(string pInFile, string pOutFile);
        string CompressHuffman(string pInFile, string pOutFile);
        string DecompressHuffman(string pInFile, string pOutFile);
    }
}