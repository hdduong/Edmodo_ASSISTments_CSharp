using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace connectToASSISTments_1.EdmodoStruct
{
    public class User
    {
        public string user_token {get; set;}
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string user_type { get; set; }
        public string access_token {get ;set; }
        public List<Group> groups { get; set; }
        
    }
}