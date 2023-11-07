using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

class Program
{
    static void Main(string[] args)
    {
        string rootFolderPath = ""; // Replace with the root folder path.
        List<string> zipFiles = GetZipFiles(rootFolderPath);
        UnzipAndDeleteFiles(zipFiles);
    }

    static List<string> GetZipFiles(string folderPath)
    {
        List<string> zipFiles = new List<string>();

        try
        {
            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                if (IsZipFile(file))
                {
                    zipFiles.Add(file);
                }
            }

            string[] subfolders = Directory.GetDirectories(folderPath);
            foreach (string subfolder in subfolders)
            {
                List<string> subfolderZipFiles = GetZipFiles(subfolder);
                zipFiles.AddRange(subfolderZipFiles);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return zipFiles;
    }

    static bool IsZipFile(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        return extension.Equals(".zip", StringComparison.OrdinalIgnoreCase);
    }

    static void UnzipAndDeleteFiles(List<string> zipFiles)
    {
        foreach (string zipFilePath in zipFiles)
        {
            UnzipFile(zipFilePath);

            try
            {
                File.Delete(zipFilePath);
                Console.WriteLine($"Unzipped and deleted {zipFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting {zipFilePath}: {ex.Message}");
            }
        }
    }

    static void UnzipFile(string zipFilePath)
    {
        string extractPath = Path.GetDirectoryName(zipFilePath);

        using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
        {    
            string folderName = Path.GetFileNameWithoutExtension(zipFilePath);

            
            string folderPath = Path.Combine(extractPath, folderName);
            Directory.CreateDirectory(folderPath);

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string entryPath = Path.Combine(folderPath, entry.FullName);

                if (entry.FullName.EndsWith("/"))
                {
                    
                    Directory.CreateDirectory(entryPath);
                }

                else
                {
                    try
                    {
                        
                        entry.ExtractToFile(entryPath, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
        }
    }
}
