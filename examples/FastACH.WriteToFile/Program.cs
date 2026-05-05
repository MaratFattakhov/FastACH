using FastACH;
using System;

string name = Environment.GetCommandLineArgs()[1];

var achFile = await AchFile.Read(name);
await achFile.WriteToFile("C:\\temp\\saved.txt", o => o.UpdateControlRecords = false);