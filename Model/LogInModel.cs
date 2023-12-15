using System;
using System.Linq;
using TMS_Project.DataLayer.Context;

namespace TMS_Project.Model
{
    /// <summary>
    /// Model for handling user authentication.
    /// </summary>
    public class LogInModel
    {
        private readonly PasswordHasher _passwordHasher;


        /// <summary>
        /// Initializes a new instance of the <see cref="LogInModel"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// 
        /*
        * METHOD NAME: LogInModel
        * DESCRIPTION: Initializes a new instance of the <see cref="LogInModel"/> class.
        * PARAM: dbContext
        * RETURN: void
        */
        public LogInModel()
        {
            _passwordHasher = new PasswordHasher();
        }


        /*
        * METHOD NAME: VerifyUser
        * DESCRIPTION: Verifies the user's credentials
        *
        * RETURN: bool - true if user is verified, otherwise false
        */
        public bool VerifyUser(string? username, string? password)
        {
            var HashedInput = _passwordHasher.Hash(password);

            try
            {
                // Check if the database context or Users collection is null.
                if (DbContextSingleton.Instance.Users == null)
                {
                    LoggerModel.Instance.LogError("Database context or Users collection is null.");
                    return false;
                }

                // Retrieve the user from the database based on the provided username.
                var user = DbContextSingleton.Instance.Users.FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    // Check if the provided password matches the user's password.
                    bool validUser = _passwordHasher.Verify(HashedInput, user.Password); // user.password is already stored as hashed value
                    if (validUser)
                    {
                        // Log successful authentication.
                        LoggerModel.Instance.LogInfo($"User {username} successfully authenticated.");
                        return true;
                    }

                    // Log incorrect password attempt.
                    LoggerModel.Instance.LogWarning($"Incorrect password for user {username}.");
                    return false;
                }

                // Log user not found in the database.
                LoggerModel.Instance.LogWarning($"User {username} not found in the database.");
                return false;
            }
            catch (Exception)
            {
                // Log exception details if an error occurs during user verification.
                LoggerModel.Instance.LogException($"Error verifying the user. Username: {username}");
                return false;
            }
        }
    }
}
