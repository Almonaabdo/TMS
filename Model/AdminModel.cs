using System;
using System.IO;
using Devart.Data.MySql;

namespace TMS_Project.Model;

public class AdminModel
{
    /// <summary>
    /// Method to perform database back up operation
    /// </summary>
    public void BackUpDatabase()
    {
        // Specify path for storing backups
        var backUpFolder = Path.Combine(Environment.CurrentDirectory, "Backup");

        // Get current date for creating backup files
        var currentDate = DateTime.Now.ToString("yyyy MMMM dd");
        var fileName = $"backup_{currentDate}.sql";
        var filePath = Path.Combine(backUpFolder, fileName);
        try
        {
            // Hardcoded connection string for now
            var connectionString = "Server=localhost;Database=tms;User=root;Password=root;";

            // Create backup folder if it doesnt exist
            Directory.CreateDirectory(backUpFolder);

            // Set up db connection and backup command
            using var connection = new MySqlConnection(connectionString);
            using var cmd = connection.CreateCommand();
            using var backup = new MySqlBackup(cmd);

            connection.Open(); // Open connection to db

            cmd.Connection = connection; //Assign connection to command

            backup.ExportToFile(filePath); //Export contents of db to filepath

            connection.Close(); // Close connecting

            LoggerModel.LogInfo(
                $"Backup operation was completed successfully by Admin. File saved to {filePath}"); // Log successfull operation
        }
        catch (Exception e)
        {
            Console.WriteLine($"Admin model: {e.Message}");
            LoggerModel.LogError($"Error: {e.Message}");

            // Optional: Print detailed information about the exception
            Console.WriteLine(e.StackTrace);
        }
    }
}