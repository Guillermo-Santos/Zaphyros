using Zaphyros.Core.IO.Text;

namespace Zaphyros.Core.Commands;

public sealed record CommandInfo(string Name, string Description, CommandType Type)
{
    public override string ToString()
    {
        return new IndentedStringBuilder('\t', 1)
              .Append(Name)
              .AppendLine(":")
              .Indent()
              .AppendLine(Description)
              .ToString();
    }
};
