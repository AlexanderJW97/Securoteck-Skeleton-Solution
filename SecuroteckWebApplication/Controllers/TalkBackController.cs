using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SecuroteckWebApplication.Controllers
{
    public class TalkBackController : ApiController
    {
        [ActionName("Hello")]
        public string Get()
        {
            return "Hello World";
        }

        [ActionName("Sort")]
        public HttpResponseMessage Get(HttpResponseMessage request, [FromUri]Object[] integers)
        {
            bool inputsOk = true;
            int initialLength = integers.Length;
            int[] checkedInts = new int[initialLength];
            int i = 0;
            foreach (var variable in integers)
            {
                try
                {
                    int num = Convert.ToInt32(variable);
                    checkedInts[i] = num;
                    i++;
                }
                catch
                {
                    inputsOk = false;
                }
            }
            string sortedInts = "";
            
            Array.Sort(checkedInts);
            
            if (inputsOk == true)
            {
                sortedInts = "[";
                int length = checkedInts.Length;
                int count = 0;
                for (int j = 0; j < length; j++)
                {
                    if (!(count == 0))
                    {
                        sortedInts = sortedInts + ",";
                    }
                    sortedInts = sortedInts + checkedInts[j].ToString();
                    count++;

                }
                sortedInts = sortedInts + "]";

                return Request.CreateResponse<string>(HttpStatusCode.OK, sortedInts);
            }
            return Request.CreateResponse<string>(HttpStatusCode.BadRequest, sortedInts);
            
            
        }
    }
}
