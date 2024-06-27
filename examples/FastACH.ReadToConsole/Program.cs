using FastACH;
using System;


var achFile = await AchFile.Read("ACH.txt");

achFile.WriteToConsole();

Console.ReadLine();

