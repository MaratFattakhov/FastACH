using FastACH;
using System;

string name = Environment.GetCommandLineArgs()[1];

var achFile = await AchFile.Read(name);

achFile.WriteToConsole();

Console.WriteLine();
Console.WriteLine($"is balanced: {achFile.IsBalanced()}");

Console.ReadLine();

