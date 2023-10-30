using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaphyros.Core.Commands
{
    internal static partial class CommandHelper
    {
        private static void CommitPendingArgument(List<string> args, StringBuilder sb)
        {
            var arg = sb.ToString();
            if (!string.IsNullOrWhiteSpace(arg))
            {
                args.Add(arg);
            }
            sb.Clear();
        }
        public static List<string> ParseCommandLine(string input)
        {
            var args = new List<string>();
            if (string.IsNullOrWhiteSpace(input)) return args;

            var sb = new StringBuilder();
            bool inQuotes = false;
            int position = 0;

            while (position < input.Length)
            {
                var c = input[position];
                var l = position + 1 >= input.Length ? '\0' : input[position + 1];

                if (char.IsWhiteSpace(c))
                {
                    if (!inQuotes)
                        CommitPendingArgument(args, sb);
                    else
                        sb.Append(c);
                }
                else if (c == '\"')
                {
                    if (!inQuotes)
                        inQuotes = true;
                    else if (l == '\"')
                    {
                        sb.Append(c);
                        position++;
                    }
                    else
                        inQuotes = false;
                }
                else
                {
                    sb.Append(c);
                }

                position++;
            }

            CommitPendingArgument(args, sb);

            return args;
        }
    }
}
