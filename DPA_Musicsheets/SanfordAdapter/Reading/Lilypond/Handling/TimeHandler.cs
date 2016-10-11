using DPA_Musicsheets.SanfordAdapter.Tonal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond.Handling
{
    class TimeHandler : IHandler
    {
        public void Handle(LilypondArraySegment.Enumerator enumerator, LilypondArraySegment allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            string timeString = allWordsIncludingKeyword.ElementAt(1); //does offset + n
            string[] timeSigString = timeString.Split('/');

            //NOTE: throws error if wrong.
            int timeSig0 = Int32.Parse(timeSigString[0]);
            int timeSig1 = Int32.Parse(timeSigString[1]);

            //NOTE: assume lilypond always has currentTrackPart.
            //NOTE: using songBuilder.GetItem(), but song does not have Sequence so falls back to 384.
            songBuilder.CurrentTrackBuilder.CurrentTrackPartBuilder.AddTimeSignature(timeSig0, timeSig1, songBuilder.GetItem());

            //NOTE: skip one word extra. "\time 4/4" is two words, one already skipped in LyReader foreach.
            allWordsIncludingKeyword.Start += 1;
            //NOTE: makes foreach skip "4/4" word.
            enumerator.CurrentIndex += 1;
        }
    }
}
