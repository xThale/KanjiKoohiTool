using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KanjiKoohiApp
{
    public partial class Form1 : Form
    {
        KanjiListDatabase kanjiListDatabase = new KanjiListDatabase();
        CardBuilder cardBuilder = new CardBuilder();
        QuizletRestApi quizletRestApi = new QuizletRestApi();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView.MouseWheel += new MouseEventHandler(dataGridView_MouseWheel);
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {

            kanjiListDatabase.parseCsv(openFileDialog.FileName);
            bindingSource.DataSource = kanjiListDatabase.getDataTable(kanjiListDatabase.getKanjiList());
            dataGridView.DataSource = bindingSource;
            dataGridView.Update();

            Dictionary<String, List<Kanji>> cardLists = cardBuilder.getCards(this.kanjiListDatabase,2,90,80);
            labelImport.Text = "Card-Count: " + this.kanjiListDatabase.getKanjiList().Count+"     ";
            foreach (KeyValuePair<String, List<Kanji>> kvp in cardLists)
            {
                labelImport.Text = labelImport.Text + kvp.Key + "-Count: " + kvp.Value.Count + "     ";
            }

            //quizletRestApi.authApp();
        }

        void dataGridView_MouseWheel(object sender, MouseEventArgs e)
        {
            int currentIndex = this.dataGridView.FirstDisplayedScrollingRowIndex;
            int scrollLines = SystemInformation.MouseWheelScrollLines;

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
}
