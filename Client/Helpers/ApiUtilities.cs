using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client.Helpers
{
    public static class ApiUtilities
    {
        public static async Task<bool> CheckIfFieldExists(string url)
        {
            var client = new HttpClient();
           
            var responseString =  await client.GetStringAsync(url);

            var result = JsonConvert.DeserializeObject<bool>(responseString);

            return result;
        }
    }
}