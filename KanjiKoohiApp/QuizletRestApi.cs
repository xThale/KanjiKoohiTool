using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KanjiKoohiApp
{
    class QuizletRestApi
    {
        private static Random random = new Random();
        private static String clientId = "PedjCTSzS7";
        private String code = "";

        private string getRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, random.Next(8,12))
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool authApp()
        {
            string randomString = getRandomString();
            string url = "https://quizlet.com/authorize?response_type=code&client_id="+ clientId +"& scope=write_set&state="+randomString;

            System.Diagnostics.Process.Start(url);

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:80/");
            listener.Start();
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            listener.Stop();

            string state = extractString(request.RawUrl, "state=", "&");
            string code = extractString(request.RawUrl, "code=", "&");

            this.code = code;

            return state.Equals(randomString);
        }

        public static string extractString(string s, string start, string end)
        {
            int startIndex = s.IndexOf(start) + start.Length;
            int endIndex = -1;
            if (s.Substring(startIndex).Contains(end))
            {
                endIndex = s.Substring(startIndex).IndexOf(end)+ startIndex;
                return s.Substring(startIndex, endIndex - startIndex);
            }
            else
                return s.Substring(startIndex);
        }
    }
}
