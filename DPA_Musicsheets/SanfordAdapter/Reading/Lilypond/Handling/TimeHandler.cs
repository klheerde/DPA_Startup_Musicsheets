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

            //TODO just throw if wrong
            int timeSig0, timeSig1;
            if (Int32.TryParse(timeSigString[0], out timeSig0) && Int32.TryParse(timeSigString[1], out timeSig1))
            {
                //Note lastAddedNote = songBuilder.CurrentTrackBuilder.CurrentTrackPartBuilder.LastAddedNote;
                //int startTime = lastAddedNote == null ? 0 : lastAddedNote.StartTime;
                //songBuilder.AddTimeSignature(startTime, timeSig0, timeSig1);
                if (songBuilder.GetItem().Tempo > 0)
                {
                    int 
                }

                songBuilder.CurrentTrackBuilder.CurrentTrackPartBuilder.AddTimeSignature(timeSig0, timeSig1, 0);
            }

            //NOTE: skip one word extra. "\time 4/4" is two words, one already skipped in LyReader foreach
            allWordsIncludingKeyword.Start += 1;
            //NOTE: makes foreach skip "4/4" word.
            enumerator.CurrentIndex += 1;
        }
    }
}
