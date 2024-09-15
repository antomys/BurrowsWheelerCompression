using System;

namespace CompressionLibrary.Huffman {

	public static class PrintHelper {
		public static void Warn (string message) {
			Write (message, ConsoleColor.Yellow);
		}

		public static void Warn (string message, params object[] data) {
			Write (message, ConsoleColor.Yellow, data);
		}

		public static void Err (string message) {
			Write (message, ConsoleColor.Red);
		}

		public static void Err (string message, params object[] data) {
			Write (message, ConsoleColor.Red, data);
		}

		public static void Notify (string message) {
			Write (message, ConsoleColor.Cyan);
		}

		public static void Notify (string message, params object[] data) {
			Write (message, ConsoleColor.Cyan, data);
		}

		private static void Write (string message, ConsoleColor color, params object[] data) {
			ConsoleColor ck = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write (message, data);
			Console.ForegroundColor = ck;
		}

		private static void Write (string message, ConsoleColor color) {
			ConsoleColor ck = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write (message);
			Console.ForegroundColor = ck;
		}
	}
}
