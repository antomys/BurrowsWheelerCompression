namespace BurrowsWheelerTransform;

public sealed class BwCompression
{
    public static async Task<byte[]> Compress(byte[] data)
    {
        var bw = await Bwt.Transform(data);
        var mtf = MoveToFrontCoding.Encode(bw);
        var hf = HuffmanCoding.Encode(mtf);
        return hf;
    }

    public static async Task<byte[]> Decompress(byte[] data)
    {
        var dhf = HuffmanCoding.Decode(data);
        var imtf = MoveToFrontCoding.Decode(dhf);
        var ibw = await Bwt.InverseTransform(imtf);
        return ibw;
    }
}
