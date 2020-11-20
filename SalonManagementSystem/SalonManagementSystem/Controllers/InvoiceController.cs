using Newtonsoft.Json.Linq;
using SalonAPI.Common.Utility;
using SalonAPI.Enum;
using SalonDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


/*
 * Author  - S Salgado
 * Date    - 28/10/2020
 */
namespace SalonAPI.Controllers
{

    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class InvoiceController : ApiController
    {

        /// <summary>
        /// Get all invoices
        /// </summary>
        /// <remarks>
        ///  Get the details of all invoices in all salons
        /// </remarks>
        /// <returns></returns>
        /// <param name="salon_id">Optional parameter. If the salon_id isn't specified, by default it will return all the 
        /// invoices of all salons. Else it will only return the invoices of the given salon.</param>
        // GET api/Invoice/GetAllInvoices
        [HttpGet]
        [Route("api/Invoice/GetAllInvoices")]
        public HttpResponseMessage GetAllInvoices(int salon_id = 0)
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

                    List<tblinvoice> allInvoices = (salon_id == 0) ? entities.tblinvoices.ToList() : entities.tblinvoices.Where(x => x.salon_id == salon_id).ToList();

                    if (allInvoices != null && allInvoices.Count != 0)
                    {
                        List<Object> responses = new List<Object>();
                        responses.Add(new { Success = true, Message = "Invoices retrieved successfully!" });

                        List<Object> services = new List<Object>();
                        foreach (var item in allInvoices)
                        {
                            int customerId = entities.tblappointments.Where(x => x.appointment_id == item.appointment_id).Select(x => x.customer_id).FirstOrDefault();
                            var requestedServices = entities.tblservice_booked.Where(x => x.appointment_id == item.appointment_id).Select(x => x.service_id).ToArray();
                            foreach (int serviceId in requestedServices)
                            {
                                services.Add(new
                                {
                                    serviceId,
                                    service_name = entities.tblservices.Where(x => x.service_id == serviceId).Select(x => x.service_name).First(),
                                    price = entities.tblservices.Where(x => x.service_id == serviceId).Select(x => x.price).First()
                                });
                            }

                            responses.Add(new
                            {
                                item.invoice_id,
                                date_created = entities.tblappointments.Where(x => x.appointment_id == item.appointment_id).Select(x => x.due_date).First(),
                                item.salon_id,
                                salon_name = entities.tblsalons.Where(x => x.salon_id == item.salon_id).Select(x => x.salon_name).First(),
                                customer_id = customerId,
                                customer_name = entities.tblcustomers.Where(x => x.customer_id == customerId).Select(x => x.name).FirstOrDefault(),
                                item.appointment_id,
                                offered_services = services,
                                item.total_price,
                                item.discount,
                                item.final_price
                            });

                            services = new List<Object>();
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No invoices found!", All_invoices = allInvoices });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve invoices.");
            }
        }




