using FastACH.Models;
using FluentAssertions;

namespace FastACH.Tests.Records
{
    public class FiveRecordTests
    {
        [Theory]
        [InlineData("200", "My company", "Discretionary", "123456789", "PPD", "", "", "200102", "", '1', "12345678", 123)]
        public void ParseRecord(
            string serviceClassCode,
            string companyName,
            string companyDiscretionaryData,
            string companyId,
            string standardEntryClassCode,
            string companyEntryDescription,
            string companyDescriptiveDate,
            string effectiveEntryDate,
            string julianSettlementDate,
            char originatorStatusCode,
            string originatorsDFINumber,
            uint batchNumber)
        {
            // Arrange
            var s = $"5{serviceClassCode}{companyName, -16}{companyDiscretionaryData, -20}{companyId, -10}{standardEntryClassCode}{companyEntryDescription, -10}{companyDescriptiveDate, -6}{effectiveEntryDate:yyMMdd}{julianSettlementDate, -3}{originatorStatusCode}{originatorsDFINumber, 8}{batchNumber:0000000}";
            var record = new FiveRecord();

            // Act
            record.ParseRecord(s);

            // Assert
            record.Should().BeEquivalentTo(new FiveRecord()
            {
                BatchNumber = batchNumber,
                CompanyDescriptiveDate = companyDescriptiveDate,
                CompanyDiscretionaryData = companyDiscretionaryData,
                CompanyEntryDescription = companyEntryDescription,
                CompanyIdentification = companyId,
                CompanyName = companyName,
                EffectiveEntryDate = DateOnly.ParseExact(effectiveEntryDate, "yyMMdd"),
                OriginatorsDFINumber = originatorsDFINumber,
                OriginatorsStatusCode = originatorStatusCode,
                ServiceClassCode = serviceClassCode,
                JulianSettlementDate = julianSettlementDate,
                StandardEntryClassCode = standardEntryClassCode
            });
        }
    }
}