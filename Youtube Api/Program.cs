using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Youtube_Api
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter Playlist ID: ");
            string playlistid = Console.ReadLine();
            var result = GetVideoInPlaylistAsync(playlistid).Result;

            PrintResult(result);

            Console.ReadKey();

        }

        private static void PrintResult(dynamic result)
        {
            var count = result.items.Count;
            var i = 0;

            if (count > 0)
                foreach (var item in result.items)
                    Console.WriteLine(string.Format($"{++i,3}) {item.snippet.title}"));
        }

        private static async Task<dynamic> GetVideoInPlaylistAsync(string playlistid)
        {
            var parameter = new Dictionary<string, string>
            {
                ["key"] = ConfigurationManager.AppSettings["ApiKey"],
                ["playlistId"] = playlistid,
                ["part"] = "snippet",
                ["fields"] = "items/snippet(title,description)",
                ["maxResults"] = "50"
            };

            string baseUrl = "https://www.googleapis.com/youtube/v3/playlistItems?";
            string fullUrl = MakeURLfromQuery(baseUrl, parameter);

            var result = await new HttpClient().GetStringAsync(fullUrl);

            if (result != null)
            {
                return JsonConvert.DeserializeObject(result);
            }
            return null;
        }

        private static string MakeURLfromQuery(string baseUrl, Dictionary<string, string> parameter)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (parameter == null || parameter.Count() == 0)
                return baseUrl;

            return parameter.Aggregate(baseUrl,
                (accumulated, kvp) => string.Format($"{accumulated}{kvp.Key}={kvp.Value}&"));
        }
    }
}
