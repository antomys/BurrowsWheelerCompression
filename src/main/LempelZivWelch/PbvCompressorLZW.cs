namespace LempelZivWelch;

// LZW Based Decompressor - basic algorithm used as described on Mark Nelson's website  http://marknelson.us
public sealed class PbvCompressorLzw : ICompressorAlgorithm
{
    private const int MaxBits = 14; //maximum bits allowed to read
    private const int HashBit = MaxBits - 8; //hash bit to use with the hashing algorithm to find correct index
    private const int MaxValue = (1 << MaxBits) - 1; //max value allowed based on max bits
    private const int MaxCode = MaxValue - 1; //max code possible
    private const int TableSize = 18041; //must be bigger than the maximum allowed by max bits and prime
    private readonly int[] _iaCharTable = new int[TableSize]; //character table

    private readonly int[] _iaCodeTable = new int[TableSize]; //code table
    private readonly int[] _iaPrefixTable = new int[TableSize]; //prefix table

    private ulong _iBitBuffer; //bit buffer to temporarily store bytes read from the files
    private int _iBitCounter; //counter for knowing how many bits are in the bit buffer

    public bool Compress(string pInputFileName, string pOutputFileName, out string fileNamePath)
    {
        Stream reader = null;
        Stream writer = null;

        try
        {
            Initialize();
            reader = new FileStream(pInputFileName, FileMode.Open);
            writer = new FileStream(pOutputFileName, FileMode.Create);
            var iNextCode = 256;

            //blank out table
            for (var i = 0; i < TableSize; i++)
            {
                _iaCodeTable[i] = -1;
            }

            var iString = reader.ReadByte();

            int iChar;
            //read until we reach end of file
            while ((iChar = reader.ReadByte()) != -1)
            {
                var iIndex = FindMatch(iString, iChar);

                //set string if we have something at that index
                if (_iaCodeTable[iIndex] != -1)
                {
                    iString = _iaCodeTable[iIndex];
                }
                //insert new entry
                else
                {
                    //otherwise we insert into the tables
                    if (iNextCode <= MaxCode)
                    {
                        _iaCodeTable[iIndex] = iNextCode++; //insert and increment next code to use
                        _iaPrefixTable[iIndex] = iString;
                        _iaCharTable[iIndex] = (byte)iChar;
                    }

                    WriteCode(writer, iString); //output the data in the string
                    iString = iChar;
                }
            }

            WriteCode(writer, iString); //output last code
            WriteCode(writer, MaxValue); //output end of buffer
            WriteCode(writer, 0); //flush
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            writer?.Close();
            File.Delete(pOutputFileName);
            fileNamePath = string.Empty;
            return false;
        }
        finally
        {
            reader?.Close();
            writer?.Close();
        }

        fileNamePath = Path.GetFullPath(pOutputFileName);
        return true;
    }

    public bool Decompress(string pInputFileName, string pOutputFileName, out string fileNamePath)
    {
        Stream reader = null;
        Stream writer = null;

        try
        {
            Initialize();
            reader = new FileStream(pInputFileName, FileMode.Open);
            writer = new FileStream(pOutputFileName, FileMode.Create);
            var iNextCode = 256;
            var baDecodeStack = new byte[TableSize];

            var iOldCode = ReadCode(reader);
            var bChar = (byte)iOldCode;
            writer.WriteByte((byte)iOldCode); //write first byte since it is plain ascii

            var iNewCode = ReadCode(reader);

            //read file all file
            while (iNewCode != MaxValue)
            {
                int iCurrentCode;
                int iCounter;
                if (iNewCode >= iNextCode)
                {
                    //fix for prefix+chr+prefix+char+prefx special case
                    baDecodeStack[0] = bChar;
                    iCounter = 1;
                    iCurrentCode = iOldCode;
                }
                else
                {
                    iCounter = 0;
                    iCurrentCode = iNewCode;
                }

                //decode string by cycling back through the prefixes
                while (iCurrentCode > 255)
                {
                    //lstDecodeStack.Add((byte)_iaCharTable[iCurrentCode]);
                    //iCurrentCode = _iaPrefixTable[iCurrentCode];
                    baDecodeStack[iCounter] = (byte)_iaCharTable[iCurrentCode];
                    ++iCounter;
                    if (iCounter >= MaxCode)
                    {
                        throw new Exception("oh crap");
                    }

                    iCurrentCode = _iaPrefixTable[iCurrentCode];
                }

                baDecodeStack[iCounter] = (byte)iCurrentCode;
                bChar = baDecodeStack[iCounter]; //set last char used

                //write out decoded stack
                while (iCounter >= 0)
                {
                    writer.WriteByte(baDecodeStack[iCounter]);
                    --iCounter;
                }

                //insert into tables
                if (iNextCode <= MaxCode)
                {
                    _iaPrefixTable[iNextCode] = iOldCode;
                    _iaCharTable[iNextCode] = bChar;
                    ++iNextCode;
                }

                iOldCode = iNewCode;

                //if (reader.PeekChar() != 0)
                iNewCode = ReadCode(reader);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            if (writer != null)
            {
                writer.Close();
            }

            File.Delete(pOutputFileName);
            fileNamePath = string.Empty;
            return false;
        }
        finally
        {
            reader?.Close();
            writer?.Close();
        }

        fileNamePath = Path.GetFullPath(pOutputFileName);
        return true;
    }

    private void
        Initialize() //used to blank  out bit buffer in case this class is called to compress and decompress from the same instance
    {
        _iBitBuffer = 0;
        _iBitCounter = 0;
    }

    //hashing function, tries to find index of prefix+char, if not found returns -1 to signify space available
    private int FindMatch(int pPrefix, int pChar)
    {
        var index = (pChar << HashBit) ^ pPrefix;

        var offset = index == 0 ? 1 : TableSize - index;

        while (true)
        {
            if (_iaCodeTable[index] == -1)
            {
                return index;
            }

            if (_iaPrefixTable[index] == pPrefix && _iaCharTable[index] == pChar)
            {
                return index;
            }

            index -= offset;
            if (index < 0)
            {
                index += TableSize;
            }
        }
    }

    private void WriteCode(Stream pWriter, int pCode)
    {
        _iBitBuffer |= (ulong)pCode << (32 - MaxBits - _iBitCounter); //make space and insert new code in buffer
        _iBitCounter += MaxBits; //increment bit counter

        //write all the bytes we can
        while (_iBitCounter >= 8)
        {
            pWriter.WriteByte((byte)((_iBitBuffer >> 24) & 255)); //write byte from bit buffer
            _iBitBuffer <<= 8; //remove written byte from buffer
            _iBitCounter -= 8; //decrement counter
        }
    }

    private int ReadCode(Stream pReader)
    {
        //fill up buffer
        while (_iBitCounter <= 24)
        {
            _iBitBuffer |= (ulong)pReader.ReadByte() << (24 - _iBitCounter); //insert byte into buffer
            _iBitCounter += 8; //increment counter
        }

        var iReturnVal = (uint)_iBitBuffer >> (32 - MaxBits);
        _iBitBuffer <<= MaxBits; //remove it from buffer
        _iBitCounter -= MaxBits; //decrement bit counter

        var temp = (int)iReturnVal;
        return temp;
    }
}
