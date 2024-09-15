using System.Threading.Tasks;

namespace BackCompression.Services.Interfaces;

public interface IBwtService
{
    ValueTask<string> Transform(string fileName);

    ValueTask<string> InverseTransform(string fileName);
}
