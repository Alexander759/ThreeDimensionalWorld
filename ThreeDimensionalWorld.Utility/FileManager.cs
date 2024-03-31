using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalWorld.Utility
{
    public static class FileManager
    {
        public static async Task<string> UploadFileAsync(IFormFile file, string pathToCopy)
        {
            // Generate a unique name using Guid
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // Combine the path to copy and the unique file name
            string fullPath = Path.Combine(pathToCopy, uniqueFileName);

            // Create the directory if it doesn't exist
            if (!Directory.Exists(pathToCopy))
            {
                Directory.CreateDirectory(pathToCopy);
            }

            // Copy the file to the specified path with the unique name
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the new name of the file
            return uniqueFileName;
        }

        public static async Task DeleteFileAsync(string filePath)
        {
            try
            {
                // Check if the file exists
                if (System.IO.File.Exists(filePath))
                {
                    // Delete the file asynchronously
                    await Task.Run(() => System.IO.File.Delete(filePath));
                    Console.WriteLine($"File at path '{filePath}' deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"File at path '{filePath}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while deleting file at path '{filePath}': {ex.Message}");
            }
        }

        public static string CopyFile(string sourceFilePath, string destinationFolderPath)
        {
            // Extracting the file name from the source file path
            string fileName = Path.GetFileName(sourceFilePath);

            // Generate a unique file name using Guid
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);

            // Combine the destination folder path with the unique file name to get the full destination path
            string destinationFilePath = Path.Combine(destinationFolderPath, uniqueFileName);

            // Check if the destination folder exists, create it if it doesn't
            if (!Directory.Exists(destinationFolderPath))
            {
                Directory.CreateDirectory(destinationFolderPath);
            }

            // Copy the file to the destination folder
            File.Copy(sourceFilePath, destinationFilePath, true);

            // Return the new file name
            return uniqueFileName;
        }
    }
}
