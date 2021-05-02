using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CompressionLibrary.Huffman {

	public static class Compressor {
		public delegate void UpdateCrc (int value);

		private static byte[] sign = { 0x5a, 0x52, 0x41, 0x48 };

		public static void HuffConsole(string inputFile, string outputFile, out string filePath) {
			try {
				Stream ifStream = new FileStream (inputFile, FileMode.Open, FileAccess.Read);
				Stream ofStream = new FileStream (outputFile, FileMode.Create, FileAccess.ReadWrite);
				var crc = new CrcCalc ();
			
				//Writing file signature
				ofStream.Write (sign, 0, sign.Length);
				//Padding for the header
				for (var i = 0; i < 12; i++)
					ofStream.WriteByte (0x0);
			
				var myCompressor = new Thread (o => Huff (ifStream, ofStream, val => crc.UpdateByte ((byte)val)));
				myCompressor.Start ();
			
				while (myCompressor.IsAlive) {
					Console.Write ("\rCompressed {0} out of {1}", ifStream.Position, ifStream.Length);
					Thread.Sleep (100);
				}
				//Writing file length to the header
				ofStream.Seek (4, SeekOrigin.Begin);
				var buffer = BitConverter.GetBytes (ofStream.Length);
				ofStream.Write (buffer, 0, buffer.Length);
			
				//Writing file crc immediately after the length
				buffer = BitConverter.GetBytes (crc.GetCrc ());
				ofStream.Write (buffer, 0, buffer.Length);
				PrintHelper.Notify ("\nOriginal file CRC32 is {0:X8}\n", crc.GetCrc ());
				ofStream.Close();
				ifStream.Close ();
				filePath = Path.GetFullPath(outputFile);
			}
			catch (Exception)
			{
				filePath = string.Empty;
                throw;
            }
		}

		private static void Huff (Stream sin, Stream sout, UpdateCrc callback) {
			var t = new Tree ();
			int rd, i, tmp;
			Stack<int> ret;
			var bw = new BitIo (sout, true);
			while ((rd = sin.ReadByte ()) != -1) {
				tmp = rd;
				callback ((byte)tmp);
				if (!t.contains (rd)) {
					ret = t.GetCode (257);
					while (ret.Count > 0)
						bw.WriteBit (ret.Pop ());
					for (i = 0; i < 8; i++) {
						bw.WriteBit ((int)(rd & 0x80));
						rd <<= 1;
					}
					t.UpdateTree (tmp);
				}				
				else {
					ret = t.GetCode (tmp);
					while (ret.Count > 0)
						bw.WriteBit (ret.Pop ());
					t.UpdateTree (tmp);
				}
			}
			
			ret = t.GetCode (256);
			while (ret.Count > 0) {
				bw.WriteBit (ret.Pop ());
			}
		}

		public static void Huff (Stream sin, Stream sout) {
			var t = new Tree ();
			int rd, i, tmp;
			Stack<int> ret;
			BitIo bw = new BitIo (sout, true);
			while ((rd = sin.ReadByte ()) != -1) {
				tmp = rd;
				if (!t.contains (rd)) {
					ret = t.GetCode (257);
					while (ret.Count > 0)
						bw.WriteBit (ret.Pop ());
					for (i = 0; i < 8; i++) {
						bw.WriteBit ((int)(rd & 0x80));
						rd <<= 1;
					}
					t.UpdateTree (tmp);
				}
				
				else {
					ret = t.GetCode (tmp);
					while (ret.Count > 0)
						bw.WriteBit (ret.Pop ());
					t.UpdateTree (tmp);
				}
			}
			
			ret = t.GetCode (256);
			while (ret.Count > 0)
				bw.WriteBit (ret.Pop ());
			bw.Close();
		}

		public static void UnHuffConsole(string inputFile, string outputFile, out string filePath) {
			Stream ifstream = new FileStream (inputFile, FileMode.Open, FileAccess.Read);
			Stream ofstream = new FileStream (outputFile, FileMode.Create, FileAccess.ReadWrite);
			CrcCalc crcCalc = new CrcCalc ();
			uint crc_old, crc_new;
			
			for (int i = 0; i < sign.Length; i++)
				if (ifstream.ReadByte () != sign[i]) {
					ifstream.Close ();
					ofstream.Close ();
					throw new IOException ("The supplied file is not a valid huff archive");
				}
			
			//Read file length
			byte[] buffer = new byte[8];
			ifstream.Read (buffer, 0, 8);
			long size = BitConverter.ToInt64 (buffer, 0);
			if (size != ifstream.Length) {
				ifstream.Close ();
				ofstream.Close ();
				throw new IOException ("Invalid file length");
			}
			
			//Read file crc
			ifstream.Read (buffer, 0, 4);
			crc_old = BitConverter.ToUInt32 (buffer, 0);
			PrintHelper.Notify ("Stored crc is {0:X8}\n", crc_old);
			
			var myDecompressor = new Thread (o => 
			{
				try {
					UnHuff (ifstream, ofstream, x => crcCalc.UpdateByte ((byte)x));
				}
				catch (Exception e) {
					PrintHelper.Err("\n" + e.Message);
				}
			});
			myDecompressor.Start ();
			while (myDecompressor.IsAlive) {
				PrintHelper.Notify ("\rDecompressed {0} bytes out of {1}", ifstream.Position, ifstream.Length);
				Thread.Sleep (100);
			}
			
			crc_new = crcCalc.GetCrc ();
			PrintHelper.Notify ("\nDecompressed CRC32 is {0:X8}\n", crc_new);
			if (crc_new == crc_old)
				PrintHelper.Notify ("Checksums match - OK\n");
			else
				PrintHelper.Err ("Checksum mismatch - decompressed" + " file may contain corrupted data\n");
			ofstream.Close();
			ifstream.Close();
			filePath = Path.GetFullPath(outputFile);
		}

		public static void UnHuff (Stream InStream, Stream OutStream, UpdateCrc callback) {
			int i = 0, count = 0, sym;
			Tree t = new Tree ();
			BitIo bitIO = new BitIo (InStream, false);
			while ((i = bitIO.ReadBit ()) != 2) {
				if ((sym = t.DecodeBinary (i)) != Tree.CharIsEof) {
					OutStream.WriteByte ((byte)sym);
					callback (sym);
					count++;
				}
			}
		}

		public static void UnHuff (Stream InStream, Stream OutStream) {
			int i = 0, count = 0;
			Tree t = new Tree ();
			BitIo bitIO = new BitIo (InStream, false);
			while ((i = bitIO.ReadBit ()) != 2) {
				if ((count = t.DecodeBinary (i)) != Tree.CharIsEof)
					OutStream.WriteByte ((byte)count);
			}
		}
	}
}
