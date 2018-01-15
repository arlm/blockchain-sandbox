using System.Reflection;
using System.Runtime.CompilerServices;
using CommandLine;

[assembly: InternalsVisibleTo("BlockChain.Test")]
//[assembly: AssemblyTitle("μChain CLI")]
[assembly: AssemblyCopyright("Copyright (C) 2014 Alexandre Rocha Lima e Marcondes")]

[assembly: AssemblyLicense(
    "                                                                             ",
    "This is free software. You may redistribute copies of it under the terms of",
    "the MIT License <https://github.com/arlm/blockchain-sandbox/LICENSE.md>.",
    "                                                                             ")]
[assembly: AssemblyUsage(
    "Usage: dotnet BlockChain.CLI.dll help",
    "       dotnet BlockChain.CLI.dll blockchain show",
    "       dotnet BlockChain.CLI.dll bitcoin mine",
    "                                                                             ")]
