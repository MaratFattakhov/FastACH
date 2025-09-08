using FluentAssertions;

namespace FastACH.Tests
{
    public class TransactionCodesTests
    {
        [Theory]
        [InlineData(22)]
        [InlineData(32)]
        [InlineData(42)]
        [InlineData(52)]
        [InlineData(23)]
        [InlineData(33)]
        [InlineData(43)]
        [InlineData(53)]
        [InlineData(48)]
        public void IsCredit_ReturnsTrue_ForCreditCodes(uint code)
        {
            TransactionCodes.IsCredit(code).Should().BeTrue();
            TransactionCodes.IsDebit(code).Should().BeFalse();
        }

        [Theory]
        [InlineData(27)]
        [InlineData(37)]
        [InlineData(47)]
        [InlineData(55)]
        [InlineData(28)]
        [InlineData(38)]
        public void IsDebit_ReturnsTrue_ForDebitCodes(uint code)
        {
            TransactionCodes.IsDebit(code).Should().BeTrue();
            TransactionCodes.IsCredit(code).Should().BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99)]
        [InlineData(11)]
        [InlineData(21)]
        [InlineData(54)]
        public void UnknownCode_ReturnsFalse_ForBoth(uint code)
        {
            TransactionCodes.IsCredit(code).Should().BeFalse();
            TransactionCodes.IsDebit(code).Should().BeFalse();
        }

        [Fact]
        public void CreditAndDebitCodes_DoNotOverlap()
        {
            var overlap = TransactionCodes.CreditCodes.Intersect(TransactionCodes.DebitCodes).ToList();
            overlap.Should().BeEmpty();
        }

        [Fact]
        public void CreditCodes_HaveNoDuplicates()
        {
            TransactionCodes.CreditCodes.Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void DebitCodes_HaveNoDuplicates()
        {
            TransactionCodes.DebitCodes.Should().OnlyHaveUniqueItems();
        }
    }
}
