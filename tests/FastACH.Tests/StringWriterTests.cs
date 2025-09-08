using FluentAssertions;
using FastStringWriter = FastACH.StringWriter;

namespace FastACH.Tests
{
    public class StringWriterTests
    {
        private static (FastStringWriter writer, System.IO.StringWriter backing) Create()
        {
            var backing = new System.IO.StringWriter();
            var fw = new FastStringWriter(backing);
            return (fw, backing);
        }

        [Fact]
        public void Write_String_Pads_When_Shorter()
        {
            var (w, backing) = Create();
            w.Write("ABC", 5);
            backing.ToString().Should().Be("ABC  ");
        }

        [Fact]
        public void Write_String_Truncates_When_Longer()
        {
            var (w, backing) = Create();
            w.Write("ABCDEFGHI", 5);
            backing.ToString().Should().Be("ABCDE");
        }

        [Fact]
        public void Write_String_Null_TreatedAsEmpty_AndPadded()
        {
            var (w, backing) = Create();
            w.Write(null!, 4);
            backing.ToString().Should().Be("    ");
        }

        [Fact]
        public void Write_Date_Formats_yyMMdd()
        {
            var (w, backing) = Create();
            w.Write(new DateOnly(2025, 09, 08));
            backing.ToString().Should().Be("250908");
        }

        [Fact]
        public void Write_Date_Null_Writes_Spaces()
        {
            var (w, backing) = Create();
            w.Write((DateOnly?)null);
            backing.ToString().Should().Be("      ");
        }

        [Fact]
        public void Write_Time_Formats_HHmm()
        {
            var (w, backing) = Create();
            w.Write(new TimeOnly(7, 5));
            backing.ToString().Should().Be("0705");
        }

        [Fact]
        public void Write_Time_Null_Writes_Spaces()
        {
            var (w, backing) = Create();
            w.Write((TimeOnly?)null);
            backing.ToString().Should().Be("    ");
        }

        [Fact]
        public void Write_ULong_PadsLeft_WithZeros()
        {
            var (w, backing) = Create();
            w.Write(123UL, 6);
            backing.ToString().Should().Be("000123");
        }

        [Fact]
        public void Write_ULong_ExactLength_Unchanged()
        {
            var (w, backing) = Create();
            w.Write(123456UL, 6);
            backing.ToString().Should().Be("123456");
        }

        [Fact]
        public void Write_ULong_TooLong_Throws()
        {
            var (w, _) = Create();
            Action act = () => w.Write(1234567UL, 6);
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Value 1234567 is too long for field length 6");
        }

        [Fact]
        public void MultipleWrites_AppendSequentially()
        {
            var (w, backing) = Create();
            w.Write("A", 3);      // A__
            w.Write(5UL, 2);      // 05
            w.Write(new TimeOnly(1,2)); // 0102
            backing.ToString().Should().Be("A  05" + "0102");
        }
    }
}
