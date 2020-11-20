using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SalonDataAccess;
using SalonAPI.Common.Utility;
using SalonAPI.Enum;
using Newtonsoft.Json.Linq;
using System.Web;

/*
 * Author  - S Salgado
 * Date    - 24/10/2020
 */
namespace SalonAPI.Controllers
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class SalonController : ApiController
    {

        /// <summary>
        /// Get all salons
        /// </summary>
        /// <remarks>
        ///  Get the details of all registered salons
        /// </remarks>
        /// <returns></returns>
        // GET api/Salon/GetAllSalons
        [HttpGet]
        [Route("api/Salon/GetAllSalons")]
        public HttpResponseMessage Get()
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    List<tblsalon> allRegisteredSalons = entities.tblsalons.ToList();
                    if (allRegisteredSalons != null && allRegisteredSalons.Count != 0)
                    {
                        List<Object> responses = new List<Object>();
                        responses.Add(new { Success = true, Message = "Salons retrieved successfully!" });

                        foreach (var item in allRegisteredSalons)
                        {
                            responses.Add(new
                            {
                                item.salon_id,
                                item.salon_name,
                                item.owner_id,
                                owner_name = entities.tblshop_owner.Where(x => x.owner_id == item.owner_id).Select(x => x.name).First(),
                                item.salon_location,
                                item.contact_no,
                                item.email,
                                item.seating_capacity,
                                item.opening_time,
                                item.closing_time
                            });
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No salons found!", All_salons = allRegisteredSalons });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve salons.");
            }
        }


        /// <summary>
        /// Get a selected salon
        /// </summary>
        /// <remarks>
        /// Get the details of a particular salon using the salon id
        /// </remarks>
        /// <returns></returns>
        // GET api/Salon/GetSelectedSalon/5
        [HttpGet]
        [Route("api/Salon/GetSelectedSalon/{salon_id}")]
        public HttpResponseMessage Get(int salon_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    tblsalon selectedSalon = entities.tblsalons.FirstOrDefault(e => e.salon_id == salon_id);
                    if (selectedSalon != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Success = true,
                            Message = "Salon retrieved successfully!",
                            Salon_details = new
                            {
                                selectedSalon.salon_id,
                                selectedSalon.salon_name,
                                selectedSalon.owner_id,
                                owner_name = entities.tblshop_owner.Where(x => x.owner_id == selectedSalon.owner_id).Select(x => x.name).First(),
                                selectedSalon.salon_location,
                                selectedSalon.contact_no,
                                selectedSalon.email,
                                selectedSalon.seating_capacity,
                                selectedSalon.opening_time,
                                selectedSalon.closing_time
                            }
                        });
                    }
                    else
                        return Messages.GetInstance().HandleException("Retrieve failed! Salon with id = ", salon_id.ToString());
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve salon details.");
            }
        }


        /// <summary>
        /// Create a new salon
        /// </summary>
        /// <remarks>
        /// Create a new salon with the relevant details
        /// </remarks>
        /// <returns></returns>
        // POST api/Salon/CreateSalon       
        [HttpPost]
        [Route("api/Salon/CreateSalon")]
        public HttpResponseMessage Post([FromBody] JObject salon_details)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                int owner_id = int.Parse(salon_details["owner_id"].ToString());
                string name = salon_details["name"].ToString();
                string location = salon_details["location"].ToString();
                string contact_no = salon_details["contact_no"].ToString().Trim();
                string email = salon_details["email"].ToString();
                int no_of_seats = int.Parse(salon_details["no_of_seats"].ToString());
                string opening_time = salon_details["opening_time"].ToString();
                string closing_time = salon_details["closing_time"].ToString();

                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Validate salon - check if the salon already exists
                    bool selectedSalon = entities.tblsalons.Any(e => e.contact_no.ToString().Trim() == contact_no || e.email.ToUpper().Trim() == email.ToUpper().Trim());

                    // If a salon already exists
                    if (selectedSalon)
                        return Messages.GetInstance().HandleRequest("Salon", ActionType.INSERT, true);
                    else
                    {
                        // Validates the contact no
                        if (!Utilities.getInstance().ValidateContactNumber(contact_no))
                            return Messages.GetInstance().ValidateFields("Salon", ActionType.INSERT, isContactNumber: true);

                        // Validates the email
                        if (email != null && !Utilities.getInstance().ValidateEmail(email))
                            return Messages.GetInstance().ValidateFields("Salon", ActionType.INSERT, isEmail: true);

                        // Validates the no of seats
                        if (no_of_seats <= 0)
                            return Messages.GetInstance().HandleException("Failed to create salon! No of seats should be > 0.");

                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            tblsalon obj = new tblsalon
                            {
                                owner_id = owner_id,
                                salon_name = name.Trim(),
                                salon_location = location.Trim(),
                                contact_no = int.Parse(contact_no),
                                email = email.Trim(),
                                seating_capacity = no_of_seats,
                                opening_time = DateTime.Parse(opening_time, System.Globalization.CultureInfo.CurrentCulture).TimeOfDay,
                                closing_time = DateTime.Parse(closing_time, System.Globalization.CultureInfo.CurrentCulture).TimeOfDay
                            };
                            entities.tblsalons.Add(obj);
                            entities.SaveChanges();

                            Utilities.getInstance().UpdateChanges(entities, transaction, obj.salon_id.ToString(), typeof(tblsalon).Name, ActionType.INSERT);

                            return Messages.GetInstance().HandleRequest("Salon", ActionType.INSERT);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to create salon.");
            }
        }


        /// <summary>
        /// Update an existing salon
        /// </summary>
        /// <remarks>
        /// Update the details of an existing salon using the salon id
        /// </remarks>
        /// <returns></returns>
        // PUT api/Salon/UpdateSalon/5
        [HttpPut]
        [Route("api/Salon/UpdateSalon/{salon_id}")]
        public HttpResponseMessage Put(int salon_id, [FromBody] JObject salon_details)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                string name = salon_details["name"].ToString();
                string location = salon_details["location"].ToString();
                string contact_no = salon_details["contact_no"].ToString().Trim();
                string email = salon_details["email"].ToString();
                int no_of_available_seats = int.Parse(salon_details["no_of_seats"].ToString());
                string opening_time = salon_details["opening_time"].ToString();
                string closing_time = salon_details["closing_time"].ToString();

                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if a salon with the specified id exist
                    var entity = entities.tblsalons.FirstOrDefault(e => e.salon_id == salon_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Update failed! Salon with id = ", salon_id.ToString());


                    // If a salon with the specified id exists
                    else
                    {
                        // Validate salon mobile
                        if (!Utilities.getInstance().ValidateContactNumber(contact_no))
                            return Messages.GetInstance().ValidateFields("Salon", ActionType.UPDATE, isContactNumber: true);

                        // Validates the email
                        if (email != null && !Utilities.getInstance().ValidateEmail(email))
                            return Messages.GetInstance().ValidateFields("Salon", ActionType.UPDATE, isEmail: true);

                        // Validates the no of seats
                        if (no_of_available_seats <= 0)
                            return Messages.GetInstance().HandleException("Update failed! No of seats should be > 0.");

                        // Check for duplicates - check if the salon already exists
                        bool selectedSalon = entities.tblsalons.Any(e => e.contact_no.ToString().Trim() == contact_no);

                        // If a another salon already exists with the entered contact no or email 
                        if (entity.contact_no.ToString().Trim() != contact_no && selectedSalon)
                            return Messages.GetInstance().HandleRequest("Salon", ActionType.UPDATE, true);


                        selectedSalon = entities.tblsalons.Any(e => e.email.ToUpper().Trim() == email.ToUpper().Trim());

                        if (entity.email.ToUpper().Trim() != email.ToUpper().Trim() && selectedSalon)
                            return Messages.GetInstance().HandleRequest("Salon", ActionType.UPDATE, true);
                        else
                        {
                            using (var transaction = entities.Database.BeginTransaction())
                            {
                                // Update necessary data fielsds
                                entity.salon_name = name.Trim();
                                entity.salon_location = location.Trim();
                                entity.contact_no = int.Parse(contact_no);
                                entity.email = email.Trim();
                                entity.seating_capacity = no_of_available_seats;
                                entity.opening_time = DateTime.Parse(opening_time, System.Globalization.CultureInfo.CurrentCulture).TimeOfDay;
                                entity.closing_time = DateTime.Parse(closing_time, System.Globalization.CultureInfo.CurrentCulture).TimeOfDay;

                                Utilities.getInstance().UpdateChanges(entities, transaction, salon_id.ToString(), typeof(tblsalon).Name, ActionType.UPDATE);

                                return Messages.GetInstance().HandleRequest("Salon", ActionType.UPDATE);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to update salon details.");
            }
        }


        /// <summary>
        /// Delete an existing salon
        /// </summary>
        /// <remarks>
        /// Delete an existing salon using the salon id
        /// </remarks>
        /// <returns></returns>
        // DELETE api/Salon/DeleteSalon/5
        [HttpDelete]
        [Route("api/Salon/DeleteSalon/{salon_id}")]
        public HttpResponseMessage DELETE(int salon_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    var entity = entities.tblsalons.FirstOrDefault(e => e.salon_id == salon_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Delete failed! Salon with id = ", salon_id.ToString());
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            entities.tblsalons.Remove(entity);
                            Utilities.getInstance().UpdateChanges(entities, transaction, salon_id.ToString(), typeof(tblsalon).Name, ActionType.DELETE);

                            return Messages.GetInstance().HandleRequest("Salon", ActionType.DELETE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to delete salon.");
            }
        }


    }
}
