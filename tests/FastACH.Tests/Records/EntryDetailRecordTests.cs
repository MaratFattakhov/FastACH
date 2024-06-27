//using FastACH.Records;
//using FluentAssertions;

//namespace FastACH.Tests.Records
//{
//    public class EntryDetailRecordTests
//    {
//        [Theory]
//        [InlineData(22, 21234567, '3', "123456789", 22.55, "ID Number", "Serrano", "13", false, 923456780000001)]
//        public void ParseRecord(
//            uint transactionCode,
//            ulong receivingDFINumber,
//            char checkDigit,
//            string DFIAccountNumber,
//            decimal amount,
//            string receiverIdentificationNumber,
//            string receiverName,
//            string discretionaryData,
//            bool addendaRecordIndicator,
//            ulong traceNumber)
//        {
//            // Arrange
//            var s = $"6{transactionCode,2}{receivingDFINumber,8}{checkDigit}{DFIAccountNumber,-17}{(uint)(amount * 100):0000000000}{receiverIdentificationNumber,-15}{receiverName,-22}{discretionaryData,2}{(addendaRecordIndicator ? "1" : "0"),1}{traceNumber:000000000000000}";

//            // Act
//            var record = new EntryDetailRecord(s);

//            // Assert
//            record.Should().BeEquivalentTo(new EntryDetailRecord()
//            {
//                TransactionCode = transactionCode,
//                AddendaRecordIndicator = addendaRecordIndicator,
//                Amount = amount,
//                CheckDigit = checkDigit,
//                DFIAccountNumber = DFIAccountNumber,
//                DiscretionaryData = discretionaryData,
//                ReceiverIdentificationNumber = receiverIdentificationNumber,
//                ReceiverName = receiverName,
//                ReceivingDFIID = receivingDFINumber,
//                TraceNumber = traceNumber.ToString(),
//            });
//        }

//        [Theory]
//        [InlineData(123456, '7', "001234567")]
//        [InlineData(123, '0', "000001230")]
//        public void FullReceivingDFIID_Set_Test(ulong receivingDFINumber, char checkDigit, string fullReceivingDFIID)
//        {
//            // Arrange
//            var target = new EntryDetailRecord()
//            {
//                TransactionCode = 22,
//                AddendaRecordIndicator = false,
//                Amount = 0,
//                CheckDigit = checkDigit,
//                DFIAccountNumber = "123",
//                ReceiverName = "receiverName",
//                ReceivingDFIID = receivingDFINumber,
//            };

//            // Assert
//            target.FullReceivingDFIID.Should().Be(fullReceivingDFIID);
//        }

//        [Theory]
//        [InlineData(123456, '7', "001234567")]
//        [InlineData(123, '0', "000001230")]
//        public void FullReceivingDFIID_Get_Test(ulong receivingDFINumber, char checkDigit, string fullReceivingDFIID)
//        {
//            // Arrange
//            var target = new EntryDetailRecord()
//            {
//                TransactionCode = 22,
//                AddendaRecordIndicator = false,
//                Amount = 0,
//                DFIAccountNumber = "123",
//                ReceiverName = "receiverName",
//                FullReceivingDFIID = fullReceivingDFIID
//            };

//            // Assert
//            target.ReceivingDFIID.Should().Be(FullReceivingDFIID);
//            target.CheckDigit.Should().Be(FullReceivingDFIID);
//        }
//    }
//}