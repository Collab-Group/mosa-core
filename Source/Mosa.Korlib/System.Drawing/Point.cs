namespace System.Drawing
{
    public class Point
    {
        public int X, Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            Point point = (Point)obj;
            return X == point.X && Y == point.Y;
        }

        public override int GetHashCode()
        {
            return unchecked(X.GetHashCode() + Y.GetHashCode());
        }

        public override string ToString()
        {
            return "X:" + X + " Y:" + Y;
        }
    }
}
