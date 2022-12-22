using System;
using System.Collections.Generic;

namespace Nonogram.Lib
{
    public class NonogramModel
    {
        private readonly int[][] _rawData;

        public NonogramModel(int[][] rawData)
        {
            var rows = rawData.Length;
            var columns = rawData[0].Length;
            if (columns != 4 || rows < 8)
                throw new ArgumentException("Incorrect data format.");

            _rawData = rawData;
            CalculateModel();
        }

        /// <summary>
        ///     Данные кроссворда [x,y], в точке - индекс цвета
        /// </summary>
        public int[][] Field { get; private set; }

        /// <summary>
        ///     Данные по горизонтальным линиям [BlockSize, ColorIndex]
        /// </summary>
        public List<int[]>[] HorizontalNumbers { get; private set; }

        /// <summary>
        ///     Данные по вертикальным линиям [BlockSize, ColorIndex]
        /// </summary>
        public List<int[]>[] VerticalNumbers { get; private set; }

        /// <summary>
        ///     Строка = R, G, B, флаг инверсии цвета шрифта (1 - инверсия, 0 - обычный)
        /// </summary>
        public int[][] Colors { get; private set; }

        public int Id { get; private set; }
        public int HorizontalSize { get; private set; }
        public int VerticalSize { get; private set; }
        public int ColorsNumber { get; private set; }

        /// <summary>
        ///     Максимальное количество чисел в описании линии
        /// </summary>
        public int MaxHorizontalNumbers { get; private set; }

        /// <summary>
        ///     Максимальное количество чисел в описании линии
        /// </summary>
        public int MaxVerticalNumbers { get; private set; }

        private void CalculateModel()
        {
            CalculateMetrics();
            InitializeModel();
            ParseColors();
            ParseModel();
            CalculateHorizontalNumbers();
            CalculateVerticalNumbers();
        }

        private void CalculateMetrics()
        {
            Id = CalculateId();
            HorizontalSize = CalculateMetric(_rawData[1]);
            VerticalSize = CalculateMetric(_rawData[2]);
            ColorsNumber = CalculateMetric(_rawData[3]);
        }

        private static int CalculateMetric(IList<int> data)
        {
            var divisor = data[3];
            var value = data[0] % divisor + data[1] % divisor - data[2] % divisor;
            return value;
        }

        private int CalculateId()
        {
            var idRow = _rawData[0];
            var divisor = idRow[3];
            var id = idRow[0] % divisor * (idRow[0] % divisor) + idRow[1] % divisor * 2 + idRow[2] % divisor;
            return id;
        }

        private void ParseColors()
        {
            const int colorsOffset = 5;
            var colorDataOffsets = _rawData[4];
            Colors = new int[ColorsNumber][];

            for (var i = 0; i < ColorsNumber; i++)
            {
                var colorRow = _rawData[i + colorsOffset];
                var r = colorRow[0] - colorDataOffsets[1];
                var g = colorRow[1] - colorDataOffsets[0];
                var b = colorRow[2] - colorDataOffsets[3];
                var foregroundInvert = colorRow[3] - r - colorDataOffsets[2];
                Colors[i] = new[] { r, g, b, foregroundInvert };
            }
        }

        private void InitializeModel()
        {
            Field = new int[VerticalSize][];
            for (var i = 0; i < VerticalSize; i++) Field[i] = new int [HorizontalSize];
        }

        private void ParseModel()
        {
            CheckModel();

            var modelHeaderOffset = ColorsNumber + 5;
            var headerRow = _rawData[modelHeaderOffset];
            var divisor = headerRow[3];
            var totalHorizontalFilledItems = headerRow[0] % divisor * (headerRow[0] % divisor)
                                             + headerRow[1] % divisor * 2
                                             + headerRow[2] % divisor;
            var offsets = _rawData[modelHeaderOffset + 1];
            var dataOffset = modelHeaderOffset + 2;
            for (var i = 0; i < totalHorizontalFilledItems; i++)
            {
                var row = _rawData[dataOffset + i];
                var filledRegionStart = row[0] - offsets[0] - 1;
                var filledRegionLen = row[1] - offsets[1];
                for (var j = 0; j < filledRegionLen; j++)
                {
                    var verticalIndex = row[3] - offsets[3] - 1;
                    var filledRegionColorIndex = row[2] - offsets[2];
                    Field[verticalIndex][filledRegionStart + j] = filledRegionColorIndex;
                }
            }
        }

        private void CalculateHorizontalNumbers()
        {
            CheckModel();

            HorizontalNumbers = new List<int[]>[VerticalSize];
            MaxHorizontalNumbers = 0;
            for (var i = 0; i < VerticalSize; i++)
            {
                HorizontalNumbers[i] = new List<int[]>();
                for (var regionEnd = 0; regionEnd < HorizontalSize;)
                {
                    var regionStart = regionEnd;
                    var cellColor = Field[i][regionEnd];

                    while (regionEnd < HorizontalSize && Field[i][regionEnd] == cellColor) regionEnd++;

                    if (regionEnd - regionStart <= 0 || cellColor == 0) continue;

                    var regionSize = regionEnd - regionStart;
                    var item = new[] { regionSize, cellColor };
                    HorizontalNumbers[i].Add(item);
                }

                if (MaxHorizontalNumbers < HorizontalNumbers[i].Count)
                    MaxHorizontalNumbers = HorizontalNumbers[i].Count;
            }
        }

        private void CalculateVerticalNumbers()
        {
            CheckModel();

            VerticalNumbers = new List<int[]>[HorizontalSize];
            MaxVerticalNumbers = 0;
            for (var i = 0; i < HorizontalSize; i++)
            {
                VerticalNumbers[i] = new List<int[]>();
                for (var regionEnd = 0; regionEnd < VerticalSize;)
                {
                    var regionStart = regionEnd;
                    var cellColor = Field[regionEnd][i];

                    while (regionEnd < VerticalSize && Field[regionEnd][i] == cellColor) regionEnd++;

                    if (regionEnd - regionStart <= 0 || cellColor == 0) continue;

                    var regionSize = regionEnd - regionStart;
                    var item = new[] { regionSize, cellColor };
                    VerticalNumbers[i].Add(item);
                }

                if (MaxVerticalNumbers < VerticalNumbers[i].Count)
                    MaxVerticalNumbers = VerticalNumbers[i].Count;
            }
        }

        private void CheckModel()
        {
            if (Field is null)
                throw new ApplicationException("Nonogram model calculation error.");
        }
    }
}