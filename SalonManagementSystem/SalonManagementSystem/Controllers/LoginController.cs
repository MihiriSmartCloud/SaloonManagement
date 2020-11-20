using Newtonsoft.Json.Linq;
using SalonAPI.Common;
using SalonAPI.Common.Utility;
using SalonAPI.Enum;
using SalonDataAccess;
using SalonManagementSystem.Common.Utility;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using Cookie = SalonManagementSystem.Common.Utility.Cookie;


/*
 * Author  - S Salgado
 * Date    - 27/10/2020
 */
namespace SalonAPI.Controllers
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class LoginController : ApiController
    {

        /// <summary>
        /// User login via username & password. 
        /// </summary>
        /// <remarks>
        /// Login to the application using the username & password
        /// </remarks>
        /// <returns></returns>
        /// <param name="credentials">Ex: { username: "abcdef", password: "qwerty123#" }</param>
        // POST api/User/Login  
        [HttpPost]
        [Route("api/User/Login")]
        public HttpResponseMessage Login([FromBody] JObject credentials)
        {
            try
            {
                string email = credentials["email"].ToString().Trim();
                string password = credentials["password"].ToString().Trim();

                // Validates the email
                if (!Utilities.getInstance().ValidateEmail(email))
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Login failed! Received invalid email." });

                // Checks if the user exists
                if (Utilities.getInstance().IsValidUser(email))
                {
                    using (SalonDbEntities entities = new SalonDbEntities())
                    {
                        // Validates the password format
                        if (!Utilities.getInstance().ValidatePassword(password))
                            return Messages.GetInstance().HandleException("Login failed! Received invalid password. Hint: password should contain atleast 8 characters including letters, atleast 1 digit & 1 special character.", isLogin: true);

                        // Checks if the password matches
                        if (Utilities.getInstance().IsValidPassword(email, password))
                        {
                            int owner_id = int.Parse(Utilities.getInstance().GetUserData(email, false, true));

                            // Update log information
                            Log.Update(owner_id.ToString(), typeof(tblshop_owner).Name, ActionType.LOGIN);

                            // Get the PIN of the user
                            string encryptedPin = entities.tblshop_owner.Where(e => e.email == email).Select(e => e.pin).First().ToString();

                            // Create a unique token
                            //string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                            string token = Token.GenerateToken(new Token
                            {
                                Id = Guid.NewGuid(),
                                Enable = false,
                                UId = owner_id
                            });

                            // Create a cookie to store the user data temporarily
                            var cookie = new HttpCookie("SMS")
                            {
                                HttpOnly = true
                            };
                            cookie.Values.Add("Token", Utilities.getInstance().CalculateHash(token));
                            //cookie.Values.Add("Pin", encryptedPin);
                            HttpContext.Current.Response.Cookies.Set(cookie);
                            // HttpContext.Current.Request.Cookies.Set(cookie);
                            // HttpContext.Current.Request.Cookies.Add(cookie);


                            // It's the first time the user has visited the main login page
                            HttpContext.Current.Session.Clear();
                            HttpContext.Current.Session.RemoveAll();

                            // Start a new session
                            var session = HttpContext.Current.Session;
                            if (session != null)
                            {
                                session["Token"] = Utilities.getInstance().CalculateHash(token);
                                session.Timeout = 5;
                            }

                            // return Messages.GetInstance().HandleRequest("Login", ActionType.LOGIN);
                            return Request.CreateResponse(HttpStatusCode.OK, new { Login = true, Pin = int.Parse(Utilities.getInstance().DecodeFrom64(encryptedPin)) });
                        }
                        else
                            return Messages.GetInstance().HandleException("Login failed! Received invalid credentials.", isLogin: true);
                    }
                }
                else
                    return Messages.GetInstance().HandleException("Login failed! User does not exist.", isLogin: true);
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! User login failed.");
            }
        }



        /// <summary>
        /// User login via pin
        /// </summary>
        /// <remarks>
        /// Login using a pin
        /// </remarks>
        /// <returns></returns>
        /// <param name="pin_details">{ pin: 61366 }</param>
        // POST api/User/LoginViaPin  
        [HttpPost]
        [Route("api/User/LoginViaPin")]
        public HttpResponseMessage LoginViaPin([FromBody] JObject pin_details)
        {
            try
            {
                using (SalonDbEntities entities = new SalonDbEntities())
                {
                    if (Cookie.CookieExist("SMS", "Token"))
                    {
                        // Get user data
                        //string token = Cookie.GetFromCookie("SMS", "Token");
                        //string encryptedPin = Cookie.GetFromCookie("SMS", "Pin");
                        //// Decrypt the pin from the cookie 
                        //int pin = int.Parse(Utilities.getInstance().DecodeFrom64(encryptedPin));

                        //string encryptedPin = Cookie.GetFromCookie("SMS", "Pin");
                        //// Decrypt the pin from the cookie 
                        //int pin = int.Parse(Utilities.getInstance().DecodeFrom64(encryptedPin));


                        string userEnteredPin = pin_details["pin"].ToString();

                        // Validates the pin
                        if (userEnteredPin.Count() != 5 || !Regex.IsMatch(userEnteredPin, @"^\d{5}$"))
                            return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Login Failed! Received invalid pin. Hint: Pin should contain only 5 digits." });

                        string encryptedToken = Cookie.GetFromCookie("SMS", "Token");
                        string token = Utilities.getInstance().DecodeFrom64(encryptedToken);
                        int userId = Token.DeserialiseToken(token).UId;
                        string encryptedPin = entities.tblshop_owner.Where(x => x.owner_id == userId).Select(x => x.pin).First();
                        string pin = Utilities.getInstance().DecodeFrom64(encryptedPin);

                        // Check if both pins match
                        if (userEnteredPin != pin)
                            return Messages.GetInstance().HandleException("Login failed! Received invalid pin.", isLogin: true);
                        else
                        {
                            // Update log information
                            Log.Update(userId.ToString(), typeof(tblshop_owner).Name, ActionType.LOGIN);

                            // Remove all previous sessions
                            HttpContext.Current.Session.Clear();
                            HttpContext.Current.Session.RemoveAll();

                            string previousToken = "";
                            if (Cookie.CookieExist("SMS", "Token"))
                                previousToken = Cookie.GetFromCookie("SMS", "Token");

                            // Start a new session
                            var session = HttpContext.Current.Session;
                            if (session != null)
                            {
                                session["Token"] = token;
                                session.Timeout = 5;
                            }

                            //return Messages.GetInstance().HandleRequest("Login", ActionType.LOGIN);
                            return Request.CreateResponse(HttpStatusCode.OK, new { Login = true, Pin = int.Parse(pin) });
                        }
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, new { Success = false, Message = "Login failed! Unable to authenticate user." });
                }
            }
            catch (Exception)
            {
                return Messages.GetInstance().HandleException("An error occured! User login failed.");
            }
        }


    }
}
