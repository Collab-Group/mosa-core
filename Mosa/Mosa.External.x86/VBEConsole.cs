// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External.x86.Drawing;
using Mosa.External.x86.Drawing.Fonts;
using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime.Plug;
using Mosa.Runtime.x86;
using System.Drawing;


namespace Mosa.External.x86
{

    public unsafe class VBEConsole
    {
        // VBE.VBEModeInfo->ScreenWidth;
        // VBE.VBEModeInfo->ScreenHeight;


        public static int X { private set; get; } // The Char Position Of the console
        public static int Y { private set; get; }
        public static uint Background { private set; get; }
        public static readonly int fontWidth = 8;
        public static readonly int fontHeight = 16;
        public static uint Defaultcolour;


        public static Graphics graphics { private set; get; }

        public static void setup(Graphics newGraphics, uint backgroundcolour)
        {
            graphics = newGraphics;
            Background = backgroundcolour;
        }
        public static void setBackgroundColour(uint colour)
        {
            Background = colour;
        }
        public static void SetPosition(int x, int y)
        {
            X = x * fontWidth;
            Y = y * fontHeight;
        }
        public static void Next()
        {
            X += fontWidth;

            if (X >= VBE.VBEModeInfo->ScreenWidth)
            {
                X = 0;
                Y += fontHeight;
            }
        }
        public static void Previous()
        {
            if (X == 0 && Y == 0)
            {
                return;
            }


            if (X != 0)
            {
                X -= fontWidth;
            }
            else
            {
                Y -= fontHeight;
                X = VBE.VBEModeInfo->ScreenWidth - fontWidth;
            }
        }
        public static void NewLine()
        {
            X = 0;
            Y += fontHeight;
        }


        public static void Write(char c, uint colour)
        {
            graphics.DrawACS16String(colour, c + "", X, Y);
            Next();
            graphics.Update();
        }
        public static void WriteLine(char c, uint colour)
        {
            Write(c, colour);
            NewLine();
        }
        public static void Write(string s, uint colour)
        {
            for (int i = 0; i < s.Length; i++)
            {
                Write(s[i], colour);
            }
        }
        public static void WriteLine(string s, uint colour)
        {
            Write(s, colour);
            NewLine();
        }

        public static void Write(char c)
        {
            Write(c, Defaultcolour);
        }
        public static void WriteLine(char c)
        {
            WriteLine(c, Defaultcolour);
        }
        public static void Write(string s)
        {
            Write(s, Defaultcolour);
        }
        public static void WriteLine(string s)
        {
            WriteLine(s, Defaultcolour);
        }

        public static string ReadLine()
        {
            string S = "";
            string Line = "";
            KeyCode code;
            for (; ; )
            {
                code = ReadKey();

                if (code == KeyCode.Enter)
                {
                    break;
                }
                else if (code == KeyCode.Delete)
                {
                    if (Line.Length != 0)
                    {
                        Previous();
                        graphics.DrawFilledRectangle(Background, X, Y, fontWidth, fontHeight);
                        Line = Line.Substring(0, Line.Length - 1);
                        graphics.Update();
                    }
                }
                else
                {
                    S = PS2Keyboard.KeyCodeToString(code);
                    Line += S;
                    Write(S);
                }
            }
            NewLine();
            return Line;
        }
        public static KeyCode ReadKey(bool WaitForKey = true)
        {
            if (WaitForKey)
            {
                KeyCode code;
                for (; ; )
                {
                    Native.Hlt();
                    code = PS2Keyboard.GetKeyPressed();
                    if (code != 0) break;
                }
                return code;
            }
            else
            {
                return PS2Keyboard.GetKeyPressed();
            }
        }

        public static void ToTop()
        {
            X = 0;
            Y = 0;
        }

        public static void Clear()
        {
            graphics.Clear(Background);
            ToTop();
        }


    }

}
