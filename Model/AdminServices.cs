using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Devart.Data.MySql;
using Microsoft.EntityFrameworkCore;
using TMS_Project.DataLayer.Model;

namespace TMS_Project.Model
{
    public class AdminServices
    {
        #region Fields

        private int _backupId = 1;
        private readonly ConfigService _configService = new();
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
            var existingCarrier = DbContextSingleton.Instance.Carriers?.FirstOrDefault(c => c.CompanyName == name);

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
                    // LtlRate = newLtlaRate,
                    ReefCharge = newReefCharge
                };

                DbContextSingleton.Instance.Carriers?.Add(newCarrier);
                DbContextSingleton.Instance.SaveChanges();
            }
            catch (Exception e)
            {
                LoggerModel.Instance.LogException($"{e.Message}")

            ;
            }
        }

        #endregion

        #region Delete

        /*
        * METHOD NAME: DeleteData
        * DESCRIPTION: Deletes data from the DB
        * 
        * RETURN: void
        */
        public void DeleteData<T>(int entityId) where T : class
        {
            try
            {
                // Find the entity by its ID
                var entityToDelete = DbContextSingleton.Instance.Set<T>().Find(entityId);

                if (entityToDelete == null)
                {
                    // Handle the case where the entity with the given ID is not found
                    LoggerModel.Instance.LogException($"Entity with ID {entityId} not found.");
                    return;
                }

                // Remove the entity and save changes
                DbContextSingleton.Instance.Set<T>().Remove(entityToDelete);
                DbContextSingleton.Instance.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // Handle any exceptions that occur during the delete operation
                LoggerModel.Instance.LogException("Exception while deleting data from entity.");
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

                var path = _configService.GetBackupPath();
                if (!Directory.Exists(path))
                {
                    LoggerModel.Instance.LogError($"{path} does not exist.");
                }

                LoggerModel.Instance.LogInfo("hello this is working");
                // Get current date for creating backup files

                _backupId++;
                var currentDate = DateTime.Now.ToString("yyyy MMMM dd");
                var fileName = $"backup {currentDate} ID {_backupId}.sql";

                if (path != null)
                {
                    var filePath = Path.Combine(path, fileName);


                    var connectionString = _configService.GetConnectionString();

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

                LoggerModel.Instance.LogInfo("Backup operation was completed successfully by Admin.");  // Log successfully operation
            }
            catch (MySqlException)
            {
                LoggerModel.Instance.LogException($"Exception thrown while backing up database");
            }
        }

        #endregion

        #region Retrieve

        /*
        * METHOD NAME: RetrievableTable
        * DESCRIPTION: Retrieves table data from the database.
        * RETURN: Returns the retrieved table as a list or empty list
        */
        public List<T> RetrieveTable<T>() where T : class
        {
            try
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

                // Check if the DbSet is not null before attempting to retrieve the data
                if (DbContextSingleton.Instance.Set<T>() != null) return DbContextSingleton.Instance.Set<T>().ToList();
            }
            catch (Exception e)
            {
                LoggerModel.Instance.LogException($"Exception thrown while loading entity data.{e.Message}");
            }

            return new List<T>(); // Return empty list if not found
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
                var existingEntity = DbContextSingleton.Instance.Set<T>().Find(GetKeyValues(updatedData));
                if (existingEntity != null)
                {
                    DbContextSingleton.Instance.Entry(existingEntity).CurrentValues.SetValues(updatedData);
                    DbContextSingleton.Instance.SaveChanges(); // Save changes
                }
                else
                {
                    LoggerModel.Instance.LogWarning($"{typeof(T).Name} not found. Unable to update.");
                    throw new InvalidOperationException();
                }
            }
            catch (DbUpdateException ex)
            {
                LoggerModel.Instance.LogException($"Exception saving changes to database. {ex.Message}");
            }
        }

        #endregion

        #region Query

        /*
        * METHOD NAME: GetJoinedRouteData
        * DESCRIPTION: Retrieves joined route data by combining information from the Routes, Cities
        * 
        * RETURN: A list containing details from tables 
        */
        public List<JoinedRouteTable> GetJoinedRouteData()
        {
            try
            {
                // Query to retrieve joined route data
                var joinedData = from route in DbContextSingleton.Instance.Routes
                                 join sourceCity in DbContextSingleton.Instance.Cities on route.SourceCityId equals sourceCity.CityId
                                 join destCity in DbContextSingleton.Instance.Cities on route.DestinationCityId equals destCity.CityId
                                 select new JoinedRouteTable
                                 {
                                     RouteId = route.RouteId,
                                     Distance = route.Distance,
                                     Duration = route.Duration,
                                     Origin = sourceCity.CityName,
                                     Destination = destCity.CityName
                                 };

                return new List<JoinedRouteTable>(joinedData); // Return the new list
            }
            catch (Exception e)
            {
                // Log exception
                LoggerModel.Instance.LogException($"Exception while performing query. {e.Message}");
            }

            return new List<JoinedRouteTable>(); // Return empty list if exception
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
                var completedOrders = from order in DbContextSingleton.Instance.Orders
                                      join citySource in DbContextSingleton.Instance.Cities on order.SourceCityId equals citySource.CityId
                                      join cityDest in DbContextSingleton.Instance.Cities on order.DestinationCityId equals cityDest.CityId
                                      join trip in DbContextSingleton.Instance.Trips on order.OrderId equals trip.OrderId
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
                LoggerModel.Instance.LogException($"Exception while performing query. {e.Message}");
            }

            // Return empty list, if there's an exception or no completed orders
            return new List<JoinedOrder>();
        }

        #endregion

        #region Helper Methods

        /*
        * METHOD NAME: GetKeyValues
        * DESCRIPTION: Gets the primary key values of an entity
        * 
        * RETURN: keyValues as object
        */
        private object[] GetKeyValues<T>(T entity) where T : class
        {
            try
            {
                // Obtain an EntityEntry instance for the provided entity
                var entry = DbContextSingleton.Instance.Entry(entity);

                // Retrieve the primary key information for the entity
                var primaryKey = entry.Metadata.FindPrimaryKey();

                // Get the properties that make up the primary key
                var primaryKeyProperties = primaryKey?.Properties;

                // Map each property to its current value in the provided entity
                if (primaryKeyProperties != null)
                {
                    var keyValues = primaryKeyProperties
                        .Select(property => entry.Property(property.Name).CurrentValue)
                        .ToArray();

                    // Return the array of primary key values
                    return keyValues!;
                }
            }
            catch (Exception e)
            {
                LoggerModel.Instance.LogException($"{e.Message}");
            }

            return null!;     // Return null if exception
        }

        #endregion
    }
}
