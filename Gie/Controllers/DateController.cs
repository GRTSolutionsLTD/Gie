using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Gie.Controllers
{
    public class DateController : ApiController
    {
        public string Post(JObject input)
        {
            string date = input["date"].ToString();
            string inputFormat = input["inputFormat"].ToString();
            string outputFormat = input["outputFormat"].ToString();
            return DateTime.ParseExact(date, inputFormat, CultureInfo.InstalledUICulture).ToString(outputFormat, CultureInfo.InstalledUICulture);
        }
    }
}
