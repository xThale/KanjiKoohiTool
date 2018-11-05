using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanjiKoohiApp
{
    class AnkiCard
    {
        public string kanji;
        public string kana;
        public string keyword;
        public string type;
        public string contextSentenceJp;
        public string contextSentenceEng;

        public AnkiCard(string kanji, string kana, string keyword, string type, string contextSentenceJp, string contextSentenceEng)
        {
            this.kana = kana;
            this.kanji = kanji;
            this.keyword = keyword;
            this.type = type;
            this.contextSentenceJp = contextSentenceJp;
            this.contextSentenceEng = contextSentenceEng;
        }
    }
}
