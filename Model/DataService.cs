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
        private readonly LoggerModel _loggerModel = LoggerModel.Instance;


        #endregion

        #region Create

     
        /*
        * METHOD NAME: CreateCarrier
        * DESCRIPTION: Creates a new carrier along with is information - depotCity, FTLA, LTLA, rates and reef charge
        *
        * RETURN: void
        */
        public void CreateCarrier(string name, string depotCity, int newFtla, int newLtla, double newFtlaRate, double newLtlaRate, double newReefCharge)
        {
            // Check if a carrier with the same name already exists
            var existingCarrier = _dbContext.Carriers?.FirstOrDefault(c => c.CompanyName == name);

            try
            {
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
            catch 
            { 

            }
           
        }

        #endregion

        #region Delete


        /*
        * METHOD NAME: DeleteData<T>
        * DESCRIPTION: Deletes data from the DB
        * 
        * RETURN: void
        */
        public void DeleteData<T>(int entityId) where T : class
        {
            try
            {
                // Find the entity by its ID
                var entityToDelete = _dbContext.Set<T>().Find(entityId);

                if (entityToDelete == null)
                {
                    // Handle the case where the entity with the given ID is not found
                    _loggerModel.LogException($"Entity with ID {entityId} not found.");
                    return;
                }

                // Remove the entity and save changes
                _dbContext.Set<T>().Remove(entityToDelete);
                _dbContext.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // Handle any exceptions that occur during the delete operation
                _loggerModel.LogException("Exception while deleting data from entity.");
            }
        }


        #endregion

        #region Backup

      
        /*
        * METHOD NAME: BackUpDatabase
        * DESCRIPTION: Performs a backup to the database
        * 
        * RETURN: void
        */
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

                 _loggerModel.LogInfo("hello this is working");
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

                _loggerModel.LogInfo("Backup operation was completed successfully by Admin.");  // Log successfully operation
            }
            catch (MySqlException)
            {
                _loggerModel.LogException($"Exception thrown while backing up database");
            }
        }

        #endregion

        #region Retrieve


        /*
        * METHOD NAME: RetrievableTable<T>
        * DESCRIPTION: Retrieves data from a table in the database.
        * 
        * RETURN: null
        */
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
                _loggerModel.LogException("Exception thrown while loading entity data.");
            }

            return null;
        }

        #endregion

        #region Update


        /*
        * METHOD NAME: SaveChanges
        * DESCRIPTION: Saves changes to the database for the updated entity.
        * 
        * RETURN: void
        */
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
                    _loggerModel.LogWarning($"{typeof(T).Name} not found. Unable to update.");
                    throw new InvalidOperationException();
                }
            }
            catch (DbUpdateException ex)
            {
                _loggerModel.LogException($"Exception saving changes to database. {ex.Message}");
            }
        }

        #endregion

        #region Query
        // Join query
        /*
        * METHOD NAME: GetJoinedRouteData
        * DESCRIPTION: Joins tables from the DB to get Route data
        * 
        * RETURN: joinedData
        */
        public List<JoinedRouteTable> GetJoinedRouteData()
        {
            try
            {
                // Query to retrieve joined route data
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

                return new List<JoinedRouteTable>(joinedData);
            }
            catch (Exception e)
            {
                // Log exception
                _loggerModel.LogException($"Exception while performing query. {e.Message}");
            }

            return new List<JoinedRouteTable>() ;
        }

        /*
        * METHOD NAME: GetCompletedOrders
        * DESCRIPTION: Joins tables from the DB to get all completed orders
        * 
        * RETURN: list of completedOrders
        */
        public List<JoinedOrder> GetCompletedOrders()
        {
            try
            {
                // Query to retrieve completed orders along with associated details
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
                                          DateInitiated = order.DateInitiated,
                                          Origin = citySource.CityName,
                                          Destination = cityDest.CityName,
                                          TripCost = trip.TripCost
                                      };

                return completedOrders.ToList();  // Return result as a list
            }
            catch (Exception e)
            {
                // Log exception
                _loggerModel.LogException($"Exception while performing query. {e.Message}");
            }

            // Return empty list, if theres an exception or no completed orders
            return new List<JoinedOrder>();
        }


        #endregion

        #region Helper Methods

 
        /*
        * METHOD NAME: GetKeyValues
        * DESCRIPTION: Gets the primary key values of an entity
        * 
        * RETURN: keyValues, null
        */
        private object[] GetKeyValues<T>(T entity) where T : class
        {
            try
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
            }
            catch
            {

            }

            return null!;
        }

        #endregion
    }
}
