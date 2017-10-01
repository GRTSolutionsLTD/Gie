using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using RestSharp;

namespace Gie.Controllers
{
    public class FormatController : ApiController
    {
        /// <summary>
        /// Build the value by the xpath parameters
        /// </summary>
        /// <param name="xpath">List of Xpathes for the value nodes</param>
        /// <param name="json">Json item that contain the value fields</param>
        /// <returns></returns>
        private string buildTextbyXpath(string[] xpath, JObject json)
        {
            string value ="";
            StringBuilder builder = new StringBuilder("'");
            foreach (string path in xpath)
            {
                string[] textFields = path.Split('?');
                if (textFields[0] != "")
                    value = findXpath(json, textFields[0]).ToString();
                if (textFields.Count() > 1)
                    value += " " + textFields[1];
                
                builder.Append(value).Append(" ");
            }
            builder.Length--;
            builder.Append("'");
            return builder.ToString();
        }

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

        /// <summary>
        /// Build a JSON string in carousel format
        /// </summary>
        /// <param name="json">Array of JSON object to be display in carousel format</param>
        /// <param name="text">List of xpath strings to search the text field</param>
        /// <param name="subText">List of xpath strings to search the subText field</param>
        /// <param name="imgURL">URL for the imageUrl field</param>
        /// <returns></returns>
        //public JObject BuildCarouselFormat(List<JObject> json, string text, string subText, string imgURL)
        public JObject BuildCarouselFormat(List<JObject> json, string text, string subText, string imgURL, string options, string key)
        {
            string[] textFields = text.Split(',');
            string[] subTextFields = subText.Split(',');
            string[] keyFields = key.Split(',');
            StringBuilder builder = new StringBuilder("{elements:[");
            foreach (JObject item in json)
            {
                builder.Append("{").Append("text: ");
                builder.Append(buildTextbyXpath(textFields, item));
                builder.Append(" ,subtext: ");
                builder.Append(buildTextbyXpath(subTextFields, item));
                builder.Append(" ,image_url: '").Append(imgURL).Append("'");
                builder.Append(" ,options: ['").Append(options).Append("']");
                builder.Append(" ,key: ");
                builder.Append(buildTextbyXpath(keyFields, item));
                builder.Append("},");
            }
            builder.Length--;
            builder.Append("]}");
            return JObject.Parse(builder.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="text"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public JObject BuildMenuFormat(List<JObject> json, string text, string options)
        {
            string[] optionFields = options.Split(',');
            StringBuilder builder = new StringBuilder("{type: 'menu', text: '").Append(text).Append("', options: [");
            foreach (JObject item in json)
            {
                builder.Append(buildTextbyXpath(optionFields, item));
                builder.Append(" ,");
            }
            builder.Length--;
            builder.Append("]}");
            return JObject.Parse(builder.ToString());
        }

        /// <summary>
        /// A post API request to return array of JSON objects in the required "Service friend" format
        /// </summary>
        /// <param name="jsonURL">URL for the json that contain json array</param>
        /// <param name="xPath">Xpath for the array in the basic json</param>
        /// <param name="type">The required "Service Friend" format</param>
        /// <param name="text">Xpath for the text field (concatenation by ',' charcter)</param>
        /// <param name="subText">Xpath for the subText field (concatenation by ',' charcter)</param>
        /// <param name="imgURL">URL for the imageURL field</param>
        /// <returns></returns>
        //public JObject Get(string jsonURL, string xPath, string type, string text, string subText, string imgURL)
        public JObject Post(JObject input)
        {
            //JObject json = null;
            //string xPath = null;
            //string type = null;
            //string text = null;
            //string subText = null;
            //string imgURL = null;

            JObject json = JObject.Parse(input["JsonContent"].ToString());
            string xPath = input["xPath"].ToString();
            string type = input["type"].ToString();
            string text = input["text"].ToString();
            string subText = input["subText"].ToString();
            string imgURL = input["image_url"].ToString();
            string options = input["options"].ToString();
            string key = input["key"].ToString();

            try
            {
                /*var client = new RestClient(jsonURL.ToString());
                var request = new RestRequest(Method.GET);
                var response = client.Execute(request);
                JObject json = JObject.Parse(response.Content);*/
                var arrayJson = findXpath(json, xPath).ToObject<List<JObject>>();
                switch (type)
                {
                    case "carousel":
                        return BuildCarouselFormat(arrayJson, text, subText, imgURL, options, key);
                    case "menu":
                        //text = findXpath(json, text).ToString();
                        return BuildMenuFormat(arrayJson, text, subText);
                    default: throw new Exception("The format type is invalid");
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
                    return JObject.Parse("{error: 'Building " + type + " format was failed.'}");
                }
            }
        }

        /*public JObject Get(string jsonURL, string xPath, string value)
        {
            try
            {
                var client = new RestClient(jsonURL);
                var request = new RestRequest(Method.GET);
                var response = client.Execute(request);
                JObject json = JObject.Parse(response.Content);
                var text = findXpath(json, xPath).ToString();
                if(value==text)
                    return JObject.Parse("{result: '1'}");
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
                    return JObject.Parse("{error: 'Geting data from the json, failed'}" );
                }
                //return JObject.Parse("{error: '" + e.Message + "'}");
            }
        }*/



    }
}
