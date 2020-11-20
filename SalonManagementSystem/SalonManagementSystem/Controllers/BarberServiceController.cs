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
    public class BarberServiceController : ApiController
    {

        /// <summary>
        /// Get all services performed by all barbers
        /// </summary>
        /// <remarks>
        ///  Get all the services done by all barbers, in all salons
        /// </remarks>
        /// <returns></returns>
        ///  /// <param name="salon_id">Optional parameter. If the salon_id isn't specified, by default it will return all the 
        /// services performed by all barbers in all salons. Else it will only return the services of barbers in the given salon.</param>
        // GET api/Barber/Service/GetServicesOfAllBarbers
        [HttpGet]
        [Route("api/Barber/Service/GetServicesOfAllBarbers")]
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

                    List<tblbarber_service> allServices = entities.tblbarber_service.ToList();
                    if (allServices != null && allServices.Count != 0)
                    {
                        if (salon_id != 0)
                        {
                            foreach (var service in allServices.ToList())
                            {
                                if (!entities.tblbarbers.Any(x => x.barber_id == service.barber_id && x.salon_id == salon_id))
                                    allServices.Remove(service);
                            }
                        }

                        if (allServices.Count == 0)
                            return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No services found!" });


                        List<Object> responses = new List<Object>();
                        responses.Add(new { Success = true, Message = "Services performed by all barbers retrieved successfully!" });

                        foreach (var item in allServices)
                            responses.Add(new
                            {
                                item.barber_service_id,
                                item.barber_id,
                                barber_name = entities.tblbarbers.Where(x => x.barber_id == item.barber_id).Select(x => x.barber_name).First(),
                                item.service_id,
                                service_name = entities.tblservices.Where(x => x.service_id == item.service_id).Select(x => x.service_name).First()
                            });

                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No services performed by any barber!", All_barber_services = allServices });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve services performed by all barbers.");
            }
        }


        /// <summary>
        /// Get selected barber service
        /// </summary>
        /// <remarks>
        ///  Get the details of a selected service, using the barber service id
        /// </remarks>
        /// <returns></returns>
        // GET api/Barber/Service/GetSelectedBarberService/5
        [HttpGet]
        [Route("api/Barber/Service/GetSelectedBarberService/{barber_service_id}")]
        public HttpResponseMessage GetSelectedBarberService(int barber_service_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    tblbarber_service selectedBarber = entities.tblbarber_service.FirstOrDefault(e => e.barber_service_id == barber_service_id);
                    if (selectedBarber != null)
                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Success = true,
                            Message = "Barber service details retrieved successfully!",
                            Barber_service_details = new
                            {
                                selectedBarber.barber_service_id,
                                selectedBarber.barber_id,
                                barber_name = entities.tblbarbers.Where(x => x.barber_id == selectedBarber.barber_id).Select(x => x.barber_name).First(),
                                selectedBarber.service_id,
                                service_name = entities.tblservices.Where(x => x.service_id == selectedBarber.service_id).Select(x => x.service_name).First()

                            }
                        });

                    else
                        return Messages.GetInstance().HandleException("Retrieve failed! Barber service with id = ", barber_service_id.ToString());
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve barber service details.");
            }
        }




        /// <summary>
        /// Get all services of a barber
        /// </summary>
        /// <remarks>
        ///  Get the details of all services done by a particular barber in a salon, using the barber id
        /// </remarks>
        /// <returns></returns>
        // GET api/Barber/Service/GetServicesOfBarber/5
        [HttpGet]
        [Route("api/Barber/Service/GetServicesOfBarber/{barber_id}")]
        public HttpResponseMessage GetServicesOfBarber(int barber_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // If the barber id doesn't exists
                    if (entities.tblbarbers.FirstOrDefault(e => e.barber_id == barber_id) == null)
                        return Messages.GetInstance().HandleException("Retrieve failed! Barber with id = ", barber_id.ToString());

                    // If the barber doesn't have any services
                    if (entities.tblbarber_service.FirstOrDefault(e => e.barber_id == barber_id) == null)
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No services found for barber id = " + barber_id });

                    else
                    {
                        List<Object> responses = new List<Object>();
                        responses.Add(new
                        {
                            Success = true,
                            Message = "All services performed by barber id = " + barber_id + " retrieved successfully!"
                        });

                        var barber_service = entities.tblbarber_service.Where(p => p.barber_id.Equals(barber_id)).Select(x => x.service_id).ToList();
                        foreach (int item in barber_service)
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
        /// Create a new barber service
        /// </summary>
        /// <remarks>
        /// Create a new barber service with the relevant details
        /// </remarks>
        /// <returns></returns>
        // POST api/Barber/Service/CreateBarberService      
        [HttpPost]
        [Route("api/Barber/Service/CreateService")]
        public HttpResponseMessage Post([FromBody] JObject barber_service_details)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    int barber_id = int.Parse(barber_service_details["barber_id"].ToString());
                    int service_id = int.Parse(barber_service_details["service_id"].ToString());

                    // Validate barber - check if the barber service already exists
                    if (entities.tblbarber_service.Any(e => e.barber_id == barber_id && e.service_id == service_id))
                        return Messages.GetInstance().HandleRequest("Barber Service", ActionType.INSERT, true);
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            tblbarber_service obj = new tblbarber_service
                            {
                                barber_id = barber_id,
                                service_id = service_id
                            };
                            entities.tblbarber_service.Add(obj);
                            entities.SaveChanges();

                            Utilities.getInstance().UpdateChanges(entities, transaction, obj.barber_service_id.ToString(), typeof(tblbarber).Name, ActionType.INSERT);

                            return Messages.GetInstance().HandleRequest("Barber Service", ActionType.INSERT);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to create barber service.");
            }
        }




        /// <summary>
        /// Delete an existing barber service
        /// </summary>
        /// <remarks>
        /// Delete an existing barber service using the barber service id
        /// </remarks>
        /// <returns></returns>
        // DELETE api/Barber/Service/DeleteService/5
        [HttpDelete]
        [Route("api/Barber/Service/DeleteService/{barber_service_id}")]
        public HttpResponseMessage DELETE(int barber_service_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    var entity = entities.tblbarber_service.FirstOrDefault(e => e.barber_service_id == barber_service_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Delete failed! Barber service with id = ", barber_service_id.ToString());
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            entities.tblbarber_service.Remove(entity);
                            Utilities.getInstance().UpdateChanges(entities, transaction, barber_service_id.ToString(), typeof(tblbarber).Name, ActionType.DELETE);

                            return Messages.GetInstance().HandleRequest("Barber service", ActionType.DELETE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to delete barber service.");
            }
        }




        /// <summary>
        /// Delete service from barber
        /// </summary>
        /// <remarks>
        /// Delete a service offered by a barber
        /// </remarks>
        /// <returns></returns>
        // DELETE api/Barber/Service/DeleteService/5
        [HttpDelete]
        [Route("api/Barber/Service/DeleteService/{barber_id}/{service_id}")]
        public HttpResponseMessage DELETE(int barber_id, int service_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    var entity = entities.tblbarber_service.FirstOrDefault(e => e.barber_id == barber_id && e.service_id == service_id);
                    if (entity == null)
                        return Request.CreateResponse(HttpStatusCode.NotFound, new { Success = false, Message = "Delete failed! No matching entry found." });
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            entities.tblbarber_service.Remove(entity);
                            Utilities.getInstance().UpdateChanges(entities, transaction, entity.barber_service_id.ToString(), typeof(tblbarber_service).Name, ActionType.DELETE);

                            return Messages.GetInstance().HandleRequest("Barber service", ActionType.DELETE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to delete barber service.");
            }
        }



    }
}
