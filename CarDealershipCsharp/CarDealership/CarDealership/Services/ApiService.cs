using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CarDealership.Services
{
    public static class ApiService
    {
        static HttpClient client = new HttpClient();
        static string baseUrl = "http://localhost:5219/api/";

        // 🔥 GET
        public static async Task<T> Get<T>(string endpoint)
        {
            var res = await client.GetAsync(baseUrl + endpoint);

            if (!res.IsSuccessStatusCode)
                return default;

            var json = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        // 🔥 POST (BOOL ONLY)
        public static async Task<bool> Post(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await client.PostAsync(baseUrl + endpoint, content);
            return res.IsSuccessStatusCode;
        }

        // 🔥 POST WITH RESPONSE (🔥 IMPORTANT FOR LOGIN)
        public static async Task<T> PostWithResponse<T>(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await client.PostAsync(baseUrl + endpoint, content);

            if (!res.IsSuccessStatusCode)
                return default;

            var response = await res.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(response);
        }

        // 🔥 PUT
        public static async Task<bool> Put(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await client.PutAsync(baseUrl + endpoint, content);
            return res.IsSuccessStatusCode;
        }

        // 🔥 DELETE
        public static async Task<bool> Delete(string endpoint)
        {
            var res = await client.DeleteAsync(baseUrl + endpoint);
            return res.IsSuccessStatusCode;
        }
    }
}