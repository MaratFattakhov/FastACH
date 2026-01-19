using FastACH.Builders;
using FluentAssertions;

namespace FastACH.Tests
{
    public class AchFileExtensionsTests
    {
        [Theory]
        [InlineData(100, 100, true)]
        [InlineData(0, 100, false)]
        [InlineData(100, 0, false)]
        [InlineData(0, 0, true)]
        public void IsBalanced_With_Debit_And_Credit_Transactions(decimal debitAmount, decimal creditAmount, bool expected)
        {
            // Arrange
            var achFile = new AchFileBuilder()
                .WithBatch(batch => batch
                    .WithCreditTransaction(creditAmount, "123", "123")
                    .WithDebitTransaction(debitAmount, "123", "123"))
                .Build();

            // Act
            var actual = achFile.IsBalanced();

            // Assert
            actual.Should().Be(expected);
        }
    }
}
