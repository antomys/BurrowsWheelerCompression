using System.Threading.Tasks;

namespace BackCompression.Services.Interfaces
{
    public interface IBwtService
    {
        Task<string> Transform(string fileName);
        Task<string> InverseTransform(string fileName);
    }
}