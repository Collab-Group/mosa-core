// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External.x86.Driver;
using Mosa.Runtime.x86;

namespace Mosa.Kernel.x86
{
	/// <summary>
	/// Screen
	/// </summary>
	public static class Console
	{
		private static uint column = 0;
		private static uint row = 0;
		private static byte color = 0;

		/// <summary>
		/// The columns
		/// </summary>
		public const uint Columns = 80;

		/// <summary>
		/// The rows
		/// </summary>
		public const uint Rows = 25;

		/// <summary>
		/// Gets or sets the column.
		/// </summary>
		/// <value>
		/// The column.
		/// </value>
		public static uint CursorLeft
		{
			get { return column; }
			private set { column = value; }
		}

		/// <summary>
		/// Gets or sets the row.
		/// </summary>
		/// <value>
		/// The row.
		/// </value>
		public static uint CursorTop
		{
			get { return row; }
			private set { row = value; }
		}

		public static byte Color
		{
			get { return (byte)(color & 0x0F); }
			set { color &= 0xF0; color |= (byte)(value & 0x0F); }
		}

		public static byte BackgroundColor
		{
			get { return (byte)(color >> 4); }
			set { color &= 0x0F; color |= (byte)((value & 0x0F) << 4); }
		}

		internal static void Setup() 
		{
			BackgroundColor = ConsoleColor.Black;
			Clear();
			SetCursorPosition(0, 0);
			Color = ConsoleColor.White;
		}

		/// <summary>
		/// Next Column
		/// </summary>
		private static void Next()
		{
			CursorLeft++;

			if (CursorLeft >= Columns)
			{
				CursorLeft = 0;
				CursorTop++;
			}
		}

		private static void Previous()
		{
			if (CursorLeft == 0 && CursorTop == 0)
			{
				return;
			}


			if (CursorLeft != 0)
			{
				CursorLeft--;
			}
			else
			{
				CursorTop--;
				CursorLeft = Columns - 1;
			}
		}

		public static void Back()
		{
			Previous();
			UpdateCursor();
		}

		/// <summary>
		/// Skips the specified skip.
		/// </summary>
		/// <param name="skip">The skip.</param>
		private static void Skip(uint skip)
		{
			for (uint i = 0; i < skip; i++)
				Next();
		}

		/// <summary>
		/// Writes the character.
		/// </summary>
		/// <param name="chr">The character.</param>
		private static void Write(char chr)
		{
			if (chr == '\n')
			{
				CursorLeft = 0;
				CursorTop++;
				MoveUpPrevious();
			}
            		else
           		{
				MoveUpPrevious();

				Native.Set8(0x0B8000 + ((CursorTop * Columns + CursorLeft) * 2), (byte)chr);
				Native.Set8(0x0B8000 + ((CursorTop * Columns + CursorLeft) * 2) + 1, color);

				Next();
			}

			UpdateCursor();
		}

		private static void MoveUpPrevious()
		{
			if (CursorTop == Rows)
			{
				ASM.MEMCPY(0x0B8000, 0x0B80A0, 0xF00);
				ASM.MEMFILL(0xB8F00, 0xA0, 0);
				SetCursorPosition(0, Rows - 1);
			}
		}

		/// <summary>
		/// Writes the string to the screen.
		/// </summary>
		/// <param name="value">The string value to write to the screen.</param>
		public static void Write(string value)
		{
			for (int index = 0; index < value.Length; index++)
			{
				char chr = value[index];
				Write(chr);
			}
		}

		public static void Write(string value, byte Color)
		{
			byte c = Console.Color;
			Console.Color = Color;
			Write(value);
			Console.Color = c;
		}

		/// <summary>
		/// Goto the top.
		/// </summary>
		private static void GotoTop()
		{
			CursorLeft = 0;
			CursorTop = 0;
			UpdateCursor();
		}

		/// <summary>
		/// Writes the line.
		/// </summary>
		public static void WriteLine()
		{
			NextLine();
		}

		/// <summary>
		/// Goto the next line.
		/// </summary>
		private static void NextLine()
		{
			CursorLeft = 0;
			CursorTop++;
			MoveUpPrevious();
			UpdateCursor();
		}

		/// <summary>
		/// Writes the line.
		/// </summary>
		/// <param name="line">The line.</param>
		public static void WriteLine(string line)
		{
			Write(line);
			NextLine();
		}

		public static void WriteLine(string line, byte Color)
        	{
			byte c = Console.Color;
			Console.Color = Color;
			WriteLine(line);
			Console.Color = c;
        	}

		public static void Clear(byte Color, bool ChangeBackBackgroundColor=false)
        	{
			byte c = BackgroundColor;
			BackgroundColor = Color;
			Clear();
			if (ChangeBackBackgroundColor)
            		{
				BackgroundColor = c;
            		}
        	}

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public static void Clear()
		{
			GotoTop();

			byte c = Color;
			Color = 0x0;

			for (int i = 0; i < Columns * Rows; i++)
				Write(' ');

			Color = c;
			GotoTop();
		}

		/// <summary>
		/// Goto the specified row and column.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <param name="col">The col.</param>
		public static void SetCursorPosition(uint left, uint top)
		{
			CursorTop = top;
			CursorLeft = left;
			UpdateCursor();
		}

		public static PS2Keyboard.KeyCode ReadKey() 
		{
			PS2Keyboard.KeyCode key;
			while ((key = PS2Keyboard.GetKeyPressed()) == 0)
			{
				Native.Hlt();
			};
			return key;
		}
		
		public static string ReadLine()
		{
			string S = "";
			string Line = "";
			PS2Keyboard.KeyCode code;
			for (; ; )
			{
				code = ReadKey();

				if (code == PS2Keyboard.KeyCode.Enter)
				{
					break;
				}
				else if (code == PS2Keyboard.KeyCode.Delete)
				{
					if (Line.Length != 0)
					{
						Console.Back();
						Line = Line.Substring(0, Line.Length - 1);
					}
				}
				else
				{
					if (PS2Keyboard.IsCapsLock)
					{
						S = code.KeyCodeToString().ToUpper();
					}
					else
					{
						S = code.KeyCodeToString().ToLower();
					}
					Line += S;
					Console.Write(S);
				}
			}
			Console.WriteLine();
			return Line;
		}

		/// <summary>
		/// Sets the cursor.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <param name="column">The column.</param>
		private static void SetCursor(uint row, uint column)
		{
			uint location = (row * Columns) + column;

			Native.Out8(0x3D4, 0x0F);
			Native.Out8(0x3D5, (byte)(location & 0xFF));

			Native.Out8(0x3D4, 0x0E);
			Native.Out8(0x3D5, (byte)((location >> 8) & 0xFF));
		}

		private static void UpdateCursor()
		{
			Native.Set8(0x0B8000 + ((CursorTop * Columns + CursorLeft) * 2), (byte)' ');
			Native.Set8(0x0B8000 + ((CursorTop * Columns + CursorLeft) * 2) + 1, color);
			SetCursor(CursorTop, CursorLeft);
		}
	}
}
