// Copyright (c) MOSA Project. Licensed under the New BSD License.
using Mosa.External.x86.Drawing.Fonts;
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System;
using System.Drawing;

namespace Mosa.External.x86.Drawing
{
    public unsafe abstract class Graphics
    {
        public int Width;
        public int Height;
        public int Bpp = 4;

        public uint VideoMemoryCacheAddr;

        public int FrameSize
        {
            get { return Width * Height * Bpp; }
        }

        public string CurrentDriver;

        public virtual void DrawFilledRectangle(uint Color, int X, int Y, int aWidth, int aHeight)
        {
            //Limit Won't Work
            aWidth = Math.Clamp(aWidth, 0, Width - X);
            aHeight = Math.Clamp(aHeight, 0, Height - Y);

            if (X >= Width)
                return;

            if (Y >= Height)
                return;

            if (X < 0)
            {
                aWidth += X;
                X = 0;
            }

            if (Y < 0)
            {
                aHeight += Y;
                Y = 0;
            }

            if (!CurrentDriver.Equals("VGA"))
            {
                for (int h = 0; h < aHeight; h++)
                    ASM.MEMFILL(
                        (uint)(VideoMemoryCacheAddr + ((Width * (Y + h) + X) * Bpp)),
                        (uint)(aWidth * Bpp),
                        Color
                        );
            }
            else
            {
                for (int w = 0; w < aWidth; w++)
                    for (int h = 0; h < aHeight; h++)
                        DrawPoint(Color, w, h);
            }
        }

        public virtual void DrawArray(int x, int y, int width, int height, int[] array, uint color)
        {
            for (int h = 0; h < height; h++)
                for (int w = 0; w < width; w++)
                    if (array[h * width + w] == 1)
                        DrawPoint(color, w + x, h + y);
        }

        public virtual void DrawRectangle(uint Color, int X, int Y, int Width, int Height, int Weight)
        {
            DrawFilledRectangle(Color, X, Y, Width, Weight);

            DrawFilledRectangle(Color, X, Y, Weight, Height);
            DrawFilledRectangle(Color, X + (Width - Weight), Y, Weight, Height);

            DrawFilledRectangle(Color, X, Y + (Height - Weight), Width, Weight);
        }

        public virtual void DrawFilledRoundedRectangle(uint color, int x, int y, int width, int height, int radius)
        {
            DrawFilledRectangle(color, x + radius, y, width - radius * 2, height);
            DrawFilledRectangle(color, x, y + radius, width, height - radius * 2);

            DrawFilledCircle(color, x + radius, y + radius, radius);
            DrawFilledCircle(color, x + width - radius - 1, y + radius, radius);

            DrawFilledCircle(color, x + radius, y + height - radius - 1, radius);
            DrawFilledCircle(color, x + width - radius - 1, y + height - radius - 1, radius);
        }

        public abstract void DrawPoint(uint Color, int X, int Y);

        public abstract uint GetPoint(int X, int Y);

        public abstract void Update();

        public virtual void Clear(uint Color)
        {
            VBEConsole.setBackgroundColour(Color);
            if (!CurrentDriver.Equals("VGA"))
            {
                ASM.MEMFILL(VideoMemoryCacheAddr, (uint)FrameSize, Color);
            }
            else
            {
                DrawFilledRectangle(Color, 0, 0, Width, Height);
            }
        }

        public abstract void Disable();

        public abstract void Enable();

        public virtual void DrawImage(Image image, int X, int Y, int TransparentColor)
        {
            for (int h = 0; h < image.Height; h++)
                for (int w = 0; w < image.Width; w++)
                    if (image.RawData[(uint)(image.Width * h + w)] != TransparentColor)
                        DrawPoint((uint)image.RawData[(uint)(image.Width * h + w)], X + w, Y + h);
        }

        //Only 32Bits
        public virtual void DrawImageASM(Image image, int X, int Y)
        {
            //Limit Won't Work
            for (int h = 0; h < Math.Clamp(image.Height, 0, Height - Y); h++)
                ASM.MEMCPY(
                    (uint)(VideoMemoryCacheAddr + ((Width * (Y + h) + X) * Bpp)),
                    (uint)((uint)image.RawData.Address + (image.Width * 4 * h)),
                    (uint)Math.Clamp(image.Width * 4, 0, (Width - X) * 4)
                    );
        }

