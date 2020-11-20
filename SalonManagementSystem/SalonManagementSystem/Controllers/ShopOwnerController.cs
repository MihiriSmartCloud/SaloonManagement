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
using System.Text.RegularExpressions;


/*
 * Author  - S Salgado
 * Date    - 24/10/2020
 */
namespace SalonAPI.Controllers
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ShopOwnerController : ApiController
    {

        /// <summary>
        /// Get all shop owners
        /// </summary>
        /// <remarks>
        ///  Get the details of all registered shop owners
        /// </remarks>
        /// <returns></returns>
        // GET api/ShopOwner/GetAllOwners
        [HttpGet]
        [Route("api/ShopOwner/GetAllOwners")]
        public HttpResponseMessage Get()
        {
            try
            {
                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    List<tblshop_owner> allShopOwners = entities.tblshop_owner.ToList();
                    if (allShopOwners != null && allShopOwners.Count != 0)
                    {
                        List<Object> responses = new List<Object>();
                        responses.Add(new { Success = true, Message = "Shop owners retrieved successfully!" });

                        List<Object> salon = new List<Object>();
                        foreach (var item in allShopOwners)
                        {
                            var salons = entities.tblsalons.Where(x => x.owner_id == item.owner_id).Select(x => x.salon_id).ToArray();
                            foreach (int salonId in salons)
                                salon.Add(new
                                {
                                    salonId,
                                    salon_name = entities.tblsalons.Where(x => x.salon_id == salonId).Select(x => x.salon_name).First(),
                                });


                            responses.Add(new
                            {
                                item.owner_id,
                                item.name,
                                item.contact_no,
                                item.email,
                                item.password,
                                item.pin,
                                owning_salons = salon
                            });

                            salon = new List<Object>();
                        }
                        return Request.CreateResponse(HttpStatusCode.OK, responses);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = "No shop owners found!", All_shop_owners = allShopOwners });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve shop owners.");
            }
        }


        /// <summary>
        /// Get a selected shop owner
        /// </summary>
        /// <remarks>
        /// Get the details of a particular shop owner using the owner id
        /// </remarks>
        /// <returns></returns>
        // GET api/ShopOwner/GetSelectedOwner/5
        [HttpGet]
        [Route("api/ShopOwner/GetSelectedOwner/{owner_id}")]
        public HttpResponseMessage Get(int owner_id)
        {
            try
            {
                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    tblshop_owner selectedOwner = entities.tblshop_owner.FirstOrDefault(e => e.owner_id == owner_id);
                    if (selectedOwner != null)
                    {
                        List<Object> salon = new List<Object>();
                        var salons = entities.tblsalons.Where(x => x.owner_id == selectedOwner.owner_id).Select(x => x.salon_id).ToArray();
                        foreach (int salonId in salons)
                            salon.Add(new
                            {
                                salonId,
                                salon_name = entities.tblsalons.Where(x => x.salon_id == salonId).Select(x => x.salon_name).First(),
                            });


                        return Request.CreateResponse(HttpStatusCode.OK, new
                        {
                            Success = true,
                            Message = "Shop owner retrieved successfully!",
                            Shop_owner_details = new
                            {
                                selectedOwner.owner_id,
                                selectedOwner.name,
                                selectedOwner.contact_no,
                                selectedOwner.email,
                                selectedOwner.password,
                                selectedOwner.pin,
                                owning_salons = salon
                            }
                        });
                    }
                    else
                        return Messages.GetInstance().HandleException("Retrieve failed! Shop owner with id = ", owner_id.ToString());
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to retrieve shop owner details.");
            }
        }



        /// <summary>
        /// Create a new shop owner
        /// </summary>
        /// <remarks>
        /// Create a new shop owner with the relevant details
        /// </remarks>
        /// <returns></returns>
        // POST api/ShopOwner/CreateOwner       
        [Route("api/ShopOwner/CreateOwner")]
        public HttpResponseMessage Post([FromBody] JObject owner_details)
        {
            try
            {
                string name = owner_details["name"].ToString().Trim();
                string contact_no = owner_details["contact_no"].ToString().Trim();
                string pin = owner_details["pin"].ToString().Trim();
                string password = owner_details["password"].ToString().Trim();

                string email = null;
                if (owner_details["email"] != null)
                    email = owner_details["email"].ToString().Trim();

                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Validate the contact no
                    if (!Utilities.getInstance().ValidateContactNumber(contact_no))
                        return Messages.GetInstance().ValidateFields("Shop owner", ActionType.INSERT, isContactNumber: true);

                    // Validates the email
                    if (email != null && !Utilities.getInstance().ValidateEmail(email))
                        return Messages.GetInstance().ValidateFields("Shop owner", ActionType.INSERT, isEmail: true);

                    // Validates the pin
                    if (pin.Count() != 5 || !Regex.IsMatch(pin, @"^\d{5}$"))
                        return Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = "Failed to create shop owner! Received invalid pin. Hint: Pin should contain only 5 digits." });

                    // Validates the password
                    if (!Utilities.getInstance().ValidatePassword(password))
                        return Messages.GetInstance().ValidateFields("Shop owner", ActionType.INSERT, isPassword: true);

                    // Check if another shop owner already exists with the same contact no or email or username
                    if (entities.tblshop_owner.Any(e => e.contact_no.ToString() == contact_no))
                        return Messages.GetInstance().HandleException("Failed to create shop owner! Contact number already exists.");

                    // Checks if the user pin alreeady exists
                    var userPins = entities.tblshop_owner.Select(x => x.pin).ToList();
                    foreach (string o in userPins)
                    {
                        if (Utilities.getInstance().DecodeFrom64(o) == pin)
                            return Messages.GetInstance().HandleException("Failed to create shop owner! Pin already exists.");
                    }


                    if (email != null && entities.tblshop_owner.Any(e => e.email != null && e.email == email))
                        return Messages.GetInstance().HandleException("Failed to create shop owner! Email already exists.");

                    else
                    {
                        // Add the new shop owner
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            tblshop_owner owner = new tblshop_owner
                            {
                                name = name,
                                contact_no = int.Parse(contact_no),
                                email = email,
                                password = Utilities.getInstance().CalculateHash(password),
                                pin = Utilities.getInstance().CalculateHash(pin)
                            };
                            entities.tblshop_owner.Add(owner);
                            entities.SaveChanges();

                            Utilities.getInstance().UpdateChanges(entities, transaction, owner.owner_id.ToString(), typeof(tblshop_owner).Name, ActionType.INSERT);

                            return Messages.GetInstance().HandleRequest("Shop owner", ActionType.INSERT);
                            //return Request.CreateResponse(HttpStatusCode.Created, new { Login = true, Pin = pin });
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to create shop owner.");
            }
        }



        /// <summary>
        /// Update an existing shop owner
        /// </summary>
        /// <remarks>
        /// Update the details of an existing shop owner, using the owner id
        /// </remarks>
        /// <returns></returns>
        // PUT api/ShopOwner/UpdateOwner/5
        [HttpPut]
        [Route("api/ShopOwner/UpdateOwner/{owner_id}")]
        public HttpResponseMessage Put(int owner_id, [FromBody] JObject owner_details)
        {
            try
            {
                string name = owner_details["name"].ToString().Trim();
                string contact_no = owner_details["contact_no"].ToString().Trim();
                string password = owner_details["password"].ToString().Trim();

                string email = null;
                if (owner_details["email"] != null)
                    email = owner_details["email"].ToString().Trim();

                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    // Check if a shop owner with the specified id exist
                    var entity = entities.tblshop_owner.FirstOrDefault(e => e.owner_id == owner_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Update failed! Shop owner with id = ", owner_id.ToString());

                    // Validate the contact no
                    if (!Utilities.getInstance().ValidateContactNumber(contact_no))
                        return Messages.GetInstance().ValidateFields("Shop owner", ActionType.UPDATE, isContactNumber: true);

                    // Validates the email
                    if (email != null && !Utilities.getInstance().ValidateEmail(email))
                        return Messages.GetInstance().ValidateFields("Shop owner", ActionType.INSERT, isEmail: true);

                    // Validates the password
                    if (!Utilities.getInstance().ValidatePassword(password))
                        return Messages.GetInstance().ValidateFields("Shop owner", ActionType.INSERT, isPassword: true);

                    var owner = entities.tblshop_owner.Any(e => e.contact_no.ToString() == contact_no);
                    if (entity.contact_no.ToString() != contact_no && owner)
                        return Messages.GetInstance().HandleException("Update failed! Contact number already exists.");

                    owner = entities.tblshop_owner.Any(e => e.email == email);
                    if (entity.email != email && owner)
                        return Messages.GetInstance().HandleException("Update failed! Email already exists.");

                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            // Update necessary fielsds
                            entity.name = name;
                            entity.contact_no = int.Parse(contact_no);
                            entity.email = email;
                            entity.password = Utilities.getInstance().CalculateHash(password);

                            Utilities.getInstance().UpdateChanges(entities, transaction, owner_id.ToString(), typeof(tblbarber).Name, ActionType.UPDATE);

                            return Messages.GetInstance().HandleRequest("Shop owner", ActionType.UPDATE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to update shop owner details.");
            }
        }



        /// <summary>
        /// Delete an existing shop owner
        /// </summary>
        /// <remarks>
        /// Delete an existing shop owner, using the owner id
        /// </remarks>
        /// <returns></returns>
        // DELETE api/ShopOwner/DeleteOwner/5
        [HttpDelete]
        [Route("api/ShopOwner/DeleteOwner/{owner_id}")]
        public HttpResponseMessage DELETE(int owner_id)
        {
            try
            {
                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    var entity = entities.tblshop_owner.FirstOrDefault(e => e.owner_id == owner_id);
                    if (entity == null)
                        return Messages.GetInstance().HandleException("Delete failed! Shop owner with id = ", owner_id.ToString());
                    else
                    {
                        using (var transaction = entities.Database.BeginTransaction())
                        {
                            entities.tblshop_owner.Remove(entity);
                            Utilities.getInstance().UpdateChanges(entities, transaction, owner_id.ToString(), typeof(tblshop_owner).Name, ActionType.DELETE);

                            return Messages.GetInstance().HandleRequest("Shop owner", ActionType.DELETE);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! Failed to delete shop owner.");
            }
        }


 
    }
}
