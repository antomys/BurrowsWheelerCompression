using System.Collections;

namespace BurrowsWheelerTransform;

public static class HuffmanCoding
{
    public static byte[] Encode(byte[] input)
    {
        var freq = new int[256];
        // Calculate Frequency
        foreach (var b in input)
        {
            freq[b]++;
        }

        // Construct Huffman Tree
        var q = new PriorityQueue<HuffmanNode>();
        for (var i = 0; i <= 255; i++)
        {
            if (freq[i] != 0)
            {
                q.Add(new HuffmanNode((byte)i, freq[i]));
            }
        }

        while (q.Count != 1)
        {
            var n1 = q.Next();
            var n2 = q.Next();
            q.Add(new HuffmanNode(n2, n1));
        }

        var root = q.Next();
        var table = ConstuctHuffmanTable(root);

        // Preorder Tree Structure Length in bits.
        var header1Size = root.Size;
        // Leaves values list size in bytes.
        var header2Size = freq.Count(e => e > 0 ? true : false);
        // Huffmanencoded output size in bits.
        var codeSize = 0;
        for (var i = 0; i < 256; i++)
        {
            if (freq[i] != 0)
            {
                codeSize += freq[i] * table[i].Length;
            }
        }

        // Total output size in bytes.
        var outputSize = BitLengthToByteLength(header1Size) + header2Size
                                                            + BitLengthToByteLength(codeSize) + 4;

        // Encoded output including headers.
        var encodedOutput = new byte[outputSize];
        // Current Length of the encoded output.
        var currEncodedLength = 0;
        // Construct preorder traversal of the Huffman tree,
        // assuming a leaf node's value equal to 1, and 0 otherwise.
        var header1BitArray = new BitArray(header1Size);
        // Current position in the bit array
        var k = 0;
        var preStack = new Stack<HuffmanNode>();
        preStack.Push(root);
        while (preStack.Count != 0)
        {
            var currNode = preStack.Pop();
            header1BitArray.Set(k++, currNode.IsLeaf());

            if (!currNode.IsLeaf())
            {
                preStack.Push(currNode.Right);
                preStack.Push(currNode.Left);
            }
        }

        // Copy header1 to encoded output.
        header1BitArray.CopyTo(encodedOutput, currEncodedLength);
        currEncodedLength += BitLengthToByteLength(header1Size);

        // Header2 is a list of leaves values ordered from left to right.
        var header2 = ConstructLeavesValues(root, header2Size);
        // Copy header2 to encoded output.
        header2.CopyTo(encodedOutput, currEncodedLength);
        currEncodedLength += header2Size;

        // Copy Code Size Int Bytes to encoded output.
        IntToByteArr(codeSize).CopyTo(encodedOutput, currEncodedLength);
        currEncodedLength += 4;

        // Encoded input bits.
        var codeBits = new BitArray(codeSize);
        // Position at the bit array.
        var currCodeBit = 0;

        // Encode every byte in the input.
        foreach (var b in input)
        {
            var bCode = table[b];
            // Copy encodedbyte to codeBits.
            foreach (bool cbc in bCode)
            {
                codeBits[currCodeBit++] = cbc;
            }
        }

        // Copy codeBits to encoded output.
        codeBits.CopyTo(encodedOutput, currEncodedLength);

        return encodedOutput;
    }

    public static byte[] Decode(byte[] input)
    {
        var bitArray = new BitArray(input);
        var root = ConstructHuffmanTree(bitArray);
        var header1Size = root.Size;
        var currBytePosition = BitLengthToByteLength(header1Size);
        var header2Size = ConstructHuffmanTreeLeaves(root, input, currBytePosition);
        currBytePosition += header2Size;

        var codeLength = ByteArrToInt(input, currBytePosition);
        currBytePosition += 4;

        var decodedOutput = new List<byte>();

        var currBitPosition = currBytePosition * 8;
        var c = 0;
        while (currBitPosition < bitArray.Length && c < codeLength)
        {
            var currNode = root;
            while (!currNode.IsLeaf())
            {
                var b = bitArray[currBitPosition++];
                c++;
                currNode = b ? currNode.Right : currNode.Left;
            }

            decodedOutput.Add(currNode.Value);
        }

        return decodedOutput.ToArray();
    }

