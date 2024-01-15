// See https://aka.ms/new-console-template for more information
using ChoETL.NACHA;

uint i = 1;
ChoNACHAConfiguration config = new ChoNACHAConfiguration();
config.DestinationBankRoutingNumber = "123456789";
config.OriginatingCompanyId = "123456789";
config.DestinationBankName = "PNC Bank";
config.OriginatingCompanyName = "Microsoft Inc.";
config.ReferenceCode = "Internal Use Only.";
config.BlockingFactor = 10;
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
        using (var entry1 = bw1.CreateDebitEntryDetail(22, "123456789", "1313131313", 22.505M, "ID Number", "ID Name", "Desc Data"))
        {
            entry1.CreateAddendaRecord("Monthly bill");
        }
        using (var entry2 = bw1.CreateCreditEntryDetail(27, "123456789", "1313131313", 22.505M, "ID Number", "ID Name", "Desc Data"))
        {

        }
    }
    using (var bw2 = nachaWriter.CreateBatch(200))
    {
    }
}

Console.WriteLine($"File generated: {filePath}.");
