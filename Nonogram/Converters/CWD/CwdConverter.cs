using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nonogram.Lib.Converters.CWD
{
    public class CwdConverter
    {
        private readonly StringBuilder _content;
        private readonly NonogramModel _nonogramModel;

        public CwdConverter(NonogramModel nonogramModel)
        {
            if (nonogramModel.ColorsNumber > 1)
                throw new ArgumentException("Multicolor nonograms is not supported.");

            _nonogramModel = nonogramModel;
            _content = new StringBuilder();
        }

        public string GetContent()
        {
            _content.AppendLine(_nonogramModel.VerticalSize.ToString());
            _content.AppendLine(_nonogramModel.HorizontalSize.ToString());

            var horizontalBlock = FormatBlock(_nonogramModel.HorizontalNumbers);
            _content.Append(horizontalBlock);

            _content.AppendLine();

            var verticalBlock = FormatBlock(_nonogramModel.VerticalNumbers);
            _content.Append(verticalBlock);
            return _content.ToString();
        }

        private static string FormatBlock(List<int[]>[] numbers)
        {
            var result = new StringBuilder();
            foreach (var row in numbers)
            {
                var line = NumbersRowToString(row);
                result.AppendLine(line);
            }

            return result.ToString();
        }

        private static string NumbersRowToString(List<int[]> row)
        {
            var result = row.Aggregate("", (current, item) => current + $"{item[0]} ");
            return result.Trim();
        }
    }
}