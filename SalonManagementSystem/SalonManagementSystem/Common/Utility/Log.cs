using SalonAPI.Enum;
using System;
using SalonDataAccess;

namespace SalonAPI.Common
{
    
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Log
    {

        /// <summary>
        /// Save data modification details to the update table
        /// </summary>
        /// <param name="id">Represents references</param>
        /// <param name="table">Represents the reference table</param>
        /// <param name="actionType">Reference the table modification type</param>
        public static void Update(string id, string table, ActionType actionType)
        {
            SalonDbEntities db = new SalonDbEntities();
            tbllog update = new tbllog
            {
                ref_table = table,
                ref_id = id,
                updated_date_time = DateTime.Now,
                action_type = System.Enum.GetName(typeof(ActionType), actionType)
            };
            db.tbllogs.Add(update);
            db.SaveChanges();
        }

    }
}