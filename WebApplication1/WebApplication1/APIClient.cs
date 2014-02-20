using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.UI;
using System.Text;
using System.IO;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace connectToASSISTments_1
{
    public class APIClient
    {
        public static string doGET(string dataWithApiAddress)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(dataWithApiAddress);
            WebRequest request = (WebRequest)WebRequest.Create(dataWithApiAddress);

            request.Method = "GET";                                                     // GET no need wirte to stream

            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            string responseFromServer = reader.ReadToEnd();                             //return JSON object

            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }
    }
}