        /// <summary>
        /// Get a selected invoice
        /// </summary>
        /// <remarks>
        /// Get the details of a particular invoice using the invoice id
        /// </remarks>
        /// <returns></returns>
        // GET api/Invoice/GetSelectedInvoice/5
        [HttpGet]
        [Route("api/Invoice/GetSelectedInvoice/{invoice_id}")]
        public HttpResponseMessage Get(int invoice_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    tblinvoice selectedInvoice = entities.tblinvoices.FirstOrDefault(e => e.invoice_id == invoice_id);
                    if (selectedInvoice != null)
                    {
                        List<Object> services = new List<Object>();

                        int customerId = entities.tblappointments.Where(x => x.appointment_id == selectedInvoice.appointment_id).Select(x => x.customer_id).FirstOrDefault();

                        var requestedServices = entities.tblservice_booked.Where(x => x.appointment_id == selectedInvoice.appointment_id).Select(x => x.service_id).ToArray();
                        foreach (int serviceId in requestedServices)
                            services.Add(new
                            {
                                serviceId,
                                service_name = entities.tblservices.Where(x => x.service_id == serviceId).Select(x => x.service_name).First(),
                                price = entities.tblservices.Where(x => x.service_id == serviceId).Select(x => x.price).First()
                            });


                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Success = true,
                            Message = "Invoice retrieved successfully!",
                            Invoice_details = new
                            {
                                selectedInvoice.invoice_id,
                                date_created = entities.tblappointments.Where(x => x.appointment_id == selectedInvoice.appointment_id).Select(x => x.due_date).First(),
                                selectedInvoice.salon_id,
                                salon_name = entities.tblsalons.Where(x => x.salon_id == selectedInvoice.salon_id).Select(x => x.salon_name).First(),
                                customer_id = customerId,
                                customer_name = entities.tblcustomers.Where(x => x.customer_id == customerId).Select(x => x.name).FirstOrDefault(),
                                selectedInvoice.appointment_id,
                                offered_services = services,
                                selectedInvoice.total_price,
                                selectedInvoice.discount,
                                selectedInvoice.final_price
                            }
                        });
                    }
                    else
                        return Messages.GetInstance().HandleException("Retrieve failed! Invoice with id = ", invoice_id.ToString());
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve invoice details.");
            }
        }





        /// <summary>
        /// Create a new invoice
        /// </summary>
        /// <remarks>
        /// Create a new invoice with the relevant details
        /// </remarks>
        /// <returns></returns>
        // POST api/Invoice/CreateInvoice       
        [HttpPost]
        [Route("api/Invoice/CreateInvoice")]
        public HttpResponseMessage Post([FromBody] JObject invoice_details)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    int salon_id = int.Parse(invoice_details["salon_id"].ToString());
                    int appointment_id = int.Parse(invoice_details["appointment_id"].ToString());
                    decimal discount = decimal.Parse(invoice_details["discount"].ToString());

                    int[] requested_services = new int[invoice_details["requested_services"].Count()];
                    int count = 0;
                    foreach (var service in invoice_details["requested_services"])
                    {
                        requested_services[count] = int.Parse(invoice_details["requested_services"][count].ToString());
                        count++;
                    }

                    // Check if an invoice is already existing for the particular appointment
                    if (entities.tblinvoices.Any(e => e.appointment_id == appointment_id))
                        return Messages.GetInstance().HandleException("Failed to create invoice! An invoice already exists for appointment id = " + appointment_id);

                    // Check if the requested services exist in the given salon
                    foreach (int service in requested_services)
                    {
                        if (!entities.tblservices.Any(x => x.salon_id == salon_id && x.service_id == service))
                            return Messages.GetInstance().HandleException("Failed to create invoice! Requested service doesn't exist in the given salon.");
                    }

                    // Check if the services have been requested in the given appointment
                    foreach (int service in requested_services)
                    {
                        if (!entities.tblservice_booked.Any(x => x.appointment_id == appointment_id && x.service_id == service))
                            return Messages.GetInstance().HandleException("Failed to create invoice! Service id = " + service + " has not been requested, in the given appointment.");
                    }


                    using (var transaction = entities.Database.BeginTransaction())
                    {
                        decimal totalPrice = CalculateTotal(salon_id, requested_services);
                        tblinvoice invoice = new tblinvoice
                        {
                            salon_id = salon_id,
                            appointment_id = appointment_id,
                            total_price = CalculateTotal(salon_id, requested_services),
                            discount = discount,
                            final_price = CalculateFinalTotal(totalPrice, discount)
                        };
                        entities.tblinvoices.Add(invoice);
                        entities.SaveChanges();

                        Utilities.getInstance().UpdateChanges(entities, transaction, invoice.invoice_id.ToString(), typeof(tblinvoice).Name, ActionType.INSERT);

                        return Messages.GetInstance().HandleRequest("Invoice", ActionType.INSERT);
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to create invoice.");
            }
        }





        /// <summary>
        /// Generate the invoice automatically for an appointment
        /// </summary>
        /// <remarks>
        /// Generate the invoice automatically, for a particular appointment in the given salon, with the relevant details
        /// </remarks>
        /// <returns></returns>
        /// <param name="discount">Optional parameter. Indicates the amount to be reduced from the total price. Ex: 100.00 or 100</param> 
        // POST api/Invoice/GenerateInvoiceAutomatically       
        [HttpPost]
        [Route("api/Invoice/GenerateInvoiceAutomatically")]
        public HttpResponseMessage GenerateInvoiceAutomatically(int salon_id, int appointment_id, decimal discount = 0)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if an invoice is already existing for the particular appointment
                    if (entities.tblinvoices.Any(e => e.appointment_id == appointment_id))
                        return Messages.GetInstance().HandleException("Failed to create invoice! An invoice already exists for appointment id = " + appointment_id);

                    using (var transaction = entities.Database.BeginTransaction())
                    {
                        int[] requested_services = entities.tblservice_booked.Where(x => x.appointment_id == appointment_id).Select(x => x.service_id).ToArray();

                        decimal totalPrice = CalculateTotal(salon_id, requested_services);
                        tblinvoice invoice = new tblinvoice
                        {
                            salon_id = salon_id,
                            appointment_id = appointment_id,
                            total_price = totalPrice,
                            discount = discount,
                            final_price = CalculateFinalTotal(totalPrice, discount),
                        };
                        entities.tblinvoices.Add(invoice);
                        entities.SaveChanges();

                        Utilities.getInstance().UpdateChanges(entities, transaction, invoice.invoice_id.ToString(), typeof(tblinvoice).Name, ActionType.INSERT);

                        return Messages.GetInstance().HandleRequest("Invoice", ActionType.INSERT);
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to create invoice.");
            }
        }







        /// <summary>
        /// Update an existing invoice
        /// </summary>
        /// <remarks>
        /// Update an existing invoice with the relevant details
        /// </remarks>
        /// <returns></returns>
        // POST api/Invoice/UpdateInvoice/1/5  
        [HttpPut]
        [Route("api/Invoice/UpdateInvoice/{invoice_id}")]
        public HttpResponseMessage Put(int invoice_id, [FromBody] JObject invoice_details)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                int salon_id = int.Parse(invoice_details["salon_id"].ToString());
                int appointment_id = int.Parse(invoice_details["appointment_id"].ToString());
                decimal discount = decimal.Parse(invoice_details["discount"].ToString());

                int[] requested_services = new int[invoice_details["requested_services"].Count()];
                int count = 0;
                foreach (var service in invoice_details["requested_services"])
                {
                    requested_services[count] = int.Parse(invoice_details["requested_services"][count].ToString());
                    count++;
                }


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    using (var transaction = entities.Database.BeginTransaction())
                    {
                        // Check if an invoice with the specified id exist
                        var entity = entities.tblinvoices.FirstOrDefault(e => e.invoice_id == invoice_id);
                        if (entity == null)
                            return Messages.GetInstance().HandleException("Update failed! Invoice with id = ", invoice_id.ToString());

                        //Check if an invoice already exists for the particular appointment
                        var invoice = entities.tblinvoices.Where(p => p.appointment_id == appointment_id).Any();
                        if (entity.appointment_id != appointment_id && invoice)
                            return Messages.GetInstance().HandleException("Update failed! An invoice already exists for appointment id = " + appointment_id);

                        // Check if the requested services exist in the given salon
                        foreach (int service in requested_services)
                        {
                            if (!entities.tblservices.Any(x => x.salon_id == salon_id && x.service_id == service))
                                return Messages.GetInstance().HandleException("Update failed! Requested service doesn't exist in the given salon.");
                        }

                        // Check if the services have been requested in the given appointment
                        foreach (int service in requested_services)
                        {
                            if (!entities.tblservice_booked.Any(x => x.appointment_id == appointment_id && x.service_id == service))
                                return Messages.GetInstance().HandleException("Update failed! Service id = " + service + " has not been requested, in the given appointment.");
                        }


                        decimal totalPrice = CalculateTotal(salon_id, requested_services);

                        // Update necessary fielsds
                        entity.appointment_id = appointment_id;
                        entity.total_price = totalPrice;
                        entity.discount = discount;
                        entity.final_price = CalculateFinalTotal(totalPrice, discount);

                        Utilities.getInstance().UpdateChanges(entities, transaction, entity.invoice_id.ToString(), typeof(tblinvoice).Name, ActionType.UPDATE);

                        return Messages.GetInstance().HandleRequest("Invoice", ActionType.UPDATE);
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to update invoice details.");
            }
        }



        /// <summary>
        /// Delete an existing invoice
        /// </summary>
        /// <remarks>
        /// Delete an existing invoice in a salon, using the invoice id
        /// </remarks>
        /// <returns></returns>
        // DELETE api/Invoice/DeleteInvoice/5
        [HttpDelete]
        [Route("api/Invoice/DeleteInvoice/{invoice_id}")]
        public HttpResponseMessage DELETE(int invoice_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    var entity = entities.tblinvoices.FirstOrDefault(e => e.invoice_id == invoice_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Delete failed! Invoice with id = ", invoice_id.ToString());
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            entities.tblinvoices.Remove(entity);
                            Utilities.getInstance().UpdateChanges(entities, transaction, invoice_id.ToString(), typeof(tblinvoice).Name, ActionType.DELETE);

                            return Messages.GetInstance().HandleRequest("Invoice", ActionType.DELETE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to delete invoice.");
            }
        }



        [NonAction]
        // Calculates the total price, for the requested services
        public decimal CalculateTotal(int salon_id, int[] requested_services)
        {
            decimal total = 0;
            using (SalonDbEntities entities = new SalonDbEntities())
            {
                foreach (int service in requested_services)
                    // Get the price of the service
                    total += entities.tblservices.Where(e => e.salon_id == salon_id && e.service_id == service).Select(x => x.price).First();

            }
            return total;
        }


        [NonAction]
        // Calculates the discounted final price, for the appointment
        public decimal CalculateFinalTotal(decimal total_price, decimal discount) => total_price - discount;


    }
}
