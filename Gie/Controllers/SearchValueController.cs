using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Gie.Controllers
{
    public class SearchValueController : ApiController
    {

        /// <summary>
        /// Find spesfic node in the JSON by xpath
        /// </summary>
        /// <param name="json">JSON item to look in</param>
        /// <param name="xPath">Xpath string to look for</param>
        /// <returns></returns>
        private IEnumerable<JToken> findXpath(JObject json, string xPath)
        {
            if (json.SelectTokens(xPath) == null)
                throw new Exception("The X-Path is not valid");
            return json.SelectTokens(xPath);
        }

        public JObject Post(JObject input)
        {
            JObject json = JObject.Parse(input["JsonContent"].ToString());
            string xPath = input["xPath"].ToString();
            try
            {
                var objectArray = findXpath(json, xPath);
                if (objectArray.Count() == 0)
                {
                    return JObject.Parse("{message: 'No matching entries'}");
                }
                else
                {
                    StringBuilder result = new StringBuilder("{result: '");
                    foreach (var objet in objectArray)
                    {
                        result.Append(objet.ToString()).Append(", ");
                    }
                    result.Length-=2;
                    result.Append("'}");
                    return JObject.Parse(result.ToString());
                }
            }
            catch (Exception e)
            {
                try
                {
                    return JObject.Parse("{error: '" + e.Message.ToString() + "'}");
                }
                catch
                {
                    return JObject.Parse("{error: 'Geting data from the json, failed'}");
                }
            }
        }
    }
}
