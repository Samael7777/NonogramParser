using System;
using System.Text;

namespace Nonogram.Lib.Container
{
    public class BinaryContainer
    {
        private const int ResizeStep = 50;

        private byte[] _container;

        public BinaryContainer(int capacity)
        {
            _container = new byte[capacity];
            Length = 0;
        }

        public BinaryContainer() : this(150)
        { }

        public int Length { get; private set; }

        public int Capacity => _container.Length;

        public byte[] Bytes => _container;

        public void Append(byte value)
        {
            Append(new[] { value });
        }

        public void Append(int value)
        {
            var data = BitConverter.GetBytes(value);
            Append(data);
        }

        public void Append(short value)
        {
            var data = BitConverter.GetBytes(value);
            Append(data);
        }

        public void Append(string value)
        {
            var data = Encoding.UTF8.GetBytes(value);
            Append(data);
        }

        public void Append(byte[] value)
        {
            if ((long)(Length + value.Length) > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(value), "No free space left.");


            if (Length + value.Length >= Capacity)
            {
                var newCapacity = (long)(Length + value.Length + ResizeStep);
                if (newCapacity > int.MaxValue) newCapacity = int.MaxValue;
                Array.Resize(ref _container, (int)newCapacity);
            }

            Array.Copy(value, 0, _container, Length, value.Length);
            Length += value.Length;
        }

        public void Trim()
        {
            if (Length == Capacity) return;
            Array.Resize(ref _container, Length);
        }
    }
}