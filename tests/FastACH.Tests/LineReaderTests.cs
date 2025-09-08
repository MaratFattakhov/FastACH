using FluentAssertions;

namespace FastACH.Tests
{
    public class LineReaderTests
    {
        private static string Pad(string s) => s.PadRight(94, ' ');

        [Fact]
        public void Ctor_InvalidLength_Throws()
        {
            // too short
            Action act = () => new LineReader("abc".AsSpan());
            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid record length: Expected 94, Actual 3.*");
        }

        [Fact]
        public void Ctor_Tab_Throws()
        {
            var data = Pad("abc\tdef");
            Action act = () => new LineReader(data.AsSpan());
            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid tab at position *");
        }

        [Fact]
        public void Ctor_HighAscii_Throws()
        {
            var chars = new char[94];
            for (int i = 0; i < chars.Length; i++) chars[i] = 'A';
            chars[10] = (char)129; // > 128
            Action act = () => new LineReader(chars);
            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid character found at position 10: *");
        }

        [Fact]
        public void ReadString_Trims()
        {
            var data = Pad("Hello   ");
            var reader = new LineReader(data.AsSpan());
            reader.ReadString(8).Should().Be("Hello");
        }

        [Fact]
        public void Skip_Skips()
        {
            var data = Pad("ABCDEFGH");
            var reader = new LineReader(data.AsSpan());
            reader.Skip(3);
            reader.ReadString(2).Should().Be("DE");
        }

        [Fact]
        public void ReadDate_Success()
        {
            var prefix = "250101"; // yyMMdd
            var data = Pad(prefix + new string(' ', 94 - prefix.Length));
            var reader = new LineReader(data.AsSpan());
            reader.ReadDate(optional:false)!.Value.Should().Be(new DateOnly(2025, 01, 01));
        }

        [Fact]
        public void ReadDate_Invalid_Throws_When_NotOptional()
        {
            var prefix = "25AB01"; // invalid
            var data = Pad(prefix + new string(' ', 94 - prefix.Length));
            Action act = () =>
            {
                var reader = new LineReader(data.AsSpan());
                reader.ReadDate(optional:false);
            };
            act.Should().Throw<ArgumentException>()
                .WithMessage("Error reading date at 0 of 6 length. Expected format: YYMMDD.");
        }

        [Fact]
        public void ReadDate_Invalid_ReturnsNull_WhenOptional()
        {
            var prefix = "25AB01"; // invalid
            var data = Pad(prefix + new string(' ', 94 - prefix.Length));
            var reader = new LineReader(data.AsSpan());
            reader.ReadDate(optional:true).Should().BeNull();
        }

        [Fact]
        public void ReadTime_Success()
        {
            var prefix = "1234"; // HHmm
            var data = Pad(prefix + new string(' ', 94 - prefix.Length));
            var reader = new LineReader(data.AsSpan());
            reader.ReadTime()!.Value.Should().Be(new TimeOnly(12,34));
        }

        [Fact]
        public void ReadTime_Invalid_ReturnsNull()
        {
            var prefix = "12A4"; // invalid
            var data = Pad(prefix + new string(' ', 94 - prefix.Length));
            var reader = new LineReader(data.AsSpan());
            reader.ReadTime().Should().BeNull();
        }

        [Fact]
        public void ReadUInt_Success()
        {
            var prefix = "123";
            var data = Pad(prefix + new string(' ', 94 - prefix.Length));
            var reader = new LineReader(data.AsSpan());
            reader.ReadUInt(3).Should().Be(123u);
        }

        [Fact]
        public void ReadUInt_Invalid_Throws()
        {
            var prefix = "12A";
            var data = Pad(prefix + new string(' ', 94 - prefix.Length));
            Action act = () =>
            {
                var reader = new LineReader(data.AsSpan());
                reader.ReadUInt(3);
            };
            act.Should().Throw<ArgumentException>()
                .WithMessage("Error reading numeric at 0 of 3 length. Read value: 12A.");
        }

        [Fact]
        public void ReadULong_Success()
        {
            var prefix = "123456";
            var data = Pad(prefix + new string(' ', 94 - prefix.Length));
            var reader = new LineReader(data.AsSpan());
            reader.ReadULong(6).Should().Be(123456ul);
        }

        [Fact]
        public void ReadULong_Invalid_Throws()
        {
            var prefix = "123A56";
            var data = Pad(prefix + new string(' ', 94 - prefix.Length));
            Action act = () =>
            {
                var reader = new LineReader(data.AsSpan());
                reader.ReadULong(6);
            };
            act.Should().Throw<ArgumentException>()
                .WithMessage("Error reading numeric at 0 of 6 length. Read value: 123A56.");
        }

        [Fact]
        public void ReadDecimal_Success()
        {
            var prefix = "123.45"; // decimal allowed
            var rest = new string(' ', 94 - prefix.Length);
            var data = prefix + rest; // length 94
            var reader = new LineReader(data.AsSpan());
            reader.ReadDecimal(prefix.Length).Should().Be(123.45m);
        }

        [Fact]
        public void ReadDecimal_Invalid_Throws()
        {
            var prefix = "123A45";
            var data = Pad(prefix + new string(' ', 94 - prefix.Length));
            Action act = () =>
            {
                var reader = new LineReader(data.AsSpan());
                reader.ReadDecimal(prefix.Length);
            };
            act.Should().Throw<ArgumentException>()
                .WithMessage("Error reading numeric at 0 of 6 length. Read value: 123A45.");
        }

        [Fact]
        public void ReadChar_ReturnsChar()
        {
            var prefix = "X";
            var data = Pad(prefix + new string(' ', 94 - prefix.Length));
            var reader = new LineReader(data.AsSpan());
            reader.ReadChar().Should().Be('X');
        }

        [Fact]
        public void SequentialReads_AdvancePosition()
        {
            var prefix = "AB12"; // will read A, B then 12
            var data = Pad(prefix + new string(' ', 94 - prefix.Length));
            var reader = new LineReader(data.AsSpan());
            reader.ReadChar().Should().Be('A');
            reader.ReadChar().Should().Be('B');
            reader.ReadUInt(2).Should().Be(12u);
        }
    }
}
