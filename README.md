# ElfParser
Parse DWARF2 data from ELF files and print type and address for global variables (incl. arrays and structs). Built solely for my own intent as I couldn't find anything to extract structs from my ELF files. Feel free to file issues and submit PRs if you have any suggestions for improvement.

## Build environment
Built using Visual Studio 2019 with the excellent NuGet package [ELFSharp](https://github.com/konrad-kruczynski/elfsharp).

Make sure to change the `List<string> Type` to include your typedefs of interest. E.g `char`, `bool`, `int`, et.c if you only use basic types.
