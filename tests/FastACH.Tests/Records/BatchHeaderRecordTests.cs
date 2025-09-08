using FastACH.Records;
using FluentAssertions;

namespace FastACH.Tests.Records
{
    public class BatchHeaderRecordTests
    {
        [Theory]
        [InlineData(200, "My company", "Discretionary", "123456789", "PPD", "", "200102", "200102", "", '1', "12345678", 123)]
        public void ParseRecord(
            uint serviceClassCode,
            string companyName,
            string companyDiscretionaryData,
            string companyId,
            string standardEntryClassCode,
            string companyEntryDescription,
            string companyDescriptiveDate,
            string effectiveEntryDate,
            string julianSettlementDate,
            char originatorStatusCode,
            string originatingDFIID,
            uint batchNumber)
        {
            // Arrange
            var s = $"5{serviceClassCode}{companyName,-16}{companyDiscretionaryData,-20}{companyId,-10}{standardEntryClassCode}{companyEntryDescription,-10}{companyDescriptiveDate}{effectiveEntryDate}{julianSettlementDate,-3}{originatorStatusCode}{originatingDFIID,8}{batchNumber:0000000}";

            // Act
            var record = new BatchHeaderRecord(s);

            // Assert
            record.Should().BeEquivalentTo(new BatchHeaderRecord()
            {
                BatchNumber = batchNumber,
                CompanyDescriptiveDate = DateOnly.ParseExact(companyDescriptiveDate, "yyMMdd"),
                CompanyDiscretionaryData = companyDiscretionaryData,
                CompanyEntryDescription = companyEntryDescription,
                CompanyId = companyId,
                CompanyName = companyName,
                EffectiveEntryDate = DateOnly.ParseExact(effectiveEntryDate, "yyMMdd"),
                OriginatingDFIID = originatingDFIID,
                OriginatorsStatusCode = originatorStatusCode,
                ServiceClassCode = serviceClassCode,
                JulianSettlementDate = julianSettlementDate,
                StandardEntryClassCode = standardEntryClassCode
            });
        }
    }
}