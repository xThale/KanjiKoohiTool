using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanjiKoohiApp
{
    class Kanji
    {
        public String frameNumber = "";
        public String kanji = "";
        public String keyword = "";
        public String box = "";
        public String failCount = "";
        public String passCount = "";

        public Kanji(String frameNumber, String kanji, String keyword, String box, String failCount, String passCount)
        {
            this.frameNumber = frameNumber;
            this.kanji = kanji;
            this.keyword = keyword;
            this.box = box;
            this.failCount = failCount;
            this.passCount = passCount;
        }

        public override string ToString()
        {
            return this.frameNumber + " " + this.kanji + " " + this.keyword;
        }
    }
}
