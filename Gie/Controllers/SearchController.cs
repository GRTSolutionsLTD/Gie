using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Gie.Controllers
{
    public class SearchController : ApiController
    {
        /// <summary>
        /// Find spesfic node in the JSON by xpath
        /// </summary>
        /// <param name="json">JSON item to look in</param>
        /// <param name="xPath">Xpath string to look for</param>
        /// <returns></returns>
        private JToken findXpath(JObject json, string xPath)
        {
            if (json.SelectToken(xPath) == null)
                throw new Exception("The X-Path is not valid");
            return json.SelectToken(xPath);
        }

        public JObject Post(JObject input)
        {
            JObject json = JObject.Parse(input["JsonContent"].ToString());
            string xPath = input["xPath"].ToString();
            string subxpath = input["subxpath"].ToString();
            string value = input["value"].ToString();
            try
            {
                if(subxpath=="")
                {
                    var text = findXpath(json, xPath).ToString();
                    if (value == text)
                        return JObject.Parse("{result: '1'}");
                }
                else
                {
                    var textArray = findXpath(json, xPath).ToObject<List<JObject>>();
                    foreach (JObject item in textArray)
                    {
                        var text = findXpath(item, subxpath).ToString();
                        if (value == text)
                            return JObject.Parse("{result: '1'}");
                    }
                }
                return JObject.Parse("{result: '0'}");

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
