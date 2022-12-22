using System;

namespace Nonogram.Lib.Converters.JCD
{
    internal class Palette
    {
        private readonly byte[] _palette =
        {
            0x0, 0x0, 0x0, 0x0, 0x80, 0x80, 0x80, 0x0, 0xC0, 0xC0, 0xC0, 0x0, 0xB5, 0x0, 0x0, 0x0,
            0xFF, 0x0, 0x0, 0x0, 0x80, 0x80, 0x0, 0x0, 0xFF, 0xFF, 0x0, 0x0, 0x0, 0x80, 0x0, 0x0,
            0x0, 0xFF, 0x0, 0x0, 0x0, 0xB5, 0xB5, 0x0, 0x0, 0xFF, 0xFF, 0x0, 0x0, 0x0, 0xB5, 0x0,
            0x0, 0x0, 0xFF, 0x0, 0xB5, 0x0, 0xB5, 0x0, 0xFF, 0x0, 0xFF, 0x0, 0xFA, 0xF0, 0xF0, 0x0
        };

        public Color this[int index]
        {
            get
            {
                var offset = GetOffset(index);
                return GetColor(offset);
            }
            set
            {
                var offset = GetOffset(index);
                SetColor(offset, value);
            }
        }

        public Color Background
        {
            get
            {
                var offset = _palette.Length - 4;
                return GetColor(offset);
            }
            set
            {
                var offset = _palette.Length - 4;
                SetColor(offset, value);
            }
        }

        public byte[] GetBytes()
        {
            return _palette;
        }

        private Color GetColor(int offset)
        {
            return new Color
            {
                R = _palette[offset++],
                G = _palette[offset++],
                B = _palette[offset]
            };
        }

        private void SetColor(int offset, Color value)
        {
            _palette[offset++] = value.R;
            _palette[offset++] = value.G;
            _palette[offset] = value.B;
        }

        private int GetOffset(int index)
        {
            if (index < 0 || index > _palette.Length - 8)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be from 0 to 14");

            return index * 4;
        }
    }
}