using System;

namespace RobotsTxt
{
    internal class Line
    {
        public LineType Type { get; private set; }
        public string Raw { get; private set; }
        public string Field { get; private set; }
        public string Value { get; private set; }

        public Line(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                throw new ArgumentException("Can't create a new instance of Line class with an empty line.", "line");
            }

            Raw = line;
            var lineParameter = line.Trim();

            if (lineParameter.StartsWith("#"))
            {
                // whole line is comment
                Type = LineType.Comment;
                return;
            }

            // if line contains comments, get rid of them
            if (lineParameter.IndexOf('#') > 0)
            {
                lineParameter = lineParameter.Remove(lineParameter.IndexOf('#'));
            }

            string field = GetField(lineParameter);
            if (String.IsNullOrWhiteSpace(field))
            {
                // If could not find the first ':' char or if there wasn't a field declaration before ':'
                Type = LineType.Unknown;
                return;
            }

            Field = field.Trim();
            Type = EnumHelper.GetLineType(field.Trim().ToLowerInvariant());
            Value = lineParameter.Substring(field.Length + 1).Trim(); //we remove <field>:
        }

        private static string GetField(string line)
        {
            var index = line.IndexOf(':');
            if (index == -1)
            {
                return String.Empty;
            }

            return line.Substring(0, index);
        }
    }
}
