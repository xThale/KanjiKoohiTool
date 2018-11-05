using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanjiKoohiApp
{
    class CardBuilder
    {
        public Dictionary<String, List<Kanji>> cardList = new Dictionary<String, List<Kanji>>();

        public void getCards(KanjiListDatabase db, int newCount, int easyThreshold)
        {

            List<Kanji> oldEasyList = new List<Kanji>();
            List<Kanji> oldHardList = new List<Kanji>();
            List<Kanji> medEasyList = new List<Kanji>();
            List<Kanji> medHardList = new List<Kanji>();
            List<Kanji> newEasyList = new List<Kanji>();
            List<Kanji> newHardList = new List<Kanji>();
            List<Kanji> unlearnedList = new List<Kanji>();

            foreach (Kanji kanji in db.getKanjiList())
            {
                if (kanji.box == 1)
                    unlearnedList.Add(kanji);
                else if (kanji.box < 3 && kanji.passPercentage >= easyThreshold)
                    newEasyList.Add(kanji);
                else if (kanji.box < 3 && kanji.passPercentage < easyThreshold)
                    newHardList.Add(kanji);
                else if (kanji.box == 3 && kanji.passPercentage >= easyThreshold)
                    medEasyList.Add(kanji);
                else if (kanji.box == 3 && kanji.passPercentage < easyThreshold)
                    medHardList.Add(kanji);
                else if (kanji.passPercentage > easyThreshold)
                    oldEasyList.Add(kanji);
                else
                    oldHardList.Add(kanji);
            }

            if(oldEasyList.Count == 1)
            {
                oldHardList.Add(oldEasyList[0]);
                oldEasyList.RemoveAt(0);
            }

            if(oldHardList.Count == 1)
            {
                if(oldEasyList.Count == 0)
                {
                    medHardList.Add(oldEasyList[0]);
                    oldHardList.RemoveAt(0);
                }
                else
                {
                    oldEasyList.Add(oldEasyList[0]);
                    oldHardList.RemoveAt(0);
                }
            }

            unlearnedList = unlearnedList.Reverse<Kanji>().ToList<Kanji>();

            this.cardList.Add("Unlearned Kanji", unlearnedList);
            //this.cardList.Add("Old Easy Kanji", oldEasyList);
            //this.cardList.Add("Old Hard Kanji", oldHardList);
            //this.cardList.Add("Med Easy Kanji", medEasyList);
            //this.cardList.Add("Med Hard Kanji", medHardList);
            //this.cardList.Add("New Easy Kanji", newEasyList);
            //this.cardList.Add("New Hard Kanji", newHardList);
            //this.cardList.Add("All Kanji", db.getKanjiList());
        }
    }
}
