using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FastACH
{
    internal ref struct LineReader
    {
        private readonly ReadOnlySpan<char> _data;
        private int _position = 0;
        public LineReader(ReadOnlySpan<char> data)
        {
            _data = data;
            ValidateData();
        }

        private void ValidateData()
        {
            if (_data.Length != 94)
            {
                throw new ArgumentException($"Invalid record length: Expected 94, Actual {_data.Length}.");
            }

            for (int i = 0; i < _data.Length; i++)
            {
                if (_data[i] == '\t')
                {
                    throw new ArgumentException($"Invalid tab at position {i}: {_data.ToString()}");
                }

                if (_data[i] >= 128)
                {
                    throw new ArgumentException($"Invalid character found at position {i}: {_data.ToString()}");
                }
            }
        }

        public LineReader(ReadOnlySpan<char> data, int skip) : this(data)
        {
            Skip(skip);
        }

        public void Skip(int count)
        {
            _position += count;
        }

        private ReadOnlySpan<char> Read(int length)
        {
            var value = _data.Slice(_position, length);
            _position += length;
            return value;
        }

        public string ReadString(int length)
        {
            return Read(length).Trim().ToString();
        }

        public DateOnly? ReadDate(bool optional)
        {
            const int length = 6;
            const string format = "yyMMdd";
            var fromPosition = _position;
            if (!DateOnly.TryParseExact(Read(length), format, out var date))
            {
                if (optional)
                    return null;
                else
                    throw new ArgumentException($"Error reading date at {fromPosition} of {length} length. Expected format: {format.ToUpper()}.");
            }
            return date;
        }

        public TimeOnly? ReadTime()
        {
            const int length = 4;
            const string format = "HHmm";
            if (!TimeOnly.TryParseExact(Read(length), format, out var time))
            {
                return null;
            }
            return time;
        }

        public uint ReadUInt(int length)
        {
            var value = _data.Slice(_position, length);
            if (!uint.TryParse(value, out var result))
            {
                throw new ArgumentException($"Error reading numeric at {_position} of {length} length. Read value: {value}.");
            }
            _position += length;
            return result;
        }

        public ulong ReadULong(int length)
        {
            var value = _data.Slice(_position, length);
            if (!ulong.TryParse(value, out var result))
            {
                throw new ArgumentException($"Error reading numeric at {_position} of {length} length. Read value: {value}.");
            }
            _position += length;
            return result;
        }

        public decimal ReadDecimal(int length)
        {
            var value = _data.Slice(_position, length);
            if (!decimal.TryParse(value, out var result))
            {
                throw new ArgumentException($"Error reading numeric at {_position} of {length} length. Read value: {value}.");
            }
            _position += length;
            return result;
        }

        public char ReadChar()
        {
            return Read(1)[0];
        }
    }
}
