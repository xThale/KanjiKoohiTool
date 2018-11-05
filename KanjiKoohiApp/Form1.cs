using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KanjiKoohiApp
{
    public partial class Form1 : Form
    {
        KanjiListDatabase kanjiListDatabase = new KanjiListDatabase();
        CardBuilder cardBuilder = new CardBuilder();
        QuizletRestApi quizletRestApi = new QuizletRestApi();
        
        private static String csvFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\rtk_flashcards.csv";
        private static String storyFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\my_stories.csv";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView.MouseWheel += new MouseEventHandler(dataGridView_MouseWheel);
            webBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
            webBrowser.ScriptErrorsSuppressed = true;


            if (File.Exists(csvFile)) {
                loadCsvFile(csvFile);
                kanjiListDatabase.parseStory(storyFile);
            }
            else
                openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            loadCsvFile(openFileDialog.FileName);
        }

        private void loadCsvFile(String path)
        {
            kanjiListDatabase.parseCsv(path);
            bindingSource.DataSource = kanjiListDatabase.getDataTable(kanjiListDatabase.getKanjiList());
            dataGridView.DataSource = bindingSource;
            dataGridView.Update();

            cardBuilder.getCards(this.kanjiListDatabase, 2, 75);
            labelImport.Text = "Card-Count: " + this.kanjiListDatabase.getKanjiList().Count + "     ";
            foreach (KeyValuePair<String, List<Kanji>> kvp in this.cardBuilder.cardList)
            {
                labelImport.Text = labelImport.Text + kvp.Key + "-Count: " + kvp.Value.Count + "     ";
            }

            this.BringToFront();
        }

        void dataGridView_MouseWheel(object sender, MouseEventArgs e)
        {
            if(dataGridView.Rows.Count > 0)
            {
                int currentIndex = this.dataGridView.FirstDisplayedScrollingRowIndex;
                int scrollLines = SystemInformation.MouseWheelScrollLines*2;

                if (e.Delta > 0)
                {
                    this.dataGridView.FirstDisplayedScrollingRowIndex
                        = Math.Max(0, currentIndex - scrollLines);
                }
                else if (e.Delta < 0)
                {
                    this.dataGridView.FirstDisplayedScrollingRowIndex
                        = currentIndex + scrollLines;
                }
            }
        }

        public void showBrowser()
        {
            tableLayoutPanel1.Controls.Add(webBrowser, 0, 2);
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.Visible = true;
            webBrowser.BringToFront();
            
            webBrowser.Update();
            dataGridView.Visible = false;
        }

        public void hideBrowser()
        {
            webBrowser.Visible = false;
            dataGridView.Visible = true;
        }

        public void showUrl(String url)
        {
            webBrowser.Url = new Uri(url);
        }

        

        async void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Console.WriteLine(webBrowser.Url.ToString());
            if (webBrowser.Url.ToString().Contains("http://127.0.0.1"))
            {
                if (quizletRestApi.getResponse(webBrowser.Url.ToString()))
                {
                    Boolean result = await quizletRestApi.requestTokenAsync();
                    if (!result)
                    {
                        throw new Exception("Failed to get access token");
                    }
                    else
                    {
                        hideBrowser();
                        buttonConnect_Click(null, null);
                        return;
                    }
                }
                throw new Exception("Url did not match excpection " + webBrowser.Url.ToString());
            }
        }
        

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            buttonConnect.Enabled = false;
            buttonConnect.Text = "Creating sets";

            if(quizletRestApi.access_token == "")
            {
                quizletRestApi.randomString = getRandomString();
                string url = "https://quizlet.com/authorize?response_type=code&client_id=" + quizletRestApi.clientId + "&scope=read write_set&state=" + quizletRestApi.randomString;
                showBrowser();
                showUrl(url);
                return;
            }


            String errorString = "";
            Dictionary<int, String> setMap = new Dictionary<int, string>();
            try
            {
                setMap = await quizletRestApi.getAllSets();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                buttonConnect.Enabled = true;
                buttonConnect.Text = "Connect";
                buttonConnect_Click(sender, e);
                return;
            }

            foreach (KeyValuePair<int, String> kvp in setMap)
            {
                foreach (KeyValuePair<String, List<Kanji>> kvpCardList in this.cardBuilder.cardList)
                {
                    if (kvp.Value.Equals(kvpCardList.Key))
                    {
                        await quizletRestApi.removeSet(kvp.Key);
                    }
                }
                if (kvp.Value.Equals("#TodayTae") || kvp.Value.Equals("#TodayTango"))
                {
                    await quizletRestApi.removeSet(kvp.Key);
                }
            }

            AnkiConnector anki = new AnkiConnector();
            try
            {
                List<long> nodeList = await anki.getAllNewNodes("#TodayTae");
                if (nodeList.Count > 0)
                {
                    List<AnkiCard> ankiCardList = await anki.getAnkiCards(nodeList, "#TodayTae");
                    if (ankiCardList.Count > 0)
                    {
                        await quizletRestApi.createAnkiSets("#TodayTae", ankiCardList);
                    }
                }

                nodeList = await anki.getAllNewNodes("#TodayTango");
                if (nodeList.Count > 0)
                {
                    List<AnkiCard> ankiCardList = await anki.getAnkiCards(nodeList, "#TodayTango");
                    if (ankiCardList.Count > 0)
                    {
                        await quizletRestApi.createAnkiSets("#TodayTango", ankiCardList);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }






            foreach (KeyValuePair<String, List<Kanji>> kvpCardList in this.cardBuilder.cardList)
            {
                if (!await quizletRestApi.createSets(kvpCardList.Key, kvpCardList.Value))
                {
                    errorString += "List has one item. Item could not be added to a list. List: " + kvpCardList.Key + " Item: " + kvpCardList.Value[0].ToString();
                }


            }



            if (!errorString.Equals(""))
            {
                MessageBox.Show(errorString, "Error adding sets", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            buttonConnect.Enabled = true;
            buttonConnect.Text = "Connect";
        }

        Random random = new Random();
        private string getRandomString()
        {
            
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, random.Next(8, 12))
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
