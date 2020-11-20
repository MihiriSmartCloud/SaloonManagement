using SalonAPI.Common;
using SalonAPI.Common.Utility;
using SalonAPI.Enum;
using SalonDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace SalonManagementSystem.Controllers
{
    
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class DisplayBoardController : ApiController
    {

        /// <summary>
        /// Get next appointment no for barber
        /// </summary>
        /// <remarks>
        ///  Get the next appointment no, of a barber, in a particular salon, for the current date
        /// </remarks>
        /// <returns></returns>
        // GET api/DisplayBoard/GetNextAppointmentNo
        [HttpGet]
        [Route("api/DisplayBoard/GetNextAppointmentNo/{salon_id}/{barber_id}")]
        public HttpResponseMessage Get(int salon_id, int barber_id)
        {
            try
            {
                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if the barber exists, in the given salon
                    if (!entities.tblbarbers.Any(e => e.salon_id == salon_id && e.barber_id == barber_id))
                        return Request.CreateResponse(HttpStatusCode.NotFound, new { Success = false, Message = "Retrieve failed! Barber doesn't exist in the given salon." });

                    DateTime currentDate = DateTime.Now.Date;
                    List<Object> response = new List<Object>();

                    // Check if the barber has any appointment(s) for today
                    List<tblappointment> appointmentsForToday = entities.tblappointments.Where(x => x.due_date.Equals(currentDate) && x.salon_id == salon_id && x.barber_Id == barber_id).ToList();
                    if (appointmentsForToday.Count == 0)
                    {
                        response.Add(new
                        {
                            Success = true,
                            Message = "No appointments scheduled for today.",
                            Current_appointment_no = 0
                        });
                        return Request.CreateResponse(HttpStatusCode.OK, response);
                    }


                    // Check if the barber has started his appointment(s) for today
                    var allAppointmentsMade = entities.tblcurrent_appointments.Where(x => x.current_date.Equals(currentDate) && x.salon_id == salon_id && x.barber_id == barber_id).FirstOrDefault();
                    int nextAppointment = 0;

                    if (!entities.tblappointments.Any(x => x.due_date.Equals(currentDate) && x.salon_id == salon_id && x.barber_Id == barber_id && x.status == AppointmentStatus.TO_DO.ToString()))
                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Success = true,
                            Message = "All appointments made to barber id = " + barber_id + " in salon id = " + salon_id + ", which are scheduled for today, have been completed!",
                            Current_appointment_no = 0
                        });


                    if (allAppointmentsMade != null)
                        nextAppointment = allAppointmentsMade.last_appointment_no + 1;

                    // If the barber has any appointment(s) scheduled for today, but if they have not started yet
                    else
                        nextAppointment = 1;


                    response.Add(new
                    {
                        Success = true,
                        Message = "Current appointment no for barber id = " + barber_id + " in salon id = " + salon_id + " retrieved successfully!",
                        Current_appointment_no = nextAppointment
                    });

                    using (var transaction = entities.Database.BeginTransaction())
                    {
                        // Update the status of the appointment
                        tblappointment entity = entities.tblappointments.Where(x => x.due_date.Equals(currentDate) && x.salon_id == salon_id && x.barber_Id == barber_id && x.appointment_no_for_day == nextAppointment).FirstOrDefault();
                        if (entity != null)
                        {
                            entity.status = AppointmentStatus.IN_PROGRESS.ToString();
                            entities.SaveChanges();
                            Log.Update(entity.appointment_id.ToString(), typeof(tblappointment).Name, ActionType.UPDATE);
                        }

                        // Update the availability of barber
                        var barber = entities.tblbarbers.Where(x => x.salon_id == salon_id && x.barber_id == barber_id).FirstOrDefault();
                        if (barber != null)
                        {
                            barber.is_available = false;
                            entities.SaveChanges();
                            Log.Update(barber.barber_id.ToString(), typeof(tblbarber).Name, ActionType.UPDATE);
                        }
                        transaction.Commit();
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve current appointment number.");
            }
        }




        /// <summary>
        /// Update the current appointment no 
        /// </summary>
        /// <remarks>
        /// Update the current appointment no of a particular barber & update the status of the appointment
        /// </remarks>
        /// <returns></returns>
        // Patch api/DisplayBoard/UpdateCurrentAppointmentNo   
        [HttpPut]
        [Route("api/DisplayBoard/UpdateCompletedAppointment/{salon_id}/{barber_id}/{current_appointment_no}")]
        public HttpResponseMessage Put(int salon_id, int barber_id, int current_appointment_no)
        {
            try
            {
                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    if (!entities.tblappointments.Any(e => e.salon_id == salon_id && e.barber_Id == barber_id && e.appointment_no_for_day == current_appointment_no))
                        return Request.CreateResponse(HttpStatusCode.NotFound, new { Success = false, Message = "Update failed! No matching entry found." });

                    using (var transaction = entities.Database.BeginTransaction())
                    {
                        DateTime currentDate = DateTime.Now.Date;

                        // Check if the barber has started any of his appointment(s) for today
                        tblcurrent_appointments currentAppointment = entities.tblcurrent_appointments.Where(x => x.current_date.Equals(currentDate) && x.salon_id == salon_id && x.barber_id == barber_id).FirstOrDefault();

                        // If no current appointments for the current date
                        if (currentAppointment == null)
                        {
                            int appointment_id = entities.tblappointments.Where(x => x.due_date.Equals(currentDate) && x.salon_id == salon_id && x.barber_Id == barber_id && x.appointment_no_for_day == 1).Select(x => x.appointment_id).FirstOrDefault();
                            tblcurrent_appointments appointment = new tblcurrent_appointments
                            {
                                appointment_id = appointment_id,
                                salon_id = salon_id,
                                barber_id = barber_id,
                                current_date = currentDate,
                                last_appointment_no = current_appointment_no
                            };
                            entities.tblcurrent_appointments.Add(appointment);
                            entities.SaveChanges();
                            Log.Update(appointment.current_appointment_id.ToString(), typeof(tblcurrent_appointments).Name, ActionType.INSERT);


                            // Update the appointment status as completed
                            var entity = entities.tblappointments.FirstOrDefault(e => e.appointment_id == appointment_id);
                            if (entity != null)
                            {
                                entity.end_time = DateTime.Now.TimeOfDay;
                                entity.status = AppointmentStatus.COMPLETED.ToString();
                                entities.SaveChanges();
                                Log.Update(appointment_id.ToString(), typeof(tblappointment).Name, ActionType.UPDATE);
                            }


                            // Update the availability of barber
                            var barber = entities.tblbarbers.Where(x => x.salon_id == salon_id && x.barber_id == barber_id).FirstOrDefault();
                            if (barber != null)
                            {
                                barber.is_available = true;
                                entities.SaveChanges();
                                Log.Update(barber.barber_id.ToString(), typeof(tblbarber).Name, ActionType.UPDATE);
                            }

                            transaction.Commit();

                            return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "Appointment details updated successfully!" });
                        }
                        else
                        {

                            List<Object> response = new List<Object>();
                            int mainAppointmentId = entities.tblappointments.Where(x => x.due_date.Equals(currentDate) && x.salon_id == salon_id && x.barber_Id == barber_id && x.appointment_no_for_day == current_appointment_no).Select(x => x.appointment_id).FirstOrDefault();

                            // Update necessary fields
                            currentAppointment.last_appointment_no = current_appointment_no;
                            currentAppointment.appointment_id = mainAppointmentId;
                            entities.SaveChanges();
                            Log.Update(currentAppointment.current_appointment_id.ToString(), typeof(tblcurrent_appointments).Name, ActionType.UPDATE);


                            // Update the appointment status as completed
                            var entity = entities.tblappointments.FirstOrDefault(e => e.appointment_id == mainAppointmentId);
                            if (entity != null)
                            {
                                entity.end_time = DateTime.Now.TimeOfDay;
                                entity.status = AppointmentStatus.COMPLETED.ToString();
                                entities.SaveChanges();
                                Log.Update(mainAppointmentId.ToString(), typeof(tblappointment).Name, ActionType.UPDATE);
                            }


                            // Update the availability of barber
                            var barber = entities.tblbarbers.Where(x => x.salon_id == salon_id && x.barber_id == barber_id).FirstOrDefault();
                            if (barber != null)
                            {
                                barber.is_available = true;
                                entities.SaveChanges();
                                Log.Update(barber.barber_id.ToString(), typeof(tblbarber).Name, ActionType.UPDATE);
                            }

                            transaction.Commit();

                            return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "Appointment details updated successfully!" });
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to update current appointment details.");
            }
        }


    }
}
