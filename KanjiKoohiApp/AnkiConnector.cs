using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KanjiKoohiApp
{
    class AnkiConnector
    {
        private string json = @"{
                            ""action"": ""findNotes"",
                            ""version"": 6,
                            ""params"": {
                                ""query"": ""deck:[SETNAME]""
                            }
                        }";

        private string nodeJson = @"{
                                        ""action"": ""notesInfo"",
                                        ""version"": 6,
                                        ""params"": {
                                            ""notes"": [NODEID]
                                        }
                                    }";

        public async Task<List<long>> getAllNewNodes(String setname)
        {
            String url = "http://localhost:8765";

            string jsonEdited = this.json.Replace("[SETNAME]", setname);

            var httpClient = new HttpClient();
            var httpContent = new StringContent(jsonEdited, Encoding.UTF8, "application/json");

            var content = await httpClient.PostAsync(url, httpContent);
            string response = await content.Content.ReadAsStringAsync();
            
            if(content.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject jobject = JObject.Parse(response);
                JArray jarray = jobject.Value<JArray>("result");
                List<long> nodeList = new List<long>();
                foreach (var i in jarray.Values())
                {
                    string s = i.ToString();
                    nodeList.Add(long.Parse(s));
                }
                return nodeList;
            }
            return null;
        }

        public async Task<List<AnkiCard>> getAnkiCards(List<long> idList, String setname)
        {
            String url = "http://localhost:8765";

            List<AnkiCard> ankiList = new List<AnkiCard>();

            string replaceString = "";
            foreach(long i in idList)
            {
                replaceString += i + ",";
            }
            replaceString = replaceString.Remove(replaceString.Length - 1);

            var httpClient = new HttpClient();
            string js = nodeJson.Replace("NODEID", replaceString);
            var httpContent = new StringContent(js, Encoding.UTF8, "application/json");

            var content = await httpClient.PostAsync(url, httpContent);
            string response = await content.Content.ReadAsStringAsync();

            if (content.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject jobject = JObject.Parse(response);
                JArray jarray = jobject.Value<JArray>("result");

                if (setname.Equals("#TodayTango"))
                {
                    foreach (JObject jobj in jarray)
                    {
                        JObject j = jobj.Value<JObject>("fields");
                        string kanji = j.Value<JObject>("Tango N5 Vocab Japanese").Value<string>("value");
                        string keyword = j.Value<JObject>("Tango N5 Vocab English").Value<string>("value");
                        string kana = j.Value<JObject>("Tango N5 Vocab Furigana").Value<string>("value");
                        //string type = j.Value<JObject>("Tae Kim Vocab Part of Speech").Value<string>("value");
                        string contextSentenceJp = j.Value<JObject>("Tango N5 Sent Japanese").Value<string>("value");
                        string contextSentenceEng = j.Value<JObject>("Tango N5 Sent English").Value<string>("value");
                        AnkiCard anki = new AnkiCard(kanji, kana, keyword, null, contextSentenceJp, contextSentenceEng);
                        ankiList.Add(anki);
                    }
                }
                else
                {
                    foreach (JObject jobj in jarray)
                    {
                        JObject j = jobj.Value<JObject>("fields");
                        string kanji = j.Value<JObject>("Tae Kim Vocab Japanese").Value<string>("value");
                        string keyword = j.Value<JObject>("Tae Kim Vocab English").Value<string>("value");
                        string kana = j.Value<JObject>("Tae Kim Vocab Kana").Value<string>("value");
                        string type = j.Value<JObject>("Tae Kim Vocab Part of Speech").Value<string>("value");
                        string contextSentenceJp = j.Value<JObject>("Tae Kim Sent Japanese").Value<string>("value");
                        string contextSentenceEng = j.Value<JObject>("Tae Kim Sent English").Value<string>("value");
                        AnkiCard anki = new AnkiCard(kanji, kana, keyword, type, contextSentenceJp, contextSentenceEng);
                        ankiList.Add(anki);
                    }
                }    
            }
            return ankiList;
        }

    }
}
