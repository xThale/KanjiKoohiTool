using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanjiKoohiApp
{
    class CardBuilder
    {
        public Dictionary<String, List<Kanji>> getCards(KanjiListDatabase db, int newCount, int easyThreshold, int mediumThreshold)
        {
            Dictionary<String,List<Kanji>> cardList = new Dictionary<String, List<Kanji>>();

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

            cardList.Add("Easy Kanji",easyList);
            cardList.Add("Medium Kanji", mediumList);
            cardList.Add("Hard Kanji", hardList);
            cardList.Add("New Easy Kanji", newEasyList);
            cardList.Add("New Hard Kanji", newHardList);

            return cardList;
        }
    }
}
