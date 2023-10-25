using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Errors
{
    public class APIException
    {
        public APIException(int statusCode,string message, string deatils) 
        {
            this.StatusCode = statusCode;
            this.Deatils = deatils;
            this.Message=message;
   
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Deatils { get; set; }
        
    }
}