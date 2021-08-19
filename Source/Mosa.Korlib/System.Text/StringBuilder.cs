namespace System.Text
{
    public class StringBuilder
    {
        // We could make it faster by using a char* and using GC.AllocateObject but Mosa.Korlib doesn't reference Mosa.Runtime
        private char[] Characters = new char[int.MaxValue];

        private int Length = 0;

        public StringBuilder Append(char c)
        {
            if (Length >= Characters.Length)
            {
                char[] chars = new char[Length + 100];
                Characters.CopyTo(chars, 0);
                Characters = chars;
            }

            Characters[Length] = c;
            Length++;

            return this;
        }

        public StringBuilder AppendLine(string s)
        {
            for (int i = 0; i < s.length; i++)
                Append(s[i]);

            return this;
        }

        public char[] ToCharArray()
        {
            return Characters;
        }

        public override string ToString()
        {
            return new string(Characters, 0, Length);
        }
    }
}
