using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SalonDataAccess;
using SalonAPI.Common;
using SalonAPI.Common.Utility;
using SalonAPI.Enum;
using System.Web;
using Newtonsoft.Json.Linq;

/*
 * Author  - S Salgado
 * Date    - 24/10/2020
 */
namespace SalonAPI.Controllers
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class AppointmentController : ApiController
    {

        /// <summary>
        /// Get all appointments
        /// </summary>
        /// <remarks>
        ///  Get the details of all appointments made
        /// </remarks>
        /// <returns></returns>
        /// <param name="salon_id">Optional parameter. If the salon_id isn't specified, by default it will return all the 
        /// appointments of all salons. Else it will only return the appointments of the given salon.</param>
        // GET api/Appointment/GetAllAppointments
        [HttpGet]
        [Route("api/Appointment/GetAllAppointments")]
        public HttpResponseMessage Get(int salon_id = 0)
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

                    List<tblappointment> allAppointmentsMade = (salon_id == 0) ? entities.tblappointments.ToList() : entities.tblappointments.Where(x => x.salon_id == salon_id).ToList();
                    if (allAppointmentsMade != null && allAppointmentsMade.Count != 0)
                    {
                        List<Object> responses = new List<Object>();
                        responses.Add(new { Success = true, Message = "Appointments retrieved successfully!" });

                        foreach (var item in allAppointmentsMade)
                            responses.Add(new
                            {
                                item.appointment_id,
                                item.appointment_no_for_day,
                                item.salon_id,
                                item.customer_id,
                                item.due_date,
                                item.start_time,
                                item.expected_end_time,
                                item.end_time,
                                barber_id = item.barber_Id,
                                item.canceled,
                                item.status
                            });

                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No appointments found!", All_appointments = allAppointmentsMade });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve appointments.");
            }
        }


        /// <summary>
        /// Get a selected appointment
        /// </summary>
        /// <remarks>
        /// Get the details of a particular appointment using the appointment id
        /// </remarks>
        /// <returns></returns>
        // GET api/Appointment/GetSelectedAppointment/5
        [HttpGet]
        [Route("api/Appointment/GetSelectedAppointment/{appointment_id}")]
        public HttpResponseMessage GetSelectedAppointment(int appointment_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    tblappointment selectedAppointment = entities.tblappointments.FirstOrDefault(e => e.appointment_id == appointment_id);
                    if (selectedAppointment != null)
                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Success = true,
                            Message = "Appointment retrieved successfully!",
                            Appointment_details = new
                            {
                                selectedAppointment.appointment_id,
                                selectedAppointment.appointment_no_for_day,
                                selectedAppointment.salon_id,
                                selectedAppointment.customer_id,
                                selectedAppointment.due_date,
                                selectedAppointment.start_time,
                                selectedAppointment.expected_end_time,
                                selectedAppointment.end_time,
                                barber_id = selectedAppointment.barber_Id,
                                selectedAppointment.canceled,
                                selectedAppointment.status
                            }
                        });

                    else
                        return Messages.GetInstance().HandleException("Retrieve failed! Appointment with id = ", appointment_id.ToString());
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve appointment details.");
            }
        }



        /// <summary>
        /// Get all appointments scheduled on a particular date 
        /// </summary>
        /// <remarks>
        /// Get the details of all appointments, scheduled on a particular date. If the date parameter is not specified, by default it will
        /// display all the appointments scheduled for today. Else it will display the appointments scheduled on the
        /// specified date.
        /// </remarks>
        /// <returns></returns>
        /// <param name="date">Optional parameter. Allowed date format: YYYY-MM-DD  Ex: 2020-10-25 </param>
        // GET api/Appointment/GetAllAppointments
        [HttpGet]
        [Route("api/Appointment/GetAllAppointments/{salon_id}")]
        public HttpResponseMessage Get(int salon_id, string date = null)
        {
            try
            {
                //// Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if the salon id exists
                    if (!entities.tblsalons.Any(e => e.salon_id == salon_id))
                        return Messages.GetInstance().HandleException("Retrieve failed! Salon with id = ", salon_id.ToString());

                    // Get the date - if the date parameter is not specified, get the current date, else get the entered date
                    DateTime appointmentDate = (date == null) ? DateTime.Now.Date : Convert.ToDateTime(date);

                    // Get all the barbers in the salon, who have any appointments scheduled for today
                    var appointments = entities.tblappointments.Where(p => p.salon_id == salon_id && p.due_date.Equals(appointmentDate))
                        .Select(p => new
                        {
                            p.appointment_id,
                            p.appointment_no_for_day,
                            p.salon_id,
                            p.customer_id,
                            p.due_date,
                            p.start_time,
                            p.expected_end_time,
                            p.end_time,
                            p.barber_Id,
                            p.canceled,
                            p.status
                        }).ToList();

                    if (appointments != null && appointments.Count != 0)
                    {
                        List<Object> responses = new List<Object>();
                        responses.Add(new { Success = true, Message = "All appointments of salon id = " + salon_id + " which are sheduled for " + appointmentDate.ToString("dd/MM/yyyy") + " retrieved successfully!" });

                        foreach (var item in appointments)
                            responses.Add(new
                            {
                                item.appointment_id,
                                item.appointment_no_for_day,
                                item.salon_id,
                                item.due_date,
                                item.start_time,
                                item.expected_end_time,
                                item.end_time,
                                item.barber_Id,
                                item.customer_id,
                                item.canceled,
                                item.status
                            });

                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No appointments found!", All_appointments = appointments });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve appointments.");
            }
        }




        /// <summary>
        /// Create a new appointment
        /// </summary>
        /// <remarks>
        /// Create a new appointment with the relevant details
        /// </remarks>
        /// <returns></returns>
        /// <param name="appointment_date">Allowed date format: YYYY-MM-DD  Ex: 2020-10-25 </param>
        /// <param name="due_time">Allowed time format: HH:mm:ss  Ex: 09:00:00 </param>
        /// <param name="barber_id">Optional parameter. If the barber id isn't specified, automatically the barber with min no of
        /// appointments on that day, will get assigned to the appointment. Else the specified barber will get assigned if available.</param>
        // POST api/Appointment/CreateAppointment     
        [HttpPost]
        [Route("api/Appointment/CreateAppointment")]
        public HttpResponseMessage Post([FromBody] JObject appointment_details, int barber_id = 0)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                int customer_id = int.Parse(appointment_details["customer_id"].ToString());
                int salon_id = int.Parse(appointment_details["salon_id"].ToString());
                string appointment_date = appointment_details["appointment_date"].ToString().Trim();
                string due_time = appointment_details["due_time"].ToString().Trim();

                int[] requested_services = new int[appointment_details["requested_services"].Count()];
                int count = 0;
                foreach (var service in appointment_details["requested_services"])
                {
                    requested_services[count] = int.Parse(appointment_details["requested_services"][count].ToString());
                    count++;
                }

     
                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if the salon id exists
                    tblsalon salon = entities.tblsalons.Where(e => e.salon_id == salon_id).FirstOrDefault();
                    if (salon == null)
                        return Messages.GetInstance().HandleException("Failed to create appointment! Salon with id = ", salon_id.ToString());

                    DateTime appointmentDate = Convert.ToDateTime(appointment_date).Date;
                    DateTime appointmentDueTime = DateTime.Parse(due_time, System.Globalization.CultureInfo.CurrentCulture);

                    // Validate the given date & time
                    if (appointmentDate <= DateTime.Now.Date && appointmentDueTime.TimeOfDay < DateTime.Now.TimeOfDay)
                        return Messages.GetInstance().HandleException("Failed to create appointment! Please select a valid date & time.");

                    // Check if the appointment falls between the opened & closed hours
                    if (!(salon.opening_time <= appointmentDueTime.TimeOfDay && appointmentDueTime.TimeOfDay < salon.closing_time))
                        return Messages.GetInstance().HandleException("Failed to create appointment! Appointment should be within the open & close hours, of the salon. Please select a valid time.");

                    // Check if an appointment has already made to the same date & time, by the particular customer
                    if (entities.tblappointments.Any(e => e.due_date.Equals(appointmentDate) && e.start_time.Equals(appointmentDueTime.TimeOfDay) && e.customer_id == customer_id))
                        return Messages.GetInstance().HandleException("Failed to create appointment! An appointment has already been made to the same date & time by customer id = " + customer_id);

                    // Check if exceeding the max seating capacity at a given time
                    int maxSeats = entities.tblsalons.Where(x => x.salon_id == salon_id).Select(x => x.seating_capacity).First();
                    int concurrentNoOfAppointments = entities.tblappointments.Where(p => p.salon_id == salon_id && p.due_date.Equals(appointmentDate.Date) && p.start_time.Equals(appointmentDueTime.TimeOfDay)).Count();
                    if (concurrentNoOfAppointments == maxSeats)
                        return Messages.GetInstance().HandleException("Failed to create appointment! No vacant seats available for the given time.");

                    // Check if exceeding the max seating capacity when appointments fall not exactly at the same time, but in between





                    // If the barber id has been specified
                    if (barber_id != 0)
                    {
                        // Check his availability at the exact given date & time
                        tblappointment appointment = entities.tblappointments.Where(e => e.salon_id == salon_id && e.due_date.Equals(appointmentDate) && e.start_time.Equals(appointmentDueTime.TimeOfDay) && e.barber_Id == barber_id).FirstOrDefault();
                        if (appointment != null)
                            return Messages.GetInstance().HandleException("Failed to create appointment! Barber already has an appointment at the given date & time.");

                        DateTime expectedEndTimeOfAppointment = DateTime.Today.Add(GetExpectedEndTimeOfAppointment(entities, salon_id, due_time, requested_services));

                        // Check if the appointment to be made, gets clashed with another
                        List<tblappointment> allAppointments = entities.tblappointments.Where(e => e.salon_id == salon_id && e.due_date.Equals(appointmentDate) && e.barber_Id == barber_id).ToList();
                        foreach (var a in allAppointments)
                        {
                            // Get the expected end time of appointment
                            int start_time = (int)a.start_time.TotalSeconds;
                            int expectedEndTime = (int)a.expected_end_time.TotalSeconds;
                            TimeSpan start = TimeSpan.FromSeconds(start_time);
                            TimeSpan end = TimeSpan.FromSeconds(expectedEndTime);
                            DateTime startTime = DateTime.Today.Add(start);
                            DateTime endTime = DateTime.Today.Add(end);

                            // Check if the appointment to be made, falls in between another appointment, of the same barber
                            if ((startTime < appointmentDueTime && appointmentDueTime < endTime) || (startTime < expectedEndTimeOfAppointment && expectedEndTimeOfAppointment < endTime))
                                return Messages.GetInstance().HandleException("Failed to create appointment! Barber already has an appointment at the given date & time.");
                        }


                        // Check if the barber provides all the requested services
                        foreach (var service in requested_services)
                        {
                            if (!entities.tblbarber_service.Any(x => x.barber_id == barber_id && x.service_id == service))
                                return Messages.GetInstance().HandleException("Failed to create appointment! Barber id = " + barber_id + " does not provide all the requested services.");
                        }


                        // Else create the appointment
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            tblappointment obj = new tblappointment
                            {
                                appointment_no_for_day = 0,
                                salon_id = salon_id,
                                due_date = appointmentDate,
                                start_time = appointmentDueTime.TimeOfDay,
                                expected_end_time = GetExpectedEndTimeOfAppointment(entities, salon_id, due_time, requested_services),
                                end_time = null,
                                barber_Id = barber_id,
                                customer_id = customer_id,
                                canceled = false,
                                status = AppointmentStatus.TO_DO.ToString()
                            };

                            entities.tblappointments.Add(obj);
                            entities.SaveChanges();

                            Log.Update(obj.appointment_id.ToString(), typeof(tblappointment).Name, ActionType.INSERT);

                            SortDailyAppointmentsOfEachBarber(entities, salon_id, appointmentDate, barber_id);


                            foreach (int service in requested_services)
                            {
                                tblservice_booked service_booked = new tblservice_booked
                                {
                                    appointment_id = obj.appointment_id,
                                    service_id = service
                                };
                                entities.tblservice_booked.Add(service_booked);
                                entities.SaveChanges();

                                Log.Update(service_booked.id.ToString(), typeof(tblservice_booked).Name, ActionType.INSERT);
                            }

                            transaction.Commit();

                            return Messages.GetInstance().HandleRequest("Appointment", ActionType.INSERT);
                        }
                    }


                    // If the customer has not selected a barber
                    else
                    {
                        // Check if a barber is available at that particular time slot, on the given date, for the requested service, in the particular salon
                        // 1. Get all barbers in the particular salon
                        var barbersInSalon = entities.tblbarbers.Where(x => x.salon_id == salon_id).Select(c => new { barber_id = c.barber_id }).ToList();

                        // 2. Get all barbers in the salon, who have any appointments scheduled at the exact date & time 
                        var barbersWithAppointments = entities.tblappointments.Where(p => p.salon_id == salon_id && p.due_date.Equals(appointmentDate) && p.start_time.Equals(appointmentDueTime.TimeOfDay)).Select(p => new { barber_id = p.barber_Id }).ToList();

                        // 3. Get the vacant barbers at the given date & time
                        var vacantBarbers = barbersInSalon.Except(barbersWithAppointments).ToList();
                        if (vacantBarbers.Count == 0)
                            return Messages.GetInstance().HandleException("Failed to create appointment! No vacant barbers available at the given date & time.");
                        else
                        {
                            DateTime expectedEndTimeOfAppointment = DateTime.Today.Add(GetExpectedEndTimeOfAppointment(entities, salon_id, due_time, requested_services));

                            // If any vacant barbers exist, for each barber
                            // Check if the appointment to be made, gets clashed with another
                            foreach (var b in vacantBarbers.ToList())
                            {
                                List<tblappointment> allAppointments = entities.tblappointments.Where(e => e.salon_id == salon_id && e.due_date.Equals(appointmentDate) && e.barber_Id == b.barber_id).ToList();
                                foreach (var a in allAppointments)
                                {
                                    int start_time = (int)a.start_time.TotalSeconds;
                                    int expectedEndTime = (int)a.expected_end_time.TotalSeconds;
                                    TimeSpan start = TimeSpan.FromSeconds(start_time);
                                    TimeSpan end = TimeSpan.FromSeconds(expectedEndTime);
                                    DateTime startTime = DateTime.Today.Add(start);
                                    DateTime endTime = DateTime.Today.Add(end);

                                    // Check if the appointment to be made, falls in between another appointment, of the same barber
                                    if ((startTime < appointmentDueTime && appointmentDueTime < endTime) || (startTime < expectedEndTimeOfAppointment && expectedEndTimeOfAppointment < endTime))
                                        vacantBarbers.Remove(b);
                                }
                            }


                            if (vacantBarbers.Count == 0)
                                return Messages.GetInstance().HandleException("Failed to create appointment! No vacant barbers available at the given time.");

                            else
                            {
                                // From all the vacant barbers, get the barbers who provide all the requested services
                                foreach (var barber in vacantBarbers.ToList())
                                {
                                    foreach (var service in requested_services)
                                    {
                                        // Filter the matching barbers
                                        if (!entities.tblbarber_service.Any(x => x.barber_id == barber.barber_id && x.service_id == service))
                                            vacantBarbers.Remove(barber);
                                    }
                                }


                                // If no barbers exist, who provide the requested service
                                if (vacantBarbers.Count == 0)
                                    return Messages.GetInstance().HandleException("Failed to create appointment! No vacant barbers available for the requested service(s).");


                                Dictionary<int, int> dict = new Dictionary<int, int>();
                                bool flag = false;
                                int barberIdToAssign = 0;

                                // Get the barber in the salon, with min no of appointments, on the given date
                                foreach (var barber in vacantBarbers)
                                {
                                    var groups = entities.tblappointments.Where(x => x.salon_id == salon_id && x.barber_Id == barber.barber_id && x.due_date.Equals(appointmentDate)).GroupBy(x => x.barber_Id).ToList();
                                    if (groups.Count == 0)
                                    {
                                        flag = true;
                                        barberIdToAssign = barber.barber_id;
                                        break;
                                    }

                                    // Get the barber id, no of appointments for each barber
                                    foreach (var b in groups)
                                        dict.Add(barber.barber_id, b.Max(a => a.appointment_no_for_day));
                                }

                                // Get the barber with min no of appointments on that day
                                if (!flag)
                                    barberIdToAssign = dict.OrderBy(x => x.Value).First().Key;


                                // Create the appointment
                                using (var transaction = entities.Database.BeginTransaction())
                                {
                                    tblappointment obj = new tblappointment
                                    {
                                        appointment_no_for_day = 0,
                                        salon_id = salon_id,
                                        due_date = appointmentDate,
                                        start_time = appointmentDueTime.TimeOfDay,
                                        expected_end_time = GetExpectedEndTimeOfAppointment(entities, salon_id, due_time, requested_services),
                                        end_time = null,
                                        barber_Id = barberIdToAssign,
                                        customer_id = customer_id,
                                        canceled = false,
                                        status = AppointmentStatus.TO_DO.ToString()
                                    };

                                    entities.tblappointments.Add(obj);
                                    entities.SaveChanges();

                                    // Update log information
                                    Log.Update(obj.appointment_id.ToString(), typeof(tblappointment).Name, ActionType.INSERT);

                                    SortDailyAppointmentsOfEachBarber(entities, salon_id, appointmentDate, barberIdToAssign);


                                    foreach (int service in requested_services)
                                    {
                                        tblservice_booked service_booked = new tblservice_booked
                                        {
                                            appointment_id = obj.appointment_id,
                                            service_id = service
                                        };
                                        entities.tblservice_booked.Add(service_booked);
                                        entities.SaveChanges();

                                        Log.Update(service_booked.id.ToString(), typeof(tblservice_booked).Name, ActionType.INSERT);
                                    }

                                    transaction.Commit();

                                    return Messages.GetInstance().HandleRequest("Appointment", ActionType.INSERT);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to create appointment.");
            }
        }





        /// <summary>
        /// Update an existing appointment
        /// </summary>
        /// <remarks>
        /// Update an existing appointment with the relevant details
        /// </remarks>
        /// <returns></returns>
        /// <param name="appointment_date">Allowed date format: YYYY-MM-DD  Ex: 2020-10-25 </param>
        /// <param name="due_time">Allowed time format: HH:mm:ss  Ex: 09:00:00 </param>
        /// <param name="end_time">Allowed time format: HH:mm:ss  Ex: 11:00:00 </param>
        /// <param name="barber_id">Optional parameter. If the barber id isn't specified, automatically the barber with min no of
        /// appointments on that day, will get assigned to the appointment. Else the specified barber will get assigned if available.</param>
        // POST api/Appointment/UpdateAppointment/5   
        [HttpPut]
        [Route("api/Appointment/UpdateAppointment/{appointment_id}")]
        public HttpResponseMessage Put(int appointment_id, [FromBody] JObject appointment_details, int barber_id = 0)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });

                int customer_id = int.Parse(appointment_details["customer_id"].ToString());
                string appointment_date = appointment_details["appointment_date"].ToString().Trim();
                string due_time = appointment_details["due_time"].ToString().Trim();
                string end_time = appointment_details["end_time"].ToString().Trim();
                bool canceled = bool.Parse(appointment_details["canceled"].ToString());

                int[] requested_services = new int[appointment_details["requested_services"].Count()];
                int count = 0;
                foreach (var service in appointment_details["requested_services"])
                {
                    requested_services[count] = int.Parse(appointment_details["requested_services"][count].ToString());
                    count++;
                }


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if the appointment_id id exists
                    var entity = entities.tblappointments.Where(e => e.appointment_id == appointment_id).First();
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Update failed! Appointment with id = ", appointment_id.ToString());

                    
                    int salon_id = entities.tblappointments.Where(x => x.appointment_id == appointment_id).Select(x => x.salon_id).First();
                    tblsalon salon = entities.tblsalons.Where(e => e.salon_id == salon_id).FirstOrDefault();
                    //if (salon == null)
                    //    return Messages.GetInstance().HandleException("Update failed! Salon with id = ", salon_id.ToString());


                    DateTime appointmentDate = Convert.ToDateTime(appointment_date).Date;
                    DateTime appointmentDueTime = DateTime.Parse(due_time, System.Globalization.CultureInfo.CurrentCulture);
                    DateTime appointmentEndTime = DateTime.Now;
                    if (end_time != null && end_time != "null" && end_time != "")
                        appointmentEndTime = DateTime.Parse(end_time, System.Globalization.CultureInfo.CurrentCulture);


                    // Validate the given date
                    if (appointmentDate <= DateTime.Now.Date && appointmentDueTime.TimeOfDay < DateTime.Now.TimeOfDay)
                        return Messages.GetInstance().HandleException("Update failed! Please select a valid date & time.");

                    // Check if the appointment falls between the opened & closed hours
                    if (!(salon.opening_time <= appointmentDueTime.TimeOfDay && appointmentDueTime.TimeOfDay < salon.closing_time))
                        return Messages.GetInstance().HandleException("Update failed! Appointment should be within the open & close hours, of the salon. Please select a valid time.");

                    // Check if an appointment has already made to the same date & time, by the particular customer
                    if (entities.tblappointments.Any(e => e.appointment_id != appointment_id && e.due_date.Equals(appointmentDate) && e.start_time.Equals(appointmentDueTime.TimeOfDay) && e.customer_id == customer_id))
                        return Messages.GetInstance().HandleException("Update failed! An appointment has already been made to the same date & time by customer id = " + customer_id);


                    // Check if exceeding the max seating capacity at a given time
                    int maxSeatingCapacty = entities.tblsalons.Where(x => x.salon_id == salon_id).Select(x => x.seating_capacity).First();
                    int concurrentNoOfAppointments = entities.tblappointments.Where(p => p.appointment_id != appointment_id && p.salon_id == salon_id && p.due_date.Equals(appointmentDate.Date) && p.start_time.Equals(appointmentDueTime.TimeOfDay)).Count();
                    if (concurrentNoOfAppointments == maxSeatingCapacty)
                        return Messages.GetInstance().HandleException("Update failed! No vacant seats available for the given time.");


                    // If the barber id has been specified
                    if (barber_id != 0)
                    {
                        // Check if the barber provides the requested service
                        foreach (int service in requested_services)
                        {
                            if (!entities.tblbarber_service.Any(x => x.barber_id == barber_id && x.service_id == service))
                                return Messages.GetInstance().HandleException("Update failed! Barber id = " + barber_id + " does not provide all the requested services.");
                        }

                        // Check his availability at the given date & time
                        if (entities.tblappointments.Any(e => e.appointment_id != appointment_id && e.salon_id == salon_id && e.due_date.Equals(appointmentDate) && e.start_time.Equals(appointmentDueTime.TimeOfDay) && e.barber_Id == barber_id))
                            return Messages.GetInstance().HandleException("Update failed! Barber is not available on the given date & time.");

                        DateTime expectedEndTimeOfAppointment = DateTime.Today.Add(GetExpectedEndTimeOfAppointment(entities, salon_id, due_time, requested_services));

                        // Check if the appointment to be made, gets clashed with another
                        List<tblappointment> allAppointments = entities.tblappointments.Where(e => e.appointment_id != appointment_id && e.salon_id == salon_id && e.due_date.Equals(appointmentDate) && e.barber_Id == barber_id).ToList();
                        foreach (var a in allAppointments)
                        {
                            int start_time = (int)a.start_time.TotalSeconds;
                            int expectedEndTime = (int)a.expected_end_time.TotalSeconds;
                            TimeSpan start = TimeSpan.FromSeconds(start_time);
                            TimeSpan end = TimeSpan.FromSeconds(expectedEndTime);
                            DateTime startTime = DateTime.Today.Add(start);
                            DateTime endTime = DateTime.Today.Add(end);

                            // Check if the appointment to be made, falls in between another appointment, of the same barber
                            if ((startTime < appointmentDueTime && appointmentDueTime < endTime) || (startTime < expectedEndTimeOfAppointment && expectedEndTimeOfAppointment < endTime))
                                return Messages.GetInstance().HandleException("Update failed! Barber already has an appointment at the given date & time.");
                        }

                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            // Update necessary fields
                            entity.due_date = appointmentDate;
                            entity.start_time = appointmentDueTime.TimeOfDay;
                            entity.expected_end_time = GetExpectedEndTimeOfAppointment(entities, salon_id, due_time, requested_services);
                            entity.barber_Id = barber_id;
                            entity.customer_id = customer_id;
                            entity.canceled = canceled;

                            if (canceled)
                            {
                                entity.end_time = null;
                                entity.status = AppointmentStatus.CANCELED.ToString();
                            }

                            if (due_time != null && end_time != null && end_time != "")
                            {
                                entity.end_time = appointmentEndTime.TimeOfDay;
                                entity.status = AppointmentStatus.COMPLETED.ToString();
                            }

                            entities.SaveChanges();

                            Log.Update(appointment_id.ToString(), typeof(tblappointment).Name, ActionType.UPDATE);

                            SortDailyAppointmentsOfEachBarber(entities, salon_id, appointmentDate, barber_id);


                            var servicesBooked = entities.tblservice_booked.Where(e => e.appointment_id == appointment_id).ToList();
                            if (servicesBooked != null)
                            {
                                foreach (var service in servicesBooked)
                                {
                                    entities.tblservice_booked.Remove(service);
                                    entities.SaveChanges();

                                    // Update log information
                                    Log.Update(service.id.ToString(), typeof(tblservice_booked).Name, ActionType.DELETE);
                                }
                            }


                            foreach (int service in requested_services)
                            {
                                tblservice_booked obj = new tblservice_booked
                                {
                                    appointment_id = appointment_id,
                                    service_id = service
                                };
                                entities.tblservice_booked.Add(obj);
                                entities.SaveChanges();

                                Log.Update(obj.id.ToString(), typeof(tblservice_booked).Name, ActionType.UPDATE);
                            }

                            transaction.Commit();

                            return Messages.GetInstance().HandleRequest("Appointment", ActionType.UPDATE);
                        }
                    }


                    // If the customer has not selected a barber
                    else
                    {
                        // Check if a barber is available for that particular time slot, on the given date, for the requested service, in the particular salon
                        // 1. Get all barbers in the particular salon
                        var barbersInSalon = entities.tblbarbers.Where(x => x.salon_id == salon_id).Select(c => new { barber_id = c.barber_id }).ToList();

                        // 2. Get all barbers in the salon, who have any appointments scheduled at the exact date & time 
                        var barbersWithAppointments = entities.tblappointments.Where(p => p.appointment_id != appointment_id && p.salon_id == salon_id && p.due_date.Equals(appointmentDate) && p.start_time.Equals(appointmentDueTime.TimeOfDay)).Select(p => new { barber_id = p.barber_Id }).ToList();

                        // 3. Get the vacant barbers at the given date & time 
                        var vacantBarbers = barbersInSalon.Except(barbersWithAppointments).ToList();
                        if (vacantBarbers.Count == 0)
                            return Messages.GetInstance().HandleException("Update failed! No barbers available at the given date & time.");
                        else
                        {
                            DateTime expectedEndTimeOfAppointment = DateTime.Today.Add(GetExpectedEndTimeOfAppointment(entities, salon_id, due_time, requested_services));

                            // If any vacant barbers exist, for each barber
                            // Check if the appointment to be made, gets clashed with another
                            foreach (var b in vacantBarbers.ToList())
                            {
                                List<tblappointment> allAppointments = entities.tblappointments.Where(e => e.appointment_id != appointment_id && e.salon_id == salon_id && e.due_date.Equals(appointmentDate) && e.barber_Id == b.barber_id).ToList();
                                foreach (var a in allAppointments)
                                {
                                    int start_time = (int)a.start_time.TotalSeconds;
                                    int expectedEndTime = (int)a.expected_end_time.TotalSeconds;
                                    TimeSpan start = TimeSpan.FromSeconds(start_time);
                                    TimeSpan end = TimeSpan.FromSeconds(expectedEndTime);
                                    DateTime startTime = DateTime.Today.Add(start);
                                    DateTime endTime = DateTime.Today.Add(end);

                                    // Check if the appointment to be made, falls in between another appointment, of the same barber
                                    if ((startTime < appointmentDueTime && appointmentDueTime < endTime) || (startTime < expectedEndTimeOfAppointment && expectedEndTimeOfAppointment < endTime))
                                        vacantBarbers.Remove(b);
                                }
                            }


                            if (vacantBarbers.Count == 0)
                                return Messages.GetInstance().HandleException("Update failed! No vacant barbers available at the given time.");

                            else
                            {
                                // If any vacant barbers exist, get the barbers who provide the requested service
                                foreach (var barber in vacantBarbers.ToList())
                                {
                                    foreach (var service in requested_services)
                                    {
                                        // Filter the matching barbers
                                        if (!entities.tblbarber_service.Any(x => x.barber_id == barber.barber_id && x.service_id == service))
                                            vacantBarbers.Remove(barber);
                                    }
                                }


                                // If no barbers exist, who provide the requested service
                                if (vacantBarbers.Count == 0)
                                    return Messages.GetInstance().HandleException("Update failed! No vacant barbers available for the requested service(s).");


                                Dictionary<int, int> dict = new Dictionary<int, int>();
                                bool flag = false;
                                int barberIdToAssign = 0;

                                // Get the barber in the salon, with min no of appointments, on the given date
                                foreach (var barber in vacantBarbers)
                                {
                                    // Ignore the currently updating appointment
                                    var groups = entities.tblappointments.Where(x => x.appointment_id != appointment_id && x.salon_id == salon_id && x.barber_Id == barber.barber_id && x.due_date.Equals(appointmentDate)).GroupBy(x => x.barber_Id).ToList();
                                    if (groups.Count == 0)
                                    {
                                        flag = true;
                                        barberIdToAssign = barber.barber_id;
                                        break;
                                    }

                                    // Get the barber id, no of appointments for each barber
                                    foreach (var b in groups)
                                        dict.Add(barber.barber_id, b.Max(a => a.appointment_no_for_day));
                                }

                                // Get the barber with min no of appointments on that day
                                if (!flag)
                                    barberIdToAssign = dict.OrderBy(x => x.Value).First().Key;


                                using (var transaction = entities.Database.BeginTransaction())
                                {
                                    // Update necessary fields
                                    entity.due_date = appointmentDate;
                                    entity.start_time = appointmentDueTime.TimeOfDay;
                                    entity.expected_end_time = GetExpectedEndTimeOfAppointment(entities, salon_id, due_time, requested_services);
                                    entity.barber_Id = barberIdToAssign;
                                    entity.customer_id = customer_id;
                                    entity.canceled = canceled;

                                    if (canceled)
                                    {
                                        entity.end_time = null;
                                        entity.status = AppointmentStatus.CANCELED.ToString();
                                    }

                                    if (due_time != null && end_time != null && end_time != "")
                                    {
                                        entity.end_time = appointmentEndTime.TimeOfDay;
                                        entity.status = AppointmentStatus.COMPLETED.ToString();
                                    }
                                    entities.SaveChanges();

                                    // Update log information
                                    Log.Update(appointment_id.ToString(), typeof(tblappointment).Name, ActionType.UPDATE);

                                    SortDailyAppointmentsOfEachBarber(entities, salon_id, appointmentDate, barberIdToAssign);


                                    var servicesBooked = entities.tblservice_booked.Where(e => e.appointment_id == appointment_id).ToList();
                                    if (servicesBooked != null)
                                    {
                                        foreach (var service in servicesBooked)
                                        {
                                            entities.tblservice_booked.Remove(service);
                                            entities.SaveChanges();

                                            // Update log information
                                            Log.Update(service.id.ToString(), typeof(tblservice_booked).Name, ActionType.DELETE);
                                        }
                                    }


                                    foreach (int service in requested_services)
                                    {
                                        tblservice_booked obj = new tblservice_booked
                                        {
                                            appointment_id = appointment_id,
                                            service_id = service
                                        };
                                        entities.tblservice_booked.Add(obj);
                                        entities.SaveChanges();

                                        Log.Update(obj.id.ToString(), typeof(tblservice_booked).Name, ActionType.UPDATE);
                                    }

                                    transaction.Commit();


                                    return Messages.GetInstance().HandleRequest("Appointment", ActionType.UPDATE);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to update appointment details.");
            }
        }





        /// <summary>
        /// Delete an existing appointment
        /// </summary>
        /// <remarks>
        /// Delete an existing appointment using the appointment id
        /// </remarks>
        /// <returns></returns>
        // DELETE api/Appointment/DeleteAppointment/5
        [HttpDelete]
        [Route("api/Appointment/DeleteAppointment/{appointment_id}")]
        public HttpResponseMessage DELETE(int appointment_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    var entity = entities.tblappointments.FirstOrDefault(e => e.appointment_id == appointment_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Delete failed! Appointment with id = ", appointment_id.ToString());
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            entities.tblappointments.Remove(entity);
                            Utilities.getInstance().UpdateChanges(entities, transaction, appointment_id.ToString(), typeof(tblappointment).Name, ActionType.DELETE);

                            return Messages.GetInstance().HandleRequest("Appointment", ActionType.DELETE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to delete appointment.");
            }
        }




        [NonAction]
        public void SortDailyAppointmentsOfEachBarber(SalonDbEntities entities, int salon_id, DateTime appointmentDate, int barber_id)
        {
            // Check if the barber has any appointment(s) on the given date
            var dailyAppointmentsOfBarber = entities.tblappointments.Where(a => a.salon_id == salon_id && a.due_date.Equals(appointmentDate) && a.barber_Id == barber_id).ToList();
            if (dailyAppointmentsOfBarber != null)
            {
                // Sort all the appointments by due time
                var orderedAppointments = dailyAppointmentsOfBarber.OrderBy(a => a.start_time).ToList();
                int appointment_no = 0;
                foreach (var appointment in orderedAppointments)
                {
                    appointment.appointment_no_for_day = appointment_no + 1;
                    appointment_no = appointment.appointment_no_for_day;
                }
                entities.SaveChanges();
            }
        }


        [NonAction]
        // Calculates the expected end time of an appointment
        public TimeSpan GetExpectedEndTimeOfAppointment(SalonDbEntities entities, int salon_id, string start_time, params int[] requested_services)
        {
            int totalDuration = 0;

            // Get the duration of each requested service
            foreach (int service in requested_services)
                totalDuration += entities.tblservices.Where(e => e.salon_id == salon_id && e.service_id == service).Select(x => x.duration).Single();

            int startTimeInSeconds = Convert.ToInt32(TimeSpan.Parse(start_time).TotalSeconds);
            TimeSpan start = TimeSpan.FromSeconds(startTimeInSeconds);
            TimeSpan end = TimeSpan.FromSeconds(startTimeInSeconds + totalDuration);
            DateTime startTime = DateTime.Today.Add(start);
            DateTime endTime = DateTime.Today.Add(end);

            return endTime.TimeOfDay;
        }



    }
}
