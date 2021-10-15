using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Delta_Minus.Util {
    internal class _1330API {
        public static BTD6Mod[] retrievedMods; 

        public static void Init() {
            var http = new HttpClient();

            var httpRequest = new HttpRequestMessage() {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://1330studios.com/api-json/")
            };

            httpRequest.Content = new StringContent(JsonConvert.SerializeObject(new { description = "Delta Minus Data Download" }), Encoding.UTF8, "application/json");
            httpRequest.Headers.Add("User-Agent", $"Delta Minus {Assembly.GetCallingAssembly().GetName().Version.ToString(3)}");

            BTD6Mod[] APImods = JsonConvert.DeserializeObject<BTD6Mod[]>(Task.Run(() => http.Send(httpRequest)).Result.Content.ReadAsStringAsync().Result);
            
            lock (APImods) {
                retrievedMods = (BTD6Mod[])APImods.Clone();
            }
        }
    }
}
