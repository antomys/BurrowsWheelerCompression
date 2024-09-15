namespace Huffman;

public sealed class BitIo
{
    private byte bc;
    private int bi;

    private byte buffer, bits;
    private readonly bool ownStream;
    private readonly bool IsOut;
    private bool open;
    private readonly Stream stream;

    private BitIo()
    {
    }

    public BitIo(Stream stream, bool isOut)
    {
        this.stream = stream;
        open = true;
        IsOut = isOut;
        if (!isOut)
        {
            bi = stream.ReadByte();
        }
    }

    public BitIo(string fileName, bool isOut)
    {
        ownStream = true;
        open = true;
        if (isOut)
        {
            stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        }
        else
        {
            stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }
    }

    public bool CanRead => open && !IsOut;

    public bool CanWrite => open && IsOut;

    public void Close()
    {
        open = false;
        if (IsOut)
        {
            BitFlush();
        }

        if (ownStream)
        {
            stream.Close();
        }
    }

    public void WriteBit(int bit)
    {
        if (!open)
        {
            throw new InvalidOperationException("Cannot write to the disposing stream");
        }

        if (!IsOut)
        {
            throw new NotSupportedException("Cannot write to the read-only bit stream");
        }

        if (bits == 8)
        {
            bits = 0;
            stream.WriteByte(buffer);
            buffer = 0;
        }

        bits++;
        buffer <<= 1;
        if (bit > 0)
        {
            buffer |= 0x1;
        }
    }

    private void BitFlush()
    {
        buffer <<= 8 - bits;
        stream.WriteByte(buffer);
    }

    public int ReadBit()
    {
        if (!open)
        {
            throw new InvalidOperationException("Cannot read from the disposing stream");
        }

        if (IsOut)
        {
            throw new NotSupportedException("Cannot read from a write-only bit stream");
        }

        bc++;
        var ret = (bi & 0x80) > 0 ? 1 : 0;
        bi <<= 1;
        if (bc == 8)
        {
            bc = 0;
            bi = stream.ReadByte();
            if (bi == -1)
            {
                return 2;
            }

            bi &= 0xff;
        }

        return ret;
    }
}
