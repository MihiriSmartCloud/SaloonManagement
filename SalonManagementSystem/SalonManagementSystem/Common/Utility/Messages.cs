using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using SalonAPI.Enum;

/*
 * Author  - S Salgado
 * Date    - 24/10/2020
 */
namespace SalonAPI.Common.Utility
{
    
    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Messages : ApiController
    {
        private static Messages message;

        private Messages()
        {
            message = null;
        }

        public static Messages GetInstance() => message ?? (message = new Messages());


        // Invokes when an invalid email, password or contact number is entered
        public HttpResponseMessage ValidateFields(string entity, ActionType action, bool isEmail = false, bool isPassword = false, bool isContactNumber = false, bool isUsername = false)
        {
            var Request = new HttpRequestMessage();
            var config = new HttpConfiguration();
            var controller = new Messages
            {
                Request = Request
            };
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            if (action == ActionType.INSERT && isContactNumber)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = "Failed to create " + entity.ToLower() + "! Received invalid contact number. Hint: contact no should contain only 9 or 10 digits." });
            else if (action == ActionType.UPDATE && isContactNumber)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = "Failed to update " + entity.ToLower() + " details! Received invalid contact number.  Hint: contact no should contain only 9 or 10 digits." });
            else if (action == ActionType.INSERT && isUsername)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = "Failed to create " + entity.ToLower() + "! Received invalid username. Hint: username should contain atleast 6 characters. It can have both letters & digits. First character must be a letter." });
            else if (action == ActionType.UPDATE && isUsername)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = "Failed to update " + entity.ToLower() + " details! Received invalid username." });
            else if (action == ActionType.INSERT && isEmail)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = "Failed to create " + entity.ToLower() + "! Received invalid email address." });
            else if (action == ActionType.UPDATE && isEmail)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = "Failed to update " + entity.ToLower() + " details! Received invalid email address." });
            else if (action == ActionType.INSERT && isPassword)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = "Failed to create " + entity.ToLower() + "! Received invalid password. Hint: password should contain a minimum of 8 characters including letters, atleast one digit & one special character." });
            else if (action == ActionType.UPDATE && isPassword)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = "Failed to update " + entity.ToLower() + " details! Received invalid password. Hint: password should contain a minimum of 8 characters including letters, atleast one digit & one special character." });

            return null;
        }


        // Invokes when a requested resource is not found or when an exception occurs or when a user login fails
        public HttpResponseMessage HandleException(string message, string id = null, bool isLogin = false)
        {
            var Request = new HttpRequestMessage();
            var config = new HttpConfiguration();
            var controller = new Messages
            {
                Request = Request
            };
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            // If the requested resource is not found
            if (id != null)
                return Request.CreateResponse(HttpStatusCode.NotFound, new { Success = false, Message = message + id + " does not exist." });
            else if(id == null && !isLogin)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = message });
            else if (id == null && isLogin)
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = message });

            return null;
        }


        // Invokes when a resouce gets inserted, deleted or updated successfully or when the resource already exists while trying to insert or update or when the user logins successfully
        public HttpResponseMessage HandleRequest(string entity, ActionType action, bool isExist = false)
        {
            var Request = new HttpRequestMessage();
            var config = new HttpConfiguration();
            var controller = new Messages
            {
                Request = Request
            };
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            if (action == ActionType.INSERT && isExist)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = "Failed to create " + entity.ToLower() + "! " + entity + " already exists." });
            else if (action == ActionType.INSERT)
                return Request.CreateResponse(HttpStatusCode.Created, new { Success = true, Message = entity + " created successfully!" });
            else if (action == ActionType.UPDATE && isExist)
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = "Update failed! " + entity + " already exists." });
            else if (action == ActionType.UPDATE)
                return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = entity + " details updated successfully!" });
            else if (action == ActionType.DELETE)
                return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = entity + " deleted successfully!" });
            else if (action == ActionType.LOGIN)
                return Request.CreateResponse(HttpStatusCode.OK, new { Success = true, Message = entity + " successful!" });

            return null;
        }


    }
}