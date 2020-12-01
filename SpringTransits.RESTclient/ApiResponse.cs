/************************************************************************************************************
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 ************************************************************************************************************/
using System;

namespace SpringTransits.RESTClient
{
    [Serializable]
    public class ApiResponse
    {
        public ApiResponse()
        {
        }

        public string Data { get; set; }

        public bool IsSuccessful { get; set; }
    }
}