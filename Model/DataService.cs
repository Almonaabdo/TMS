using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Devart.Data.MySql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TMS_Project.DataLayer.Context;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model
{
    public class DataService
    {
        #region Fields

        private readonly TmsDbContext _dbContext = DbContextSingleton.Instance;
        private int _backupId = 1;

        #endregion

        #region Create

        /// <summary>
        /// Creates a new carrier if it doesn't already exist.
        /// </summary>
        public void CreateCarrier(string name, string depotCity, int newFtla, int newLtla, double newFtlaRate, double newLtlaRate, double newReefCharge)
        {
            // Check if a carrier with the same name already exists
            var existingCarrier = _dbContext.Carriers?.FirstOrDefault(c => c.CompanyName == name);

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
               // FtlRate = newFtlaRate,
              //  LtlRate = newLtlaRate,
                ReefCharge = newReefCharge
            };

            _dbContext.Carriers?.Add(newCarrier);
            _dbContext.SaveChanges();
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes data from the database.
        /// </summary>
        /// <typeparam name="T">Type of entity to delete.</typeparam>
        public void DeleteData<T>(int entityId) where T : class
        {
            try
            {
                // Find the entity by its ID
                var entityToDelete = _dbContext.Set<T>().Find(entityId);

                if (entityToDelete == null)
                {
                    // Handle the case where the entity with the given ID is not found
                    LoggerModel.LogException($"Entity with ID {entityId} not found.");
                    return;
                }

                // Remove the entity and save changes
                _dbContext.Set<T>().Remove(entityToDelete);
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // Handle any exceptions that occur during the delete operation
                LoggerModel.LogException("Exception while deleting data from entity.");
            }
        }


        #endregion

        #region Backup

        /// <summary>
        /// Performs a backup of the database.
        /// </summary>
        /// <returns>True if the backup operation is successful, otherwise false.</returns>
        public void BackUpDatabase()
        {
            try
            {
                // Specify path for storing backups
                IConfigurationRoot backUpFolder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
                 var path = backUpFolder["Backups:BackupFolder"];
                 if (!Directory.Exists(path))
                 {
                     Console.WriteLine("Does not exist");
                 }
                
                 LoggerModel.LogInfo("hello this is working");
                // Get current date for creating backup files
                
                _backupId++;
                var currentDate = DateTime.Now.ToString("yyyy MMMM dd");
                var fileName = $"backup {currentDate} ID {_backupId}.sql";

                
                if (path != null)
                {
                    var filePath = Path.Combine(path, fileName);

                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json")
                        .Build();

                    var connectionString = configuration.GetConnectionString("RemoteDB");

                    // Create backup folder if it doesnt exist
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException());

                    // Set up db connection and backup command
                    using var connection = new MySqlConnection(connectionString);
                    using var cmd = connection.CreateCommand();
                    using var backup = new MySqlBackup(cmd);

                    connection.Open(); // Open connection to db

                    cmd.Connection = connection; // Assign connection to command

                    backup.ExportToFile(filePath); // Export contents of db to filepath

                    connection.Close();    // Close connecting
                }

                LoggerModel.LogInfo("Backup operation was completed successfully by Admin.");  // Log successfully operation
            }
            catch (MySqlException)
            {
                LoggerModel.LogException($"Exception thrown while backing up database");
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
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (_dbContext.Set<T>() != null) return _dbContext.Set<T>().ToList();
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

        #region Query
        // Join query
        public IQueryable<JoinedRouteTable> GetJoinedRouteData()
        {
            var joinedData = from route in _dbContext.Routes
                join sourceCity in _dbContext.Cities on route.SourceCityId equals sourceCity.CityId
                join destCity in _dbContext.Cities on route.DestinationCityId equals destCity.CityId
                select new JoinedRouteTable
                {
                    RouteId = route.RouteId,
                    Distance = route.Distance,
                    Duration = route.Duration,
                    Origin = sourceCity.CityName,
                    Destination = destCity.CityName
                };

            return joinedData;
        }

        public List<JoinedOrder> GetCompletedOrders()
        {
            var completedOrders = from order in _dbContext.Orders
                join citySource in _dbContext.Cities on order.SourceCityId equals citySource.CityId
                join cityDest in _dbContext.Cities on order.DestinationCityId equals cityDest.CityId
                join trip in _dbContext.Trips on order.OrderId equals trip.OrderId
                where order.OrderStatus == OrderStatus.Completed
                select new JoinedOrder
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    OrderStatus = order.OrderStatus,
                    DateCompleted = order.DateCompleted ?? DateTime.MinValue,
                    DateInititated = order.DateInitiated,
                    Origin = citySource.CityName,
                    Destination = cityDest.CityName,
                    TripCost = trip.TripCost
                };

            return completedOrders.ToList();
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
