using System.Threading.Tasks;
using CompressionLibrary.Bwt;
using CompressionLibrary.Lzw;
using CompressionLibrary.Mtf;

namespace CompressionLibrary.Huffman
{
    public class HuffmanBwt
    {
        public static async Task<byte[]> Compress(byte[] data)
        {
            var bw = await Bwt.Bwt.Transform(data);
            var mtf = MoveToFrontCoding.Encode(bw);
            var hf = HuffmanCoding.Encode(mtf);
            return hf;
        }

        public static async Task<byte[]> Decompress(byte[] data)
        {
            var dhf = HuffmanCoding.Decode(data);
            var imtf = MoveToFrontCoding.Decode(dhf);
            var ibw = await Bwt.Bwt.InverseTransform(imtf);
            return ibw;
        }
    }
}
