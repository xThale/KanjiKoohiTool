using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KanjiKoohiApp
{
    class QuizletRestApi
    {
        private static Random random = new Random();
        private static String clientId = "PedjCTSzS7";
        private String code = "";
        private String type = "";
        public String access_token = "";
        private String username = "";

        public async Task<Dictionary<int, String>> getAllSets()
        {
            String url = "https://api.quizlet.com/2.0/users/" + this.username + "/sets?access_token="+this.access_token;
            JArray jarray = await getAsync(url);

            Dictionary<int, String> setMap = new Dictionary<int, String>();

            foreach (JObject jobject in jarray)
            {
                setMap.Add(Int32.Parse(jobject.GetValue("id").ToString()), jobject.GetValue("title").ToString());
            }
            return setMap;
        }



        public async Task<JArray> getAsync(string uri)
        {
            var httpClient = new HttpClient();
            var content = await httpClient.GetStringAsync(uri);
            return await Task.Run(() => JArray.Parse(content));
        }

        public async Task<Boolean> delAsync(string uri)
        {
            var httpClient = new HttpClient();
            var content = await httpClient.DeleteAsync(uri);
            var response = await content.Content.ReadAsStringAsync();
            if (content.StatusCode == HttpStatusCode.NoContent)
                return true;
            else
                throw new Exception(response.ToString());
        }


        public async Task<Boolean> removeSet(int id)
        {
            String url = "https://api.quizlet.com/2.0/sets/" + id+ "?access_token="+this.access_token;
            return await delAsync(url);
        }


        public async Task<Boolean> createSets(String title, List<Kanji> kanjiList)
        {
            String url = "https://api.quizlet.com/2.0/sets?access_token="+this.access_token;
            var httpClient = new HttpClient();
            List<KeyValuePair<string, string>> bodyProperties = new List<KeyValuePair<string, string>>();
            bodyProperties.Add(new KeyValuePair<string, string>("title", title));
            bodyProperties.Add(new KeyValuePair<string, string>("lang_terms", "ja-ro"));
            bodyProperties.Add(new KeyValuePair<string, string>("lang_definitions", "en"));
            bodyProperties.Add(new KeyValuePair<string, string>("visibility", "only_me"));

            foreach (Kanji kanji in kanjiList)
            {
                bodyProperties.Add(new KeyValuePair<string, string>("terms[]", kanji.frameNumber+" "+kanji.kanji));
                bodyProperties.Add(new KeyValuePair<string, string>("definitions[]", kanji.keyword));
            }

            var dataContent = new FormUrlEncodedContent(bodyProperties.ToArray());

            var content = await httpClient.PostAsync(url, dataContent);
            var response = await content.Content.ReadAsStringAsync();

            if (content.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }
            else
            {
                throw new Exception(response.ToString());
            }
        }







        private string getRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, random.Next(8,12))
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool getResponse(String listenUrl)
        {
            string randomString = getRandomString();
            string url = "https://quizlet.com/authorize?response_type=code&client_id="+ clientId + "&scope=read write_set&state=" + randomString;

            System.Diagnostics.Process.Start(url);

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(listenUrl);
            listener.Start();
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            listener.Stop();

            string state = extractString(request.RawUrl, "state=", "&");
            string code = extractString(request.RawUrl, "code=", "&");

            if(code != null)
            {
                this.code = code;
                return state.Equals(randomString);
            }

            return false;
        }

        public async Task<Boolean> requestTokenAsync()
        {
            string url = "https://api.quizlet.com/oauth/token";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Basic UGVkakNUU3pTNzo5cVhjdGJzN3V4dEtTYUh1UXg0SzJ3");
            client.DefaultRequestHeaders.Add("redirect_uri", "http://127.0.0.1:80/request");
            client.DefaultRequestHeaders.Add("code", this.code);
            client.DefaultRequestHeaders.Add("grant_type", "authorization_code");
            client.DefaultRequestHeaders.Add("charset", "UTF-8");

            var values = new Dictionary<string, string>
                {
                    { "Content-Type", "application/x-www-form-urlencoded" },
                    { "redirect_uri", "http://127.0.0.1:80/request"},
                    { "code", this.code},
                    { "grant_type", "authorization_code"},
                    { "charset", "UTF-8"}
                };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(url, content);

            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseString);

            string access_token = extractString(responseString, "access_token\":\"", "\"");
            if (access_token != null)
            {
                this.access_token = access_token;
                this.username = extractString(responseString, "user_id\":\"", "\"");
                this.type = extractString(responseString, "token_type\":\"", "\"");
                Console.WriteLine(this.access_token);
                return true;
            }
            else
                return false;
        }

         public static string extractString(string s, string start, string end)
        {
            if (s.Contains(start))
            {
                int startIndex = s.IndexOf(start) + start.Length;
                int endIndex = -1;
                if (s.Substring(startIndex).Contains(end))
                {
                    endIndex = s.Substring(startIndex).IndexOf(end) + startIndex;
                    return s.Substring(startIndex, endIndex - startIndex);
                }
                else
                    return s.Substring(startIndex);
            }
            return null;
        }
    }
}
