﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CompressionLibrary.Bwt
{
    public static class HuffmanCoding
    {
        private class HuffmanNode : IComparable<HuffmanNode>
        {
            public HuffmanNode Left;
            public HuffmanNode Right;
            public byte Value = 0;
            public int Frequency = 0;
            public int Size = 1;

            public HuffmanNode() { }

            public HuffmanNode(byte Value, int Frequency)
            {
                this.Value = Value;
                this.Frequency = Frequency;
                this.Left = null;
                this.Right = null;
            }

            public HuffmanNode(HuffmanNode Left, HuffmanNode Right)
            {
                this.Frequency = Left.Frequency + Right.Frequency;
                this.Size = Left.Size + Right.Size + 1;
                this.Left = Left;
                this.Right = Right;
            }

            public bool IsLeaf()
            {
                return this.Left == null && this.Right == null;
            }

            public int CompareTo(HuffmanNode other)
            {
                if (this.Frequency != other.Frequency)
                    return this.Frequency.CompareTo(other.Frequency);
                else
                    return this.Value.CompareTo(other.Value);
            }

            public override string ToString()
            {
                return this.Value + ", " + this.Frequency;
            }
        }

        public static byte[] Encode(byte[] input)
        {
            int[] freq = new int[256];
            // Calculate Frequency
            foreach (byte b in input)
            {
                freq[b]++;
            }

            // Construct Huffman Tree
            PriorityQueue<HuffmanNode> q = new PriorityQueue<HuffmanNode>();
            for (int i = 0; i <= 255; i++)
            {
                if (freq[i] != 0)
                    q.Add(new HuffmanNode((byte)i, freq[i]));
            }
            while (q.Count != 1)
            {
                HuffmanNode n1 = q.Next();
                HuffmanNode n2 = q.Next();
                q.Add(new HuffmanNode(n2, n1));
            }
            HuffmanNode root = q.Next();
            BitArray[] table = ConstuctHuffmanTable(root);

            // Preorder Tree Structure Length in bits.
            int header1Size = root.Size;
            // Leaves values list size in bytes.
            int header2Size = freq.Count((e) => (e > 0) ? true : false);
            // Huffmanencoded output size in bits.
            int codeSize = 0;
            for (int i = 0; i < 256; i++)
            {
                if (freq[i] != 0)
                    codeSize += freq[i] * table[i].Length;
            }
            // Total output size in bytes.
            int outputSize = BitLengthToByteLength(header1Size) + header2Size
                           + BitLengthToByteLength(codeSize) + 4;

            // Encoded output including headers.
            byte[] encodedOutput = new byte[outputSize];
            // Current Length of the encoded output.
            int currEncodedLength = 0;
            // Construct preorder traversal of the Huffman tree,
            // assuming a leaf node's value equal to 1, and 0 otherwise.
            BitArray header1BitArray = new BitArray(header1Size);
            // Current position in the bit array
            int k = 0;
            Stack<HuffmanNode> preStack = new Stack<HuffmanNode>();
            preStack.Push(root);
            while (preStack.Count != 0)
            {
                HuffmanNode currNode = preStack.Pop();
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
            byte[] header2 = ConstructLeavesValues(root, header2Size);
            // Copy header2 to encoded output.
            header2.CopyTo(encodedOutput, currEncodedLength);
            currEncodedLength += header2Size;

            // Copy Code Size Int Bytes to encoded output.
            IntToByteArr(codeSize).CopyTo(encodedOutput, currEncodedLength);
            currEncodedLength += 4;

            // Encoded input bits.
            BitArray codeBits = new BitArray(codeSize);
            // Position at the bit array.
            int currCodeBit = 0;

            // Encode every byte in the input.
            foreach (byte b in input)
            {
                BitArray bCode = table[b];
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
            BitArray bitArray = new BitArray(input);
            HuffmanNode root = ConstructHuffmanTree(bitArray);
            int header1Size = root.Size;
            int currBytePosition = BitLengthToByteLength(header1Size);
            int header2Size = ConstructHuffmanTreeLeaves(root, input, currBytePosition);
            currBytePosition += header2Size;

            int codeLength = ByteArrToInt(input, currBytePosition);
            currBytePosition += 4;

            List<Byte> decodedOutput = new List<byte>();
            
            int currBitPosition = currBytePosition * 8;
            HuffmanNode currNode;
            int c = 0;
            while(currBitPosition < bitArray.Length && c < codeLength)
            {
                currNode = root;
                while (!currNode.IsLeaf())
                {
                    bool b = bitArray[currBitPosition++];
                    c++;
                    if (b)
                        currNode = currNode.Right;
                    else
                        currNode = currNode.Left;
                }
                decodedOutput.Add(currNode.Value);
            }
            
            return decodedOutput.ToArray();
        }

        #region EncodingHelpers

        private static BitArray[] ConstuctHuffmanTable(HuffmanNode Root)
        {
            // Init code length to the worst case.
            BitArray code = new BitArray(256);
            // Init table to alphabet size
            BitArray[] table = new BitArray[256];
            _ConstuctHuffmanTable(Root, 0, code, table);
            return table;
        }

        private static void _ConstuctHuffmanTable(HuffmanNode Node, int Depth, BitArray Code, BitArray[] Table)
        {
            if (Node.IsLeaf())
            {
                Table[Node.Value] = new BitArray(Depth);
                for (int i = 0; i < Depth; i++)
                {
                    bool b = Code.Get(i);
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
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        private static byte[] ConstructLeavesValues(HuffmanNode Root, int LeavesNum)
        {
            byte[] values = new byte[LeavesNum];
            int i = 0;
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
            int i = 0;
            return _ConstructHuffmanTree(bitArray, ref i);
        }

        private static HuffmanNode _ConstructHuffmanTree(BitArray bitArray, ref int I)
        {
            HuffmanNode curr = new HuffmanNode();
            curr.Size = 1;
            if (bitArray[I] == true)
                return curr;
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
            int j = i;
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

        private static byte[] IntToByteArr(int i)
        {
            return BitConverter.GetBytes(i);
        }

        private static int ByteArrToInt(byte[] input, int StartIndex)
        {
            return BitConverter.ToInt32(input, StartIndex);
        }

        private static int BitLengthToByteLength(int bitLength)
        {
            return (bitLength - 1) / 8 + 1;
        }
    }
}
