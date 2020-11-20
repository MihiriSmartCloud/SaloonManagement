using System;

namespace SalonManagementSystem.Common.Utility
{

    #pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Token
    {
        public Guid Id { get; set; }

        public int UId { get; set; }

        public bool Enable { get; set; }


        public static string GenerateToken(Token claims)
        {
            string serialised = Newtonsoft.Json.JsonConvert.SerializeObject(claims);
            return serialised;
        }

        public static Token DeserialiseToken(string token)
        {
            Token deserialised = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(token);
            return deserialised;
        }
    }
}