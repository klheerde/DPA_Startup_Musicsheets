using DPA_Musicsheets.SanfordAdapter.Tonal;
using PSAMControlLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond.Handling
{
    class ClefHandler : IHandler
    {
        private Dictionary<string, ClefType> types = new Dictionary<string, ClefType>()
        {
            { "G", ClefType.GClef }, { "treble", ClefType.GClef },
            { "C", ClefType.CClef }, { "alto",   ClefType.CClef },
            { "F", ClefType.FClef }, { "bass",   ClefType.FClef },
        };

        public void Handle(LilypondArraySegment.Enumerator enumerator, LilypondArraySegment allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            string valueString = allWordsIncludingKeyword.ElementAt(1); //does offset + n through IEnumerator
            ClefType clefType;
            if (types.TryGetValue(valueString, out clefType))
            {
                songBuilder.AddClef(clefType);
            }

            //NOTE: indicates word has been handled.
            allWordsIncludingKeyword.Start += 1;
            //NOTE: makes foreach skip "x=xxx" word.
            enumerator.CurrentIndex += 1;
        }
    }
}
