using FastACH;
using System;

var reader = new AchFileReader();

var achFile = await reader.Read("ACH.txt");

var achFileWriter = new AchFileWriter();

achFileWriter.WriteToConsole(achFile);

Console.ReadLine();

