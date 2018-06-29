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

        public void getCards(KanjiListDatabase db, int newCount, int easyThreshold, int mediumThreshold)
        {

            List<Kanji> easyList = new List<Kanji>();
            List<Kanji> mediumList = new List<Kanji>();
            List<Kanji> hardList = new List<Kanji>();
            List<Kanji> newEasyList = new List<Kanji>();
            List<Kanji> newHardList = new List<Kanji>();

            foreach (Kanji kanji in db.getKanjiList())
            {
                if (kanji.totalCount <= newCount && kanji.passPercentage > easyThreshold)
                    newEasyList.Add(kanji);
                else if (kanji.totalCount <= newCount && kanji.passPercentage <= easyThreshold)
                    newHardList.Add(kanji);
                else if (kanji.passPercentage > easyThreshold)
                    easyList.Add(kanji);
                else if (kanji.passPercentage > mediumThreshold)
                    mediumList.Add(kanji);
                else
                    hardList.Add(kanji);
            }

            this.cardList.Add("Easy Kanji",easyList);
            this.cardList.Add("Medium Kanji", mediumList);
            this.cardList.Add("Hard Kanji", hardList);
            this.cardList.Add("New Easy Kanji", newEasyList);
            this.cardList.Add("New Hard Kanji", newHardList);
        }
    }
}
