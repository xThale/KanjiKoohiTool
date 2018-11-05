using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KanjiKoohiApp
{
    class QuizletRestApi
    {
        private static Random random = new Random();
        public String clientId = "PedjCTSzS7";
        private String code = "";
        private String type = "";
        public String access_token = "";
        private String username = "";
        public String randomString = "";

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
            httpClient.Timeout = TimeSpan.FromSeconds(2);
            int retry = 0;
            while(retry < 2)
            {
                try
                {
                    var content = await httpClient.GetStringAsync(uri);
                    return await Task.Run(() => JArray.Parse(content));
                }
                catch (HttpRequestException ex)
                {
                    retry++;
                    if (retry == 1)
                        Thread.Sleep(1000);
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Exception while getting all sets for an user. Message: " + ex.Message + ". Try again?", "Exception while deleting sets", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            await getAsync(uri);
                        }
                        throw ex;
                    }
                }
            }
            return null;
        }

        public async Task<Boolean> delAsync(string uri)
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(2);

            var content = await httpClient.DeleteAsync(uri);
            var response = await content.Content.ReadAsStringAsync();
            if (content.StatusCode == HttpStatusCode.NoContent || content.StatusCode == HttpStatusCode.Gone)
                return true;
            else
            {
                DialogResult dialogResult = MessageBox.Show("HTTP-ERROR " + content.StatusCode + ": " + response.ToString() + "Error deleating set. Try again?", "Exception while deleting sets", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Thread.Sleep(1000);
                    await delAsync(uri);
                }
            }
            return false;
        }


        public async Task<Boolean> removeSet(int id)
        {
            String url = "https://api.quizlet.com/2.0/sets/" + id+ "?access_token="+this.access_token;
            Console.WriteLine(url);
            Thread.Sleep(1000);
            bool returnValue = await delAsync(url);

            if (!returnValue)
                throw new Exception("Exception while removing existing sets");

            return false;
        }


        public async Task<Boolean> createSets(String title, List<Kanji> kanjiList)
        {
            

            if(kanjiList.Count > 1)
            {
                String url = "https://api.quizlet.com/2.0/sets?access_token=" + this.access_token;
                var httpClient = new HttpClient();
                List<KeyValuePair<string, string>> bodyProperties = new List<KeyValuePair<string, string>>();
                bodyProperties.Add(new KeyValuePair<string, string>("title", title));
                bodyProperties.Add(new KeyValuePair<string, string>("lang_terms", "en"));
                bodyProperties.Add(new KeyValuePair<string, string>("lang_definitions", "ja"));
                bodyProperties.Add(new KeyValuePair<string, string>("visibility", "only_me"));

                foreach (Kanji kanji in kanjiList.Reverse<Kanji>())
                {
                    bodyProperties.Add(new KeyValuePair<string, string>("terms[]", kanji.keyword));
                    
                    if(kanji.story != "")
                        bodyProperties.Add(new KeyValuePair<string, string>("definitions[]", kanji.frameNumber + " " + kanji.kanji + Environment.NewLine + kanji.story));
                    else
                        bodyProperties.Add(new KeyValuePair<string, string>("definitions[]", kanji.frameNumber + " " + kanji.kanji));

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
                    MessageBox.Show("HTTP-ERROR "+content.StatusCode+": "+response.ToString(), "Error creating sets", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return true;
            }

            throw new Exception("Exception while creating new sets");
        }


        public async Task<Boolean> createAnkiSets(String title, List<AnkiCard> ankiList)
        {


            if (ankiList.Count > 1)
            {
                String url = "https://api.quizlet.com/2.0/sets?access_token=" + this.access_token;
                var httpClient = new HttpClient();
                List<KeyValuePair<string, string>> bodyProperties = new List<KeyValuePair<string, string>>();
                bodyProperties.Add(new KeyValuePair<string, string>("title", title));
                bodyProperties.Add(new KeyValuePair<string, string>("lang_terms", "en"));
                bodyProperties.Add(new KeyValuePair<string, string>("lang_definitions", "ja"));
                bodyProperties.Add(new KeyValuePair<string, string>("visibility", "only_me"));

                foreach (AnkiCard anki in ankiList)
                {
                    bodyProperties.Add(new KeyValuePair<string, string>("terms[]", anki.kanji));

                    if(anki.type != null)
                        bodyProperties.Add(new KeyValuePair<string, string>("definitions[]", anki.keyword + " | " + anki.kana + "["+anki.type+"]" + " | " +anki.contextSentenceJp));
                    else
                        bodyProperties.Add(new KeyValuePair<string, string>("definitions[]", anki.keyword + " | " + anki.kana + " | " + anki.contextSentenceJp));

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
                    MessageBox.Show("HTTP-ERROR " + content.StatusCode + ": " + response.ToString(), "Error creating anki sets", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return true;
            }

            throw new Exception("Exception while creating new anki sets");
        }




        private string getRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, random.Next(8,12))
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool getResponse(String url)
        {
            string state = extractString(url, "state=", "&");
            string code = extractString(url, "code=", "&");

            if (code != null)
            {
                this.code = code;
                return state.Equals(this.randomString);
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
