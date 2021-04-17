using System;

namespace Bwt.Demo.Bwt
{
    public static class Bwt
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Type byte[], should return transformed byte[]</param>
        /// <returns></returns>
        public static byte[] Transform(byte[] input)
        {
            var output = new byte[input.Length + 4];
            var newInput = new short[input.Length + 1];

            for (var i = 0; i < input.Length; i++)
                newInput[i] = (short)(input[i] + 1);

            newInput[input.Length] = 0;
            var suffixArray = SuffixArray.Construct(newInput);
            var end = 0;
            var outputInd = 0;
            for (var i = 0; i < suffixArray.Length; i++)
            {
                if (suffixArray[i] == 0)
                {
                    end = i;
                    continue;
                }
                output[outputInd] = (byte)(newInput[suffixArray[i] - 1] - 1);
                outputInd++;
            }
            var endByte = IntToByteArr(end);
            endByte.CopyTo(output, input.Length);
            return output;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Type byte[] should return inverse transformed byte[]</param>
        /// <returns></returns>
        public static byte[] InverseTransform(byte[] input)
        {
            var length = input.Length - 4;
            var I = ByteArrToInt(input, input.Length - 4);
            var freq = new int[256];
            Array.Clear(freq, 0, freq.Length);
            // T1: Number of Preceding Symbols Matching Symbol in Current Position.
            var T1 = new int[length];
            // T2: Number of Symbols Lexicographically Less Than Current Symbol
            var T2 = new int[256];
            Array.Clear(T2, 0, T2.Length);
            // Construct T1
            for (var i = 0; i < length; i++)
            {
                T1[i] = freq[input[i]];
                freq[input[i]]++;
            }

            // Construct T2
            // Add $ special symbol in consideration to be less than any symbol
            T2[0] = 1;
            for (var i = 1; i < 256; i++)
            {
                T2[i] = T2[i - 1] + freq[i - 1];
            }

            var output = new byte[length];
            var nxt = 0;
            for (var i = length - 1; i >= 0; i--)
            {
                output[i] = input[nxt];
                var a = T1[nxt];
                var b = T2[input[nxt]];
                nxt = a + b;
                // Add $ special symbol index in consideration
                if (nxt >= I)
                {
                    nxt--;
                }
            }
            return output;
        }
        private static byte[] IntToByteArr(int i)
        {
            return BitConverter.GetBytes(i);
        }
        private static int ByteArrToInt(byte[] input, int startIndex)
        {
            return BitConverter.ToInt32(input, startIndex);
        }
    }
}