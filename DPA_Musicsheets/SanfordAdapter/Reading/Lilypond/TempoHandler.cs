using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond
{
    class TempoHandler : IHandler
    {
        public Song Handle(ArraySegment<string> allWordsAfterKeyword, Song.Builder songBuilder)
        {
            string tempoString = allWordsAfterKeyword.ElementAt(2); //does offset + n

            int tempo;
            if (Int32.TryParse(tempoString, out tempo))
                songBuilder.AddTempo(tempo);

            return songBuilder.GetItem();
        }
    }
}
