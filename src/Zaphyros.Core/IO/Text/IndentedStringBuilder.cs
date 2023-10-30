using System.Text;

namespace Zaphyros.Core.IO.Text
{
    public class IndentedStringBuilder
    {
        // StringBuilder is sealed, so it can not be inherited
        private readonly StringBuilder sb;
        private readonly char indentChar;
        private readonly int indentSize;
        private string _indent = string.Empty;
        private int _lineSize = 0;

        public int Width { get; set; } = Console.WindowWidth;

        // IndentedStringBuilder => StringBuilder
        public static implicit operator StringBuilder(IndentedStringBuilder isb) => isb.sb;
        // StringBuilder => IndentedStringBuilder
        public static explicit operator IndentedStringBuilder(StringBuilder sb) => new(sb);

        public IndentedStringBuilder(StringBuilder sb, char indentChar, int indentSize)
        {
            this.sb = sb;
            this.indentChar = indentChar;
            this.indentSize = indentSize;
        }
        public IndentedStringBuilder(char indentChar, int indentSize) : this(new(), indentChar, indentSize) { }
        public IndentedStringBuilder(StringBuilder sb) : this(sb, '\t', 1) { }
        public IndentedStringBuilder() : this(new()) { }

        public IndentedStringBuilder Indent()
        {
            _indent += new string(indentChar, indentSize);
            return this;
        }

        public IndentedStringBuilder Clear()
        {
            sb.Clear();
            return this;
        }

        public StringBuilder.ChunkEnumerator GetChunk() => sb.GetChunks();

        public IndentedStringBuilder Append(string str)
        {
            if (_lineSize > 0)
            {
                sb.Append(str);
                _lineSize += str.Length;
            }
            else
            {
                sb.Append(_indent).Append(str);
                _lineSize += _indent.Length + str.Length;
            }

            return this;
        }
        public IndentedStringBuilder AppendLine(string str)
        {
            if (_lineSize == 0 && str.Length + _indent.Length < Width)
            {
                sb.Append(_indent).AppendLine(str + string.Empty);
                return this;
            }

            var firstWord = true;
            string[] words = str.Split(' ');

            foreach (string word in words)
            {
                if (_lineSize + word.Length + 1 > Width)
                {
                    sb.AppendLine();
                    _lineSize = 0;
                    firstWord = true;
                }

                if (!firstWord)
                {
                    Append(" ");
                }
                Append(word);
                firstWord = false;
            }

            if (_lineSize > 0)
            {
                sb.AppendLine();
            }

            _lineSize = 0;
            return this;
        }
        public IndentedStringBuilder Unindent()
        {
            if (_indent.Length >= indentSize)
                _indent = new string(indentChar, _indent.Length - indentSize);
            else
                _indent = string.Empty;
            return this;
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
