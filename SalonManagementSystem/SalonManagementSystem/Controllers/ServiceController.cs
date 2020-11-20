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
    public class ServiceController : ApiController
    {

        /// <summary>
        /// Get all services
        /// </summary>
        /// <remarks>
        ///  Get the details of all available services. If the salon_id isn't specified, it will retrieve all the available services in all salons. Else it will only retrieve the services available in the specified salon.
        /// </remarks>
        /// <returns></returns>
        /// <param name="salon_id">Optional parameter </param>
        // GET api/Salon/Service/GetAllServices
        [HttpGet]
        [Route("api/Salon/Service/GetAllServices")]
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

                    // If the salon id isn't specified, then retrieve all services, else get only the services available in the specified salon
                    List<tblservice> allAvailableServices = (salon_id == 0) ? entities.tblservices.ToList() : entities.tblservices.Where(s => s.salon_id == salon_id).ToList();

                    if (allAvailableServices != null && allAvailableServices.Count != 0)
                    {
                        List<Object> responses = new List<Object>();
                        responses.Add(new { Success = true, Message = "Services retrieved successfully!" });

                        foreach (var item in allAvailableServices)
                            responses.Add(new
                            {
                                item.service_id,
                                item.service_name,
                                item.salon_id,
                                item.price,
                                item.duration
                            });

                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No services found!", All_services = allAvailableServices });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve services.");
            }
        }


        /// <summary>
        /// Get a selected service
        /// </summary>
        /// <remarks>
        /// Get the details of a particular service available in a salon, using the service id
        /// </remarks>
        /// <returns></returns>
        // GET api/Salon/Service/GetSelectedService/5
        [HttpGet]
        [Route("api/Salon/Service/GetSelectedService/{service_id}")]
        public HttpResponseMessage GetSelectedService(int service_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    tblservice selectedService = entities.tblservices.FirstOrDefault(e => e.service_id == service_id);
                    if (selectedService != null)
                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Success = true,
                            Message = "Service retrieved successfully!",
                            Service_details = new
                            {
                                selectedService.service_id,
                                selectedService.service_name,
                                selectedService.salon_id,
                                selectedService.price,
                                selectedService.duration
                            }
                        });

                    else
                        return Messages.GetInstance().HandleException("Retrieve failed! Service with id = ", service_id.ToString());
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve service details.");
            }
        }


        /// <summary>
        /// Create a new service
        /// </summary>
        /// <remarks>
        /// Create a new service with the relevant details
        /// </remarks>
        /// <returns></returns>
        /// <param name="price">Ex: 500.00 or 500</param>
        /// <param name="duration">Allowed duration format: HH:mm:ss  Ex: 01:00:00 </param>
        // POST api/Salon/Service/CreateService       
        [HttpPost]
        [Route("api/Salon/Service/CreateService")]
        public HttpResponseMessage Post([FromBody] JObject service_details)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                string service_name = service_details["service_name"].ToString().Trim();
                int salon_id = int.Parse(service_details["salon_id"].ToString());
                Decimal price = Decimal.Parse(service_details["price"].ToString());
                string duration = service_details["duration"].ToString().Trim();

                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Validate service - check if the service already exists in that particular salon
                    bool selectedService = entities.tblservices.Any(e => e.service_name.ToUpper().Trim() == service_name.ToUpper().Trim() && e.salon_id == salon_id);

                    // If a service already exists
                    if (selectedService)
                        return Messages.GetInstance().HandleRequest("Service", ActionType.INSERT, true);
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            tblservice obj = new tblservice
                            {
                                service_name = service_name,
                                salon_id = salon_id,
                                price = price,
                                duration = Convert.ToInt32(TimeSpan.Parse(duration).TotalSeconds)
                            };
                            entities.tblservices.Add(obj);
                            entities.SaveChanges();

                            Utilities.getInstance().UpdateChanges(entities, transaction, obj.service_id.ToString(), typeof(tblservice).Name, ActionType.INSERT);

                            return Messages.GetInstance().HandleRequest("Service", ActionType.INSERT);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to create service.");
            }
        }



        /// <summary>
        /// Update an existing service
        /// </summary>
        /// <remarks>
        /// Update the details of an existing service available in a salon, using the service id
        /// </remarks>
        /// <returns></returns>
        /// <param name="price">Ex: 500.00 or 500</param>
        /// <param name="duration">Allowed duration format: HH:mm:ss  Ex: 01:00:00 </param>
        // PUT api/Salon/Service/UpdateService/5
        [HttpPut]
        [Route("api/Salon/Service/UpdateService/{service_id}")]
        public HttpResponseMessage Put(int service_id, [FromBody] JObject service_details)
        {
            try
            {
                //// Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    string service_name = service_details["service_name"].ToString().Trim();
                    Decimal price = Decimal.Parse(service_details["price"].ToString());
                    string duration = service_details["duration"].ToString().Trim();

                    // Check if a service with the specified id exist
                    var entity = entities.tblservices.FirstOrDefault(e => e.service_id == service_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Update failed! Service with id = ", service_id.ToString());

                    // If a service with the specified id exists
                    else
                    {
                        // Check for duplicates - check if the service already exists in the particular salon
                        bool selectedService = entities.tblservices.Any(e => e.service_name.ToUpper().Trim() == service_name.ToUpper().Trim() && e.salon_id == entity.salon_id);

                        // If a another service already exists with the entered name & salon id 
                        if (entity.service_name.ToUpper().Trim() != service_name.ToUpper().Trim() && selectedService)
                            return Messages.GetInstance().HandleRequest("Service", ActionType.UPDATE, true);

                        // If a service doesn't exist then update the service
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            // Update necessary fields
                            entity.service_name = service_name;
                            entity.price = price;
                            entity.duration = Convert.ToInt32(TimeSpan.Parse(duration).TotalSeconds);

                            Utilities.getInstance().UpdateChanges(entities, transaction, service_id.ToString(), typeof(tblservice).Name, ActionType.UPDATE);

                            return Messages.GetInstance().HandleRequest("Service", ActionType.UPDATE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to update service details.");
            }
        }


        /// <summary>
        /// Delete an existing service
        /// </summary>
        /// <remarks>
        /// Delete an existing service available in a salon, using the service id
        /// </remarks>
        /// <returns></returns>
        // DELETE api/Salon/Service/DeleteService/5
        [HttpDelete]
        [Route("api/Salon/Service/DeleteService/{service_id}")]
        public HttpResponseMessage DELETE(int service_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    var entity = entities.tblservices.FirstOrDefault(e => e.service_id == service_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Delete failed! Service with id = ", service_id.ToString());
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            entities.tblservices.Remove(entity);
                            Utilities.getInstance().UpdateChanges(entities, transaction, service_id.ToString(), typeof(tblservice).Name, ActionType.DELETE);

                            return Messages.GetInstance().HandleRequest("Service", ActionType.DELETE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to delete service.");
            }
        }


    }
}


