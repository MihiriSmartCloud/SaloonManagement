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
    public class BarberController : ApiController
    {

        /// <summary>
        /// Get all barbers
        /// </summary>
        /// <remarks>
        ///  Get the details of all barbers in all salons
        /// </remarks>
        /// <returns></returns>
        /// <param name="salon_id">Optional parameter. If the salon_id isn't specified, by default it will return all the 
        /// barbers of all salons. Else it will only return the barbers of the given salon.</param>
        // GET api/Barber/GetAllBarbers
        [HttpGet]
        [Route("api/Barber/GetAllBarbers")]
        public HttpResponseMessage GetAllBarbers(int salon_id = 0)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if the salon id exists
                    if (salon_id != 0 && !entities.tblsalons.Any(e => e.salon_id == salon_id))
                        return Messages.GetInstance().HandleException("Retrieve failed! Salon with id = ", salon_id.ToString());

                    List<tblbarber> allBarbers = (salon_id == 0) ? entities.tblbarbers.ToList() : entities.tblbarbers.Where(x => x.salon_id == salon_id).ToList();

                    if (allBarbers != null && allBarbers.Count != 0)
                    {
                        List<Object> responses = new List<Object>();
                        responses.Add(new { Success = true, Message = "Barbers retrieved successfully!" });

                        foreach (var item in allBarbers)
                            responses.Add(new
                            {
                                item.barber_id,
                                item.barber_name,
                                item.allocated_seat_no,
                                item.salon_id,
                                item.is_available
                            });

                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No barbers found!", All_barbers = allBarbers });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve barbers.");
            }
        }


        /// <summary>
        /// Get a selected barber
        /// </summary>
        /// <remarks>
        /// Get the details of a particular barber using the barber id
        /// </remarks>
        /// <returns></returns>
        // GET api/Barber/GetSelectedBarber/5
        [HttpGet]
        [Route("api/Barber/GetSelectedBarber/{barber_id}")]
        public HttpResponseMessage Get(int barber_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    tblbarber selectedBarber = entities.tblbarbers.FirstOrDefault(e => e.barber_id == barber_id);
                    if (selectedBarber != null)
                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Success = true,
                            Message = "Barber retrieved successfully!",
                            Barber_details = new
                            {
                                selectedBarber.barber_id,
                                selectedBarber.barber_name,
                                selectedBarber.allocated_seat_no,
                                selectedBarber.salon_id,
                                selectedBarber.is_available
                            }
                        });

                    else
                        return Messages.GetInstance().HandleException("Retrieve failed! Barber with id = ", barber_id.ToString());
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve barber details.");
            }
        }



        /// <summary>
        /// Get currently available barbers
        /// </summary>
        /// <remarks>
        /// Get the details of all barbers, who are currently available, in the given salon.
        /// </remarks>
        /// <returns></returns>
        // GET api/Barber/GetCurrentlyAvailableBarbers/1
        [HttpGet]
        [Route("api/Barber/GetCurrentlyAvailableBarbers/{salon_id}")]
        public HttpResponseMessage GetCurrentlyAvailableBarbers(int salon_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if the salon id exists
                    if (!entities.tblsalons.Any(e => e.salon_id == salon_id))
                        return Messages.GetInstance().HandleException("Retrieve failed! Salon with id = ", salon_id.ToString());

                    // Get the currently available barbers in the particular salon
                    var allAvailableBarbers = entities.tblbarbers.Where(x => x.salon_id == salon_id && x.is_available == true).ToList();
                    if (allAvailableBarbers != null && allAvailableBarbers.Count != 0)
                    {
                        List<Object> responses = new List<Object>();
                        responses.Add(new
                        {
                            Success = true,
                            Message = "Currently available barbers retrieved successfully!"
                        });

                        foreach (var item in allAvailableBarbers)
                            responses.Add(new
                            {
                                item.barber_id,
                                item.barber_name,
                                item.salon_id,
                                item.allocated_seat_no,
                                item.is_available
                            });

                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No barbers found!", All_barbers = allAvailableBarbers });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve currently available barbers.");
            }
        }




        /// <summary>
        /// Get all barbers with no appointments on a particular date
        /// </summary>
        /// <remarks>
        /// Get the details of all barbers in a salon, who do not have any appointments. If the date parameter is not specified, by default it will
        /// display all the barbers who do not have any appointments scheduled for today. Else it will display the barbers who do not have any appointments scheduled on the
        /// specified date.
        /// </remarks>
        /// <param name="date">Optional parameter. Allowed date format: YYYY-MM-DD  Ex: 2020-10-25 </param>
        /// <returns></returns>
        // GET api/Barber/GetAllAvailableBarbers/5
        [HttpGet]
        [Route("api/Barber/GetVacantBarbers/{salon_id}")]
        public HttpResponseMessage GetVacantBarbers(int salon_id, String date = null)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if the salon id exists
                    if (!entities.tblsalons.Any(e => e.salon_id == salon_id))
                        return Messages.GetInstance().HandleException("Retrieve failed! Salon with id = ", salon_id.ToString());

                    // Get the date - if the date parameter is not specified, get the current date, else get the entered date
                    DateTime appointmentDate = (date == null) ? DateTime.Now.Date : Convert.ToDateTime(date);

                    // Get all the barbers in the particular salon
                    var barbersInParticularSalon = entities.tblbarbers.Where(x => x.salon_id == salon_id).Select(c => new { barber_id = c.barber_id }).ToList();

                    // Get all the barbers in the salon, who have any appointments scheduled for today
                    var barbersWithAppointments = entities.tblappointments.Where(p => p.salon_id == salon_id && p.due_date.Equals(appointmentDate)).Select(p => new { barber_id = p.barber_Id }).Distinct().ToList();

                    var barbersWithNoAppointments = barbersInParticularSalon.Except(barbersWithAppointments).ToList();

                    if (barbersWithNoAppointments != null && barbersWithNoAppointments.Count != 0)
                    {
                        List<Object> responses = new List<Object>();
                        responses.Add(new
                        {
                            Success = true,
                            Message = "Barbers who do not have any appointments sheduled for " + appointmentDate.ToString("dd/MM/yyyy") + " retrieved successfully!"
                        });

                        foreach (var item in barbersWithNoAppointments)
                        {
                            var barber = entities.tblbarbers.Where(p => p.barber_id == item.barber_id).Select(p => new { barber_id = p.barber_id, barber_name = p.barber_name, salon_id = p.salon_id, allocated_seat_no = p.allocated_seat_no, is_available = p.is_available }).FirstOrDefault();
                            responses.Add(new
                            {
                                barber.barber_id,
                                barber.barber_name,
                                barber.salon_id,
                                barber.allocated_seat_no,
                                barber.is_available
                            });
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No barbers found!", All_barbers = barbersWithNoAppointments });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve barber details.");
            }
        }






        /// <summary>
        /// Create a new barber
        /// </summary>
        /// <remarks>
        /// Create a new barber with the relevant details
        /// </remarks>
        /// <returns></returns>
        // POST api/Barber/CreateBarber       
        [HttpPost]
        [Route("api/Barber/CreateBarber")]
        public HttpResponseMessage Post([FromBody] JObject barber_details)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                string barber_name = barber_details["barber_name"].ToString().Trim();
                int salon_id = int.Parse(barber_details["salon_id"].ToString());
                int allocated_seat_no = int.Parse(barber_details["allocated_seat_no"].ToString());

                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if the barber name already exists in the particular salon. Otherwise there will be confusions.
                    if (entities.tblbarbers.Any(e => e.barber_name.ToUpper().Trim() == barber_name.ToUpper().Trim() && e.salon_id == salon_id))
                        return Messages.GetInstance().HandleException("Failed to create barber! A barber with the same name already exists in salon id = " + salon_id + ". Please enter another name.");


                    // Check if the user entered seat no exists in the salon
                    // 1. Get the no of seats available in the salon
                    // 2. Check if the entered seat no is within that range
                    var obj = entities.tblsalons.Where(p => p.salon_id.Equals(salon_id)).Select(p => new { seating_capacity = p.seating_capacity }).FirstOrDefault();
                    if (1 <= allocated_seat_no && allocated_seat_no <= obj.seating_capacity)
                    {
                        // If the seat no exists, then check if a barber in that salon, has already been assigned to that particular seat no
                        if (entities.tblbarbers.Any(e => e.salon_id == salon_id && e.allocated_seat_no == allocated_seat_no))
                            return Messages.GetInstance().HandleException("Failed to create barber! A barber has already been assigned to seat no = " + allocated_seat_no + " in salon id = " + salon_id + ". Please enter another seat number.");
                        else
                        {
                            // Add the new barber, & allocate the entered seat no to him
                            using (var transaction = entities.Database.BeginTransaction())
                            {
                                tblbarber barber = new tblbarber
                                {
                                    barber_name = barber_name,
                                    salon_id = salon_id,
                                    allocated_seat_no = allocated_seat_no,
                                    is_available = true
                                };
                                entities.tblbarbers.Add(barber);
                                entities.SaveChanges();

                                Utilities.getInstance().UpdateChanges(entities, transaction, barber.barber_id.ToString(), typeof(tblbarber).Name, ActionType.INSERT);

                                return Messages.GetInstance().HandleRequest("Barber", ActionType.INSERT);
                            }
                        }
                    }
                    else
                        return Messages.GetInstance().HandleException("Failed to create barber! The entered seat number is not found in the salon. Please enter a valid seat number.");
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to create barber.");
            }
        }



        /// <summary>
        /// Update an existing barber
        /// </summary>
        /// <remarks>
        /// Update the details of an existing barber in a salon, using the barber id
        /// </remarks>
        /// <returns></returns>
        // PUT api/Barber/UpdateBarber/5
        [HttpPut]
        [Route("api/Barber/UpdateBarber/{barber_id}")]
        public HttpResponseMessage Put(int barber_id, [FromBody] JObject barber_details)
        {
            try
            {
                //// Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                string barber_name = barber_details["barber_name"].ToString().Trim();
                int allocated_seat_no = int.Parse(barber_details["allocated_seat_no"].ToString());
                bool is_available = bool.Parse(barber_details["is_available"].ToString());

                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if a barber with the specified id exist
                    var entity = entities.tblbarbers.FirstOrDefault(e => e.barber_id == barber_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Update failed! Barber with id = ", barber_id.ToString());


                    // Check if the barber name already exists in the particular salon. Otherwise there will be confusions.
                    var barber = entities.tblbarbers.Where(p => p.salon_id == entity.salon_id && p.barber_name.ToUpper().Trim().Equals(barber_name.ToUpper().Trim())).Any();
                    if (entity.barber_name.ToUpper().Trim() != barber_name.ToUpper().Trim() && barber)
                        return Messages.GetInstance().HandleException("Update failed! A barber with the same name already exists in salon id = " + entity.salon_id + ". Please enter another name.");


                    // Check if the user entered seat no exists in the salon
                    // 1. Get the no of seats available in the salon
                    // 2. Check if the entered seat no is within that range
                    var salon = entities.tblsalons.Where(p => p.salon_id.Equals(entity.salon_id)).Select(p => new { seating_capacity = p.seating_capacity }).FirstOrDefault();
                    if (1 <= allocated_seat_no && allocated_seat_no <= salon.seating_capacity)
                    {
                        // If the seat no exists, then check if another barber in that salon, has already been assigned to that particular seat no
                        barber = entities.tblbarbers.Where(p => p.salon_id == entity.salon_id && p.allocated_seat_no == allocated_seat_no).Any();
                        if (entity.allocated_seat_no != allocated_seat_no && barber)
                            return Messages.GetInstance().HandleException("Update failed! A barber has already been assigned to seat no = " + allocated_seat_no + " in salon id = " + entity.salon_id + ". Please enter another seat number.");

                        else
                        {
                            using (var transaction = entities.Database.BeginTransaction())
                            {
                                // Update necessary fielsds
                                entity.barber_name = barber_name;
                                entity.allocated_seat_no = allocated_seat_no;
                                entity.is_available = is_available;

                                Utilities.getInstance().UpdateChanges(entities, transaction, barber_id.ToString(), typeof(tblbarber).Name, ActionType.UPDATE);

                                return Messages.GetInstance().HandleRequest("Barber", ActionType.UPDATE);
                            }
                        }
                    }
                    else
                        return Messages.GetInstance().HandleException("Update failed! The entered seat number is not found in the salon. Please enter a valid seat number.");
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to update barber details.");
            }
        }



        /// <summary>
        /// Delete an existing barber
        /// </summary>
        /// <remarks>
        /// Delete an existing barber in a salon, using the barber id
        /// </remarks>
        /// <returns></returns>
        // DELETE api/Barber/DeleteBarber/5
        [HttpDelete]
        [Route("api/Barber/DeleteBarber/{barber_id}")]
        public HttpResponseMessage DELETE(int barber_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    var entity = entities.tblbarbers.FirstOrDefault(e => e.barber_id == barber_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Delete failed! Barber with id = ", barber_id.ToString());
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            entities.tblbarbers.Remove(entity);
                            Utilities.getInstance().UpdateChanges(entities, transaction, barber_id.ToString(), typeof(tblbarber).Name, ActionType.DELETE);

                            return Messages.GetInstance().HandleRequest("Barber", ActionType.DELETE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to delete barber.");
            }
        }


    }
}