        public virtual void DrawImage(Image image, int X, int Y, bool DrawWithAlpha)
        {
            for (int h = 0; h < image.Height; h++)
                for (int w = 0; w < image.Width; w++)
                    if (DrawWithAlpha)
                    {
                        Color foreground = Color.FromArgb(image.RawData[(uint)(image.Width * h + w)]);
                        Color background = Color.FromArgb((int)GetPoint(X + w, Y + h));

                        int alpha = foreground.A;
                        int inv_alpha = 255 - alpha;

                        byte newR = (byte)(((foreground.R * alpha + inv_alpha * background.R) >> 8) & 0xFF);
                        byte newG = (byte)(((foreground.G * alpha + inv_alpha * background.G) >> 8) & 0xFF);
                        byte newB = (byte)(((foreground.B * alpha + inv_alpha * background.B) >> 8) & 0xFF);

                        DrawPoint((uint)Color.ToArgb(newR, newG, newB), X + w, Y + h);
                    }
                    else DrawPoint((uint)image.RawData[(uint)(image.Width * h + w)], X + w, Y + h);
        }

        /* Functions from Cosmos */

        public virtual void DrawHorizontalLine(uint Color, int DX, int X1, int Y1)
        {
            for (int i = 0; i < DX; i++)
                DrawPoint(Color, X1 + i, Y1);
        }

        public virtual void DrawVerticalLine(uint Color, int DY, int X1, int Y1)
        {
            for (int i = 0; i < DY; i++)
                DrawPoint(Color, X1, Y1 + i);
        }

        public virtual void DrawDiagonalLine(uint Color, int DX, int DY, int X1, int Y1)
        {
            int dxabs = Math.Abs(DX), dyabs = Math.Abs(DY);
            int sdx = Math.Sign(DX), sdy = Math.Sign(DY);
            int x = dyabs >> 1, y = dxabs >> 1;
            int px = X1, py = Y1;

            if (dxabs >= dyabs) /* the line is more horizontal than vertical */
            {
                for (int i = 0; i < dxabs; i++)
                {
                    y += dyabs;
                    if (y >= dxabs)
                    {
                        y -= dxabs;
                        py += sdy;
                    }
                    px += sdx;
                    DrawPoint(Color, px, py);
                }
            }
            else /* the line is more vertical than horizontal */
            {
                for (int i = 0; i < dyabs; i++)
                {
                    x += dxabs;
                    if (x >= dyabs)
                    {
                        x -= dyabs;
                        px += sdx;
                    }
                    py += sdy;
                    DrawPoint(Color, px, py);
                }
            }
        }

        public virtual void DrawLine(uint Color, int X1, int Y1, int X2, int Y2)
        {
            // trim the given line to fit inside the graphics boundries
            TrimLine(X1, Y1, X2, Y2);

            int dx = X2 - X1;      /* the horizontal distance of the line */
            int dy = Y2 - Y1;      /* the vertical distance of the line */

            if (dy == 0) /* The line is horizontal */
            {
                DrawHorizontalLine(Color, dx, X1, Y1);
                return;
            }

            if (dx == 0) /* the line is vertical */
            {
                DrawVerticalLine(Color, dy, X1, Y1);
                return;
            }

            /* the line is neither horizontal neither vertical, it's diagonal then! */
            DrawDiagonalLine(Color, dx, dy, X1, Y1);
        }

        public virtual void DrawCircle(uint Color, int XC, int YC, int RA)
        {
            int x = RA, y = 0, e = 0;
            while (x >= y)
            {
                DrawPoint(Color, XC + x, YC + y);
                DrawPoint(Color, XC + y, YC + x);
                DrawPoint(Color, XC - y, YC + x);
                DrawPoint(Color, XC - x, YC + y);
                DrawPoint(Color, XC - x, YC - y);
                DrawPoint(Color, XC - y, YC - x);
                DrawPoint(Color, XC + y, YC - x);
                DrawPoint(Color, XC + x, YC - y);

                y++;

                if (e <= 0)
                    e += 2 * y + 1;

                if (e > 0)
                {
                    x--;
                    e -= 2 * x + 1;
                }
            }
        }

        public virtual void DrawFilledCircle(uint Color, int X0, int Y0, int RA)
        {
            int x = RA, y = 0, xChange = 1 - (RA << 1), yChange = 0, radiusError = 0;

            while (x >= y)
            {
                for (int i = X0 - x; i <= X0 + x; i++)
                {
                    DrawPoint(Color, i, Y0 + y);
                    DrawPoint(Color, i, Y0 - y);
                }

                for (int i = X0 - y; i <= X0 + y; i++)
                {
                    DrawPoint(Color, i, Y0 + x);
                    DrawPoint(Color, i, Y0 - x);
                }

                y++;
                radiusError += yChange;
                yChange += 2;

                if (((radiusError << 1) + xChange) > 0)
                {
                    x--;
                    radiusError += xChange;
                    xChange += 2;
                }
            }
        }

