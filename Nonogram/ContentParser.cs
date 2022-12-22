using System;
using System.Text.RegularExpressions;

namespace Nonogram.Lib
{
    public class ContentParser
    {
        private readonly string _content;

        public ContentParser(string content)
        {
            _content = content.Replace(" ", "");
        }

        public int[][] GetNonogramData()
        {
            var rawData = FindPattern();
            return ParseRawNonogramString(rawData);
        }

        private string FindPattern()
        {
            const string pattern = @"\[(\[\d+,\d+,\d+,\d+\](,|]))+\;";
            const RegexOptions options = RegexOptions.Multiline;

            var matches = Regex.Matches(_content, pattern, options);

            if (matches.Count == 0)
                throw new ApplicationException("Parsing error. Pattern not found.");

            if (matches.Count > 1)
                throw new ApplicationException("Parsing error. Found too many pattern matching");

            return matches[0].Value;
        }

        private static int[][] ParseRawNonogramString(string rawNonogramScript)
        {
            const string pattern = @"\[\d+,\d+,\d+,\d+\]";
            var rows = Regex.Matches(rawNonogramScript, pattern);
            if (rows.Count == 0)
                throw new ApplicationException("Nonogram raw data not found.");

            var result = new int[rows.Count][];

            for (var i = 0; i < rows.Count; i++)
            {
                var cols = rows[i].Value.TrimStart('[').TrimEnd(']').Split(',');
                result[i] = new int[cols.Length];
                for (var j = 0; j < cols.Length; j++) result[i][j] = int.Parse(cols[j]);
            }

            return result;
        }
    }
}