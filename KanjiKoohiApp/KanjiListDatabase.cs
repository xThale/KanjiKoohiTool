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
                int index = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    Kanji kanji = null;
                    if (index == 0)
                        index++;
                    else
                        kanji = new Kanji(Int32.Parse(values[0]), values[1], values[2], Int32.Parse(values[5]), Int32.Parse(values[6]), Int32.Parse(values[7]));

                    if(kanji != null && kanji.frameNumber != -1)
                        kanjiList.Add(kanji);
                }
            }
            this.kanjiList=kanjiList;
        }

        public DataTable getDataTable(List<Kanji> kanjiList)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add(new DataColumn("Index",typeof(int)));
            dataTable.Columns.Add(new DataColumn("Kanji", typeof(String)));
            dataTable.Columns.Add(new DataColumn("Keyword", typeof(String)));
            dataTable.Columns.Add(new DataColumn("Current Box", typeof(int)));
            dataTable.Columns.Add(new DataColumn("Total Count", typeof(int)));
            dataTable.Columns.Add(new DataColumn("Fail Count", typeof(int)));
            dataTable.Columns.Add(new DataColumn("Pass Count", typeof(int)));
            dataTable.Columns.Add(new DataColumn("Pass Percentage", typeof(int)));

            foreach (Kanji entity in kanjiList)
            {
                if(entity.frameNumber != -1)
                    dataTable.Rows.Add(entity.frameNumber, entity.kanji, entity.keyword, entity.box,entity.totalCount, entity.failCount, entity.passCount, (int)entity.passPercentage);
            }
            return dataTable;
        }

        public List<Kanji> getKanjiList()
        {
            return this.kanjiList;
        }

    }
}
