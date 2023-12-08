using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Devart.Data.MySql;
using Microsoft.EntityFrameworkCore;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model
{
    public class DataService
    {
        #region Fields

        private readonly TmsDbContext _dbContext = DbContextSingleton.Instance;

        #endregion

        #region Create

        /// <summary>
        /// Creates a new carrier if it doesn't already exist.
        /// </summary>
        public void CreateCarrier(string name, string depotCity, int newFtla, int newLtla, double newFtlaRate, double newLtlaRate, double newReefCharge)
        {
            // Check if a carrier with the same name already exists
            var existingCarrier = _dbContext?.Carriers?.FirstOrDefault(c => c.CompanyName == name);

            if (existingCarrier != null)
            {
                throw new Exception("Carrier already exists.");
            }

            // Create a new carrier if it doesn't already exist
            var newCarrier = new Carrier
            {
                CompanyName = name,
                DepotCity = depotCity,
                Ftla = newFtla,
                Ltla = newLtla,
                FtlRate = newFtlaRate,
                LtlRate = newLtlaRate,
                ReefCharge = newReefCharge
            };

            _dbContext?.Carriers?.Add(newCarrier);
            _dbContext?.SaveChanges();
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes data from the database.
        /// </summary>
        /// <typeparam name="T">Type of entity to delete.</typeparam>
        public void DeleteData<T>(T? entityToDelete) where T : class
        {
            try
            {
                if (entityToDelete == null) return;
                _dbContext.Set<T>().Remove(entityToDelete);
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                LoggerModel.LogException("Exception while deleting data from entity.");
            }
        }

        #endregion

        #region Backup
        /// <summary>
        /// Performs a backup of the database.
        /// </summary>
        /// <param name="connectionString">Connection string to the database.</param>
        /// <returns>True if the backup operation is successful, otherwise false.</returns>
        public bool BackUpDatabase(string connectionString)
        {
            try
            {
                // Specify path for storing backups
                var backUpFolder = Path.Combine(Environment.CurrentDirectory, "Backup");

                // Get current date for creating backup files
                var currentDate = DateTime.Now.ToString("yyyy MMMM dd");
                var fileName = $"backup_{currentDate}.sql";
                var filePath = Path.Combine(backUpFolder, fileName);

                // Hardcoded connection string for now
                //connectionString = "Server=localhost;Database=tms;User=root;Password=root;";

                // Create backup folder if it doesnt exist
                Directory.CreateDirectory(backUpFolder);

                // Set up db connection and backup command
                using var connection = new MySqlConnection(connectionString);
                using var cmd = connection.CreateCommand();
                using var backup = new MySqlBackup(cmd);

                connection.Open(); // Open connection to db

                cmd.Connection = connection; // Assign connection to command

                backup.ExportToFile(filePath); // Export contents of db to filepath

                connection.Close();    // Close connecting

                LoggerModel.LogInfo("Backup operation was completed successfully by Admin.");  // Log successfully operation

                return true;
            }
            catch (MySqlException e)
            {
                LoggerModel.LogException($"Exception thrown while backing up database");
                return false;
            }
        }

        #endregion

        #region Retrieve

        /// <summary>
        /// Retrieves data from a table in the database.
        /// </summary>
        /// <typeparam name="T">Type of entity to retrieve.</typeparam>
        /// <returns>List of retrieved data.</returns>
        public List<T>? RetrieveTable<T>() where T : class
        {
            try
            {
                if (_dbContext?.Set<T>() != null) return _dbContext.Set<T>().ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                LoggerModel.LogException("Exception thrown while loading entity data.");
            }

            return null;
        }

        #endregion

        #region Update

        /// <summary>
        /// Saves changes to the database for the updated entity.
        /// </summary>
        /// <typeparam name="T">Type of entity to update.</typeparam>
        /// <param name="updatedData">Updated data entity.</param>
        public void SaveChanges<T>(T updatedData) where T : class
        {
            try
            {
                var existingEntity = _dbContext?.Set<T>().Find(GetKeyValues(updatedData));
                if (existingEntity != null)
                {
                    _dbContext?.Entry(existingEntity).CurrentValues.SetValues(updatedData);
                    _dbContext?.SaveChanges(); // Save changes
                }
                else
                {
                    LoggerModel.LogWarning($"{typeof(T).Name} not found. Unable to update.");
                    throw new InvalidOperationException();
                }
            }
            catch (DbUpdateException ex)
            {
                LoggerModel.LogException($"Exception saving changes to database. {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the primary key values of an entity.
        /// </summary>
        /// <typeparam name="T">Type of entity.</typeparam>
        /// <param name="entity">Entity for which to get primary key values.</param>
        /// <returns>Array of primary key values.</returns>
        private object[] GetKeyValues<T>(T entity) where T : class
        {
            // Obtain an EntityEntry instance for the provided entity
            var entry = _dbContext?.Entry(entity);

            // Retrieve the primary key information for the entity
            var primaryKey = entry?.Metadata.FindPrimaryKey();

            // Get the properties that make up the primary key
            var primaryKeyProperties = primaryKey?.Properties;

            // Map each property to its current value in the provided entity
            if (primaryKeyProperties != null)
            {
                var keyValues = primaryKeyProperties
                    .Select(property => entry?.Property(property.Name).CurrentValue)
                    .ToArray();

                // Return the array of primary key values
                return keyValues!;
            }

            return null!;
        }

        #endregion
    }
}
