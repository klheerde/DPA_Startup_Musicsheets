using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond.Handling
{
    class TempoHandler : IHandler
    {
        public void Handle(LilypondArraySegment.Enumerator enumerator, LilypondArraySegment allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            string tempoString = allWordsIncludingKeyword.ElementAt(1); //does offset + n through IEnumerator
            string[] tempoStringSplit = tempoString.Split('=');

            //TODO for now assume always 4=xxx
            int tempo = Int32.Parse(tempoStringSplit[1]);
            songBuilder.AddTempo(tempo);

            //NOTE: indicates word has been handled.
            allWordsIncludingKeyword.Start += 1;
            //NOTE: makes foreach skip "x=xxx" word.
            enumerator.CurrentIndex += 1;
        }
    }
}
