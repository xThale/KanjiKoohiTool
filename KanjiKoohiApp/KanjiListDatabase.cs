using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KanjiKoohiApp
{
    class KanjiListDatabase
    {

        private List<Kanji> kanjiList = new List<Kanji>();

        public void parseCsv(String filepath)
        {
            List<Kanji> kanjiList = new List<Kanji>();
            using (var reader = new StreamReader(filepath, Encoding.UTF8))
            {

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    Kanji kanji = new Kanji(values[0], values[1], values[2], values[5], values[6], values[7]);
                    kanjiList.Add(kanji);
                }
            }
            this.kanjiList=kanjiList;
        }

        public DataTable getDataTable()
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add(new DataColumn("Index",typeof(String)));
            dataTable.Columns.Add(new DataColumn("Kanji", typeof(String)));
            dataTable.Columns.Add(new DataColumn("Keyword", typeof(String)));
            dataTable.Columns.Add(new DataColumn("Current Box", typeof(String)));
            dataTable.Columns.Add(new DataColumn("Fail Count", typeof(String)));
            dataTable.Columns.Add(new DataColumn("Pass Count", typeof(String)));

            foreach (Kanji entity in this.kanjiList)
            {
                String[] values = new String[dataTable.Columns.Count];
                values[0] = entity.frameNumber;
                values[1] = entity.kanji;
                values[2] = entity.keyword;
                values[3] = entity.box;
                values[4] = entity.failCount;
                values[5] = entity.passCount;

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

    }
}
