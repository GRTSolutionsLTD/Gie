
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;

namespace Gie.Controllers
{
    public class GetGieDetailsController : ApiController
    {
        private string getAccessToken(bool log)
        {
            try
            {
                var client = new RestClient(ConfigurationManager.AppSettings["GetTokenURL"]);

                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Basic NzRiM1E2WU1xYkJYTm5mejc1cjhoRGZjNWFnSWMyZDA6NnpFRndzNmVqbk1Va2hQQQ==");
                var response = client.Execute(request);

                if (log)
                {
                    using (StreamWriter _writeToLog = new StreamWriter(HttpContext.Current.Server.MapPath("~/Info.txt"), true))
                    {
                        _writeToLog.WriteLine(DateTime.Now.ToString("HH:mm:ss tt") + " : Request - URL:" + client.BaseUrl + "   Method:" + request.Method);
                        _writeToLog.WriteLine(DateTime.Now.ToString("HH:mm:ss tt") + " : Response - " + response.Content);
                    }
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var obj = JObject.Parse(response.Content);
                    return (string)obj["accessToken"];
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private JObject sendToken(string token, string subscriber, bool log)
        {
            try
            {
                var client = new RestClient(ConfigurationManager.AppSettings["SendTokenURL1"] + subscriber + ConfigurationManager.AppSettings["SendTokenURL2"]);
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", "Bearer " + token);
                request.AddHeader("Content-Type", "application/json");

                var response = client.Execute(request);
                if (log)
                {
                    using (StreamWriter _writeToLog = new StreamWriter(HttpContext.Current.Server.MapPath("~/Info.txt"), true))
                    {
                       
                        _writeToLog.WriteLine(DateTime.Now.ToString("HH:mm:ss tt") + " : Request - URL:"+client.BaseUrl + "    Method:" + request.Method);
                        _writeToLog.WriteLine(DateTime.Now.ToString("HH:mm:ss tt") + " : Response - " + response.Content);
                    }
                }
                var returenedJson = JsonConvert.SerializeObject(new { headers = response.Headers, content = JObject.Parse(response.Content) });
                return JObject.Parse(returenedJson.ToString());
                //return JObject.Parse(response.Content);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public JObject Post(JObject input)
        {
            bool log = false;
            if (input["logging"] != null)
                log = (bool)input["logging"];
            try
            {
                if (log)
                {
                    using (StreamWriter _writeToLog = new StreamWriter(HttpContext.Current.Server.MapPath("~/Info.txt"), true))
                    {
                        _writeToLog.WriteLine(DateTime.Now.ToString("HH:mm:ss tt") + " : Request - Body Parameters:" + input);
                    }
                }
                string token = getAccessToken(log);
                return sendToken(token, (string)input["subscriber"], log);
            }
            catch (Exception e)
            {
                if (log)
                {
                    using (StreamWriter _writeToLog = new StreamWriter(HttpContext.Current.Server.MapPath("~/Error.txt"), true))
                    {
                        _writeToLog.WriteLine(DateTime.Now.ToString("HH:mm:ss tt") + " : " + e.Message);
                    }
                }
                return JObject.Parse("{error: '" + e.Message + "'}");
            }
        }
    }
}
