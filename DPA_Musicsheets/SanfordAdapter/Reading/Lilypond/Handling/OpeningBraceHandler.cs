using DPA_Musicsheets.SanfordAdapter.Tonal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DPA_Musicsheets.SanfordAdapter.Reading.Lilypond.Handling
{
    class OpeningBraceHandler : IHandler
    {
        public virtual void Handle(LilypondArraySegment.Enumerator enumerator, LilypondArraySegment allWordsIncludingKeyword, Song.Builder songBuilder)
        {
            Track.Builder trackBuilder;
            try
            {
                trackBuilder = songBuilder.CurrentTrackBuilder;
            }
            //NOTE: no trackbuilder exists yet, create first.
            catch (ArgumentOutOfRangeException)
            {
                trackBuilder = new Track.Builder();
                songBuilder.AddTrackBuilder(trackBuilder);
            }

            TrackPart.Builder trackPartBuilder = new TrackPart.Builder();
            trackBuilder.AddTrackPartBuilder(trackPartBuilder);
            CopyPreviousTimeSignature(trackPartBuilder, songBuilder);
        }

        protected void CopyPreviousTimeSignature(TrackPart.Builder trackPartBuilder, Song.Builder songBuilder)
        {
            TrackPart previousTrackPart;
            try
            {
                //NOTE: get previous trackPart in current Track.
                previousTrackPart = songBuilder.CurrentTrackBuilder.PreviousTrackPartBuilder.GetItem();
            }
            //NOTE: both ArgumentOutOfRangeException and NullPointer
            catch (Exception)
            {
                try
                {
                    //NOTE: get last trackPart from previous track.
                    previousTrackPart = songBuilder.PreviousTrackBuilder.CurrentTrackPartBuilder.GetItem();
                }
                //NOTE: no previous track, do nothing.
                catch (Exception)
                {
                    return;
                }
            }

            int amountInBar = previousTrackPart.TimeSignature(0);
            if (amountInBar > 0)
            {
                int countsPerBeat = previousTrackPart.TimeSignature(1);
                int ticksPerBeat = previousTrackPart.TimeSignature(2);
                trackPartBuilder.AddTimeSignature(amountInBar, countsPerBeat, ticksPerBeat);
            }
        }
    }
}