        public virtual void DrawEllipse(uint Color, int XC, int YC, int XR, int YR)
        {
            int a = 2 * XR, b = 2 * YR;
            int b1 = b & 1;
            int dx = 4 * (1 - a) * b * b, dy = 4 * (b1 + 1) * a * a;
            int err = dx + dy + b1 * a * a, e2;
            int y = 0, x = XR;

            a *= 8 * a;
            b1 = 8 * b * b;

            while (x >= 0)
            {
                DrawPoint(Color, XC + x, YC + y);
                DrawPoint(Color, XC - x, YC + y);
                DrawPoint(Color, XC - x, YC - y);
                DrawPoint(Color, XC + x, YC - y);

                e2 = 2 * err;

                if (e2 <= dy) { y++; err += dy += a; }
                if (e2 >= dx || 2 * err > dy) { x--; err += dx += b1; }
            }
        }

        public virtual void DrawFilledEllipse(uint Color, int X, int Y, int Width, int Height)
        {
            for (int y = -Height; y <= Height; y++)
                for (int x = -Width; x <= Width; x++)
                    if (x * x * Height * Height + y * y * Width * Width <= Height * Height * Width * Width)
                        DrawPoint(Color, X + x, Y + y);
        }

        public virtual void DrawPolygon(uint Color, params Point[] Points)
        {
            if (Points.Length < 3)
                throw new ArgumentException("A polygon requires more than 3 points.");

            for (int i = 0; i < Points.Length - 1; i++)
            {
                Point point1 = Points[i];
                Point point2 = Points[i + 1];

                DrawLine(Color, point1.X, point1.Y, point2.X, point2.Y);
            }

            Point point3 = Points[0];
            Point point4 = Points[Points.Length - 1];

            DrawLine(Color, point3.X, point3.Y, point4.X, point4.Y);
        }

        public virtual void DrawSquare(uint Color, int X, int Y, int Size)
        {
            DrawRectangle(Color, X, Y, Size, Size, 1);
        }

        public virtual void DrawTriangle(uint Color, int V1x, int V1y, int V2x, int V2y, int V3x, int V3y)
        {
            DrawLine(Color, V1x, V1y, V2x, V2y);
            DrawLine(Color, V1x, V1y, V3x, V3y);
            DrawLine(Color, V2x, V2y, V3x, V3y);
        }

        public virtual void TrimLine(int x1, int y1, int x2, int y2)
        {
            // in case of vertical lines, no need to perform complex operations
            if (x1 == x2)
            {
                x1 = Math.Min(Width - 1, Math.Max(0, x1));
                x2 = x1;
                y1 = Math.Min(Height - 1, Math.Max(0, y1));
                y2 = Math.Min(Height - 1, Math.Max(0, y2));

                return;
            }

            // never attempt to remove this part,
            // if we didn't calculate our new values as floats, we would end up with inaccurate output
            float x1_out = x1, y1_out = y1;
            float x2_out = x2, y2_out = y2;

            // calculate the line slope, and the entercepted part of the y axis
            float m = (y2_out - y1_out) / (x2_out - x1_out);
            float c = y1_out - m * x1_out;

            // handle x1
            if (x1_out < 0)
            {
                x1_out = 0;
                y1_out = c;
            }
            else if (x1_out >= Width)
            {
                x1_out = Width - 1;
                y1_out = (Width - 1) * m + c;
            }

            // handle x2
            if (x2_out < 0)
            {
                x2_out = 0;
                y2_out = c;
            }
            else if (x2_out >= Width)
            {
                x2_out = Width - 1;
                y2_out = (Width - 1) * m + c;
            }

            // handle y1
            if (y1_out < 0)
            {
                x1_out = -c / m;
                y1_out = 0;
            }
            else if (y1_out >= Height)
            {
                x1_out = (Height - 1 - c) / m;
                y1_out = Height - 1;
            }

            // handle y2
            if (y2_out < 0)
            {
                x2_out = -c / m;
                y2_out = 0;
            }
            else if (y2_out >= Height)
            {
                x2_out = (Height - 1 - c) / m;
                y2_out = Height - 1;
            }

            // final check, to avoid lines that are totally outside bounds
            if (x1_out < 0 || x1_out >= Width || y1_out < 0 || y1_out >= Height)
            {
                x1_out = 0; x2_out = 0;
                y1_out = 0; y2_out = 0;
            }

            if (x2_out < 0 || x2_out >= Width || y2_out < 0 || y2_out >= Height)
            {
                x1_out = 0; x2_out = 0;
                y1_out = 0; y2_out = 0;
            }

            // replace inputs with new values
            x1 = (int)x1_out; y1 = (int)y1_out;
            x2 = (int)x2_out; y2 = (int)y2_out;
        }
    }
}

