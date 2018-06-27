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
        KanjiListDatabase csvParser = new KanjiListDatabase();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openCsv_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {

            csvParser.parseCsv(openFileDialog.FileName);
            bindingSource.DataSource = csvParser.getDataTable();
            dataGridView.DataSource = bindingSource;
            dataGridView.Update();
            
        }
    }
}
