using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml;

namespace SecuroteckWebApplication.Models
{
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key 
        #endregion

        [Key]
        public string ApiKey { get; set; }

        public string UserName { get; set; }
        public User()
        {

        }
        
    }

    #region Task11?
    // TODO: You may find it useful to add code here for Log
    #endregion

    public class UserDatabaseAccess
    {
        #region Task3 
        // TODO: Make methods which allow us to read from/write to the database 
        #endregion

        /// <summary>
        /// Creating a record in the User database
        /// </summary>
        /// <param name="username">the chosen name for the new user</param>
        /// <returns></returns>
        public static string NewUser(string username)
        {
            Guid id = Guid.NewGuid();
            string idString = id.ToString();
            
            using (var db = new UserContext())
            {
                User user = new User() { UserName = username, ApiKey = idString  };
                db.Users.Add(user);
                db.SaveChanges();
                db.Dispose();
            }

            return idString;
        }

        /// <summary>
        /// Checks for the existence of a user in the database using a given username
        /// </summary>
        /// <param name="username">username of user being searched for</param>
        /// <returns></returns>
        public static bool checkUserName(string username)
        {
            bool userExists = false;

            using (var db = new UserContext())
            {
                if (db.Users.Any(o => o.UserName == username))
                {
                    userExists = true;
                }
                db.Dispose();
            }
            return userExists;
        }

        /// <summary>
        /// Checks for the existence of a user in the database using a given ApiKey
        /// </summary>
        /// <param name="ApiKey">ApiKey of user being searched for</param>
        /// <returns></returns>
        public static bool checkUserKey(string ApiKey)
        {
            bool userExists = false;

            using (var db = new UserContext())
            {
                if (db.Users.Any(o => o.ApiKey == ApiKey))
                {
                    userExists = true;
                }
                db.Dispose();
            }
                return userExists;
        }

        /// <summary>
        /// Checks for the existence of a user in the database using a given ApiKey and username 
        /// </summary>
        /// <param name="ApiKey">api key of user being searched</param>
        /// <param name="username">username of user being searched</param>
        /// <returns></returns>
        public bool checkUserKeyandName(string ApiKey, string username)
        {
            bool userDoesExist = false;

            using (var db = new UserContext())
            {
                if ((db.Users.Any(o => o.ApiKey == ApiKey)) && (db.Users.Any(o => o.UserName == username)))
                {
                    userDoesExist = true;
                }
                db.Dispose();
            }
            return userDoesExist;
        }

        /// <summary>
        /// /// Checks for the existence of a user in the database using a given ApiKey and returns the user object
        /// </summary>
        /// <param name="ApiKey">api key of user being searched for</param>
        /// <returns></returns>
        public static User checkUserRtnUsr(string ApiKey)
        {
            User user;
            using (var db = new UserContext())
            {
                var query = 
                            from p in db.Users
                            where p.ApiKey == ApiKey
                            select p;

                user = query.Single();
                db.Dispose();
            }
            return user;
        }

        /// <summary>
        /// deleted a user from the database
        /// </summary>
        /// <param name="ApiKey">api key of user to be deleted</param>
        public static void deleteUser(string ApiKey)
        {
            User user;
            using (var db = new UserContext())
            {
                var query =
                            from p in db.Users
                            where p.ApiKey == ApiKey
                            select p;

                user = query.Single();
                db.Users.Remove(user);
                db.Dispose();
            }
        }
    }


}
