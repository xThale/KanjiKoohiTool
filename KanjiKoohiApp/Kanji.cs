using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanjiKoohiApp
{
    class Kanji
    {
        public int frameNumber = -1;
        public String kanji = "";
        public String keyword = "";
        public int box = -1;
        public int totalCount = -1;
        public int failCount = -1;
        public int passCount = -1;
        public double passPercentage = -1;

        public Kanji(int frameNumber, String kanji, String keyword, int box, int failCount, int passCount)
        {
            this.frameNumber = frameNumber;
            this.kanji = kanji;
            this.keyword = keyword;
            this.box = box;
            this.failCount = failCount;
            this.passCount = passCount;

            this.totalCount = failCount + passCount;
            this.passPercentage = (double)((double)passCount / (double)totalCount) * 100;
        }

        public override string ToString()
        {
            return this.frameNumber + " " + this.kanji + " " + this.keyword;
        }
    }
}
