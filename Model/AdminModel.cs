using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Devart.Data.MySql;
using Microsoft.Win32;
using NLog.Fluent;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model;

public class AdminModel
{

    /// <summary>
    /// Method to perform database back up operation
    /// </summary>
    /// 
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

            connection.Close();    // Close connecting

            LoggerModel.LogInfo($"Backup operation was completed successfully by Admin. File saved to {filePath}");  // Log successfull operation
        }
        catch (Exception e)
        {
            Console.WriteLine($"Admin model: {e.Message}");
            LoggerModel.LogError($"Error: {e.Message}");

            // Optional: Print detailed information about the exception
            Console.WriteLine(e.StackTrace);
        }
    }

   
    /// <summary>
    /// Method to retrieve Carrier data from table
    /// </summary>
    /// <returns>Carrier data in a list</returns>
    public List<Carrier>? LoadCarrierTable()
    {
        try
        {
            using (var dbContext = new TmsDbContext())
            {
                if (dbContext.Carriers != null) return dbContext.Carriers.ToList();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            LoggerModel.LogException("Error loading carrier data.");
        }

        return null;
    }

    /// <summary>
    /// Method to update carrier table to latest changes
    /// </summary>
    /// <param name="updatedCarrierData">The table to update</param>
    public void SaveChanges(List<Carrier> updatedCarrierData)
    {
        using var dbContext = new TmsDbContext();
        foreach (var updatedCarrier in updatedCarrierData)
        {
            var existingCarrier = dbContext.Carriers?.Find(updatedCarrier.CarrierId);
            if (existingCarrier != null)
            {
                dbContext.Entry(existingCarrier).CurrentValues.SetValues(updatedCarrier); // Replace current table values with
            }
            else
            {
                LoggerModel.LogWarning("Carrier not found. Unable to update.");  // Log if error
            }
        }
        dbContext.SaveChanges();  // Save changes to db
    }
}