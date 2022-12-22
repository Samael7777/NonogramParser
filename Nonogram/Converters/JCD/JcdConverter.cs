using System.Collections.Generic;
using System.Linq;
using Nonogram.Lib.Container;

namespace Nonogram.Lib.Converters.JCD
{
    public class JcdConverter
    {
        private readonly BinaryContainer _contents;

        private readonly byte[] _footer =
        {
            0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
            0x01, 0x01, 0x05, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x8F, 0x92, 0xB8, 0x00, 0x02, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x80, 0x80, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF7, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x41, 0x72, 0x69, 0x61, 0x6C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00, 0xC6, 0xCD, 0xE8, 0x00, 0xA9, 0xB9,
            0xE2, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x6A, 0x6A, 0x00, 0x47, 0x54,
            0x96, 0x00, 0xFF, 0xFF, 0xFF, 0x00, 0x01
        };

        private readonly NonogramModel _nonogramModel;
        private readonly Palette _palette;

        public JcdConverter(NonogramModel nonogramModel)
        {
            _nonogramModel = nonogramModel;
            _contents = new BinaryContainer(512);
            _palette = new Palette();
            BuildPalette();
            CreateContents();
        }

        public byte[] Contents => _contents.Bytes;

        private void BuildPalette()
        {
            var colors = _nonogramModel.ColorsNumber;
            if (colors == 1) return; //Monochrome nonogram

            for (var i = 0; i < colors; i++)
            {
                var colorData = _nonogramModel.Colors[i];
                var color = new Color
                {
                    R = (byte)colorData[0],
                    G = (byte)colorData[1],
                    B = (byte)colorData[2]
                };
                _palette[i] = color;
            }
        }

        private void CreateContents()
        {
            AddHeader();
            AddNumbers();
            AddNumbersInfo();
            AddPalette();
            AddFooter();
            _contents.Trim();

            //For debug   
            //using (var file = File.Open("contents", FileMode.Create))
            //{
            //    file.Write(_contents.Bytes, 0, _contents.Length);
            //    file.Close();
            //}
        }

        private void AddHeader()
        {
            _contents.Append(5);
            _contents.Append(0);
            _contents.Append(_nonogramModel.MaxHorizontalNumbers);
            _contents.Append(_nonogramModel.MaxVerticalNumbers);
            _contents.Append(_nonogramModel.HorizontalSize);
            _contents.Append(_nonogramModel.VerticalSize);
        }

        private void AddNumbers()
        {
            var horizontalNumbers = NumbersToContainer(_nonogramModel.HorizontalNumbers);
            var verticalNumbers = NumbersToContainer(_nonogramModel.VerticalNumbers);
            _contents.Append(horizontalNumbers.Bytes);
            _contents.Append(verticalNumbers.Bytes);
        }

        private void AddNumbersInfo()
        {
            _contents.Append(0);
            _contents.Append(0);
            _contents.Append(0);
            _contents.Append(0);
            _contents.Append(0);

            var horizontalNumbersCount = GetNumbersCount(_nonogramModel.HorizontalNumbers);
            var verticalNumbersCount = GetNumbersCount(_nonogramModel.VerticalNumbers);
            var block = new byte[horizontalNumbersCount + verticalNumbersCount];

            _contents.Append(block);
        }

        private void AddPalette()
        {
            _contents.Append(_palette.GetBytes());
        }

        private void AddFooter()
        {
            _contents.Append(_footer);
        }

        private static BinaryContainer NumbersToContainer(List<int[]>[] numbers)
        {
            var content = new BinaryContainer();
            foreach (var line in numbers)
            {
                content.Append((short)line.Count);
                foreach (var number in line)
                {
                    content.Append(number[0]);
                    content.Append((byte)(number[1] - 1));
                }
            }

            content.Trim();
            return content;
        }

        private static int GetNumbersCount(List<int[]>[] numbers)
        {
            return numbers.Sum(line => line.Count);
        }
    }
}