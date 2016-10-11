using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond.Handling
{
    class RepeatHandler : IHandler
    {
        public void Handle(LilypondArraySegment.Enumerator enumerator, LilypondArraySegment allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            TrackPart.Builder trackPartBuilder = new TrackPart.Builder();
            songBuilder.CurrentTrackBuilder.AddTrackPartBuilder(trackPartBuilder);

            int currentIndex = 0;
            string repeatKindString = allWordsIncludingKeyword.ElementAt(++currentIndex);
            switch (repeatKindString)
            {
                case "volta": //NOTE: assume always volta for now
                    string repeatAmountString = allWordsIncludingKeyword.ElementAt(++currentIndex);
                    int repeatAmount = Int32.Parse(repeatAmountString);
                    trackPartBuilder.AddRepeat(repeatAmount);
                    //NOTE: indicates "volta" and number e.g. "2" and "{" handled
                    allWordsIncludingKeyword.Start += 3;
                    //NOTE: skips "volta" and number e.g. "2" and "{" in foreach.
                    enumerator.CurrentIndex += 3; 
                    break;
                case "unfold" :
                    break;
                case "percent" :
                    break;
                case "tremolo" :
                    break;
            }
        }
    }
}
