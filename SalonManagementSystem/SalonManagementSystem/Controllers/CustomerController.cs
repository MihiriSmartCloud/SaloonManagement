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

/*
 * Author  - S Salgado
 * Date    - 24/10/2020
 */
namespace SalonAPI.Controllers
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CustomerController : ApiController
    {

        /// <summary>
        /// Get all customers
        /// </summary>
        /// <remarks>
        ///  Get the details of all customers in all salons
        /// </remarks>
        /// <returns></returns>
        /// <param name="salon_id">Optional parameter. If the salon_id isn't specified, by default it will return all the 
        /// customers of all salons. Else it will only return the customers of the given salon.</param>
        // GET api/Customer/GetAllCustomers
        [HttpGet]
        [Route("api/Customer/GetAllCustomers")]
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

                    List<tblcustomer> allCustomers = (salon_id == 0) ? entities.tblcustomers.ToList() : entities.tblcustomers.Where(x => x.salon_id == salon_id).ToList();
                    if (allCustomers != null && allCustomers.Count != 0)
                    {
                        List<Object> responses = new List<Object>();
                        responses.Add(new { Success = true, Message = "Customers retrieved successfully!" });

                        foreach (var item in allCustomers)
                        {
                            responses.Add(new
                            {
                                item.customer_id,
                                item.salon_id,
                                item.name,
                                item.mobile_no,
                                item.login_time
                            });
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No customers found!", All_customers = allCustomers });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve customers.");
            }
        }


        /// <summary>
        /// Get a selected customer
        /// </summary>
        /// <remarks>
        /// Get the details of a particular customer using the customer id
        /// </remarks>
        /// <returns></returns>
        // GET api/Customer/GetSelectedCustomer/5
        [HttpGet]
        [Route("api/Customer/GetSelectedCustomer/{customer_id}")]
        public HttpResponseMessage GetSelectedCustomer(int customer_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    tblcustomer selectedCustomer = entities.tblcustomers.FirstOrDefault(e => e.customer_id == customer_id);
                    if (selectedCustomer != null)
                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Success = true,
                            Message = "Customer retrieved successfully!",
                            Customer_details = new
                            {
                                selectedCustomer.customer_id,
                                selectedCustomer.salon_id,
                                selectedCustomer.name,
                                selectedCustomer.mobile_no,
                                selectedCustomer.login_time
                            }
                        });

                    else
                        return Messages.GetInstance().HandleException("Retrieve failed! Customer with id = ", customer_id.ToString());
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve customer details.");
            }
        }


        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <remarks>
        /// Create a new customer with the relevant details
        /// </remarks>
        /// <returns></returns>
        // POST api/Customer/CreateCustomer       
        [HttpPost]
        [Route("api/Customer/CreateCustomer")]
        public HttpResponseMessage Post([FromBody] JObject customer_details)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                string name = customer_details["name"].ToString().Trim();
                string mobile_no = customer_details["mobile_no"].ToString().Trim();
                int salon_id = int.Parse(customer_details["salon_id"].ToString());

                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if the salon id exists
                    if (salon_id != 0 && !entities.tblsalons.Any(e => e.salon_id == salon_id))
                        return Messages.GetInstance().HandleException("Retrieve failed! Salon with id = ", salon_id.ToString());

                    // Validate customer mobile
                    if (!Utilities.getInstance().ValidateContactNumber(mobile_no))
                        return Messages.GetInstance().ValidateFields("Customer", ActionType.INSERT, isContactNumber: true);

                    // Check if the customer mobile already exists in the particular salon. 
                    if (entities.tblcustomers.Any(e => e.mobile_no.ToString().Trim() == mobile_no && e.salon_id == salon_id))
                        return Messages.GetInstance().HandleException("Failed to create customer! A customer with the same mobile no exists in salon id = " + salon_id);

                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            tblcustomer obj = new tblcustomer
                            {
                                name = name,
                                mobile_no = int.Parse(mobile_no),
                                salon_id = salon_id,
                                login_time = DateTime.Now
                            };
                            entities.tblcustomers.Add(obj);
                            entities.SaveChanges();

                            Utilities.getInstance().UpdateChanges(entities, transaction, obj.customer_id.ToString(), typeof(tblcustomer).Name, ActionType.INSERT);

                            return Messages.GetInstance().HandleRequest("Customer", ActionType.INSERT);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to create customer.");
            }
        }


        /// <summary>
        /// Update an existing customer
        /// </summary>
        /// <remarks>
        /// Update the details of an existing customer using the customer id
        /// </remarks>
        /// <returns></returns>
        // PUT api/Customer/UpdateCustomer/5
        [HttpPut]
        [Route("api/Customer/UpdateCustomer/{customer_id}")]
        public HttpResponseMessage Put(int customer_id, [FromBody] JObject customer_details)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if a customer with the specified Id exist
                    var entity = entities.tblcustomers.FirstOrDefault(e => e.customer_id == customer_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Update failed! Customer with id = ", customer_id.ToString());

                    string name = customer_details["name"].ToString().Trim();
                    string mobile_no = customer_details["mobile_no"].ToString().Trim();

                    // Validate customer mobile
                    if (!Utilities.getInstance().ValidateContactNumber(mobile_no))
                        return Messages.GetInstance().ValidateFields("Customer", ActionType.UPDATE, isContactNumber: true);

                    else
                    {
                        // Check if the customer mobile already exists in the particular salon.
                        var customer = entities.tblcustomers.Where(p => p.salon_id == entity.salon_id && p.mobile_no.ToString().Trim() == mobile_no).Any();
                        if (entity.mobile_no.ToString().Trim() != mobile_no && customer)
                            return Messages.GetInstance().HandleException("Update failed! A customer with the same mobile no already exists in salon id = " + entity.salon_id);

                        else
                        {
                            using (var transaction = entities.Database.BeginTransaction())
                            {
                                // Update necessary data fielsds
                                entity.name = name;
                                entity.mobile_no = int.Parse(mobile_no);

                                Utilities.getInstance().UpdateChanges(entities, transaction, customer_id.ToString(), typeof(tblcustomer).Name, ActionType.UPDATE);

                                return Messages.GetInstance().HandleRequest("Customer", ActionType.UPDATE);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to update customer details.");
            }
        }


        /// <summary>
        /// Delete an existing customer
        /// </summary>
        /// <remarks>
        /// Delete an existing customer using the customer id
        /// </remarks>
        /// <returns></returns>
        // DELETE api/Customer/DeleteCustomer/5
        [HttpDelete]
        [Route("api/Customer/DeleteCustomer/{customer_id}")]
        public HttpResponseMessage DELETE(int customer_id)
        {
            try
            {
                // Check if a session already exists or if it's expired
                //if (HttpContext.Current.Session["Token"] == null)
                //    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Session expired! Unable to authenticate user." });


                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    var entity = entities.tblcustomers.FirstOrDefault(e => e.customer_id == customer_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Delete failed! Customer with id = ", customer_id.ToString());
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            entities.tblcustomers.Remove(entity);
                            Utilities.getInstance().UpdateChanges(entities, transaction, customer_id.ToString(), typeof(tblcustomer).Name, ActionType.DELETE);

                            return Messages.GetInstance().HandleRequest("Customer", ActionType.DELETE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to delete customer.");
            }
        }


    }
}
