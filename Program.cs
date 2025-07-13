// Cybersec4.com DefenderDecompress
// Author: Andrea Cristaldi 2025 - https://github.com/andreacristaldi

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

class DefenderDecompress
{
    static void Main(string[] args)
    {
        Console.WriteLine("Cybersec4.com DefenderDecompress\n");

        if (args.Length != 2)
        {
            Console.WriteLine("Usage: DefenderDecompress <FilePath> <OutputFileName>");
            return;
        }

        string filePath = args[0];
        string outputFileName = args[1];

        if (!File.Exists(filePath) || !filePath.EndsWith(".vdm", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Error: {filePath} does not exist or is not a valid .vdm file.");
            return;
        }

        try
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Check for valid PE header
            if (fileBytes.Length < 2 || fileBytes[0] != 'M' || fileBytes[1] != 'Z')
            {
                Console.WriteLine($"{filePath} is not a valid PE file.");
                return;
            }

            // Search for RMDX signature in binary
            byte[] signature = Encoding.ASCII.GetBytes("RMDX");
            int headerIndex = SearchBytes(fileBytes, signature);

            if (headerIndex == -1)
            {
                Console.WriteLine("Defender AV signature database header (\"RMDX\") not found.");
                return;
            }

            int headerSize = 0x40;
            if (fileBytes.Length < headerIndex + headerSize)
            {
                Console.WriteLine("Incomplete RMDX header.");
                return;
            }

            byte[] headerBytes = fileBytes.Skip(headerIndex).Take(headerSize).ToArray();

            int options = BitConverter.ToInt32(headerBytes, 0x0C);
            int maybeChecksum = BitConverter.ToInt32(headerBytes, 0x1C);
            int lastFieldUnknown = BitConverter.ToInt32(headerBytes, 0x3C);

            bool isCompressed = ((options >> 1) & 0x1) == 1;

            if (!isCompressed)
            {
                Console.WriteLine("The signature database is scrambled, not compressed. Unable to continue.");
                return;
            }

            int compressedDataInfoOffset = BitConverter.ToInt32(headerBytes, 0x18);

            if (((options & 0x200000) == 0) || (maybeChecksum == 0) || (lastFieldUnknown == 0))
            {
                Console.WriteLine("Invalid Defender AV signature database header.");
                return;
            }

            int compressedDataLength = BitConverter.ToInt32(fileBytes, headerIndex + compressedDataInfoOffset);
            int compressedDataIndex = headerIndex + compressedDataInfoOffset + 8;

            if (fileBytes.Length < compressedDataIndex + compressedDataLength)
            {
                Console.WriteLine("Compressed data is incomplete or corrupted.");
                return;
            }

            byte[] compressedData = fileBytes.Skip(compressedDataIndex).Take(compressedDataLength).ToArray();
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), outputFileName);

            using (MemoryStream memoryStream = new MemoryStream(compressedData))
            using (FileStream outputStream = File.Create(outputPath))
            using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
            {
                deflateStream.CopyTo(outputStream);
            }

            Console.WriteLine($"File successfully decompressed to: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
        }
    }

    // Utility function to search for a byte sequence
    static int SearchBytes(byte[] body, byte[] pattern)
    {
        for (int i = 0; i <= body.Length - pattern.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (body[i + j] != pattern[j])
                {
                    match = false;
                    break;
                }
            }
            if (match) return i;
        }
        return -1;
    }
}
