using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EmailingSystem.Common.Common
{
    public class Util
    {
        public static async Task PostApi(string uri, string data)
        {
            using (var client = new HttpClient())
            {
                await client.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
            }
                
        }

        public static async Task<int> GetNoDaysFromNow(DateTime startDate, DateTime endDate)
        {
            return await Task.Run(() =>
            {
                return (int)Math.Round((endDate.Date - startDate.Date).TotalDays);

            }).ConfigureAwait(false);
        }
    }
}
