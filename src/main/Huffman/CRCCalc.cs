namespace Huffman;

public sealed class CrcCalc
{
    private static readonly uint poly = 0x82608edb;
    private static readonly uint[] table = new uint[256];

    private uint crc;

    static CrcCalc()
    {
        for (uint i = 0; i < 256; i++)
        {
            var cs = i;
            for (uint j = 0; j < 8; j++)
            {
                cs = (cs & 1) > 0 ? (cs >> 1) ^ poly : cs >> 1;
            }

            table[i] = cs;
        }
    }

    public CrcCalc()
    {
        crc = 0xffffffff;
    }

    public uint GetCrc() { return crc; }

    public uint UpdateByte(byte b)
    {
        crc = table[(crc ^ b) & 0xff] ^ (crc >> 8);
        crc ^= 0xffffffff;
        return crc;
    }
}
