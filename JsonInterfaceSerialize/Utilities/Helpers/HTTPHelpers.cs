using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace JsonInterfaceSerialize.Utilities.Helpers
{
    public class HTTPHelpers
    {
        static HttpClient client = new HttpClient();
        static bool Is_Init = false;

        private static void Init_Httpclient()
        {
            if (Is_Init) return;
            // Below two line is to bypass SSL method
            var clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            client = new HttpClient(clientHandler);

            Is_Init = true;
        }
        public static string callurl(Uri url)
        {
            WebRequest request = HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string urlText = reader.ReadToEnd(); // it takes the response from your url. now you can use as your need  
            return urlText;
        }
        ///// <summary>
        ///// Method to call an API with get method.
        ///// </summary>
        ///// <param name="URL">Complete URL of the API to be called</param>
        ///// <returns>Return Task. HttpResponseMessage content.</returns>
        //public static async Task<dynamic> GetAsync(
        //    string URL)
        //{
        //    return await GetAsync(URL, null);
        //}

        /// <summary>
        /// Method to call an API with get method.
        /// </summary>
        /// <param name="URL">Complete URL of the API to be called</param>
        /// <param name="ApiKey">Parameter to the called url</param>
        /// <param name="SearchQuery">Parameter to the called url</param>
        /// <returns>Return Task. HttpResponseMessage content.</returns>
        public static async Task<dynamic> GetAsync(
            string URL,
            string ApiKey,
            dynamic SearchQuery = null)
        {
            Init_Httpclient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ApiKey);

            var Stringcontent = new StringContent(SearchQuery);
            HttpResponseMessage response = await client.GetAsync(URL + Stringcontent);
            if (response.StatusCode == HttpStatusCode.OK)
                return response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.StatusCode.ToString());
        }
        /// <summary>
        /// Method to call an API with get method and BASIC Auth.
        /// </summary>
        /// <param name="URL">Complete URL of the API to be called</param>
        /// <param name="username">user name for basic auth of the URL</param>
        /// <param name="password">password for basic auth of the URL</param>
        /// <param name="SearchQuery">Parameter to the called url</param>
        /// <returns>Return Task. HttpResponseMessage content.</returns>
        public static async Task<dynamic> GetAsyncWithBasicAuth(
           string URL,
           string username,
           string password,
           dynamic SearchQuery = null)
        {
            Init_Httpclient();
            using (var requestMessage =
                    new HttpRequestMessage(HttpMethod.Get, URL))
            {
                string creds = string.Format("{0}:{1}", username, password);
                //string creds = string.Format("devwrite:superdev");
                byte[] bytes = Encoding.ASCII.GetBytes(creds);
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.SendAsync(requestMessage);
                if (response.StatusCode == HttpStatusCode.OK)
                    return response.Content.ReadAsStringAsync();
                else
                    throw new Exception(response.StatusCode.ToString());
            }
        }


        /// <summary>
        /// Method to call an API with get method and list of arguments.
        /// </summary>
        /// <param name="URL">Complete URL of the API to be called</param>
        /// <param name="SearchQuery">Parameter to the called url</param>
        /// <returns>Return Task. HttpResponseMessage content.</returns>
        public static async Task<dynamic> GetAsync(
            string URL,
             string ApiKey,
            List<dynamic> SearchQuery)
        {
            Init_Httpclient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

            var Stringcontent = new StringContent(JsonConvert.SerializeObject(SearchQuery, Formatting.Indented), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.GetAsync(URL);
            if (response.StatusCode == HttpStatusCode.OK)
                return response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.StatusCode.ToString());
        }

        /// <summary>
        /// Method to call an API with POST method.
        /// </summary>
        /// <param name="URL">Complete URL of the API to be called</param>
        /// <param name="PostValue">Parameter object to the called url</param>
        /// <param name="username">Parameter object to the called url</param>
        /// <param name="password">password object to the called url</param>
        /// <returns>Return Task. HttpResponseMessage content.</returns>
        public static async Task<dynamic> PostAsyncWithBasicAuth(
            string URL,
            string username,
            string password,
            dynamic PostValue)
        {
            Init_Httpclient();

            string creds = string.Format("{0}:{1}", username, password);
            byte[] bytes = Encoding.ASCII.GetBytes(creds);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var Stringcontent = new StringContent(JsonConvert.SerializeObject(PostValue, Formatting.Indented), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(URL, Stringcontent);
            if (response.StatusCode == HttpStatusCode.OK)
                return response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.StatusCode.ToString());

        }

        /// <summary>
        /// Method to call an API with POST method.
        /// </summary>
        /// <param name="URL">Complete URL of the API to be called</param>
        /// <param name="PostValue">Parameter object to the called url</param>
        /// <returns>Return Task. HttpResponseMessage content.</returns>
        public static async Task<dynamic> PostAsync(
            string URL, string ApiKey,
            dynamic PostValue)
        {
            Init_Httpclient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(ApiKey);

            var Stringcontent = new StringContent(JsonConvert.SerializeObject(PostValue, Formatting.Indented), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(URL, Stringcontent);
            if (response.StatusCode == HttpStatusCode.OK)
                return response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.StatusCode.ToString());
        }



        /// <summary>
        /// Method to call an API with POST method.
        /// </summary>
        /// <param name="URL">Complete URL of the API to be called</param>
        /// <param name="PostValue">Parameter List object to the called url</param>
        /// <returns>Return Task. HttpResponseMessage content.</returns>
        public static async Task<dynamic> PostAsync(
            string URL, string ApiKey,
            List<dynamic> PostValue)
        {
            Init_Httpclient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ApiKey);

            var Stringcontent = new StringContent(JsonConvert.SerializeObject(PostValue, Formatting.Indented), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(URL, Stringcontent);
            if (response.StatusCode == HttpStatusCode.OK)
                return response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.StatusCode.ToString());
        }
        /// <summary>
        /// Method to call an API with PATCH method.
        /// </summary>
        /// <param name="URL">Complete URL of the API to be called</param>
        /// <param name="PatchValue">Parameter object to the called url</param>
        /// <returns>Return Task. HttpResponseMessage content.</returns>
        public static async Task<dynamic> PatchAsync(
            string URL, string ApiKey,
            dynamic PatchValue)
        {
            Init_Httpclient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(ApiKey);

            var Stringcontent = new StringContent(JsonConvert.SerializeObject(PatchValue, Formatting.Indented), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PatchAsync(URL, Stringcontent);
            if (response.StatusCode == HttpStatusCode.OK)
                return response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.StatusCode.ToString());
        }
        /// <summary>
        /// Method to call an API with PATCH method.
        /// </summary>
        /// <param name="URL">Complete URL of the API to be called</param>
        /// <param name="PatchValue">Parameter List object to the called url</param>
        /// <returns>Return Task. HttpResponseMessage content.</returns>
        public static async Task<dynamic> PatchAsync(
            string URL, string ApiKey,
            List<dynamic> PatchValue)
        {
            Init_Httpclient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(ApiKey);

            var Stringcontent = new StringContent(JsonConvert.SerializeObject(PatchValue, Formatting.Indented), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PatchAsync(URL, Stringcontent);
            if (response.StatusCode == HttpStatusCode.OK)
                return response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.StatusCode.ToString());
        }
        /// <summary>
        /// Method to call an API with Put method.
        /// </summary>
        /// <param name="URL">Complete URL of the API to be called</param>
        /// <param name="PutValue">Parameter object to the called url</param>
        /// <returns>Return Task. HttpResponseMessage content.</returns>
        public static async Task<dynamic> PutAsync(
            string URL, string ApiKey,
            dynamic PutValue)
        {
            Init_Httpclient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(ApiKey);

            var Stringcontent = new StringContent(JsonConvert.SerializeObject(PutValue, Formatting.Indented), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(URL, Stringcontent);
            if (response.StatusCode == HttpStatusCode.OK)
                return response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.StatusCode.ToString());
        }
        /// <summary>
        /// Method to call an API with Put method.
        /// </summary>
        /// <param name="URL">Complete URL of the API to be called</param>
        /// <param name="PutValue">Parameter List object to the called url</param>
        /// <returns>Return Task. HttpResponseMessage content.</returns>
        public static async Task<dynamic> PutAsync(
            string URL, string ApiKey,
            List<dynamic> PutValue)
        {
            Init_Httpclient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ApiKey);

            var Stringcontent = new StringContent(JsonConvert.SerializeObject(PutValue, Formatting.Indented), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(URL, Stringcontent);
            if (response.StatusCode == HttpStatusCode.OK)
                return response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.StatusCode.ToString());
        }
        /// <summary>
        /// Method to call an API with Delete method.
        /// </summary>
        /// <param name="URL">Complete URL of the API to be called</param>
        /// <param name="DeleteRecord">Parameter object to the called url</param>
        /// <returns>Return Task. HttpResponseMessage content.</returns>
        public static async Task<dynamic> DeleteAsync(
            string URL, string ApiKey,
            dynamic DeleteRecord)
        {
            Init_Httpclient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(ApiKey);

            var Stringcontent = new StringContent(DeleteRecord);
            HttpResponseMessage response = await client.DeleteAsync(URL + Stringcontent);
            if (response.StatusCode == HttpStatusCode.OK)
                return response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.StatusCode.ToString());
        }
    }
}
