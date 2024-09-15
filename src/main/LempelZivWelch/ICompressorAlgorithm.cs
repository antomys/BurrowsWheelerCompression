using System.Diagnostics.CodeAnalysis;

namespace LempelZivWelch;

[SuppressMessage("Design", "CA1021:Avoid out parameters", Justification = "Approved")]
public interface ICompressorAlgorithm
{
    bool Compress(string pInputFileName, string pOutputFileName, out string fileNamePath);

    bool Decompress(string pInputFileName, string pOutputFileName, out string fileNamePath);
}
