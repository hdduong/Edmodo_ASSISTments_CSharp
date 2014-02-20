using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using connectToASSISTments_1.EdmodoStruct;
using connectToASSISTments_1.Utilities;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace connectToASSISTments_1.EdmodoJob
{
    public class GetStudentsByGroup
    {
        public static List<Member> GetStudents(string access_token, string group_id)
        {
            string getMembers = String.Format("{0}/members?api_key={1}&access_token={2}&group_id={3}",
                                        Global.EdmodoAPIBase,
                                        Global.EdmodoAPIKey,
                                        access_token,
                                        group_id);

            string jsonReturn = APIClient.doGET(getMembers);
            List<Member> members = JsonConvert.DeserializeObject<List<Member>>(jsonReturn);

            return members;
        }



    }
}