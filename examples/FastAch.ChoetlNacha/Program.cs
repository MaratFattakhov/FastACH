// See https://aka.ms/new-console-template for more information
using ChoETL.NACHA;

uint i = 1;
ChoNACHAConfiguration config = new ChoNACHAConfiguration();
config.DestinationBankName = "PNC Bank";
config.DestinationBankRoutingNumber = "123456789".PadLeft(10, ' ');
config.OriginatingCompanyId = "123456789".PadLeft(10, ' ');
config.OriginatingCompanyName = "Microsoft Inc.";
config.BatchNumber = 1;
config.BatchNumberGenerator = delegate () { return i++; };
config.BlockingFactor = 10;
config.TurnOffOriginatingCompanyIdValidation = true;
config.ReferenceCode = "00000000";
var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\ACH.txt");

using (var nachaWriter = new ChoNACHAWriter("..\\..\\..\\ACH.txt", config))
{
    using (var bw1 = nachaWriter.CreateBatch(200,
        effectiveEntryDate: new DateTime(2011, 01, 02),
        companyEntryDescription: "EntryDescr",
        companyDescriptiveDate: new DateTime(2011, 02, 03),
        companyDiscretionaryData: "companyDiscretionary",
        companyID: "companyID",
        companyName: "companyName",
        originatingDFIID: "DFINumber"))
    {
        using (var entry1 = bw1.CreateCreditEntryDetail(22, "123456789", "1313131313", 22.0M, "ID Number", "ID Name", "Desc Data"))
        {
            entry1.CreateAddendaRecord("Monthly bill", 5);
        }
        using (var entry2 = bw1.CreateDebitEntryDetail(27, "123456789", "1313131313", 27.0M, "ID Number", "ID Name", "Desc Data"))
        {

        }
    }
}

Console.WriteLine($"File generated: {filePath}.");
