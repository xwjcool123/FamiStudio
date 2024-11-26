using System;
using System.Diagnostics;

// This is basically the handful of simple structs we still used from System.Drawing
// and is done in preparation for an eventual migration to more modern .NET versions.
namespace FamiStudio
{
    // Unlike the System.Drawing.Color, the internal encoding is AABBGGRR as opposed
    // to AARRGGBB. This is done in order to match our OpenGL color packing and avoid
    // a bunch of conversions.
    public struct Color
    {
        private int color; // 0xAABBGGRR

        public static readonly Color Empty = default(Color);
                                        
        public static Color White       => new Color(255, 255, 255);
        public static Color Black       => new Color(0, 0, 0);
        public static Color Azure       => new Color(240, 255, 255);
        public static Color Transparent => new Color(0, 0, 0, 0);
        public static Color SpringGreen => new Color(0, 255, 127);
        public static Color Pink        => new Color(255, 192, 203);

        public byte A
        {
            get => (byte)((color >> 24) & 0xff);
            set => color = (color & (int)~0xff000000) | (value << 24);
        }

        public byte B
        {
            get => (byte)((color >> 16) & 0xff);
            set => color = (color & ~0xff0000) | (value << 16);
        }

        public byte G
        {
            get => (byte)((color >> 8) & 0xff);
            set => color = (color & ~0xff00) | (value << 8);
        }

        public byte R
        {
            get => (byte)((color >> 0) & 0xff);
            set => color = (color & ~0xff) | value;
        }

        public Color(int packed)
        {
            color = packed;
        }

        public Color(int r, int g, int b, int a = 255)
        {
            Debug.Assert(r >= 0 && r <= 255 && g >= 0 && g <= 255 && b >= 0 && b <= 255 && a >= 0 && a <= 255);
            color = (a << 24) | (b << 16) | (g << 8) | r;
        }

        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color(r, g, b, a);
        }

        public static Color FromArgb(int r, int g, int b)
        {
            return new Color(r, g, b);
        }

        public static Color FromArgb(int a, Color c)
        {
            return new Color((a << 24) | (c.ToAbgr() & 0xffffff));
        }

        public static Color FromArgb(float a, Color c)
        {
            return new Color(((int)(a * 255) << 24) | (c.ToAbgr() & 0xffffff));
        }

        public static Color FromArgb(int argb)
        {
            return new Color(
                (argb >> 16) & 0xff, 
                (argb >>  8) & 0xff, 
                (argb >>  0) & 0xff,
                (argb >> 24) & 0xff);
        }

        public int ToArgb()
        {
            return (A << 24) | (R << 16) | (G << 8) | B;
        }

        public int ToAbgr()
        {
            return color;
        }

        public Color Scaled(float scale)
        {
            return new Color((int)(R * scale), (int)(G * scale), (int)(B * scale), A);
        }

        public static bool operator ==(Color left, Color right)
        {
            return left.color == right.color;
        }

        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Color c)
            {
                return this.color == c.color;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return color;
        }

        public string ToHexString()
        {
            return $"{R:x2}{G:x2}{B:x2}";
        }

        public static Color FromHexString(string str)
        {
            int parsed = Convert.ToInt32(str, 16);
            return new Color((parsed >> 16) & 0xff, (parsed >> 8) & 0xff, (parsed >> 0) & 0xff);
        }

        public override string ToString()
        {
            return $"R={R} G={G} B={B} A={A}";
        }
    }

    public struct Point
    {
        public static readonly Point Empty;

        private int x;
        private int y;

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator +(Point pt, Size sz)
        {
            return new Point(pt.x + sz.Width, pt.y + sz.Height);
        }
    }

    public struct PointF
    {
        public static readonly PointF Empty;

        private float x;
        private float y;

        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }

        public PointF(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct Size
    {
        public static readonly Size Empty;

        private int width;
        private int height;

        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }

        public Size(int w, int h)
        {
            width  = w;
            height = h;
        }
    }

    public struct Rectangle
    {
        public static readonly Rectangle Empty;

        private int x;
        private int y;
        private int width;
        private int height;

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }

        public int Left => x;
        public int Top => y;
        public int Right => x + width;
        public int Bottom => y + height;
        public bool IsEmpty => height == 0 && width == 0 && x == 0 && y == 0;

        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Rectangle(Point p, Size s)
        {
            x = p.X;
            y = p.Y;
            width  = s.Width;
            height = s.Height;
        }

        public Size Size
        {
            get
            {
                return new Size(width, height);
            }
            set
            {
                width  = value.Width;
                height = value.Height;
            }
        }

        public bool Contains(int px, int py)
        {
            return px >= x && px < x + width && 
                   py >= y && py < y + height;
        }

        public bool Contains(Point p)
        {
            return p.X >= x && p.X < x + width &&
                   p.Y >= y && p.Y < y + height;
        }

        public void Offset(int x, int y)
        {
            this.x += x;
            this.y += y;
        }

        public Rectangle Resized(int sx, int sy)
        {
            return new Rectangle(x, y, width + sx, height + sy);
        }

        public static Rectangle Union(Rectangle a, Rectangle b)
        {
            int minX = Math.Min(a.X, b.X);
            int maxX = Math.Max(a.X + a.Width, b.X + b.Width);
            int minY = Math.Min(a.Y, b.Y);
            int maxY = Math.Max(a.Y + a.Height, b.Y + b.Height);
            
            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            int maxX = Math.Max(a.X, b.X);
            int minX = Math.Min(a.X + a.Width, b.X + b.Width);
            int maxY = Math.Max(a.Y, b.Y);
            int minY = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (minX >= maxX && minY >= maxY)
            {
                return new Rectangle(maxX, maxY, minX - maxX, minY - maxY);
            }

            return Empty;
        }
    }


    public struct RectangleF
    {
        public static readonly RectangleF Empty;

        private float x;
        private float y;
        private float width;
        private float height;

        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
        public float Width { get => width; set => width = value; }
        public float Height { get => height; set => height = value; }

        public float Left => x;
        public float Top => y;
        public float Right => x + width;
        public float Bottom => y + height;
        public bool IsEmpty => height == 0 && width == 0 && x == 0 && y == 0;

        public RectangleF(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public static RectangleF Intersect(RectangleF a, RectangleF b)
        {
            float maxX = Math.Max(a.X, b.X);
            float minX = Math.Min(a.X + a.Width, b.X + b.Width);
            float maxY = Math.Max(a.Y, b.Y);
            float minY = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (minX >= maxX && minY >= maxY)
            {
                return new RectangleF(maxX, maxY, minX - maxX, minY - maxY);
            }

            return Empty;
        }
    }
}
