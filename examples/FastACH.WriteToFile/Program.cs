using FastACH;

var achFile = await AchFile.Read("C:\\temp\\395194_PT073025-04ASUREGS.txt");
await achFile.WriteToFile("C:\\temp\\saved.txt", o => o.UpdateControlRecords = false);