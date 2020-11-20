using SalonAPI.Common;
using SalonAPI.Enum;
using SalonDataAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

/*
 * Author  - S Salgado
 * Date    - 24/10/2020
 */
namespace SalonAPI
{
    
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Utilities
    {
        private static Utilities utilities;

        private Utilities()
        {
            utilities = null;
        }

        public static Utilities getInstance() => utilities ?? (utilities = new Utilities());


        // Validates email addresses
        public bool ValidateEmail(string email) => Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);


        // Validates usernames
        public bool ValidateUsername(string username) => Regex.Match(username, @"^[a-zA-Z][a-zA-Z0-9]{5,11}$").Success;


        // Validates contact numbers
        //public bool ValidateContactNumber(string contactNumber) => Regex.Match(contactNumber, @"^([0-9]{9,10})$").Success;
        public bool ValidateContactNumber(string contactNumber) => Regex.IsMatch(contactNumber, @"^\d{9}$") || Regex.IsMatch(contactNumber, @"^\d{10}$");
        

        // Validates passwords
        public bool ValidatePassword(string password) => Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", RegexOptions.IgnoreCase);


        // Encodes the password 
        public string CalculateHash(string password) => System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        

        // Decodes the password
        public string DecodeFrom64(string encodedData) => System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(encodedData));


        // Checks if the entered password matches the exact password
        public bool IsValidPassword(string username, string password)
        {
            using (SalonDbEntities entities = new SalonDbEntities())
            {
                // Get the encrypted password of the user from the db
                // Decrypt the db password
                string decryptedPwd = Utilities.getInstance().DecodeFrom64(GetUserData(username));

                // Validate passwords - compare the two passwords
                return password.Equals(decryptedPwd) ? true : false;
            }
        }


        // Checks if the user exists
        public Boolean IsValidUser(string username)
        {
            using (SalonDbEntities entities = new SalonDbEntities())
            {
                tblshop_owner selectedOwner = entities.tblshop_owner.FirstOrDefault(e => e.email == username);
                return (selectedOwner != null) ? true : false;
            }
        }


        // Get password of the user from the db
        public string GetUserData(string username, bool isPwd = true, bool isUserId = false)
        {
            using (SalonDbEntities entities = new SalonDbEntities())
            {
                if (isPwd)
                {
                    tblshop_owner selectedOwner = entities.tblshop_owner.FirstOrDefault(e => e.email == username);
                    return (selectedOwner != null) ? selectedOwner.password : null;
                }
                else if (isUserId)
                {
                    tblshop_owner selectedOwner = entities.tblshop_owner.FirstOrDefault(e => e.email == username);
                    return (selectedOwner != null) ? selectedOwner.owner_id.ToString() : null;
                }

                return null;
            }
        }


        // Updates changes in the database
        public void UpdateChanges(SalonDbEntities entities, DbContextTransaction transaction, string id, string table, ActionType actionType)
        {
            if (!actionType.Equals("INSERT"))
                entities.SaveChanges();

            // Update log information
            Log.Update(id, table, actionType);

            transaction.Commit();
        }

    }
}