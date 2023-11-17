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
        // Set connection string
        const string connectionString = "server=localhost; port=3306; database=lab8; user=root; password=PHW#84#jeor;";
        // Path to the backup folder in the exe directory
        var backUpFolder = Path.Combine(Environment.CurrentDirectory, "Backup");

        // Get current date and time
        var currentDate = DateTime.Now.ToString("yyyy MMMM dd");

        // Construct backup file name with current date
        var fileName = $"backup_{currentDate}.sql";
        // Construct file path
        var filePath = Path.Combine(backUpFolder, fileName);

        try
        {
            // Create back up folder directory if it doesnt exist
            Directory.CreateDirectory(backUpFolder);

            // Establish connection to mySql database
            using var connection = new MySqlConnection();
            // Create mySql command
            using var cmd = new MySqlCommand();
            // Use backup library to perform backup operation
            using var backup = new MySqlBackup();
            
            // Assign mySql connection to command
            cmd.Connection = connection;
            // Open db connection
            connection.Open();
            // Export db to file
            backup.ExportToFile(filePath);
            // Close connection
            connection.Close();

            LoggerModel.LogInfo($"Backup operation was completed successfully by Admin. File saved to {filePath}");
        }
        catch (Exception e)
        {
            LoggerModel.LogError($"Error:{e.Message}");
        }


    }

}