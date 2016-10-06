using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond
{
    class TimeHandler : IHandler
    {
        public Song Handle(ArraySegment<string> allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            string timeString = allWordsIncludingKeyword.ElementAt(1); //doest offset + n
            string[] timeSigString = timeString.Split('/');
            int timeSig0, timeSig1;
            if (Int32.TryParse(timeSigString[0], out timeSig0) && Int32.TryParse(timeSigString[1], out timeSig1))
            {
                int startTime = songBuilder.CurrentTrackBuilder.CurrentTrackPartBuilder.GetItem().StartTime;
                songBuilder.AddTimeSignature(startTime, timeSig0, timeSig1);
            }
            return songBuilder.GetItem();
        }
    }
}
