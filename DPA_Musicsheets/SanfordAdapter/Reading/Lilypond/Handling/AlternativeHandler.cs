using DPA_Musicsheets.SanfordAdapter.Tonal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond.Handling
{
    class AlternativeHandler : IHandler
    {
        public void Handle(LilypondArraySegment.Enumerator enumerator, LilypondArraySegment allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            TrackPart.Builder trackPartBuilder = songBuilder.CurrentTrackBuilder.CurrentTrackPartBuilder;

            int barCount = 1;
            foreach (string word in allWordsIncludingKeyword.Skip(3))
            {
                if (word == "|")
                {
                    barCount++;
                }
                else if (word == "}")
                {
                    trackPartBuilder.AddAlternativeBarCount(barCount);
                    return;
                }
            }
        }
    }
}
