using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SalonDataAccess;
using SalonAPI.Common.Utility;
using SalonAPI.Enum;
using System.Web;
using Newtonsoft.Json.Linq;

/*
 * Author  - S Salgado
 * Date    - 31/10/2020
 */
namespace SalonAPI.Controllers
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class AppointmentServiceController : ApiController
    {

        /// <summary>
        /// Get all services requested in all appointments
        /// </summary>
        /// <remarks>
        ///  Get all the services requested in all appointments, in all salons
        /// </remarks>
        /// <returns></returns>
        ///  /// <param name="salon_id">Optional parameter. If the salon_id isn't specified, by default it will return all the 
        /// services requested in all appointments in all salons. Else it will only return the services of appointments, in the given salon.</param>
        // GET api/Appointment/Service/GetServicesOfAllAppointments
        [HttpGet]
        [Route("api/Appointment/Service/GetServicesOfAllAppointments")]
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

                    List<tblservice_booked> allServices = entities.tblservice_booked.ToList();
                    if (allServices != null && allServices.Count != 0)
                    {
                        if (salon_id != 0)
                        {
                            foreach (var service in allServices.ToList())
                            {
                                if (!entities.tblappointments.Any(x => x.appointment_id == service.appointment_id && x.salon_id == salon_id))
                                    allServices.Remove(service);
                            }
                        }

                        if (allServices.Count == 0)
                            return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No appointments found!" });


                        List<Object> responses = new List<Object>();
                        responses.Add(new { Success = true, Message = "Services requested in all appointments retrieved successfully!" });

                        foreach (var item in allServices)
                            responses.Add(new
                            {
                                item.id,
                                item.appointment_id,
                                item.service_id,
                                service_name = entities.tblservices.Where(x => x.service_id == item.service_id).Select(x => x.service_name).First()
                            });

                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No services requested in any appointment!", All_appointment_services = allServices });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve services requested in all appointments.");
            }
        }


        /// <summary>
        /// Get selected appointment service
        /// </summary>
        /// <remarks>
        ///  Get the details of a selected service, using the appointment service id
        /// </remarks>
        /// <returns></returns>
        // GET api/Appointment/Service/GetSelectedAppointmentService/5
        [HttpGet]
        [Route("api/Appointment/Service/GetSelectedAppointmentService/{appointment_service_id}")]
        public HttpResponseMessage GetSelectedAppointmentService(int appointment_service_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    tblservice_booked selectedAppointment = entities.tblservice_booked.FirstOrDefault(e => e.id == appointment_service_id);
                    if (selectedAppointment != null)
                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Success = true,
                            Message = "Appointment service details retrieved successfully!",
                            Appointment_service_details = new
                            {
                                selectedAppointment.id,
                                selectedAppointment.appointment_id,
                                selectedAppointment.service_id,
                                service_name = entities.tblservices.Where(x => x.service_id == selectedAppointment.service_id).Select(x => x.service_name).First()
                            }
                        });

                    else
                        return Messages.GetInstance().HandleException("Retrieve failed! Appointment service with id = ", appointment_service_id.ToString());
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve appointment service details.");
            }
        }




        /// <summary>
        /// Get services of an appointment
        /// </summary>
        /// <remarks>
        ///  Get the details of all services requested in a particular appointment in a salon, using the appointment id
        /// </remarks>
        /// <returns></returns>
        // GET api/Appointment/Service/GetServicesOfAppointment/5
        [HttpGet]
        [Route("api/Appointment/Service/GetServicesOfAppointment/{appointment_id}")]
        public HttpResponseMessage GetServicesOfAppointment(int appointment_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // If the appointment id doesn't exists
                    if (entities.tblappointments.FirstOrDefault(e => e.appointment_id == appointment_id) == null)
                        return Messages.GetInstance().HandleException("Retrieve failed! Appointment with id = ", appointment_id.ToString());

                    // If the appointment doesn't have any services
                    if (entities.tblservice_booked.FirstOrDefault(e => e.appointment_id == appointment_id) == null)
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No services found for appointment id = " + appointment_id });

                    else
                    {
                        List<Object> responses = new List<Object>();
                        responses.Add(new { Success = true, Message = "All services requested in appointment id = " + appointment_id + " retrieved successfully!" });

                        var appointment_service = entities.tblservice_booked.Where(p => p.appointment_id.Equals(appointment_id)).Select(p => p.service_id).ToList();
                        foreach (int item in appointment_service)
                        {
                            responses.Add(new
                            {
                                service_id = item,
                                service_name = entities.tblservices.Where(x => x.service_id == item).Select(x => x.service_name).First()
                            });
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve services performed by barber.");
            }
        }




        /// <summary>
        /// Add a new service to an appointment
        /// </summary>
        /// <remarks>
        /// Add a new service to an appointment with the relevant details
        /// </remarks>
        /// <returns></returns>
        // POST api/Appointment/Service/AddService      
        [HttpPost]
        [Route("api/Appointment/Service/AddService")]
        public HttpResponseMessage Post([FromBody] JObject appointment_service_details)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });

                int appointment_id = int.Parse(appointment_service_details["appointment_id"].ToString());
                int service_id = int.Parse(appointment_service_details["service_id"].ToString());

                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Validate appointment - check if the appointment service already exists
                    if (entities.tblservice_booked.Any(e => e.appointment_id == appointment_id && e.service_id == service_id))
                        return Messages.GetInstance().HandleRequest("Appointment Service", ActionType.INSERT, true);
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            tblservice_booked obj = new tblservice_booked
                            {
                                appointment_id = appointment_id,
                                service_id = service_id
                            };
                            entities.tblservice_booked.Add(obj);
                            entities.SaveChanges();

                            Utilities.getInstance().UpdateChanges(entities, transaction, obj.id.ToString(), typeof(tblservice_booked).Name, ActionType.INSERT);

                            return Messages.GetInstance().HandleRequest("Appointment Service", ActionType.INSERT);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to create appointment service.");
            }
        }




        /// <summary>
        /// Delete service from appointment
        /// </summary>
        /// <remarks>
        /// Delete a requested service from an appointment using the appointment service id
        /// </remarks>
        /// <returns></returns>
        // DELETE api/Appointment/Service/DeleteService/5
        [HttpDelete]
        [Route("api/Appointment/Service/DeleteService/{appointment_service_id}")]
        public HttpResponseMessage DELETE(int appointment_service_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    var entity = entities.tblservice_booked.FirstOrDefault(e => e.id == appointment_service_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Delete failed! Appointment service with id = ", appointment_service_id.ToString());
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            entities.tblservice_booked.Remove(entity);
                            Utilities.getInstance().UpdateChanges(entities, transaction, appointment_service_id.ToString(), typeof(tblservice_booked).Name, ActionType.DELETE);

                            return Messages.GetInstance().HandleRequest("Appointment service", ActionType.DELETE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to delete appointment service.");
            }
        }



        /// <summary>
        /// Delete service from appointment
        /// </summary>
        /// <remarks>
        /// Delete a requested service from an appointment using the appointment id & service id
        /// </remarks>
        /// <returns></returns>
        // DELETE api/Appointment/Service/DeleteService/5
        [HttpDelete]
        [Route("api/Appointment/Service/DeleteService/{appointment_id}/{service_id}")]
        public HttpResponseMessage DELETE(int appointment_id, int service_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    var entity = entities.tblservice_booked.FirstOrDefault(e => e.appointment_id == appointment_id && e.service_id == service_id);
                    if (entity == null)
                        return Request.CreateResponse(HttpStatusCode.NotFound, new { Success = false, Message = "Delete failed! No matching entry found." });
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            entities.tblservice_booked.Remove(entity);
                            Utilities.getInstance().UpdateChanges(entities, transaction, entity.id.ToString(), typeof(tblservice_booked).Name, ActionType.DELETE);

                            return Messages.GetInstance().HandleRequest("Appointment service", ActionType.DELETE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to delete appointment service.");
            }
        }


    }
}
