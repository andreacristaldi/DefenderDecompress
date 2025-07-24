# DefenderDecompress
Tool to decompress Microsoft Defender AV signature databases

# Description
DefenderDecompress is a C# tool for extracting and decompressing .vdm files used by Microsoft Defender as part of its antivirus signature updates.
These files contain a signature database (RMDX) that, if compressed, can be extracted and saved in raw format for analysis or reverse engineering.

The tool scans for the RMDX header, checks if the content is compressed, and if so, decompresses it using a Deflate stream.

# Requirements
.NET Core or .NET Framework (compatible with C# 8.0+)

Windows OS (required to access Defender's update folders)

# Usage
DefenderDecompress <PathToFile.vdm> <OutputFileName>

Use RunMe.bat to automate the extracting of files (please raccord path in bat file)

# Example
DefenderDecompress "C:\ProgramData\Microsoft\Windows Defender\Definition Updates\mpasbase.vdm" mpasbase.vdm.decompressed


# Features
Validates if the file is a PE file (checks MZ header)
Detects RMDX signature database marker
Parses the header and checks compression flags
Decompresses content using Deflate if compression is detected
Saves output file in the current working directory

# Limitations
If the .vdm file is scrambled but not compressed, the tool will stop and notify the user. Additional reverse engineering is required in such cases.
Does not support corrupted or malformed .vdm files.

# Disclaimer
This tool is intended for educational and security analysis purposes only.
Improper use may violate Microsoft's license agreements. Use responsibly and only on systems you own or are authorized to analyze.

# Author
Project: Andrea Cristaldi <a href="https://www.linkedin.com/in/andreacristaldi/" target="blank_">Linkedin</a>, <a href="https://www.cybersec4.com" target="blank_">Cybersec4</a>

# License
This project is licensed under the MIT License.