    private static byte[] IntToByteArr(int i)
    {
        return BitConverter.GetBytes(i);
    }

    private static int ByteArrToInt(byte[] input, int startIndex)
    {
        return BitConverter.ToInt32(input, startIndex);
    }

    private static int BitLengthToByteLength(int bitLength)
    {
        return ((bitLength - 1) / 8) + 1;
    }

    #region EncodingHelpers

    private static BitArray[] ConstuctHuffmanTable(HuffmanNode Root)
    {
        // Init code length to the worst case.
        var code = new BitArray(256);
        // Init table to alphabet size
        var table = new BitArray[256];
        _ConstuctHuffmanTable(Root, 0, code, table);
        return table;
    }

    private static void _ConstuctHuffmanTable(HuffmanNode Node, int Depth, BitArray Code, BitArray[] Table)
    {
        if (Node.IsLeaf())
        {
            Table[Node.Value] = new BitArray(Depth);
            for (var i = 0; i < Depth; i++)
            {
                var b = Code.Get(i);
                Table[Node.Value].Set(i, b);
            }

            return;
        }

        Code.Set(Depth, false);
        _ConstuctHuffmanTable(Node.Left, Depth + 1, Code, Table);

        Code.Set(Depth, true);
        _ConstuctHuffmanTable(Node.Right, Depth + 1, Code, Table);
    }

    private static byte[] BitArrayToByteArray(BitArray bits)
    {
        var ret = new byte[((bits.Length - 1) / 8) + 1];
        bits.CopyTo(ret, 0);
        return ret;
    }

    private static byte[] ConstructLeavesValues(HuffmanNode Root, int LeavesNum)
    {
        var values = new byte[LeavesNum];
        var i = 0;
        _ConstructLeavesValues(Root, values, ref i);
        return values;
    }

    private static void _ConstructLeavesValues(HuffmanNode Node, byte[] Values, ref int I)
    {
        if (Node.IsLeaf())
        {
            Values[I++] = Node.Value;
            return;
        }

        _ConstructLeavesValues(Node.Left, Values, ref I);
        _ConstructLeavesValues(Node.Right, Values, ref I);
    }

    #endregion

    #region DecodingHelpers

    private static HuffmanNode ConstructHuffmanTree(BitArray bitArray)
    {
        var i = 0;
        return _ConstructHuffmanTree(bitArray, ref i);
    }

    private static HuffmanNode _ConstructHuffmanTree(BitArray bitArray, ref int I)
    {
        var curr = new HuffmanNode();
        curr.Size = 1;
        if (bitArray[I])
        {
            return curr;
        }

        I++;
        curr.Left = _ConstructHuffmanTree(bitArray, ref I);
        curr.Size += curr.Left.Size;
        I++;
        curr.Right = _ConstructHuffmanTree(bitArray, ref I);
        curr.Size += curr.Right.Size;
        return curr;
    }

    private static int ConstructHuffmanTreeLeaves(HuffmanNode Node, byte[] bArray, int i)
    {
        var j = i;
        _ConstructHuffmanTreeLeaves(Node, bArray, ref j);
        return j - i;
    }

    private static void _ConstructHuffmanTreeLeaves(HuffmanNode Node, byte[] bArray, ref int i)
    {
        if (Node.IsLeaf())
        {
            Node.Value = bArray[i++];
            return;
        }

        _ConstructHuffmanTreeLeaves(Node.Left, bArray, ref i);
        _ConstructHuffmanTreeLeaves(Node.Right, bArray, ref i);
    }

    #endregion
}
